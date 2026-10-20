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
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();   
        }

        private void regButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtbxFIO.Text) || string.IsNullOrWhiteSpace(txtbxLog.Text) ||
                string.IsNullOrWhiteSpace(passBxFrst.Password) || string.IsNullOrWhiteSpace(passBxScnd.Password))
            {
                Core.ShowError("Заполните все поля!");
                return;
            }

            if (txtbxLog.Text.Contains(" "))
            {
                Core.ShowError("Логин не должен содержать пробелов!");
                return;
            }
            if (Core.LoginExists(txtbxLog.Text))
            {
                Core.ShowError("Пользователь с таким логином уже существует!");
                return;
            }

            if (!Core.CheckPassword(passBxFrst.Password))
                return;

            if (passBxFrst.Password != passBxScnd.Password)
            {
                Core.ShowError("Пароли не совпадают!");
                return;
            }

            Customers customer = new Customers
            {
                FullName = txtbxFIO.Text,
                Login = txtbxLog.Text,
                PasswordHash = Core.GetHash(passBxFrst.Password)
            };
            Core.Context.Customers.Add(customer);
            Core.Context.SaveChanges();
            Core.ShowInfo("Вы зарегистрированы! Теперь войдите под своим логином.");
            Core.ShowInfo("Пользователь успешно зарегистрирован!");
            NavigationService.GoBack(); 
        }

        private void txtbxFIO_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsLetter(e.Text, 0) || e.Text == " " || e.Text == "-"))
                e.Handled = true;   
        }

        private void txtbxLog_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = e.Text[0];
            bool en = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
            bool digit = c >= '0' && c <= '9';
            if (!en && !digit)
                e.Handled = true;
        }

        private void txtbxFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblFioHitn.Visibility = Visibility.Visible;
            if (txtbxFIO.Text.Length > 0)
                lblFioHitn.Visibility = Visibility.Hidden;
        }

        private void txtbxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            lblLogHitn.Visibility = Visibility.Visible;
            if (txtbxLog.Text.Length > 0)
                lblLogHitn.Visibility = Visibility.Hidden;
        }

        private void passBxFrst_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassHitn.Visibility = Visibility.Visible;
            if (passBxFrst.Password.Length > 0)
                lblPassHitn.Visibility = Visibility.Hidden;
        }

        private void passBxScnd_PasswordChanged(object sender, RoutedEventArgs e)
        {
            lblPassSecHitn.Visibility = Visibility.Visible;
            if (passBxScnd.Password.Length > 0)
                lblPassSecHitn.Visibility = Visibility.Hidden;
        }

        private void lblFioHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxFIO.Focus();
        }

        private void lblLogHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            txtbxLog.Focus();
        }

        private void lblPassHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxFrst.Focus();
        }

        private void lblPassSecHitn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            passBxScnd.Focus();
        }
    }
}
