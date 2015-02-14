using System.Drawing;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Klasa przechowywująca ustawienia globalne Workflowa dla edytora
    /// </summary>
    public class DesingerGlobalInfo
    {

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="DesingerGlobalInfo"/>.
        /// </summary>
        public DesingerGlobalInfo()
        {
            this.Size = new Size();
        }



        /// <summary>
        /// Pozwala pobrać i ustawić rozmiar dokumentu
        /// </summary>
        /// <value>The size.</value>
        public Size Size { get; set; }




        /// <summary>
        /// Konwertuje <see cref="System.String"/> na <see cref="SoftwareMind.SimpleWorkflow.Misc.DesingerGlobalInfo"/>.
        /// </summary>
        /// <param name="strRepresentation">Reprezentacja obiektu w postaci łańcucha znaków.</param>
        /// <returns>Wynik konwersji.</returns>
        public static explicit operator DesingerGlobalInfo(string strRepresentation)
        {
            DesingerGlobalInfo info = new DesingerGlobalInfo();
            string[] tokens = strRepresentation.Split(',');

            Size size = new Size();
            size.Width = int.Parse(tokens[0]);
            size.Height = int.Parse(tokens[1]);
            info.Size = size;

            return info;
        }

        /// <summary>
        /// Zwraca <see cref="System.String"/> reprezentujący dany obiekt.
        /// </summary>
        /// <returns>
        /// <see cref="System.String"/> reprezentujący dany obiekt .
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0},{1}", Size.Width, Size.Height);
        }
    }
}

