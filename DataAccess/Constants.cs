using System.Text;
using System.Web;

namespace DataAccess
{
    class Constants
    {
        /// <summary>
        /// Adds a new encoded parameter to an string builder
        /// </summary>
        public static void AppendParameter(StringBuilder stringBuilder, string name, string value)
        {
            string encodedValue = HttpUtility.UrlEncode(value);
            stringBuilder.AppendFormat("{0}={1}", name, encodedValue);
        }
    }
}
