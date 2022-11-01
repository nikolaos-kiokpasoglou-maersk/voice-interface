using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using System.Collections.Generic;
using Alexa.NET;
using demo.maersk.IntentHandlers;
using demo.maersk.Intents;
using Alexa.NET.Request;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace demo.Maersk
{
    public static class Entrypoint
    {
        private static readonly IDictionary<string, Func<SkillRequest, Task<SkillResponse>>> IntentHandlers =
            new Dictionary<string, Func<SkillRequest, Task<SkillResponse>>>
            {
                {"AMAZON.HelpIntent", HelpIntent.Handler},
                {"AMAZON.StopIntent", StopIntent.Handler},
                {"shipmentStatus", ShipmentStatus.Handler},
                {"shipmentETA", ShipmentEta.Handler},
            };

        private const string LaunchMessage = "Thank you for connecting with Maersk, you can say help anytime to listen to brief instructions on how to use this service";

        [FunctionName(nameof(Entrypoint))]
        public static async Task<SkillResponse> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] 
            HttpRequest req)
        {
            var requestBody = await req.ReadAsStringAsync();

            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(requestBody);

            var requestType = skillRequest.GetRequestType();

            if (requestType == typeof(LaunchRequest))
            {
                var response = ResponseBuilder.Ask(
                    new PlainTextOutputSpeech(LaunchMessage),
                    DefaultReprompt());

                return response;
            }

            if (requestType != typeof(IntentRequest)) return DontKnow();

            var intentRequest = skillRequest.Request as IntentRequest;

            return IntentHandlers.TryGetValue(intentRequest.Intent.Name, out var handler)
                ? await handler.Invoke(skillRequest)
                : DontKnow();
        }

        private static SkillResponse DontKnow()
        {
            const string DontKnowMessage = "Sorry, I am not able to help you with this request";

            return ResponseBuilder.Ask(new PlainTextOutputSpeech(DontKnowMessage), DefaultReprompt());
        }

        private static Reprompt DefaultReprompt()
        {
            return new Reprompt("Try asking, where is my maersk shipment, followed by a number.");
        }
    }
}
