using GoldFish.Classes;
using GoldFish.Models;
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
using System.Windows.Shapes;

namespace GoldFish.View
{
    /// <summary>
    /// Логика взаимодействия для Orders.xaml
    /// </summary>
    public partial class Orders : Window
    {
        public Orders()
        {
            InitializeComponent();
            comboBoxPoint.ItemsSource = Helper.ContextFish.Point.ToList();
        }

        private void buttonOnCatalog_Click(object sender, RoutedEventArgs e)
        {
            Catalog window = new Catalog();
            window.Show();
            this.Close();
        }

        private void windowOrders_Loaded(object sender, RoutedEventArgs e)
        {
            dataGridOrderProduct.ItemsSource = Helper.ListOrderProduct.ToList();

            if (Helper.User != null)
                labelNameClient.Content = $"{Helper.User.UserFullName}";

            int countProducts = Helper.ListOrderProduct.Select(x => x.OrderProductCount).Count();
            decimal totalDiscount = (decimal)Helper.ListOrderProduct.Select(x => x.Product.ProductDiscountInMoney).Sum();
            decimal totalSum = Helper.ListOrderProduct.Select(x => x.OrderProductCount * x.Product.ProductCost).Sum();

            textBoxDescriptionOrder.Text = $"Позиций в заказе: {Helper.ListOrderProduct.Count}{Environment.NewLine}";
            textBoxDescriptionOrder.Text += $"Всего товаров: {countProducts}{Environment.NewLine}";
            textBoxDescriptionOrder.Text += $"Общая сумма за весь товар: {totalSum}{Environment.NewLine}";
            textBoxDescriptionOrder.Text += $"Общая сумма скидки: {totalDiscount}{Environment.NewLine}";
            textBoxDescriptionOrder.Text += $"Общая сумма за весь товар со скидкой: {totalSum - totalDiscount}";
        }

        private void dataGridOrderProduct_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var orderProduct = (OrderProduct)dataGridOrderProduct.SelectedItem;
            if (orderProduct != null)
            {
                listDescriptionProduct.DataContext = orderProduct.Product;
                listDescriptionProduct.Visibility = Visibility.Visible;
            }
        }

        private void buttonPlaceAnOrder_Click(object sender, RoutedEventArgs e)
        {
            if (Helper.ListOrderProduct == null || Helper.ListOrderProduct.Count == 0)
            {
                MessageBox.Show("Нет товаров для формирования заказа");
                return;
            }

            int countInStock = Helper.ListOrderProduct.Select(x => x.Product.ProductQuantityInStock < 3).Count();
            int daysDelivery = countInStock > 0 ? 6 : 3;

            DateTime dateNow = DateTime.Now;
            DateTime dateDelivery = dateNow.AddDays(daysDelivery);

            int idPickupPoint = comboBoxPoint.SelectedIndex + 1;

            string uniqueCode = "";
            int orderId = Helper.ContextFish.Order.ToList().Last().OrderID;
            var newOrder = new Order
            {
                OrderID = orderId,
                OrderDate = dateNow.Date,
                OrderTime = dateNow.TimeOfDay,
                OrderDeliveryDate = dateDelivery.Date,
                PointID = idPickupPoint,
                OrderCodeForGetOrder = $"{dateNow.Date}{dateNow.TimeOfDay}",
                StatusID = 2
            };

            Order order;
            if (Helper.User != null && Helper.User.Client != null)
            {
                newOrder.ClientID = Helper.User.Client.UserID;
                newOrder.OrderCodeForGetOrder = $"{Helper.User.Client.ClientCode}{dateNow.Year}{dateNow.Month}{dateNow.Day}{dateNow.Hour}{dateNow.Minute}";
            }
            else
            {
                do
                {
                    int code = new Random().Next(10000000, 99999999);
                    uniqueCode = $"{code}{dateNow.Year}{dateNow.Month}{dateNow.Day}{dateNow.Hour}{dateNow.Minute}";
                    order = Helper.ContextFish.Order.Where(x => x.Client.ClientCode == uniqueCode).FirstOrDefault();
                }
                while (order != null);
                newOrder.OrderCodeForGetOrder = uniqueCode;
            }

            try
            {
                Helper.ContextFish.Order.Add(newOrder);
                Helper.ContextFish.SaveChanges();
                orderId = Helper.ContextFish.Order.ToList().Last().OrderID;
                foreach (var item in Helper.ListOrderProduct)
                    item.OrderID = orderId;
                Helper.ContextFish.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Не удалось оформить заказ");
                Catalog catalog1 = new Catalog();
                catalog1.Show();
                this.Close();
                return;
            }
            MessageBox.Show($"Заказ оформлен {Environment.NewLine} " +
                $"Код Вашего заказа: {newOrder.OrderCodeForGetOrder}{Environment.NewLine} " +
                $"Дата получения заказа: " +
                $"{newOrder.OrderDeliveryDate.ToLongDateString()}");
            Catalog catalog = new Catalog();
            catalog.Show();
            this.Close();
        }
    }
}
