using FlightQuery.Sdk;
using System.IO;
using System.Reflection;

namespace FlightQuery.Tests
{
    public static class TestHelper
    {
        public static ExecuteResult LoadJson(string resource)
        {
            string source = string.Empty;
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(resource))
            using (StreamReader reader = new StreamReader(stream))
            {
                source = reader.ReadToEnd();
            }
            return new ExecuteResult() { Result = source };
        }
    }
}
