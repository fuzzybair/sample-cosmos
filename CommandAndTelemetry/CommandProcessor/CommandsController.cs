using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Collections.Generic;

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
    public IActionResult Post([FromBody] JsonElement payload)
    {
      string packetName = null;
      if (payload.ValueKind == JsonValueKind.Object)
      {
        if (payload.TryGetProperty("PacketName", out var p) || payload.TryGetProperty("packetName", out p) || payload.TryGetProperty("packet_name", out p))
        {
          try { packetName = p.GetString(); } catch { packetName = p.ToString(); }
        }
      }

      var parameters = new Dictionary<string, string>();

      JsonElement fieldsEl;
      if (payload.ValueKind == JsonValueKind.Object && (payload.TryGetProperty("Fields", out fieldsEl) || payload.TryGetProperty("fields", out fieldsEl)))
      {
        if (fieldsEl.ValueKind == JsonValueKind.Object)
        {
          foreach (var prop in fieldsEl.EnumerateObject())
          {
            parameters[prop.Name] = prop.Value.ToString();
          }
        }
      }
      else if (payload.ValueKind == JsonValueKind.Object)
      {
        foreach (var prop in payload.EnumerateObject())
        {
          var name = prop.Name;
          if (name == "PacketName" || name == "packetName" || name == "packet_name") continue;
          parameters[name] = prop.Value.ToString();
        }
      }

      var command = new Command
      {
        PacketName = packetName ?? string.Empty,
        Parameters = parameters
      };

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
