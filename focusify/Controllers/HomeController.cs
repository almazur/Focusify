using focusify.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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

            String scope = "user-modify-playback-state";

            String authorizeQuery = "https://accounts.spotify.com/authorize" 
                + "?response_type=code"
                + "&client_id=" + clientID
                + "&scope=" + scope
                + "&redirect_uri=" + redirectURI;

            Response.Redirect(authorizeQuery);

            return View();
        }

        public ActionResult Auth([FromUri]String code, [FromUri]String state, [FromUri]String error)
        {
            if (!error.IsEmpty())
            {
                ViewBag.Message = "Something went wrong";
            } else
            {
                String tokenQuery = "https://accounts.spotify.com/api/token"
                    + "?grant_type=authorization_code"
                    + "&code=" + code
                    + "&redirect_uri=" + redirectURI;
                    //+ "&client_id=" + clientID
                    //+ "&client_secret" + clientSecret;

                Task<String> task = Task.Run<String>(async () => await PostAsync(tokenQuery));
                ViewBag.Message = task.Result;
            }
            return View();
        }

        private async Task<string> PostAsync(String tokenQuery)
        {
            using (var client = new HttpClient())
            {
                string c = clientID + ":" + clientSecret;
                string clientEncoded = Convert.ToBase64String(Encoding.ASCII.GetBytes(c));
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + clientEncoded);
                var response = await client.PostAsync(tokenQuery, new StringContent("", Encoding.UTF8, "application/json"));
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}