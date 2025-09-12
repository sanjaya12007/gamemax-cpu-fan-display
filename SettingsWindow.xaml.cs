using System.Windows;
using System.Windows.Input;

namespace GameMax_CPU_Fan_Display
{
    public partial class SettingsWindow
    {
        private static SettingsWindow? _instance;

        private SettingsWindow()
        {
            InitializeComponent();

            int currentInterval = Settings.Interval;
            SliderInterval.Value = currentInterval;
            TextInterval.Text = $"{currentInterval} ms";

            CheckAutostart.IsChecked = Settings.Autostart;

            SliderInterval.ValueChanged += (_, _) =>
            {
                int newInterval = (int)SliderInterval.Value;
                TextInterval.Text = $"{newInterval} ms";
                Settings.Interval = newInterval;
                TemperatureDisplayService.SetInterval(newInterval);
            };

            CheckAutostart.Checked += (_, _) => Settings.Autostart = true;
            CheckAutostart.Unchecked += (_, _) => Settings.Autostart = false;

            BtnClose.Click += (_, _) => Close();
        }

        public static void ShowSettings()
        {
            if (_instance == null)
            {
                _instance = new SettingsWindow();
                _instance.Closed += (_, _) => _instance = null;
                _instance.Show();
            }
            else
            {
                if (_instance.WindowState == WindowState.Minimized)
                    _instance.WindowState = WindowState.Normal;
                _instance.Activate();
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}