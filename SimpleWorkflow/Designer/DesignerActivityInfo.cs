using System;
using System.Drawing;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Klasa przechowywuje ustawienia edytora dla akrywności
    /// </summary>
    [Serializable]
    public class DesignerActivityInfo
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="DesignerActivityInfo"/>.
        /// </summary>
        public DesignerActivityInfo()
        {
            this.Location = new Point();
            this.Size = new Size();
        }



        /// <summary>
        /// Pozwala pobrać i ustawić położenie na planszy
        /// </summary>
        /// <value>The location.</value>
        public Point Location { get; set; }

        /// <summary>
        /// Pozwala pobrać i ustawić rozmiar elementu
        /// </summary>
        /// <value>The size.</value>
        public Size Size { get; set; }




        /// <summary>
        /// Przeprowadza konwersję <see cref="System.String"/> na <see cref="SoftwareMind.SimpleWorkflow.Misc.DesignerActivityInfo"/>.
        /// </summary>
        /// <param name="strRepresentation">Łańcuch znaków do konwersji</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator DesignerActivityInfo(string strRepresentation)
        {
            DesignerActivityInfo info = new DesignerActivityInfo();
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

        /// <summary>
        /// Zwraca reprezentacje klasy <see cref="System.String"/> w postaci stringu.
        /// </summary>
        /// <returns>
        /// Instancja klasy <see cref="System.String"/>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0},{1},{2},{3}", Location.X, Location.Y, Size.Width, Size.Height);
        }
    }
}
