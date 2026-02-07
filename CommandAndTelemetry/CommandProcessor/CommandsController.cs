using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CommandProcessor
{
  [Route("api/[controller]")]
  public class CommandsController : ControllerBase
  {
    private readonly ApplicationDbContext _db;

    public CommandsController(ApplicationDbContext db)
    {
      _db = db;
    }

    [HttpPost]
    public IActionResult Post([FromBody] Command command)
    {
      _db.Commands.Add(command);
      _db.SaveChanges();

      return Ok(new { Message = "Command received and persisted", Command = command });
    }

    [HttpGet]
    public IActionResult Get()
    {
      var list = _db.Commands.ToList();
      return Ok(list);
    }
  }
}
