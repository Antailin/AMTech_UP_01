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
    /// Логика взаимодействия для CatalogPage.xaml
    /// </summary>
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();

            List<string> categories = Core.Context.Categories.Select(x => x.Name).ToList();
            categories.Insert(0, "Все категории");
            CategoryBox.ItemsSource = categories;
            CategoryBox.SelectedIndex = 0;

            UpdateComponents();
        }

        private void UpdateComponents()
        {
            if (!IsInitialized)
                return;

            List<Components> list = Core.Context.Components.ToList();

            if (!string.IsNullOrWhiteSpace(SearchBox.Text))
            {
                string text = SearchBox.Text.ToLower();
                list = list.Where(x => x.Name.ToLower().Contains(text) ||
                                       (x.Manufacturer != null && x.Manufacturer.ToLower().Contains(text))).ToList();
            }

            string category = CategoryBox.SelectedItem as string;
            if (category != null && category != "Все категории")
                list = list.Where(x => x.Categories.Name == category).ToList();

            if (InStockBox.IsChecked == true)
                list = list.Where(x => x.Stock.Any(s => s.Quantity > 0)).ToList();

            if (SortBox.SelectedIndex == 0)
                ListComponents.ItemsSource = list.OrderBy(x => x.SalePrice).ToList();
            else
                ListComponents.ItemsSource = list.OrderByDescending(x => x.SalePrice).ToList();
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchHint.Visibility = Visibility.Visible;
            if (SearchBox.Text.Length > 0)
                SearchHint.Visibility = Visibility.Hidden;
            UpdateComponents();
        }

        private void SearchHint_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SearchBox.Focus();
        }

        private void CategoryBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComponents();
        }

        private void SortBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateComponents();
        }

        private void InStockBox_Changed(object sender, RoutedEventArgs e)
        {
            UpdateComponents();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CategoryBox.SelectedIndex = 0;
            SortBox.SelectedIndex = 0;
            InStockBox.IsChecked = false;
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ListComponents.SelectedItem is Components component))
            {
                Core.ShowError("Выберите товар в каталоге");
                return;
            }

            Stock stock = Core.Context.Stock.FirstOrDefault(x => x.ComponentId == component.Id);
            if (stock == null || stock.Quantity == 0)
            {
                Core.ShowError("Этого товара нет в наличии");
                return;
            }

            if (!Core.AskYesNo("Купить «" + component.Name + "» за " + component.SalePrice.ToString("0") + " руб.?"))
                return;

            string payment = (PayBox.SelectedItem as ComboBoxItem).Content.ToString();

            Orders order = new Orders
            {
                CustomerId = Core.CurrentCustomer.Id,
                OrderDate = DateTime.Now,
                Status = "Новый",
                TotalAmount = component.SalePrice
            };
            Core.Context.Orders.Add(order);
            Core.Context.SaveChanges();

            OrderItems item = new OrderItems
            {
                OrderId = order.Id,
                ComponentId = component.Id,
                Quantity = 1,
                Price = component.SalePrice,
                WarrantyEndDate = DateTime.Today.AddMonths(component.WarrantyMonths)
            };
            Core.Context.OrderItems.Add(item);

            Payments pay = new Payments
            {
                OrderId = order.Id,
                Amount = component.SalePrice,
                Method = payment,
                Status = "Оплачено",
                PaymentDate = DateTime.Now
            };
            Core.Context.Payments.Add(pay);

            stock.Quantity = stock.Quantity - 1;

            Core.Context.SaveChanges();
            Core.ShowInfo("Заказ оформлен! Гарантия до " + item.WarrantyEndDate.ToString("dd.MM.yyyy"));
            UpdateComponents();
        }

        private void MyOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new MyOrdersPage());
        }
    }
}