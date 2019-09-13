using MasterLibrary.Bindable;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Downloader
{
    public partial class Form1 : Form
    {
        private ObservableConcurrentQueue<string> LinksToGrab = new ObservableConcurrentQueue<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            LinksToGrab.Enqueue(textBox1.Text);
        }
    }


    public class WorkerPool
    {
        private static Semaphore _AvailableWorkers
    }

    public class Worker
    {

    }

}
