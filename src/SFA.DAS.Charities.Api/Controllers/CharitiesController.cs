using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Charities.Data.Repositories;
using System.Threading.Tasks;

namespace SFA.DAS.Charities.Api.Controllers
{
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
            var charity = await _charityReadRepository.GetCharityById(registrationNumber);

            if (charity == null)
            {
                _logger.LogInformation("Charity with registration number: {registrationNumber} not found", registrationNumber);
                return new NotFoundObjectResult($"Charity with registration number: {registrationNumber} not found");
            }

            return new OkObjectResult(charity);
        }
    }
}
