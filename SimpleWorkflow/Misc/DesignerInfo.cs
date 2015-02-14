using System.Drawing;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    public class DesignerInfo
    {
        public Point Location { get; set; }
        public Size Size { get; set; }

        public DesignerInfo()
        {
            this.Location = new Point();
            this.Size = new Size();
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Location.X, Location.Y, Size.Width, Size.Height);
        }

        public static explicit operator DesignerInfo(string strRepresentation)
        {
            DesignerInfo info = new DesignerInfo();
            string[] tokens = strRepresentation.Split(',');
            
            Point location = new Point();
            Size size = new Size();
            location.X = int.Parse(tokens[0]);
            location.Y = int.Parse(tokens[1]);
            size.Width = int.Parse(tokens[2]);
            size.Height = int.Parse(tokens[3]);

            info.Size = size;
            info.Location = location;

            return info;
        }
    }
}
