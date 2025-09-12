using HidSharp;
using LibreHardwareMonitor.Hardware;

namespace GameMax_CPU_Fan_Display;

public static class TemperatureDisplayService
{
    // HID Address
    private const int Vid = 0x5131;
    private const int Pid = 0x2007;

    // Update interval
    private static int _intervalMs = 1000;

    private static int _outLen;
    private static Computer? _computer;
    private static HidStream? _stream;

    private static Thread? _thread;
    private static bool _running;

    private static void Initialize()
    {
        _computer = new Computer { IsCpuEnabled = true };
        _computer.Open();

        HidDevice device = DeviceList.Local.GetHidDevices(Vid, Pid).FirstOrDefault()
                           ?? throw new Exception("Display not found");

        _outLen = device.GetMaxOutputReportLength();
        if (_outLen <= 0) throw new Exception("The device does not support writing HID reports.");

        if (!device.TryOpen(out _stream)) throw new Exception("Failed to open HID device");

        _thread = new Thread(Worker)
        {
            IsBackground = true
        };
    }

    public static void Start()
    {
        if (_running) return;
        if (_thread == null) Initialize();

        _running = true;
        _thread!.Start();
    }

    public static void Stop()
    {
        _running = false;
        _thread?.Join();
    }
    
    public static int GetInterval() => _intervalMs;
    public static bool SetInterval(int ms)
    {
        if (ms < 100 || ms > 2000) return false;
        _intervalMs = ms;
        return true;
    }

    private static void Worker()
    {
        while (_running)
        {
            int temp = GetCpuTemperature();
            SendTemperature(temp);

            Thread.Sleep(_intervalMs);
        }
    }

    private static int GetCpuTemperature()
    {
        if (_computer == null) return 0;

        int temp = 0;
        foreach (var hardware in _computer.Hardware)
        {
            if (hardware.HardwareType == HardwareType.Cpu)
            {
                hardware.Update();
                var coreTemps = hardware.Sensors
                    .Where(s => s.SensorType == SensorType.Temperature && s.Value.HasValue)
                    .Select(s => s.Value!.Value)
                    .ToList();

                if (coreTemps.Count > 0)
                {
                    temp = (int)coreTemps.Average();
                }
            }
        }
        return temp;
    }

    private static void SendTemperature(int temp)
    {
        if (_stream == null) return;

        var report = new byte[_outLen];
        report[2] = (byte)Math.Clamp(temp, 0, 255);

        try
        {
            _stream.Write(report);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while sending the package: {ex.Message}");
        }
    }
}
