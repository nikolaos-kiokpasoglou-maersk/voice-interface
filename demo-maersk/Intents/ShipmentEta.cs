using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace demo.maersk.Intents
{
    public static class ShipmentEta
    {
        public static async Task<SkillResponse> Handler(SkillRequest skillRequest)
        {
            var session = skillRequest.Session;
            session.Attributes.TryGetValue("shipmentNo", out var shipmentNo);

            var request = skillRequest.Request as IntentRequest;
            shipmentNo ??= request?.Intent.Slots.FirstOrDefault(s => s.Key == "shipmentNo").Value?.Value;

            session.Attributes ??= new Dictionary<string, object>();

            session.Attributes["shipmentNo"] = shipmentNo;

            var shipmentStatus = shipmentNo is null
                ? "Sorry, I did not recognize your shipment number, please try again."
                : await GetResponse((string)shipmentNo);

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
                : $"Your shipment ETA is {shipment.BookingTimestamp}";
        }

        private static async Task<Shipment> GetShipment(string shipmentNo)
        {
            var data = await DataHelper.LoadShipmentsData();

            return data.Shipments.FirstOrDefault(x => x.ShipmentNo.EndsWith(shipmentNo));
        }
    }
}
