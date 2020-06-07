using System;
using System.Text;
using System.IO;
using System.Net.Sockets;

using Newtonsoft.Json;
using System.Threading;

namespace focusify.Models
{
    public class ThinkgearController
    {
        TcpClient client;
        Stream stream;
        byte[] buffer = new byte[4096];
        int bytesRead;
        FixedSizedQueue<int> attentionBuffer;

        public ThinkgearController(FixedSizedQueue<int> attentionBuffer)
        {
            this.attentionBuffer = attentionBuffer;
        }

        public void InitConnection()
        {
            client = new TcpClient("127.0.0.1", 13854);
            stream = client.GetStream();

            System.Diagnostics.Debug.WriteLine("Sending configuration packet to device.");

            var com = @"{""enableRawOutput"": false, ""format"": ""Json""}";
            byte[] myWriteBuffer = Encoding.ASCII.GetBytes(com);
            stream.Write(myWriteBuffer, 0, myWriteBuffer.Length);
        }

        public void CollectData()
        {
            System.Diagnostics.Debug.WriteLine("Starting data collection.");
            while (true)
            {
                bytesRead = stream.Read(buffer, 0, 4096);
                string[] packets = Encoding.UTF8.GetString(buffer, 0, bytesRead).Split('\r');

                foreach (string s in packets)
                {
                    if (String.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(s);
                        //System.Diagnostics.Debug.WriteLine(data);

                        if (data["status"] != null)
                        {
                            //System.Diagnostics.Debug.WriteLine("Device status: " + data["status"]);
                        }
                        if (data["eSense"] != null)
                        {
                            attentionBuffer.Enqueue((int) data.eSense.attention);
                            //System.Diagnostics.Debug.WriteLine("attention: " + data.eSense.attention.ToString() + ", meditation: " + data.eSense.meditation.ToString() + ",0");
                            //System.Diagnostics.Debug.WriteLine(attentionBuffer.ToString());
                        }
                        if (data["blinkStrength"] != null)
                        {
                            //System.Diagnostics.Debug.WriteLine("blink: " + data.blinkStrength.ToString(), ",0");
                        }
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Error in data collection: " + e.Message);
                    }
                }
                Thread.Sleep(500);
            }
        }

    }
}
