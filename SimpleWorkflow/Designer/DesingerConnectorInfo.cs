using System;
using System.Drawing;
using System.Linq;

namespace SoftwareMind.SimpleWorkflow.Designer
{
    /// <summary>
    /// Klasa przechowywująca ustawienia edytora dotyczące połączeń
    /// </summary>
    [Serializable]
    public class DesingerConnectorInfo
    {
        /// <summary>
        /// Inicjalizuje obiekt klasy <see cref="DesingerConnectorInfo"/>.
        /// </summary>
        public DesingerConnectorInfo ()
        {
            ControlPoints = new Point[0];
        }

        /// <summary>
        /// Pozwala pobrać i ustawić kolekcje punktw opisujczch połączenie.
        /// </summary>
        /// <value>The control points.</value>
        public Point[] ControlPoints { get; set; }

        /// <summary>
        /// Zwraca <see cref="System.String"/>, który reprezentuję klasę.
        /// </summary>
        /// <returns>
        /// <see cref="System.String"/> reprezentujący klasę.
        /// </returns>
        public override string ToString()
        {
            return String.Join(";", ControlPoints.Select(x => new int[] { x.X, x.Y }).SelectMany(x => x));
        }

        /// <summary>
        /// Konwertuje <see cref="System.String"/> na <see cref="DesingerConnectorInfo"/>.
        /// </summary>
        /// <param name="strRepresentation">Repreyentacja w postaci a}cuch ynakw.</param>
        /// <returns>Wznik konwersji</returns>
        public static explicit operator DesingerConnectorInfo(string strRepresentation)
        {
            DesingerConnectorInfo info = new DesingerConnectorInfo();

            int[] tab = strRepresentation.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            Point[] points = new Point[tab.Length / 2];
            for (int i = 0; i < points.Length; i++)
                points[i] = new Point(tab[i * 2], tab[i * 2 + 1]);

            info.ControlPoints = points;

            return info;
        }
    }
}
