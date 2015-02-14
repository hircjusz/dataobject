using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    public class DesingerGlobalInfo
    {
        public Size Size { get; set; }
        public List<String> AssemblyList { get; set; }

        public DesingerGlobalInfo()
        {
            this.Size = new Size();
            this.AssemblyList = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Size.Width, Size.Height,
                AssemblyList.Count > 0 ? String.Join(";", AssemblyList) : "");
        }

        public static explicit operator DesingerGlobalInfo(string strRepresentation)
        {
            DesingerGlobalInfo info = new DesingerGlobalInfo();
            string[] tokens = strRepresentation.Split(',');

            Size size = new Size();
            size.Width = int.Parse(tokens[0]);
            size.Height = int.Parse(tokens[1]);
            info.Size = size;
            info.AssemblyList = tokens[2].Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            return info;
        }
    }
}

