using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProcessKiller
{
    public static class Program
    {
        private static Config _config = new Config();

        private static void Main()
        {
            try
            {
                var location = Assembly.GetExecutingAssembly().Location;
                if (location == null)
                    throw new ArgumentNullException(nameof(location));
                _config = ImportConfiguration(Path.Combine(Directory.GetParent(location).FullName, "Config.xml"));
                foreach (var p in _config.Processes)
                foreach (var process in System.Diagnostics.Process.GetProcessesByName(p.Name))
                    process.Kill();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private static Config ImportConfiguration(string fileName)
        {
            var xDocument = XDocument.Load(fileName);
            return CreateObjectFromString<Config>(xDocument);
        }

        private static T CreateObjectFromString<T>(XDocument xDocument)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T) xmlSerializer.Deserialize(new StringReader(xDocument.ToString()));
        }
    }
}