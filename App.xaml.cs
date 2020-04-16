using System.Windows;
using System.Windows.Controls;

namespace Minesweeper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(
                typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler(TextBox_GotFocus));

            EventManager.RegisterClassHandler(
                typeof(Window), UIElement.GotMouseCaptureEvent, new RoutedEventHandler(Window_GotMouseCapture));

            base.OnStartup(e);
        }

        /// <summary>
        /// Select all text when tabbing into a TextBox.
        /// </summary>
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        /// <summary>
        /// Select all text when clicking into a TextBox.
        /// </summary>
        private void Window_GotMouseCapture(object sender, RoutedEventArgs e)
        {
            (e.OriginalSource as TextBox)?.SelectAll();
        }
    }
}
