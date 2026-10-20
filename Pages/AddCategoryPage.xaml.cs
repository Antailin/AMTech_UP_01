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
    /// Логика взаимодействия для AddCategoryPage.xaml
    /// </summary>
    public partial class AddCategoryPage : Page
    {
        private Categories _currentCategory = new Categories();

        public AddCategoryPage(Categories selectedCategory)
        {
            InitializeComponent();

            if (selectedCategory != null)
            {
                _currentCategory = selectedCategory;
                Title = "Редактирование категории";
            }
            else
            {
                Title = "Добавление категории";
            }

            DataContext = _currentCategory;
        }

        private void ButtonSaveCategory_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentCategory.Name))
                errors.AppendLine("Укажите название категории!");
            else if (Core.Context.Categories.Any(x => x.Name == _currentCategory.Name && x.Id != _currentCategory.Id))
                errors.AppendLine("Такая категория уже есть!");

            if (errors.Length > 0)
            {
                Core.ShowError(errors.ToString());
                return;
            }

            if (_currentCategory.Id == 0)
                Core.Context.Categories.Add(_currentCategory);

            try
            {
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные успешно сохранены!");
                NavigationService.GoBack();
            }
            catch (Exception ex)
            {
                Core.ShowError(ex.Message);
            }
        }
    }
}

