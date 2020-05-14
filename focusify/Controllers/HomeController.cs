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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Launch()
        {
            ViewBag.Message = "Your contact page.";

            string scope = "user-modify-playback-state";

            string authorizeQuery = "https://accounts.spotify.com/authorize" 
                + "?response_type=code"
                + "&client_id=" + clientID
                + "&scope=" + scope
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
    }
}