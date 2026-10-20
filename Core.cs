using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AMTech
{
    internal class Core
    {
        public static CursachEntities Context = new CursachEntities();

        public static Customers CurrentCustomer;
        public static Employees CurrentEmployee;

        public static bool Authenticate(string login, string password)
        {
            string hash = GetHash(password);
            CurrentCustomer = Context.Customers.FirstOrDefault(c => c.Login == login && c.PasswordHash == hash);
            CurrentEmployee = Context.Employees.FirstOrDefault(x => x.Login == login && x.PasswordHash == hash);
            return CurrentCustomer != null || CurrentEmployee != null;
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }


        public static bool CheckPassword(string password)
        {
            if (password.Length < 6)
            {
                ShowError("Пароль слишком короткий, должно быть минимум 6 символов!");
                return false;
            }

            bool en = true;
            bool number = false;
            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] >= '0' && password[i] <= '9')
                    number = true;
                else if (!((password[i] >= 'A' && password[i] <= 'Z') || (password[i] >= 'a' && password[i] <= 'z')))
                    en = false;
            }

            if (!en)
            {
                ShowError("Используйте только английскую раскладку!");
                return false;
            }
            if (!number)
            {
                ShowError("Добавьте хотя бы одну цифру!");
                return false;
            }
            return true;
        }

        public static bool LoginExists(string login)
        {
            return Context.Customers.Any(c => c.Login == login) || Context.Employees.Any(x => x.Login == login);
        }

        public static void ShowError(string text)
        {
            MessageBox.Show(text, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfo(string text)
        {
            MessageBox.Show(text, "Сообщение", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static bool AskYesNo(string text)
        {
            return MessageBox.Show(text, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
