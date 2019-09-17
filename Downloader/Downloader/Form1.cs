using MasterLibrary.Bindable;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using MasterLibrary.Threading;
using MasterLibrary.Extentions;

namespace Downloader
{
    public partial class Form1 : Form
    {
        WorkerPool<LinkGrabber, Uri> GrabberPool;
        WorkerPool<Downloader, DLPar> DLPool;
        public Form1()
        {
            InitializeComponent();
            GrabberPool = new WorkerPool<LinkGrabber, Uri>(1);
            DLPool = new WorkerPool<Downloader, DLPar>(1);
            listBox1.DataSource = DLPool._Workers;
            listBox2.DataSource = DLPool._Work;
            listBox3.DataSource = GrabberPool._Work;
            GrabberPool.OnWorkerDone += Pool_OnWorkerDone;
            treeView1.PathSeparator = "/";
        }


        private void Pool_OnWorkerDone(Uri work, object result)
        {
            this.Text = GrabberPool.WorkCount.ToString();

            List<string> links = result as List<string>;

            foreach (string link in links)
            {
                string fLink = work.ToString() + link.TrimStart('/');

                int ind = treeView1.Nodes.FindIndex(n => fLink.StartsWith(n.Text));

                if (fLink.EndsWith("/"))
                    GrabberPool.AddWork(new Uri(fLink));

                if (ind != -1)
                    treeView1.Nodes[ind].AddPath(fLink.TrimEnd('/'));

            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {

            var checkedNodes = from n in treeView1.Nodes.Descendants()
                               where n.Checked
                               where n.Nodes.Count == 0
                               where Path.GetExtension(n.Tag as string) != ""
                               select n;

            foreach (TreeNode n in checkedNodes)
            {
                DLPar par = new DLPar();
                par.Source = new Uri(n.Tag as string);
                par.Dest = (n.Tag as string).Replace(textBox1.Text, @"C:\a\" + par.Source.Host + "\\");
                DLPool.AddWork(par);
            }

        }

        public static int Dig(int i, int dig)
        {
            return (i / (int)Math.Pow(10, dig - 1)) % 10;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Add(textBox1.Text);
            GrabberPool.AddWork(new Uri(textBox1.Text));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void TreeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            SetChildrenChecked(e.Node, e.Node.Checked);

        }

        private void SetChildrenChecked(TreeNode treeNode, bool checkedState)
        {
            foreach (TreeNode item in treeNode.Nodes)
                item.Checked = checkedState;
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            DLPool._Workers.Add(new Downloader());
        }
    }


    public static class H
    {
        private static readonly Dictionary<int, string> byteFix = new Dictionary<int, string>()
        {
            { 0, "B" },
            { 1, "kB" },
            { 2, "MB" },
            { 3, "GB" },
            { 4, "TB" }
        };


        public static string ToHRBytes(double n)
        {
            int div = 1000;
            double log = Math.Log(n, div);
            int mag = (int)(log < 0 ? Math.Ceiling(log) : Math.Floor(log));
            double rslt = n / Math.Pow(div, mag);
            if (!byteFix.ContainsKey(mag))
                return 0 + byteFix[0];
            return rslt.ToString("0.000") + byteFix[mag];
        }

    }

}
