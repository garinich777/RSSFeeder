using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace RSSFeeder.Models
{
    public static class ListConverter
    {
        public static string GetJSON(List<Uri> uris)
        {
            return JsonSerializer.Serialize<List<Uri>>(uris);
        }

        public static List<Uri> GetList(string uris)
        {
            if (uris == string.Empty || uris == null)
                return new List<Uri>();
            else
                return JsonSerializer.Deserialize<List<Uri>>(uris);
        }
    }
}
