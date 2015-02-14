using System.Text;

namespace SoftwareMind.Utils.Extensions.DoubleExtensions
{
    public static class DoubleExtensions
    {
        public static string ToString(this double value, int decimals)
        {
            StringBuilder format = new StringBuilder("0");
            if (decimals > 0)
            {
                format.Append(".");
            }
            for (int i = 0; i < decimals; i++)
            {
                format.Append("0");
            }
            return value.ToString(format.ToString());
        }
    }
}