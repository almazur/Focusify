using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace focusify.Models
{
    public class SpotifyController
    {
        
        private TokenInfo tokenInfo;
        public SpotifyController(TokenInfo tokenInfo)
        {
            this.tokenInfo = tokenInfo;
        }

        public void AddItemToPlaybackQueue(string uri)
        {
            string query = "https://api.spotify.com/v1/me/player/queue?uri=" + uri;
            Task<string> task = Task.Run(async () => await PostAsync(query, ""));
            string response = task.Result;
            //System.Diagnostics.Debug.WriteLine(">> addToQueue: " + response);
        }

        public List<string> getPlaylistsItems(string playlistId)
        {
            string query = "https://api.spotify.com/v1/playlists/" + playlistId + "/tracks";
            Task<string> task = Task.Run(async () => await GetStringAsync(query));
            dynamic response = JsonConvert.DeserializeObject(task.Result);

            Newtonsoft.Json.Linq.JArray items = response["items"];
            return items.Select(i => i["track"].Value<string>("uri")).ToList();
        }

        public double getTimeToEnd()
        {
            string query1 = "https://api.spotify.com/v1/me/player/currently-playing";
            Task<string> task1 = Task.Run(async () => await GetStringAsync(query1));
            dynamic response1 = JsonConvert.DeserializeObject(task1.Result);

            if (response1 != null && response1["item"] != null && response1["item"]["id"] != null)
            {
                string query2 = "https://api.spotify.com/v1/audio-features/" + response1["item"]["id"];
                Task<string> task2 = Task.Run(async () => await GetStringAsync(query2));
                dynamic response2 = JsonConvert.DeserializeObject(task2.Result);

                var timeToEnd = response2["duration_ms"] - response1["progress_ms"];
                //System.Diagnostics.Debug.WriteLine(">> setVolume: " + progress);
                return timeToEnd;
            }
            return 0.0;
        }

        public void setVolume(int level)
        {
            string query = "https://api.spotify.com/v1/me/player/volume?volume_percent=" + level;
            Task<string> task = Task.Run(async () => await PutAsync(query, ""));
            string response = task.Result;
            //System.Diagnostics.Debug.WriteLine(">> setVolume: " + response);
        }

        public void play(string songId)
        {
            string query = "https://api.spotify.com/v1/me/player/play";
            string body = "{\"uris\": [\"" + songId + "\"]}";

            Task<string> task = Task.Run(async () => await PutAsync(query, body));
            string response = task.Result;
            //System.Diagnostics.Debug.WriteLine(">> play: " + response);
        }

        private async Task<string> GetStringAsync(string query)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenInfo.accessToken);

                var response = await client.GetStringAsync(query);
                //System.Diagnostics.Debug.WriteLine(response);
                return response;
            }
        }

        private async Task<string> PutAsync(string query, string body)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenInfo.accessToken);

                var response = await client.PutAsync(query, new StringContent(body, Encoding.UTF8, "application/json"));
                //System.Diagnostics.Debug.WriteLine(response.StatusCode);
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private async Task<string> PostAsync(string query, string body)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenInfo.accessToken);

                var response = await client.PostAsync(query, new StringContent(body, Encoding.UTF8, "application/json"));
                //System.Diagnostics.Debug.WriteLine(response.StatusCode);
                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}