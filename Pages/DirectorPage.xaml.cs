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
    /// Логика взаимодействия для DirectorPage.xaml
    /// </summary>
    public partial class DirectorPage : Page
    {
        public DirectorPage()
        {
            InitializeComponent();
            HelloLabel.Content = "Директор: " + Core.CurrentEmployee.FullName;
        }

        private void BtnComponents_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ComponentsTabPage());
        }

        private void BtnCategories_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new CategoriesTabPage());
        }

        private void BtnSuppliers_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new SuppliersTabPage());
        }

        private void BtnEmployees_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new EmployeesTabPage());
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new ReportPage());
        }
    }
}
