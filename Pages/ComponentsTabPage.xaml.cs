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
    /// Логика взаимодействия для ComponentsTabPage.xaml
    /// </summary>
    public partial class ComponentsTabPage : Page
    {
        public ComponentsTabPage()
        {
            InitializeComponent();

            List<string> suppliers = Core.Context.Suppliers.Select(x => x.Name).ToList();
            suppliers.Insert(0, "Все поставщики");
            SupplierBox.ItemsSource = suppliers;
            SupplierBox.SelectedIndex = 0;

            UpdateComponents();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                UpdateComponents();
            }
        }

        private void UpdateComponents()
        {
            if (!IsInitialized)
                return;

            List<Components> list = Core.Context.Components.ToList();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                list = list.Where(x => x.Name.ToLower().Contains(SearchBox.Text.ToLower())).ToList();

            string supplier = SupplierBox.SelectedItem as string;
            if (supplier != null && supplier != "Все поставщики")
                list = list.Where(x => x.Suppliers.Name == supplier).ToList();

            if (SortBox.SelectedIndex == 0)
                list = list.OrderBy(x => x.Name).ToList();
            else if (SortBox.SelectedIndex == 1)
                list = list.OrderBy(x => x.SalePrice).ToList();
            else
                list = list.OrderByDescending(x => x.SalePrice).ToList();

            DataGridComponents.ItemsSource = list;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchHint.Visibility = Visibility.Visible;
            if (SearchBox.Text.Length > 0)
                SearchHint.Visibility = Visibility.Hidden;
            UpdateComponents();
        }

        private void SearchHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComponents();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddComponentPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddComponentPage((sender as Button).DataContext as Components));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var componentsForRemoving = DataGridComponents.SelectedItems.Cast<Components>().ToList();
            if (componentsForRemoving.Count == 0)
            {
                Core.ShowError("Выберите товары для удаления");
                return;
            }

            if (!Core.AskYesNo($"Вы точно хотите удалить записи в количестве {componentsForRemoving.Count()} элементов?\nВместе с товаром удалятся его остатки и продажи."))
                return;

            try
            {
                Core.Context.Components.RemoveRange(componentsForRemoving);
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные успешно удалены!");
                UpdateComponents();
            }
            catch (Exception ex)
            {
                Core.ShowError(ex.Message);
            }
        }
    }
}