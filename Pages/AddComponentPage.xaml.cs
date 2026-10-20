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
    /// Логика взаимодействия для AddComponentPage.xaml
    /// </summary>
    public partial class AddComponentPage : Page
    {
        private Components _currentComponent = new Components();

        public AddComponentPage(Components selectedComponent)
        {
            InitializeComponent();

            CBCategory.ItemsSource = Core.Context.Categories.ToList();
            CBCategory.DisplayMemberPath = "Name";
            CBSupplier.ItemsSource = Core.Context.Suppliers.ToList();
            CBSupplier.DisplayMemberPath = "Name";

            if (selectedComponent != null)
            {
                _currentComponent = selectedComponent;
                Title = "Редактирование товара";

                TBCostPrice.Text = _currentComponent.CostPrice.ToString("0");
                TBSalePrice.Text = _currentComponent.SalePrice.ToString("0");
                TBWarranty.Text = _currentComponent.WarrantyMonths.ToString();
                Stock stock = Core.Context.Stock.FirstOrDefault(x => x.ComponentId == _currentComponent.Id);
                if (stock != null)
                    TBQuantity.Text = stock.Quantity.ToString();
            }
            else
            {
                Title = "Добавление товара";
            }

            DataContext = _currentComponent;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();

            if (string.IsNullOrWhiteSpace(_currentComponent.Name))
                errors.AppendLine("Укажите название!");
            if (string.IsNullOrWhiteSpace(_currentComponent.Manufacturer))
                errors.AppendLine("Укажите производителя!");
            if (_currentComponent.CategoryId == 0)
                errors.AppendLine("Выберите категорию!");
            if (_currentComponent.SupplierId == 0)
                errors.AppendLine("Выберите поставщика!");

            if (!int.TryParse(TBCostPrice.Text, out int costPrice) || costPrice < 1 || costPrice > 1000000)
                errors.AppendLine("Цена закупки — целое число от 1 до 1 000 000!");
            if (!int.TryParse(TBSalePrice.Text, out int salePrice) || salePrice < 1 || salePrice > 1000000)
                errors.AppendLine("Цена продажи — целое число от 1 до 1 000 000!");
            if (!int.TryParse(TBWarranty.Text, out int warranty) || warranty < 0 || warranty > 60)
                errors.AppendLine("Гарантия — от 0 до 60 месяцев!");
            if (!int.TryParse(TBQuantity.Text, out int quantity) || quantity < 0 || quantity > 10000)
                errors.AppendLine("Остаток — от 0 до 10 000 штук!");

            if (errors.Length > 0)
            {
                Core.ShowError(errors.ToString());
                return;
            }

            _currentComponent.CostPrice = costPrice;
            _currentComponent.SalePrice = salePrice;
            _currentComponent.WarrantyMonths = warranty;

            if (_currentComponent.Id == 0)
                Core.Context.Components.Add(_currentComponent);

            try
            {
                Core.Context.SaveChanges();  

                Stock stock = Core.Context.Stock.FirstOrDefault(x => x.ComponentId == _currentComponent.Id);
                if (stock == null)
                {
                    stock = new Stock
                    {
                        ComponentId = _currentComponent.Id,
                        Quantity = quantity,
                        MinThreshold = 3
                    };
                    Core.Context.Stock.Add(stock);
                }
                else
                {
                    stock.Quantity = quantity;
                }

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