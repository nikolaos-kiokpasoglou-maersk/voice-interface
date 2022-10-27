using Alexa.NET;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace demo.maersk.Intents
{
    public static class ShipmentStatus
    {
        private const string ResponseMessagePrefix = "Your Maersk shipment";

        private static IList<string> Events = new List<string>() 
        {
            "Loaded",
            "Sailing",
            "Reached Destination",
            "UnLoaded",
            "Gated-In",
            "Gated-Out",
            "Enroute to delivery",
            "Delivered",
            "Transport Plan Changed"
        };

        private static IDictionary<string, Func<Shipment, string>> EventResponseMap = new Dictionary<string, Func<Shipment, string>>
        {
            { Events[0], x => $"{ResponseMessagePrefix} {x.ShipmentNo} is now loaded in {x.Origin}." },
            { Events[1], x => $"{ResponseMessagePrefix} {x.ShipmentNo} is now sailing from {x.Origin}." },
            { Events[2], x => $"{ResponseMessagePrefix} {x.ShipmentNo} has now reached destination in {x.ImportCountry}." },
            { Events[3], x => $"{ResponseMessagePrefix} {x.ShipmentNo} has now unloaded in {x.ImportCountry}." },
            { Events[4], x => $"{ResponseMessagePrefix} {x.ShipmentNo} has now gated in {x.ImportCountry}." },
            { Events[5], x => $"{ResponseMessagePrefix} {x.ShipmentNo} has now gated out in {x.ImportCountry}." },
            { Events[6], x => $"{ResponseMessagePrefix} {x.ShipmentNo} is now enroute to delivery." },
            { Events[7], x => $"{ResponseMessagePrefix} {x.ShipmentNo} has now delivered." },
            { Events[8], x => $"{ResponseMessagePrefix} {x.ShipmentNo} has transport plan changes, would you like to reach out to your Maersk representative for more details?" },
        };

        public static async Task<SkillResponse> Handler(IntentRequest request)
        {
            var shipmentNo = request?.Intent.Slots.FirstOrDefault(s => s.Key == "shipmentNo").Value?.Value;

            var shipmentStatus = shipmentNo is null 
                ? "Sorry, I did not recognize your shipment number, please try again."
                : await GetResponse(shipmentNo);

            var response = ResponseBuilder.Tell(new PlainTextOutputSpeech(shipmentStatus));

            response.Response.ShouldEndSession = false;
            
            return response;
        }

        private static async Task<string> GetResponse(string shipmentNo)
        {
            var shipment = await GetShipment(shipmentNo);

            return shipment is null
                ? "Sorry, I could not find any information for this shipment number, please try again."
                : EventResponseMap[PickRandomEvent()].Invoke(shipment);
        }

        private static async Task<Shipment> GetShipment(string shipmentNo)
        {
            var data = await LoadShipmentsData();

            return data.Shipments.FirstOrDefault(x => x.ShipmentNo.EndsWith(shipmentNo));
        }

        private static string PickRandomEvent()
            => Events[new Random().Next(0, Events.Count)];

        private static async Task<ShipmentsDto> LoadShipmentsData()
        {
            var binDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            
            var rootDirectory = Path.GetFullPath(Path.Combine(binDirectory, ".."));
            
            return JsonConvert.DeserializeObject<ShipmentsDto>(
                await File.ReadAllTextAsync(Path.Combine(rootDirectory, "shipments.json")));
        }
    }
}
