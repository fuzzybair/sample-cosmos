using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace TelemetryGenerator
{
  [Route("api/[controller]")]
  public class TelemeryController : ControllerBase
  {
    private readonly ApplicationDbContext _db;

    public TelemeryController(ApplicationDbContext db)
    {
      _db = db;
    }

    [HttpGet]
    public IActionResult Get()
    {
      var list = _db.Telemetry.ToList();
      return Ok(list);
    }

    [HttpPost]
    public IActionResult Post([FromBody] TelemetryData telemetryData)
    {
      _db.Telemetry.Add(telemetryData);
      _db.SaveChanges();

      return Ok(new { Message = "Telemetry data received and persisted", TelemetryData = telemetryData });
    }
  }
}