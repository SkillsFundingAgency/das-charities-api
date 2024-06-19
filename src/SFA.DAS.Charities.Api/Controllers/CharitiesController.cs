﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using System.Threading.Tasks;

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
                return new BadRequestObjectResult(new {Error = "RegistrationNumber is expected to have a positive non-zero value."});
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
        [Route("search/{searchTerm}")]
        public async Task<IActionResult> SearchCharities(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                return new BadRequestObjectResult(new { Error = "SearchTerm is empty" });
            }

            var charities = await _charityReadRepository.SearchCharities(searchTerm);

            if (charities == null || charities.Count == 0)
            {
                _logger.LogInformation("Not Charities found with search term: {searchTerm}", searchTerm);
                return NotFound("No charities found matching the search criteria.");
            }

            _logger.LogInformation("Found {count} charities matching:search term {searchTerm}", charities.Count, searchTerm);
            return new OkObjectResult(charities);
        }
    }
}
