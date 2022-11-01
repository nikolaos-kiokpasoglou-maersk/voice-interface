using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demo.maersk.Intents
{
    public static class ShipmentStatus
    {
        private const string ResponseMessagePrefix = "Your Maersk shipment";

        private static readonly IList<string> Events = new List<string>() 
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

        private static readonly IDictionary<string, Func<Shipment, string>> EventResponseMap = new Dictionary<string, Func<Shipment, string>>
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

        public static async Task<SkillResponse> Handler(SkillRequest skillRequest)
        {
            var request = skillRequest.Request as IntentRequest;
            var shipmentNo = request?.Intent.Slots.FirstOrDefault(s => s.Key == "shipmentNo").Value?.Value;

            var session = skillRequest.Session;

            session.Attributes ??= new Dictionary<string, object>();

            session.Attributes["shipmentNo"] = shipmentNo;

            var shipmentStatus = shipmentNo is null 
                ? "Sorry, I did not recognize your shipment number, please try again."
                : await GetResponse(shipmentNo);

            var response = ResponseBuilder.Ask(
                new PlainTextOutputSpeech(shipmentStatus),
                shipmentNo is not null ? new Reprompt("Is there anything more you would like to know about this shipment?") : default,
                session);

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
            var data = await DataHelper.LoadShipmentsData();

            return data.Shipments.FirstOrDefault(x => x.ShipmentNo.EndsWith(shipmentNo));
        }

        private static string PickRandomEvent()
            => Events[new Random().Next(0, Events.Count)];
    }
}
