using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace focusify.Models
{
    public class ThinkgearWorker
    {
        /* 
         * TODO implement core logic for reacting to user focus, using ThinkgearController and functions play and setVolume
         */

        private TokenInfo tokenInfo;

        public ThinkgearWorker(TokenInfo tokenInfo)
        {
            this.tokenInfo = tokenInfo;
        }

        public void StartProcessing(CancellationToken cancellationToken = default)
        {
            try
            {
                play("spotify:track:10PjojnHeipKeQ04nrg9dq");
                Thread.Sleep(2000);
                
                for (int i=0; i<10; i++)
                {
                    setVolume(10 * i);
                    Thread.Sleep(2000);
                }
            }
            catch (Exception ex)
            {
                ProcessCancellation();
            }
        }

        private void setVolume(int level)
        {
            string query = "https://api.spotify.com/v1/me/player/volume?volume_percent=" + level;
            Task<string> task = Task.Run(async () => await PutAsync(query, ""));
            string response = task.Result;
            //TODO proper debugging
            System.Diagnostics.Debug.WriteLine(">>" + response);
        }

        private void play(string songId)
        {
            string query = "https://api.spotify.com/v1/me/player/play";
            string body = "{\"uris\": [\"" + songId + "\"]}";

            Task<string> task = Task.Run(async () => await PutAsync(query, body));
            string response = task.Result;
            //TODO proper debugging
            System.Diagnostics.Debug.WriteLine(">>" + response);
        }

        private async Task<string> PutAsync(string query, string body)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenInfo.accessToken);

                var response = await client.PutAsync(query, new StringContent(body, Encoding.UTF8, "application/json"));
                System.Diagnostics.Debug.WriteLine(response.StatusCode);
                return response.Content.ReadAsStringAsync().Result;
            }
        }

        private void ProcessCancellation()
        {

        }
    }
}