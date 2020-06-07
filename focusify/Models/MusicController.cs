using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace focusify.Models
{
    public class MusicController
    {
        private SpotifyController spotifyController;
        private FixedSizedQueue<int> attentionBuffer;

        private static AppSettings appSettings = AppSettings.Get();
        private string calmPlayList = appSettings.Focused;
        private string absorbingPlayList = appSettings.NotFocused;

        private bool setNextSong = true;

        public MusicController(FixedSizedQueue<int> attentionBuffer, TokenInfo tokenInfo)
        {
            this.attentionBuffer = attentionBuffer;
            spotifyController = new SpotifyController(tokenInfo);
        }

        public void ControlVolume()
        {           
            while (true)
            {
                if (attentionBuffer.Take(5).Length != 0)
                {
                    var volume = 100 - (int)attentionBuffer.Take(5).Average();
                    spotifyController.setVolume(volume);
                    //System.Diagnostics.Debug.WriteLine("> volume: " + volume);
                    Thread.Sleep(500);
                }
            }
        }      

        public void ControlPlaylist()
        {
            var calmPlaylistsItems = new Queue<string>(spotifyController.getPlaylistsItems(calmPlayList));
            var absorbingPlaylistsItems = new Queue<string>(spotifyController.getPlaylistsItems(absorbingPlayList));

            while (true)
            {
                if (attentionBuffer.Take(5).Length != 0)
                {
                    var timeToEnd = spotifyController.getTimeToEnd();

                    if (!setNextSong && timeToEnd >= 6000)
                        setNextSong = true;

                    if (setNextSong && timeToEnd <= 4000 && attentionBuffer.Take(10).Length != 0)
                    {
                        var attention = attentionBuffer.Take(10).Average();

                        var item = attention <= 50.0 ? calmPlaylistsItems.Dequeue() : absorbingPlaylistsItems.Dequeue();
                        spotifyController.AddItemToPlaybackQueue(item);
                        //System.Diagnostics.Debug.WriteLine(">> addToQueue: " + item);
                        setNextSong = false;

                        if (calmPlaylistsItems.Count == 0)
                            calmPlaylistsItems = new Queue<string>(spotifyController.getPlaylistsItems(calmPlayList));

                        if (absorbingPlaylistsItems.Count == 0)
                            absorbingPlaylistsItems = new Queue<string>(spotifyController.getPlaylistsItems(absorbingPlayList));
                    }
                    //System.Diagnostics.Debug.WriteLine("> timeToEnd: " + timeToEnd);
                    Thread.Sleep(500);
                }
            }
        }
    }
}