using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace GameMax_CPU_Fan_Display
{
    public partial class AboutWindow
    {
        private static AboutWindow? _instance;

        private AboutWindow()
        {
            InitializeComponent();
            BtnClose.Click += (_, _) => Close();
        }

        public static void ShowAbout()
        {
            if (_instance == null)
            {
                _instance = new AboutWindow();
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
    }
}