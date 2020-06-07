using focusify.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.WebPages;

namespace focusify.Controllers
{
    public class HomeController : Controller
    {
        private readonly string redirectURI = "http://localhost:8888/home/auth";
        private readonly string clientSecret = ClientConfig.Get().secret;
        private readonly string clientID = ClientConfig.Get().id;
        private static readonly string settingsFile = System.Web.HttpContext.Current.Server.MapPath(@"\Resources\settings.json");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            if (!System.IO.File.Exists(settingsFile))
            {
                string defaultFocused = "224djalW5N9R0mi72HOQba";
                string defaultNotFocused = "0bpVDliLq1xXITzGePfkyc";
                string json = "{\"Focused\":\"" + defaultFocused + "\", \"NotFocused\":\"" + defaultNotFocused + "\"}";
                System.IO.File.WriteAllText(settingsFile, json);
            }
            return View(AppSettings.Get());
        }

        public ActionResult Launch()
        {
            ViewBag.Message = "Your contact page.";

            string scopes = "user-modify-playback-state user-read-playback-state user-read-currently-playing";

            string authorizeQuery = "https://accounts.spotify.com/authorize" 
                + "?response_type=code"
                + "&client_id=" + clientID
                + "&scope=" + Uri.EscapeDataString(scopes)
                + "&redirect_uri=" + Uri.EscapeDataString(redirectURI);

            Response.Redirect(authorizeQuery);

            return View();
        }

        public ActionResult Auth([FromUri]string code, [FromUri]string state, [FromUri]string error)
        {
            if (!error.IsEmpty())
            {
                ViewBag.Message = "Something went wrong";
            } else
            {
                string query = "https://accounts.spotify.com/api/token";
                string body = "grant_type=authorization_code"
                    + "&code=" + code
                    + "&redirect_uri=" + Uri.EscapeDataString(redirectURI);

                Task<string> task = Task.Run(async () => await PostAsync(query, body));
                string response = task.Result;
                HostingEnvironment.QueueBackgroundWorkItem(cancellationToken => new ThinkgearWorker(TokenInfo.fromJson(response)).StartProcessing(cancellationToken));
                ViewBag.Message = "SUCCESS";
            }
            return View();
        }

        private async Task<string> PostAsync(string query, string body)
        {
            using (var client = new HttpClient())
            {
                string c = clientID + ":" + clientSecret;
                string clientEncoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(c));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + clientEncoded);

                var response = await client.PostAsync(query, new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"));
                return response.Content.ReadAsStringAsync().Result;
            }
        }
        public ActionResult Playlists(AppSettings appSettings)
        {
            string focused = appSettings.Focused;
            string notFocused = appSettings.NotFocused;
            string json = "{\"Focused\":\"" + focused + "\", \"NotFocused\":\"" + notFocused + "\"}";
            System.IO.File.WriteAllText(settingsFile, json);
            return RedirectToAction("Settings", "Home");
        }
    }
}