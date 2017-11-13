using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AIMLBot
{
    static class Extension
    {
        public static string ToUpper(this string value, CultureInfo culture)
        {
#if !NETFX_CORE
            return value.ToUpper(culture);
#else
            return value.ToUpper();
#endif
        }
        public static string ToLower(this string value, CultureInfo culture)
        {
#if !NETFX_CORE
            return value.ToLower(culture);
#else
            return value.ToLower();
#endif
        }

        public static void Load(this XmlDocument xmlDoc, string path)
        {
            Stream stream = File.OpenRead(path);
            xmlDoc.Load(stream);
        }
    }
}
