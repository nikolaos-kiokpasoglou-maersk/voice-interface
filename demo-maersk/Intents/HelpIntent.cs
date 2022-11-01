using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Alexa.NET;
using System.Threading.Tasks;

namespace demo.maersk.IntentHandlers
{
    public static class HelpIntent
    {
        public static async Task<SkillResponse> Handler(IntentRequest request)
        {
            var response = ResponseBuilder.Ask(
                new PlainTextOutputSpeech("Try asking, where is my maersk shipment, followed by a number."), default);

            return await Task.FromResult(response);
        }
    }
}
