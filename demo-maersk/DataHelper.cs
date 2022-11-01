using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace demo.maersk
{
    public static class DataHelper
    {
        public static async Task<ShipmentsDto> LoadShipmentsData()
        {
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));

            return JsonConvert.DeserializeObject<ShipmentsDto>(
                await File.ReadAllTextAsync(Path.Combine(rootDirectory, "shipments.json")));
        }
    }
}
