using System.Linq;
using System.Windows.Controls;
using WpfDotNetMvvmTemplate.Models;
using WpfDotNetMvvmTemplate.ViewModels;

namespace WpfDotNetMvvmTemplate.Views
{
    public partial class MainWindowView : System.Windows.Window
    {
        public MainWindowView()
        {
            InitializeComponent();
        }
        private void DataGrid_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            (sender as DataGrid).SelectedItem = null;
        }

        private void DataGrid1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((MainWindowViewModel)this.DataContext).EditMemberCommand.RaiseCanExecuteChanged();
        }
    }
}
