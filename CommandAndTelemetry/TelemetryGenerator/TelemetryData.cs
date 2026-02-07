using System;

namespace TelemetryGenerator
{
  public class TelemetryData
  {
    public DateTime Timestamp { get; set; }
    public Guid DeviceId { get; set; }
    public double TemperatureC { get; set; }
    public double HumidityPercent { get; set; }
    public string Status { get; set; } = string.Empty;
  }
}