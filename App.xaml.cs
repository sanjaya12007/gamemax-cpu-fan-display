using System.Diagnostics;
using System.Windows;

namespace GameMax_CPU_Fan_Display;

public partial class App
{
    protected override void OnStartup(StartupEventArgs e)
    {
        try
        {
            base.OnStartup(e);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString(), "Ошибка запуска");
        }
        FindResource("TrayIcon");

        try
        {
            TemperatureDisplayService.SetInterval(Settings.Interval);
            TemperatureDisplayService.Start();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
    
    private void OnAbout(object sender, RoutedEventArgs e)
    {
        AboutWindow.ShowAbout();
    }
    
    private void OnGitHub(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo("https://github.com/lovenek0/gamemax-cpu-fan-display")
        {
            UseShellExecute = true
        });
    }

    private void OnSettings(object sender, RoutedEventArgs e)
    {
        SettingsWindow.ShowSettings();
    }
    
    private void OnExit(object sender, RoutedEventArgs e)
    {
        TemperatureDisplayService.Stop();
        Shutdown();
    }
}