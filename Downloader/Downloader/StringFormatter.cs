using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Downloader
{
    public class StringFormatter
    {
        private static List<int> lengths = new List<int>();
        public string Columns(params string[] list)
        {
            StringBuilder sb = new StringBuilder();
            for(int i=0; i< list.Length; i++)
            {
                if (i < lengths.Count)
                {
                    if (list[i].Length > lengths[i])
                        lengths[i] = list[i].Length;
                }
                else
                    lengths.Add(list[i].Length);


                sb.Append(list[i] + new string(' ', lengths[i] - list[i].Length + 4));
            }
            return sb.ToString();
        }
    }

}
