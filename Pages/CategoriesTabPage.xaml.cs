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
    /// Логика взаимодействия для CategoriesTabPage.xaml
    /// </summary>
    public partial class CategoriesTabPage : Page
    {
        public CategoriesTabPage()
        {
            InitializeComponent();
            DataGridCategory.ItemsSource = Core.Context.Categories.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridCategory.ItemsSource = Core.Context.Categories.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategoryPage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddCategoryPage((sender as Button).DataContext as Categories));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var categoryForRemoving = DataGridCategory.SelectedItems.Cast<Categories>().ToList();
            if (categoryForRemoving.Count == 0)
            {
                Core.ShowError("Выберите категории для удаления");
                return;
            }

            if (!Core.AskYesNo($"Вы точно хотите удалить записи в количестве {categoryForRemoving.Count()} элементов?\nВместе с категорией удалятся все её товары."))
                return;

            try
            {
                Core.Context.Categories.RemoveRange(categoryForRemoving);
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные успешно удалены!");
                DataGridCategory.ItemsSource = Core.Context.Categories.ToList();
            }
            catch (Exception ex)
            {
                Core.ShowError(ex.Message);
            }
        }
    }
}