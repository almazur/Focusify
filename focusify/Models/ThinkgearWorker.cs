
﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace focusify.Models
{
    public class ThinkgearWorker
    {
        private TokenInfo tokenInfo;

        public ThinkgearWorker(TokenInfo tokenInfo)
        {
            this.tokenInfo = tokenInfo;
        }

        public void StartProcessing(CancellationToken cancellationToken = default)
        {
            try
            {
                var attentionBuffer = new FixedSizedQueue<int>(20);
                var thinkgearController = new ThinkgearController(attentionBuffer);
                var volumeController = new MusicController(attentionBuffer, tokenInfo);
                var playlistController = new MusicController(attentionBuffer, tokenInfo);

                thinkgearController.InitConnection();

                Task task1 = Task.Factory.StartNew(() => thinkgearController.CollectData());
                Task task2 = Task.Factory.StartNew(() => volumeController.ControlVolume());
                Task taks3 = Task.Factory.StartNew(() => playlistController.ControlPlaylist());
            }
            catch (Exception ex)
            {
                ProcessCancellation();
            }
        }
        private void ProcessCancellation()
        {

        }
    }
}