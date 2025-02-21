using System.Windows;

namespace FileDeletionService_Test
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        MainWindow _mainWindow;

        public App()
        {
            _mainWindow = new MainWindow();

            _mainWindow.Show();
        }
    }
}