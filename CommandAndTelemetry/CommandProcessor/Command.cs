namespace CommandProcessor
{
  public class Command
  {
    public long Id { get; set; }
    public string PacketName { get; set; } = string.Empty;
    public string Target { get; set; } = string.Empty;
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
  }
}