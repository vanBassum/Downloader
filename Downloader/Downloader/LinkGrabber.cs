using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using MasterLibrary.Threading;

namespace Downloader
{
    public class LinkGrabber : AWorker<Uri>
    {
        public override event WorkDone<Uri> OnWorkerDone;
        private WebClient client = new WebClient();

        public LinkGrabber()
        {
            
        }

        protected override void StartWorker(Uri parameter)
        {
            client = new WebClient();
            client.DownloadStringCompleted += Client_DownloadStringCompleted;
            client.DownloadStringAsync(parameter);
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            List<string> links = new List<string>();
            if (e.Error != null)
            {
                OnWorkerDone?.Invoke(this.Work, links);
                return;
            }


            MatchCollection mc = Regex.Matches(e.Result, "href *= *[\"'`](.+?)[\"'`]");
           
            foreach(Match m in mc)
            {
                if(m.Groups.Count > 0)
                {
                    if (!m.Groups[1].Value.StartsWith("?"))
                    {
                        if (!m.Groups[1].Value.StartsWith("/"))
                        {
                            links.Add(m.Groups[1].Value);
                        }
                        else
                        {

                        }

                    }
                    else
                    {

                    }

                }
            }

            OnWorkerDone?.Invoke(this.Work, links);
        }
    }
}
