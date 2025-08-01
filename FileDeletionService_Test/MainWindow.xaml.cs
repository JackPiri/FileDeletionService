using FileDeletionService;
using System.Windows;

namespace FileDeletionService_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileDeletionManager _fileDeletionManager;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            FileDeletionServiceErrorCodes errorCode = FileDeletionServiceErrorCodes.NoError;

            _fileDeletionManager = new FileDeletionManager(1); // hours

            _fileDeletionManager.AddDisk("C:", 100);
            _fileDeletionManager.AddFolderToDisk("C:", "C:\\Users\\MyUser\\Desktop", false, false, 0, 0);
            _fileDeletionManager.AddFileToFolderToDisk("C:", "C:\\Users\\MyUser\\Desktop", ".xaml", 1);
        }

        private void Start_OnClick(object sender, RoutedEventArgs e)
        {
            _fileDeletionManager.Start();
        }

        private void Stop_OnClick(object sender, RoutedEventArgs e)
        {
            _fileDeletionManager.Stop();
        }
    }
}
