using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saas_Car_Management.Core.Interfaces;

namespace Saas_Car_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DriverAppController : BaseApiController
    {
        private readonly IDriverAppRepository _repository;

        public DriverAppController(IDriverAppRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeData()
        {
            var data = await _repository.GetHomeDataAsync(GetUserId());
            if (data == null) return Unauthorized("Not registered as a Driver.");

            return Ok(data);
        }

        [HttpGet("live")]
        public async Task<IActionResult> GetLiveRides()
        {
            var rides = await _repository.GetLiveRidesAsync(GetUserId());
            return Ok(rides);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryRides()
        {
            var history = await _repository.GetHistoryRidesAsync(GetUserId());
            return Ok(history);
        }

        [HttpPost("attendance/punch")]
        public async Task<IActionResult> PunchAttendance()
        {
            var result = await _repository.PunchAttendanceAsync(GetUserId());
            if (result == null) return Unauthorized("Not registered as a Driver.");

            if (result.Message.Contains("Already punched out"))
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
    }
}
