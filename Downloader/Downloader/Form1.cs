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
using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace Downloader
{
    public partial class Form1 : Form
    {
        WorkerPool<LinkGrabber, Uri> pool;
        public Form1()
        {
            InitializeComponent();
            pool = new WorkerPool<LinkGrabber, Uri>(1);
            pool.OnWorkerDone += Pool_OnWorkerDone;
            treeView1.PathSeparator = "/";


        }


        private void Pool_OnWorkerDone(Uri work, object result)
        {
            this.Text = pool.WorkCount.ToString();

            List<string> links = result as List<string>;

            foreach (string link in links)
            {
                string fLink = work.ToString() + link.TrimStart('/');

                int ind = treeView1.Nodes.FindIndex(n => fLink.StartsWith(n.Text));

                if (fLink.EndsWith("/"))
                    pool.AddWork(new Uri(fLink));

                if (ind != -1)
                    treeView1.Nodes[ind].AddPath(fLink.TrimEnd('/'));

            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {


        }

        public static int Dig(int i, int dig)
        {
            return (i / (int)Math.Pow(10, dig - 1)) % 10;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Add(textBox1.Text);
            pool.AddWork(new Uri(textBox1.Text));
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

    }
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

 


   

    public static class H
    {
        public static void AddPath(this TreeNode tn, string path, char seperator = '/')
        {
            string relPath = path.Replace(tn.Text, "").TrimStart(seperator);
            string[] split = relPath.Split(seperator);


            TreeNode curNode = tn;
            for (int i = 0; i < split.Length; i++)
            {

                int ind = curNode.Nodes.FindIndex(n => n.Text == split[i]);
                if (ind == -1)
                {
                    TreeNode newNode = new TreeNode(split[i]);
                    curNode.Nodes.Add(newNode);
                    curNode = newNode;
                }
                else
                {
                    curNode = curNode.Nodes[ind];
                }
            }
        }

        static public int FindIndex(this TreeNodeCollection col, Predicate<TreeNode> predicate)
        {
            for (int i = 0; i < col.Count; i++)
            {
                if (predicate(col[i]))
                    return i;
            }
            return -1;
        }
    }
}
