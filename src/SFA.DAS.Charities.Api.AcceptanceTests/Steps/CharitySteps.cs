using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Charities.Api.AcceptanceTests.Infrastructure;
using SFA.DAS.Charities.Domain.Entities;
using TechTalk.SpecFlow;

namespace SFA.DAS.Charities.Api.AcceptanceTests.Steps
{
    [Binding]
    public class CharitySteps
    {
        private readonly ScenarioContext _context;

        public CharitySteps(ScenarioContext context)
        {
            _context = context;
        }

        [Given(@"I want to retrieve details for charity with registration number (.*)")]
        public void GivenIWantToRetriveDetailsForCharityWithRegistrationNumber(int registrationNumber)
        {
            _context.Set(registrationNumber, ContextKeys.RegistrationNumber);
        }

        [Then(@"the charity with registration number equal to (.*) is returned")]
        public async Task ThenTheCharityWithRegistrationNumberEqualToIsReturned(int registrationNumber)
        {
            if (!_context.TryGetValue<HttpResponseMessage>(ContextKeys.HttpResponse, out var result))
            {
                Assert.Fail($"scenario context does not contain value for key [{ContextKeys.HttpResponse}]");
            }

            var model = await HttpUtilities.ReadContent<Charity>(result.Content);
            var expected = DbUtilities.GetCharity(registrationNumber);

            model.Should().BeEquivalentTo(expected, options => options.Excluding(c => c.Trustees));
        }
    }
}
