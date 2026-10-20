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
    /// Логика взаимодействия для AddEmployeePage.xaml
    /// </summary>
    public partial class AddEmployeePage : Page
    {
        private Employees _currentEmployee = new Employees();

        public AddEmployeePage(Employees selectedEmployee)
        {
            InitializeComponent();

            if (selectedEmployee != null)
            {
                _currentEmployee = selectedEmployee;
                Title = "Редактирование сотрудника";
                PasswordLabel.Content = "Новый пароль (оставьте пустым, чтобы не менять)";
                if (_currentEmployee.Rate != null)
                    TBRate.Text = _currentEmployee.Rate.Value.ToString("0");
            }
            else
            {
                Title = "Добавление сотрудника";
            }

            DataContext = _currentEmployee;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentEmployee.FullName))
                errors.AppendLine("Укажите ФИО!");

            if (string.IsNullOrWhiteSpace(_currentEmployee.Login))
                errors.AppendLine("Укажите логин!");
            else if (_currentEmployee.Login.Contains(" "))
                errors.AppendLine("Логин не должен содержать пробелов!");
            else if (Core.Context.Customers.Any(c => c.Login == _currentEmployee.Login) ||
                     Core.Context.Employees.Any(x => x.Login == _currentEmployee.Login && x.Id != _currentEmployee.Id))
                errors.AppendLine("Такой логин уже занят!");

            if (string.IsNullOrWhiteSpace(_currentEmployee.Position))
                errors.AppendLine("Выберите должность!");

            if (!int.TryParse(TBRate.Text, out int rate) || rate < 0 || rate > 1000000)
                errors.AppendLine("Ставка — целое число от 0 до 1 000 000!");

            if (_currentEmployee.Id == 0 && string.IsNullOrEmpty(PBPassword.Password))
                errors.AppendLine("Укажите пароль!");

            if (errors.Length > 0)
            {
                Core.ShowError(errors.ToString());
                return;
            }

            if (!string.IsNullOrEmpty(PBPassword.Password))
            {
                if (!Core.CheckPassword(PBPassword.Password))
                    return;
                _currentEmployee.PasswordHash = Core.GetHash(PBPassword.Password);
            }

            _currentEmployee.Rate = rate;

            if (_currentEmployee.Id == 0)
                Core.Context.Employees.Add(_currentEmployee);

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

        private void Letters_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsLetter(e.Text, 0) || e.Text == " " || e.Text == "-"))
                e.Handled = true;
        }

        private void Login_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text[0];
            bool en = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
            bool digit = c >= '0' && c <= '9';
            if (!en && !digit)
                e.Handled = true;
        }

        private void OnlyDigits_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true;
        }
        // плейсхолдеры (методичка, шаг 6)
        private void TBFullName_TextChanged(object sender, TextChangedEventArgs e)
        {
            FullNameHint.Visibility = Visibility.Visible;
            if (TBFullName.Text.Length > 0)
                FullNameHint.Visibility = Visibility.Hidden;
        }

        private void TBLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            LoginHint.Visibility = Visibility.Visible;
            if (TBLogin.Text.Length > 0)
                LoginHint.Visibility = Visibility.Hidden;
        }

        private void PBPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordHint.Visibility = Visibility.Visible;
            if (PBPassword.Password.Length > 0)
                PasswordHint.Visibility = Visibility.Hidden;
        }

        private void CBPosition_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PositionHint.Visibility = Visibility.Visible;
            if (CBPosition.SelectedItem != null)
                PositionHint.Visibility = Visibility.Hidden;
        }

        private void TBRate_TextChanged(object sender, TextChangedEventArgs e)
        {
            RateHint.Visibility = Visibility.Visible;
            if (TBRate.Text.Length > 0)
                RateHint.Visibility = Visibility.Hidden;
        }

        private void FullNameHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBFullName.Focus();
        }

        private void LoginHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBLogin.Focus();
        }

        private void PasswordHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PBPassword.Focus();
        }

        private void PositionHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CBPosition.IsDropDownOpen = true; 
        }

        private void RateHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TBRate.Focus();
        }
    }
}
