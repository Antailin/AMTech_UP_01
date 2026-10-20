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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;   

        public AuthPage()
        {
            InitializeComponent();
        }

        private void ButtonEnter_OnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TextBoxLogin.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                Core.ShowError("Введите логин и пароль");
                return;
            }

            if (!Core.Authenticate(TextBoxLogin.Text, PasswordBox.Password))
            {
                Core.ShowError("Пользователь с такими данными не найден!");
                failedAttempts++;
                if (failedAttempts >= 3)
                {
                    if (CaptchaPanel.Visibility != Visibility.Visible)
                        CaptchaSwitch();
                    CaptchaChange();
                }
                return;
            }

            if (Core.CurrentCustomer != null)
            {
                Core.ShowInfo("Добро пожаловать, " + Core.CurrentCustomer.FullName + "!");
                NavigationService.Navigate(new CatalogPage());
                return;
            }

            switch (Core.CurrentEmployee.Position)
            {
                case "Director":
                    Core.ShowInfo("Добро пожаловать, " + Core.CurrentEmployee.FullName + "!");
                    NavigationService.Navigate(new DirectorPage());
                    break;
                case "Manager":
                    Core.ShowInfo("Добро пожаловать, " + Core.CurrentEmployee.FullName + "!");
                    NavigationService.Navigate(new ManagerPage());
                    break;
                default:
                    Core.CurrentEmployee = null;
                    Core.ShowError("Для сборщиков вход в приложение не предусмотрен");
                    break;
            }
        }

        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
        }

        private void TextBoxLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            txtHintLogin.Visibility = Visibility.Visible;
            if (TextBoxLogin.Text.Length > 0)
                txtHintLogin.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            txtHintPass.Visibility = Visibility.Visible;
            if (PasswordBox.Password.Length > 0)
                txtHintPass.Visibility = Visibility.Hidden;
        }

        private void txtHintLogin_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBoxLogin.Focus();
        }

        private void txtHintPass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            PasswordBox.Focus();
        }

        public void CaptchaSwitch()
        {
            if (CaptchaPanel.Visibility == Visibility.Visible)
            {
                TextBoxLogin.Clear();
                PasswordBox.Clear();
                CaptchaPanel.Visibility = Visibility.Collapsed;
                LoginPanel.Visibility = Visibility.Visible;
            }
            else
            {
                captchaInput.Clear();
                CaptchaPanel.Visibility = Visibility.Visible;
                LoginPanel.Visibility = Visibility.Collapsed;
            }
        }

        public void CaptchaChange()
        {
            String allowchar = "A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,";
            allowchar += "a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,";
            allowchar += "1,2,3,4,5,6,7,8,9,0";
            char[] a = { ',' };
            String[] ar = allowchar.Split(a);
            String pwd = "";
            string temp = "";
            Random r = new Random();

            for (int i = 0; i < 6; i++)
            {
                temp = ar[(r.Next(0, ar.Length))];
                pwd += temp;
            }
            captcha.Text = pwd;
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                Core.ShowError("Неверно введена капча");
                captchaInput.Clear();
                CaptchaChange();
            }
            else
            {
                Core.ShowInfo("Капча введена успешно, можете продолжить авторизацию");
                CaptchaSwitch();
                failedAttempts = 0;
            }
        }

        private void textBox_PreviewExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == ApplicationCommands.Copy ||
                e.Command == ApplicationCommands.Cut ||
                e.Command == ApplicationCommands.Paste)
            {
                e.Handled = true;
            }
        }
    }
}