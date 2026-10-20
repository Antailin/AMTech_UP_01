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
    /// Логика взаимодействия для AddSupplierPage.xaml
    /// </summary>
    public partial class AddSupplierPage : Page
    {
        private Suppliers _currentSupplier = new Suppliers();

        public AddSupplierPage(Suppliers selectedSupplier)
        {
            InitializeComponent();

            if (selectedSupplier != null)
            {
                _currentSupplier = selectedSupplier;
                Title = "Редактирование поставщика";
            }
            else
            {
                Title = "Добавление поставщика";
            }

            DataContext = _currentSupplier;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentSupplier.Name))
                errors.AppendLine("Укажите название поставщика!");
            if (string.IsNullOrWhiteSpace(_currentSupplier.Contacts))
                errors.AppendLine("Укажите контакты!");
            if (string.IsNullOrWhiteSpace(_currentSupplier.TaxId) ||
                (_currentSupplier.TaxId.Length != 10 && _currentSupplier.TaxId.Length != 12))
                errors.AppendLine("ИНН — 10 или 12 цифр!");

            if (errors.Length > 0)
            {
                Core.ShowError(errors.ToString());
                return;
            }

            if (_currentSupplier.Id == 0)
                Core.Context.Suppliers.Add(_currentSupplier);

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

        private void OnlyDigits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
    }
}
