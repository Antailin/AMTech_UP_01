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
    /// Логика взаимодействия для EmployeesTabPage.xaml
    /// </summary>
    public partial class EmployeesTabPage : Page
    {
        public EmployeesTabPage()
        {
            InitializeComponent();
            DataGridEmployee.ItemsSource = Core.Context.Employees.ToList();
            this.IsVisibleChanged += Page_IsVisibleChanged;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                Core.Context.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DataGridEmployee.ItemsSource = Core.Context.Employees.ToList();
            }
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployeePage(null));
        }

        private void ButtonEdit_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEmployeePage((sender as Button).DataContext as Employees));
        }

        private void ButtonDel_Click(object sender, RoutedEventArgs e)
        {
            var employeesForRemoving = DataGridEmployee.SelectedItems.Cast<Employees>().ToList();
            if (employeesForRemoving.Count == 0)
            {
                Core.ShowError("Выберите сотрудников для удаления");
                return;
            }
            if (employeesForRemoving.Contains(Core.CurrentEmployee))
            {
                Core.ShowError("Нельзя удалить свою учётную запись");
                return;
            }

            if (!Core.AskYesNo($"Вы точно хотите удалить записи в количестве {employeesForRemoving.Count()} элементов?\nВ их заказах поле «сотрудник» станет пустым."))
                return;

            try
            {
                Core.Context.Employees.RemoveRange(employeesForRemoving);
                Core.Context.SaveChanges();
                Core.ShowInfo("Данные успешно удалены!");
                DataGridEmployee.ItemsSource = Core.Context.Employees.ToList();
            }
            catch (Exception ex)
            {
                Core.ShowError(ex.Message);
            }
        }
    }
}
