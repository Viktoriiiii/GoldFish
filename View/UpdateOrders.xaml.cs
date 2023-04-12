using GoldFish.Classes;
using GoldFish.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
namespace GoldFish.View
{
    /// <summary>
    /// Логика взаимодействия для UpdateOrders.xaml
    /// </summary>
    public partial class UpdateOrders : Window
    {
        List<Order> orders;
        public UpdateOrders()
        {
            InitializeComponent();
        }

        private void buttonOnCatalog_Click(object sender, RoutedEventArgs e)
        {
            Catalog window = new Catalog();
            window.Show();
            this.Close();
        }

        private void OpdateOrders_Loaded(object sender, RoutedEventArgs e)
        {
            ShowOrders();
        }

        private void dataGridOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridOrders.SelectedItem is Order order)
            {
                dataGridOrderProduct.ItemsSource = order.OrderProduct.ToList();

                decimal totalDiscount = (decimal) order.OrderProduct.Select(x => x.Product.ProductDiscountInMoney).Sum();
                decimal totalSum = (decimal)order.OrderProduct.Select(x => x.OrderProductCount * x.Product.ProductCost).Sum();
                textBoxDescription.Text = $"Общая сумма за весь товар: {totalSum}{Environment.NewLine}";
                textBoxDescription.Text += $"Общая сумма скидки: {totalDiscount}{Environment.NewLine}";
                textBoxDescription.Text += $"Общая сумма за весь товар со скидкой: {totalSum - totalDiscount}";
            }
        }

        private void ShowOrders()
        {
            orders = Helper.ContextFish.Order.ToList();
            dataGridOrders.ItemsSource = orders;
        }

        private void changeStatus_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOrders.SelectedItem is Order order)
            {
                order.StatusID = order.StatusID is 1 ? 2 : 1;
                Helper.ContextFish.SaveChanges();
                ShowOrders();
            }
        }

        private void changeDate_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOrders.SelectedItem is Order order)
            {
                if (datePickerForOrder.SelectedDate != null)
                {
                    order.OrderDeliveryDate = (DateTime)datePickerForOrder.SelectedDate;
                    Helper.ContextFish.SaveChanges();
                    ShowOrders();
                }                
            }
        }
    }
}
