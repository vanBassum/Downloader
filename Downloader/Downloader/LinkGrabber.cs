using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using MasterLibrary.Threading;

namespace Downloader
{
    public class LinkGrabber : AWorker<Uri>, INotifyPropertyChanged
    {
        public override event WorkDone<Uri> OnWorkerDone;
        public event PropertyChangedEventHandler PropertyChanged;
        bool working = false;
        private ExtendedWebClient client = new ExtendedWebClient();

        public LinkGrabber()
        {
            
        }

        protected override void StartWorker(Uri parameter)
        {
            client = new ExtendedWebClient();
            client.DownloadStringCompleted += Client_DownloadStringCompleted;
            client.DownloadStringAsync(parameter);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Work"));
            working = true;
        }

        public override string ToString()
        {
            return working?Work.ToString():"idle";
        }

        private void Client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            List<string> links = new List<string>();
            if (e.Error != null)
            {
                OnWorkerDone?.Invoke(this.Work, links);
                working = false;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Work"));
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
                    }
                }
            }

            OnWorkerDone?.Invoke(this.Work, links);
            working = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Work"));
        }
    }
}
