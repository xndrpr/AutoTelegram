using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace AutoTelegram.Views
{
    public partial class MainWindow : Window
    {
        private bool mouseMove = false;
        public MainWindow()
        {
            InitializeComponent();

            SourceInitialized += Window_SourceInitialized;
        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr mWindowHandle = (new WindowInteropHelper(this)).Handle;
            HwndSource.FromHwnd(mWindowHandle).AddHook(new HwndSourceHook(WindowSizing.WindowProc));
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            Application.Current.Shutdown();

            base.OnClosing(e);
        }

        private void TitleMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if ((ResizeMode == ResizeMode.CanResize) || (ResizeMode == ResizeMode.CanResizeWithGrip))
                {
                    maximizeButton_Click(sender, e);
                }

                return;
            }

            else if (WindowState == WindowState.Maximized)
            {
                mouseMove = true;
                return;
            }

            DragMove();
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private void maximizeButton_Click(object sender, RoutedEventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                case WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
            }
        }

        private void minimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Application.Current?.Shutdown();
        }

        private void TitleMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            mouseMove = false;
        }


        private void TitleMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (mouseMove)
            {
                mouseMove = false;

                double percentHorizontal = e.GetPosition(this).X / ActualWidth;
                double targetHorizontal = RestoreBounds.Width * percentHorizontal;

                double percentVertical = e.GetPosition(this).Y / ActualHeight;
                double targetVertical = RestoreBounds.Height * percentVertical;

                WindowState = WindowState.Normal;

                WindowSizing.POINT lMousePosition;
                WindowSizing.GetCursorPos(out lMousePosition);

                Left = lMousePosition.X - targetHorizontal;
                Top = lMousePosition.Y - targetVertical;

                DragMove();
            }
        }
    }
}
