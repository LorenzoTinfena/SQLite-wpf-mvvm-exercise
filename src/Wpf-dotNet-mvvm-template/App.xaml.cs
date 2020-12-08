using System.Windows;
using WpfDotNetMvvmTemplate.Views;

namespace WpfDotNetMvvmTemplate
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            this.MainWindow = new Views.MainWindowView() { DataContext = new ViewModels.MainWindowViewModel() };
            this.MainWindow.Show();
        }
    }
}