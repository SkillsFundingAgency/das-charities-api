using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Charities.Api.AcceptanceTests.Infrastructure;
using TechTalk.SpecFlow;

namespace SFA.DAS.Charities.Api.AcceptanceTests.Steps
{
    [Binding]
    public class HttpSteps
    {
        private readonly ScenarioContext _context;

        public HttpSteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given("I have a HTTP client")]
        public void GivenIHaveAHttpClient()
        {
            var client = new AcceptanceTestingWebApplicationFactory<Startup>().CreateClient();
            client.DefaultRequestHeaders.Add("X-Version", "1");
            _context.Set(client, ContextKeys.HttpClient);
        }

        [When(@"I request the following url: (.*)")]
        public async Task WhenIGETTheFollowingUrl(string url)
        {
            var client = _context.Get<HttpClient>(ContextKeys.HttpClient);
            var registrationNumber = _context.Get<int>(ContextKeys.RegistrationNumber);
            var uri = $"/api/{url}/{registrationNumber}";
            var response = await client.GetAsync(uri);
            _context.Set(response, ContextKeys.HttpResponse);
        }

        [Then(@"a response with HTTP status code of (.*) is received")]
        public void ThenAResponseWithHTTPStatusCodeOfIsReceived(int httpStatusCode)
        {
            if (!_context.TryGetValue<HttpResponseMessage>(ContextKeys.HttpResponse, out var result))
            {
                Assert.Fail($"scenario context does not contain value for key [{ContextKeys.HttpResponse}]");
            }

            result.StatusCode.Should().Be((HttpStatusCode)httpStatusCode);
        }
    }
}
