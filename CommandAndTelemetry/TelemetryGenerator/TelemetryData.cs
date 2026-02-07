namespace TelemetryGenerator
{
  public class TelemetryData
  {
    public long Id { get; set; }
    public string PacketName { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public DateTimeOffset TimeReceived { get; set; }
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
  }
}