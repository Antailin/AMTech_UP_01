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
    /// Логика взаимодействия для ChangePassPage.xaml
    /// </summary>
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(CurrentPasswordBox.Password) ||
                string.IsNullOrEmpty(NewPasswordBox.Password) ||
                string.IsNullOrEmpty(ConfirmPasswordBox.Password) ||
                string.IsNullOrEmpty(TbLogin.Text))
            {
                Core.ShowError("Все поля обязательны к заполнению!");
                return;
            }

            string hashedPass = Core.GetHash(CurrentPasswordBox.Password);
            Customers customer = Core.Context.Customers
                .FirstOrDefault(c => c.Login == TbLogin.Text && c.PasswordHash == hashedPass);
            Employees employee = Core.Context.Employees
                .FirstOrDefault(x => x.Login == TbLogin.Text && x.PasswordHash == hashedPass);

            if (customer == null && employee == null)
            {
                Core.ShowError("Текущий пароль/Логин неверный!");
                return;
            }

            if (!Core.CheckPassword(NewPasswordBox.Password))
                return;

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                Core.ShowError("Пароли не совпадают!");
                return;
            }

            if (customer != null)
                customer.PasswordHash = Core.GetHash(NewPasswordBox.Password);
            else
                employee.PasswordHash = Core.GetHash(NewPasswordBox.Password);

            Core.Context.SaveChanges();
            Core.ShowInfo("Пароль успешно изменен!");
            NavigationService.GoBack();
        }
    }
}