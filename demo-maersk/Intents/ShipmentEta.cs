using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Bogus;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demo.maersk.Intents
{
    public static class ShipmentEta
    {
        private const string ShipmentNoKey = "shipmentNo";

        public static Task<SkillResponse> Handler(SkillRequest skillRequest)
        {
            var session = skillRequest.Session;
            session.Attributes.TryGetValue(ShipmentNoKey, out var shipmentNo);

            var request = skillRequest.Request as IntentRequest;
            shipmentNo ??= request?.Intent.Slots.FirstOrDefault(s => s.Key == ShipmentNoKey).Value?.Value;

            session.Attributes ??= new Dictionary<string, object>();

            session.Attributes[ShipmentNoKey] = shipmentNo;

            var shipmentStatus = shipmentNo is null
                ? "Sorry, I did not recognize your shipment number, please try again."
                : GetResponse((string)shipmentNo);

            var response = ResponseBuilder.Ask(
                new PlainTextOutputSpeech(shipmentStatus),
                shipmentNo is not null ? new Reprompt("Is there anything more you would like to know about this shipment?") : default,
                session);

            return Task.FromResult(response);
        }

        private static string GetResponse(string shipmentNo)
        {
            var eta = new Faker().Date.SoonOffset(15);

            return shipmentNo is null
                ? "Sorry, I could not find any information for this shipment number, please try again."
                : $"Your shipment ETA is {eta}";
        }
    }
}
