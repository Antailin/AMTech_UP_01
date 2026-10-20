using AMTech.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace AMTech
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplyTheme(Properties.Settings.Default.Theme);   
            MainFrame.Navigate(new AuthPage());             
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.IsEnabled = true;
            timer.Tick += (o, t) => { DateTimeNow.Text = DateTime.Now.ToString(); };
            timer.Start();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!Core.AskYesNo("Вы действительно хотите закрыть приложение?"))
                e.Cancel = true;   
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.CanGoBack)
                MainFrame.GoBack();
        }

        private void BtnTheme_Click(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.Theme == "Dictionary.xaml")
                Properties.Settings.Default.Theme = "PurpleDictionary.xaml";
            else
                Properties.Settings.Default.Theme = "Dictionary.xaml";

            Properties.Settings.Default.Save();   
            ApplyTheme(Properties.Settings.Default.Theme);
        }

        private void ApplyTheme(string fileName)
        {
            var uri = new Uri(fileName, UriKind.Relative);
            ResourceDictionary resourceDict = Application.LoadComponent(uri) as ResourceDictionary;
            Application.Current.Resources.Clear();
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(resourceDict);
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            if (!Core.AskYesNo("Вы действительно хотите выйти?"))
                return;

            Core.CurrentCustomer = null;
            Core.CurrentEmployee = null;
            MainFrame.Navigate(new AuthPage());
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            Page page = MainFrame.Content as Page;
            if (page == null)
                return;

            Title = "Компьютерный Мир — " + page.Title;
            PageTitle.Text = page.Title;

            if (page is AuthPage || page is CatalogPage || page is ManagerPage || page is DirectorPage)
                BtnBack.Visibility = Visibility.Hidden;
            else
                BtnBack.Visibility = Visibility.Visible;

            if (Core.CurrentCustomer != null || Core.CurrentEmployee != null)
                BtnLogout.Visibility = Visibility.Visible;
            else
                BtnLogout.Visibility = Visibility.Hidden;
        }
    }
}