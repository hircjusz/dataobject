using System.Drawing;
using SoftwareMind.SimpleWorkflow.Designer;
using System;

namespace SoftwareMind.SimpleWorkflow.Misc
{
    /// <summary>
    /// Klasa przechowywująca ustawienia globalne Workflowa dla edytora
    /// </summary>
    /// 
    [Serializable]
    public class DesignerGlobalInfo
    {
        /// <summary>
        /// Pozwala pobrać i ustawić rozmiar dokumentu
        /// </summary>
        /// <value>The size.</value>
        public Size Size { get; set; }

        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="DesignerGlobalInfo"/>.
        /// </summary>
        public DesignerGlobalInfo()
        {
            this.Size = new Size();
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

        /// <summary>
        /// Konwertuje <see cref="System.String"/> na <see cref="SoftwareMind.SimpleWorkflow.Misc.DesignerGlobalInfo"/>.
        /// </summary>
        /// <param name="strRepresentation">Reprezentacja obiektu w postaci łańcucha znaków.</param>
        /// <returns>Wynik konwersji.</returns>
        public static explicit operator DesignerGlobalInfo(string strRepresentation)
        {
            DesignerGlobalInfo info = new DesignerGlobalInfo();
            string[] tokens = strRepresentation.Split(',');

            Size size = new Size();
            size.Width = int.Parse(tokens[0]);
            size.Height = int.Parse(tokens[1]);
            info.Size = size;

            return info;
        }
    }
}

