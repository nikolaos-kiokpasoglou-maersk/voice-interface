using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET;
using System.Threading.Tasks;

namespace demo.maersk.IntentHandlers
{
    public static class StopIntent
    {
        public static async Task<SkillResponse> Handler(IntentRequest request)
        {
            var response = ResponseBuilder.Tell(
                new PlainTextOutputSpeech("Thank you for using Maersk facts."));

            return await Task.FromResult(response);
        }
    }
}
