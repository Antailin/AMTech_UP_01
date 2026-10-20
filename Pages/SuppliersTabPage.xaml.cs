using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AMTech.Pages
{
    /// <summary>
    /// Логика взаимодействия для SuppliersTabPage.xaml
    /// </summary>
    public partial class SuppliersTabPage : Page
    {
        public SuppliersTabPage()
        {
            InitializeComponent();
            DataGridSupplier.ItemsSource = Core.Context.Suppliers.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridSupplier.ItemsSource = Core.Context.Suppliers.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddSupplierPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddSupplierPage((sender as Button).DataContext as Suppliers));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var suppliersForRemoving = DataGridSupplier.SelectedItems.Cast<Suppliers>().ToList();
            if (suppliersForRemoving.Count == 0)
            {
                Core.ShowError("Выберите поставщиков для удаления");
                return;
            }

            if (!Core.AskYesNo($"Вы точно хотите удалить записи в количестве {suppliersForRemoving.Count()} элементов?\nВместе с поставщиком удалятся его товары."))
                return;

            try
            {
                Core.Context.Suppliers.RemoveRange(suppliersForRemoving);
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные успешно удалены!");
                DataGridSupplier.ItemsSource = Core.Context.Suppliers.ToList();
            }
            catch (Exception ex)
            {
                Core.ShowError(ex.Message);
            }
        }
    }
}