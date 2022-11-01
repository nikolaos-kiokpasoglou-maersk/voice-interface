using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demo.maersk.Intents
{
    public static class ShipmentNotify
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
                : GetResponse((string)shipmentNo, request?.Intent.ConfirmationStatus);

            var response = ResponseBuilder.Ask(
                new PlainTextOutputSpeech(shipmentStatus),
                shipmentNo is not null ? new Reprompt("Is there anything more I can help you with this shipment?") : default,
                session);

            return Task.FromResult(response);
        }

        private static string GetResponse(string shipmentNo, string confirmationStatus)
        {
            return confirmationStatus == "CONFIRMED"
                ? "I have set an alert for this shipment. I will notify you when a status change has happened."
                : "No alert has been set for this shipment.";
        }
    }
}
