using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using SFA.DAS.Charities.Domain.Entities;

namespace SFA.DAS.Charities.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [ApiController]
    public class CharitiesController : ControllerBase
    {
        private readonly ICharitiesReadRepository _charityReadRepository;
        private readonly ILogger<CharitiesController> _logger;

        public CharitiesController(ICharitiesReadRepository charityReadRepository, ILogger<CharitiesController> logger)
        {
            _charityReadRepository = charityReadRepository;
            _logger = logger;
        }

        [HttpGet]
        [Route("{registrationNumber}")]
        public async Task<IActionResult> GetCharityDetails(int registrationNumber)
        {
            if (registrationNumber <= 0)
            {
                return new BadRequestObjectResult(new { Error = "RegistrationNumber is expected to have a positive non-zero value." });
            }

            var charity = await _charityReadRepository.GetCharityById(registrationNumber);

            if (charity == null)
            {
                _logger.LogInformation("Charity with registration number: {registrationNumber} not found", registrationNumber);
                return new NotFoundObjectResult($"Charity with registration number: {registrationNumber} not found");
            }

            _logger.LogInformation("Found charity with registration number: {registrationNumber} and name {charityName}", registrationNumber, charity.Name);
            return new OkObjectResult(charity);
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchCharities([FromQuery] string searchTerm, [FromQuery] int maximumResults = 500)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new OkObjectResult(new List<Charity>());
            }

            string decodedSearchTerm = HttpUtility.UrlDecode(searchTerm);

            var charities = await _charityReadRepository.SearchCharities(decodedSearchTerm, maximumResults);

            if (charities == null || charities.Count == 0)
            {
                _logger.LogInformation("No Charities found with search term: {searchTerm}", decodedSearchTerm);
                return NotFound("No charities found matching the search criteria.");
            }

            _logger.LogInformation("Found {count} charities matching:search term {searchTerm}", charities.Count, decodedSearchTerm);
            return new OkObjectResult(charities);
        }
    }
}
