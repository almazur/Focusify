
﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FLS;
using FLS.Rules;

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

                var controller = new ThinkgearController();
                var connectionId = controller.connect();
                for (int i = 0; i < 50; i++)
                {
                    Thread.Sleep(1000);
                    controller.readPackets(connectionId, 1);
                }

                play("spotify:track:10PjojnHeipKeQ04nrg9dq");
                Thread.Sleep(2000);
                
                for (int i=0; i<10; i++)
                {
                    setVolume(10 * i);
                    Thread.Sleep(2000);

                    //controller.readPackets(connectionId, 1);

                    //TODO
                    var water = new LinguisticVariable("Water");
                    var cold = water.MembershipFunctions.AddTrapezoid("Cold", 0, 0, 20, 40);
                    var warm = water.MembershipFunctions.AddTriangle("Warm", 30, 50, 70);
                    var hot = water.MembershipFunctions.AddTrapezoid("Hot", 50, 80, 100, 100);

                    var power = new LinguisticVariable("Power");
                    var low = power.MembershipFunctions.AddTriangle("Low", 0, 25, 50);
                    var high = power.MembershipFunctions.AddTriangle("High", 25, 50, 75);

                    IFuzzyEngine fuzzyEngine = new FuzzyEngineFactory().Default();

                    var rule1 = Rule.If(water.Is(cold).Or(water.Is(warm))).Then(power.Is(high));
                    var rule2 = Rule.If(water.Is(hot)).Then(power.Is(low));
                    fuzzyEngine.Rules.Add(rule1, rule2);

                    var result = fuzzyEngine.Defuzzify(new { water = 60 });
                    System.Diagnostics.Debug.WriteLine("This is: " + result);
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
            System.Diagnostics.Debug.WriteLine(">>>" + response);
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