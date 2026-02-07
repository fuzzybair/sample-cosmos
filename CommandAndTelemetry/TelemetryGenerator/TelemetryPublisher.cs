using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TelemetryGenerator
{
  public class TelemetryPublisher : BackgroundService
  {
    private readonly ILogger<TelemetryPublisher> _logger;
    private TcpClient? _client;
    private NetworkStream? _stream;
    private PipeWriter? _writer;
    private readonly string _host;
    private readonly int _port;
    private readonly JsonSerializerOptions _jsonOptions;

    public TelemetryPublisher(ILogger<TelemetryPublisher> logger)
    {
      _logger = logger;

      // Read endpoint from environment variables (fallback to defaults)
      _host = Environment.GetEnvironmentVariable("TELEMETRY_HOST") ?? "127.0.0.1";

      var portEnv = Environment.GetEnvironmentVariable("TELEMETRY_PORT");
      if (!int.TryParse(portEnv, out _port))
      {
        _port = 6000;
      }

      _jsonOptions = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
      };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("TelemetryPublisher starting. Target TCP {Host}:{Port}", _host, _port);

      try
      {
        while (!stoppingToken.IsCancellationRequested)
        {
          try
          {
            await EnsureConnectedAsync(stoppingToken).ConfigureAwait(false);

            var telemetry = CreateTelemetry();
            var json = JsonSerializer.Serialize(telemetry, _jsonOptions) + "\n";
            var bytes = Encoding.UTF8.GetBytes(json);

            if (_writer == null)
              throw new InvalidOperationException("Pipe writer is not available.");

            // Write to the PipeWriter and flush to the underlying stream.
            _writer.Write(bytes);
            var flushResult = await _writer.FlushAsync(stoppingToken).ConfigureAwait(false);

            // If the consumer has completed or cancelled, trigger reconnect on next iteration
            if (flushResult.IsCompleted || flushResult.IsCanceled)
            {
              _logger.LogWarning("Pipe flush reported completed/canceled; disposing connection to reconnect.");
              DisposeConnection();
            }
            else
            {
              _logger.LogInformation("Sent telemetry: {Json}", json.TrimEnd('\n'));
            }
          }
          catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
          {
            // graceful shutdown
            break;
          }
          catch (Exception ex)
          {
            _logger.LogWarning(ex, "Failed to send telemetry; will attempt reconnect on next iteration.");
            DisposeConnection();
          }

          await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken).ConfigureAwait(false);
        }
      }
      finally
      {
        DisposeConnection();
        _logger.LogInformation("TelemetryPublisher stopped.");
      }
    }

    private TelemetryData CreateTelemetry()
    {
      var rnd = new Random();
      return new TelemetryData
      {
        Timestamp = DateTime.UtcNow,
        DeviceId = Guid.NewGuid(),
        TemperatureC = Math.Round(15.0 + rnd.NextDouble() * 20.0, 2),
        HumidityPercent = Math.Round(20.0 + rnd.NextDouble() * 60.0, 2),
        Status = "OK"
      };
    }

    private async Task EnsureConnectedAsync(CancellationToken token)
    {
      if (_client != null && _client.Connected && _stream != null && _writer != null)
        return;

      DisposeConnection();

      _client = new TcpClient();
      try
      {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        cts.CancelAfter(TimeSpan.FromSeconds(5)); // connection timeout

        // ConnectAsync + WaitAsync to allow cancellation/timeout
        await _client.ConnectAsync(_host, _port).WaitAsync(cts.Token).ConfigureAwait(false);

        _stream = _client.GetStream();

        // Create a PipeWriter that writes directly to the NetworkStream
        _writer = PipeWriter.Create(_stream, new StreamPipeWriterOptions(leaveOpen: false));

        _logger.LogInformation("Connected to telemetry endpoint {Host}:{Port}", _host, _port);
      }
      catch (Exception)
      {
        DisposeConnection();
        throw;
      }
    }

    private void DisposeConnection()
    {
      try
      {
        // Complete the writer synchronously (safe API)
        try
        {
          _writer?.Complete();
        }
        catch
        {
          // ignore
        }
      }
      finally
      {
        _writer = null;
      }

      try
      {
        _stream?.Dispose();
      }
      catch { /* ignore */ }
      _stream = null;

      try
      {
        _client?.Close();
        _client?.Dispose();
      }
      catch { /* ignore */ }
      _client = null;
    }

    public override void Dispose()
    {
      DisposeConnection();
      base.Dispose();
    }
  }
}