using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET;
using System.Threading.Tasks;
using Alexa.NET.Request;

namespace demo.maersk.IntentHandlers
{
    public static class StopIntent
    {
        public static async Task<SkillResponse> Handler(SkillRequest skillRequest)
        {
            var response = ResponseBuilder.Tell(
                new PlainTextOutputSpeech("Thank you for using this Maersk service."));

            return await Task.FromResult(response);
        }
    }
}
