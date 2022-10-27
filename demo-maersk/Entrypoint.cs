using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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
        private static readonly IDictionary<string, Func<IntentRequest, Task<SkillResponse>>> IntentHandlers =
            new Dictionary<string, Func<IntentRequest, Task<SkillResponse>>>
            {
                {"AMAZON.HelpIntent", HelpIntent.Handler},
                {"shipmentStatus", ShipmentStatus.Handler},
            };

        private const string LaunchMessage = "Thank you for connecting with Maersk, you can say help anytime to listen to brief instructions on how to use this service";

        [FunctionName(nameof(Entrypoint))]
        public static async Task<SkillResponse> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] 
            HttpRequest req,
            ILogger log)
        {
            var requestBody = await req.ReadAsStringAsync();

            var skillRequest = JsonConvert.DeserializeObject<SkillRequest>(requestBody);

            log.LogInformation(skillRequest.Request.Type);

            var requestType = skillRequest.GetRequestType();

            if (requestType == typeof(LaunchRequest))
                return ResponseBuilder.Tell(LaunchMessage);

            if (requestType != typeof(IntentRequest)) return DontKnow();

            var intentRequest = skillRequest.Request as IntentRequest;

            log.LogInformation(intentRequest.Intent.Name);

            return IntentHandlers.TryGetValue(intentRequest.Intent.Name, out var handler)
                ? await handler.Invoke(intentRequest)
                : DontKnow();
        }
       

        private static SkillResponse DontKnow()
        {
            const string DontKnowMessage = "Sorry, I am not able to help you with this request";

            var dontKnow = new SsmlOutputSpeech(DontKnowMessage);

            return ResponseBuilder.TellWithCard(dontKnow, "Sorry :(", DontKnowMessage);
        }
    }
}
