using System;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

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

            string shipmentStatus;

            if (shipmentNo == null)
            {
                shipmentStatus = "Sorry, I did not recognize your shipment number, please try again.";
            }
            else
            {
                SendMail();
                shipmentStatus = GetResponse((string)shipmentNo, request?.Intent.ConfirmationStatus);
            }
            shipmentStatus = shipmentNo is null
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

        private static void SendJsonEmail()
        {

        }

        private static void SendMail()
        {
            var mailMessage = new MimeMessage();
            mailMessage.From.Add(new MailboxAddress("Captain Laura", ""));
            mailMessage.To.Add(new MailboxAddress("Maersk customer", "siah.derin@fallinhay.com"));
            mailMessage.Subject = "subject";
            mailMessage.Body = new TextPart("plain")
            {
                Text = "Hello"
            };

            using var smtpClient = new SmtpClient();
            smtpClient.Connect("smtp.gmail.com", 465, true);
            smtpClient.Authenticate("", "");
            try
            {
                smtpClient.Send(mailMessage);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            smtpClient.Disconnect(true);
        }
    }
}
