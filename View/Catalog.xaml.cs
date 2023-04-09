using GoldFish.Classes;
using GoldFish.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GoldFish.View
{
    /// <summary>
    /// Логика взаимодействия для Catalog.xaml
    /// </summary>
    public partial class Catalog : Window
    {
        public Catalog()
        {
            InitializeComponent();
        }

        private void btnClickBack(object sender, RoutedEventArgs e)
        {
            Authorization window = new Authorization();
            Helper.User = null;
            window.Show();
            this.Close();
        }

        private void buttonAddProduct_Click(object sender, RoutedEventArgs e)
        {
            Products products = new Products();
            this.Hide();
            products.ShowDialog();
            this.Close();
        }

        private void buttonHystory_Click(object sender, RoutedEventArgs e)
        {
            Hystory hystory = new Hystory();
            this.Hide();
            hystory.ShowDialog();
            this.Close();
        }

        private void buttonOrder_Click(object sender, RoutedEventArgs e)
        {
            Orders orders = new Orders();
            this.Hide();
            orders.ShowDialog(); 
            this.Close();
        }

        private void buttonUpdateOrders_Click(object sender, RoutedEventArgs e)
        {
            UpdateOrders updateOrders = new UpdateOrders();
            this.Hide();
            updateOrders.ShowDialog();
            this.Close();
        }

        private void windowCatalog_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.User != null)
                labelGuest.Content = $"{Helper.User.UserFullName}";
            else
                labelGuest.Content = "Гость";

            List<string> categories = new List<string>();
            categories.Add("Все категории");
            foreach (var c in Helper.ContextFish.Category)
                categories.Add(c.CategoryName);
            comboBoxCategoryProduct.ItemsSource = categories;
            comboBoxCategoryProduct.SelectedIndex = 0;
            FilterOn();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Product product = (Product)dataGridProducts.Items.GetItemAt(dataGridProducts.SelectedIndex);

            var orderProduct = new OrderProduct
            {
                OrderProductCount = 1,
                ProductArticleNumber = product.ProductArticleNumber
            };

            if (Helper.ListOrderProduct == null || Helper.ListOrderProduct.Count == 0)
            {
                Helper.ListOrderProduct = new List<OrderProduct>
                {
                    orderProduct,
                };
            }
            else
            {
                if (!Helper.ListOrderProduct.Exists(x => x.ProductArticleNumber == product.ProductArticleNumber))
                    Helper.ListOrderProduct.Add(orderProduct);
                else
                {
                    int ind = Helper.ListOrderProduct.FindIndex(x => x.ProductArticleNumber == product.ProductArticleNumber);
                    Helper.ListOrderProduct[ind].OrderProductCount++;
                }
            }
        }

        private void FilterOn()
        {
            int sortIndex = comboBoxSortProduct.SelectedIndex;
            var products = sortIndex == 1 ? Helper.ContextFish.Product.OrderBy(p => p.ProductCost).ToList() : 
                Helper.ContextFish.Product.OrderByDescending(p => p.ProductCost).ToList();
            int categoryIndex = comboBoxCategoryProduct.SelectedIndex;
            products = categoryIndex == 0 ? products.ToList() : products.Where(x => x.CategoryID == categoryIndex).ToList();
            int discountIndex = comboBoxDiscountProduct.SelectedIndex;
            switch (discountIndex)
            {
                case 0:
                    products = products.Where(x => x.ProductDiscountAmount >= 0).ToList();
                    break;
                case 1:
                    products = products.Where(x => x.ProductDiscountAmount < 11).ToList();
                    break;
                case 2:
                    products = products.Where(x => x.ProductDiscountAmount < 15 && x.ProductDiscountAmount >= 11).ToList();
                    break;
                case 3:
                    products = products.Where(x => x.ProductDiscountAmount >= 15).ToList();
                    break;
            }
            string word = textBoxNameProduct.Text;
            if (word != null || word != "")
                products = products.Where(x => x.ProductName.ToLower().Contains(word.ToLower())).ToList();

            dataGridProducts.ItemsSource = products;
            labelCount.Content = $"Количество записей {dataGridProducts.Items.Count} из {Helper.ContextFish.Product.Count()}";
        }

        private void resetSort_Click(object sender, RoutedEventArgs e)
        {
            dataGridProducts.ItemsSource = Helper.ContextFish.Product.ToList();
            labelCount.Content = $"Количество записей {dataGridProducts.Items.Count} из {Helper.ContextFish.Product.Count()}";
            comboBoxCategoryProduct.SelectedIndex = 0;
            comboBoxDiscountProduct.SelectedIndex = 0;
            comboBoxSortProduct.SelectedIndex = 0;
            textBoxNameProduct.Text = "";
        }

        private void comboBoxCategoryProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridProducts != null)
                FilterOn();
        }

        private void comboBoxDiscountProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridProducts != null)
                FilterOn();
        }

        private void comboBoxSortProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dataGridProducts != null)
                FilterOn();
        }

        private void textBoxNameProduct_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (dataGridProducts != null)
                FilterOn();
        }

        private void dataGridProducts_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                Helper.Product = (Product)dataGridProducts.Items.GetItemAt(dataGridProducts.SelectedIndex);
            }
            catch
            {
                return;
            }
            Products products = new Products();
            this.Hide();
            products.ShowDialog();
            this.Close();
        }
    }
}
