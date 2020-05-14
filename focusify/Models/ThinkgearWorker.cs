using System;
using System.IO;
using System.Threading;

namespace focusify.Models
{
    public class ThinkgearWorker
    {
        /* TODO implement reading from neurosky mindwave here
         * - spotify API call (changing music)
         * - volume up/down - depends on above (question - volume of which app/device)
         * - implement core logic for reacting to user focus, using ThinkgearController and functions above
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
                //TODO
            }
            catch (Exception ex)
            {
                ProcessCancellation();
            }
        }

        private void setVolume(int level)
        {
            // TODO
        }

        private void play(string songId)
        {
            // TODO
        }

        private void ProcessCancellation()
        {

        }
    }
}