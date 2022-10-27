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
            var outputSpeech = new PlainTextOutputSpeech
            {
                Text = "Try saying, where is my maersk shipment one two five four five two"
            };

            return await Task.FromResult(ResponseBuilder.Tell(outputSpeech));
        }
    }
}
