using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverPortalController : ControllerBase
    {
        private readonly IDriverPortalRepository _repository;

        public DriverPortalController(IDriverPortalRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{token}")]
        public async Task<IActionResult> GetTripDetails(string token)
        {
            var trip = await _repository.GetTripByTokenAsync(token);
            if (trip == null) return NotFound("Invalid or expired magic link.");
            return Ok(trip);
        }

        [HttpPost("{token}/start")]
        public async Task<IActionResult> StartTrip(string token)
        {
            var success = await _repository.StartTripAsync(token);
            if (!success) return BadRequest("Could not start trip. Invalid token or booking already started.");
            return Ok(new { message = "Trip started successfully." });
        }
    }
}
