using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using MasterLibrary.Threading;

namespace Downloader
{
    public class Downloader : AWorker<DLPar>, INotifyPropertyChanged
    {
        public static StringFormatter stringColumn = new StringFormatter();
        public override event WorkDone<DLPar> OnWorkerDone;
        public event PropertyChangedEventHandler PropertyChanged;
        public double DownloadSpeed { get; private set; }
        public long BytesRecieved { get; private set; }
        public long BytesTotal { get; private set; }
        public int Progress { get; private set; }
        private ExtendedWebClient wc;
        private DateTime lastUpdate;
        protected override void StartWorker(DLPar parameter)
        {
            wc = new ExtendedWebClient();
            wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
            wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
            if (!Directory.Exists(Path.GetDirectoryName(parameter.Dest)))
                Directory.CreateDirectory(Path.GetDirectoryName(parameter.Dest));
            wc.DownloadFileAsync(parameter.Source, parameter.Dest);
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {

            if (BytesRecieved == 0)
            {
                lastUpdate = DateTime.Now;
                BytesRecieved = e.BytesReceived;
            }
            else
            {
                var now = DateTime.Now;
                var timeSpan = now - lastUpdate;
                var bytesChange = e.BytesReceived - BytesRecieved;
                if (timeSpan.Seconds != 0)
                {
                    var bytesPerSecond = bytesChange / timeSpan.Seconds;

                    BytesRecieved = e.BytesReceived;
                    lastUpdate = now;
                    DownloadSpeed = bytesPerSecond;
                }
            }

            Progress = e.ProgressPercentage;
            BytesRecieved = e.BytesReceived;

            if (BytesTotal != e.TotalBytesToReceive)
            {
                BytesTotal = e.TotalBytesToReceive;
                PropertyChanged(this, new PropertyChangedEventArgs("TotalBytesToReceive"));
            }

            PropertyChanged(this, new PropertyChangedEventArgs("DownloadSpeed"));
            PropertyChanged(this, new PropertyChangedEventArgs("BytesRecieved"));
            PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            OnWorkerDone?.Invoke(this.Work, null);
            Progress = 0;
            DownloadSpeed = 0;
            BytesRecieved = 0;
            BytesTotal = 0;
            PropertyChanged(this, new PropertyChangedEventArgs("DownloadSpeed"));
            PropertyChanged(this, new PropertyChangedEventArgs("BytesRecieved"));
            PropertyChanged(this, new PropertyChangedEventArgs("Progress"));
            PropertyChanged(this, new PropertyChangedEventArgs("TotalBytesToReceive"));
        }

        public override string ToString()
        {
            string totSize = H.ToHRBytes(BytesTotal);
            string recSize = H.ToHRBytes(BytesRecieved);
            string speed = H.ToHRBytes(DownloadSpeed) + "/s";

            return stringColumn.Columns(Progress.ToString() + "%", recSize + " / " + totSize, speed);
        }
    }

}
