using GoldFish.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace GoldFish.View
{
    /// <summary>
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : Window
    {
        public Products()
        {
            InitializeComponent();
            comboBoxCategory.ItemsSource = Helper.ContextFish.Category.ToList();
            comboBoxManufacturer.ItemsSource = Helper.ContextFish.Manufacturer.ToList();
            comboBoxUnit.ItemsSource = Helper.ContextFish.Unit.ToList();
        }

        private void buttonOnCatalog_Click(object sender, RoutedEventArgs e)
        {
            Helper.Product = null;
            Catalog window = new Catalog();
            window.Show();
            this.Close();
        }

        private void Products_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.Product == null)
            {
                labelTitle.Content = "Новый товар";
                var uriSource = new Uri("/Resources/logo.png", UriKind.Relative);
                imageProductPhoto.Source = new BitmapImage(uriSource);
            }
            else
            {
                labelTitle.Content = "Редактирование товара";
                textBoxArticle.Text = Helper.Product.ProductArticleNumber;
                textBoxProductName.Text = Helper.Product.ProductName;
                textBoxCost.Text = Helper.Product.ProductCost.ToString();
                textBoxDiscount.Text = Helper.Product.ProductDiscountAmount.ToString();
                textBoxMaxDicsount.Text = Helper.Product.ProductMaxDiscountAmount.ToString();
                textBoxOnStore.Text = Helper.Product.ProductQuantityInStock.ToString();
                textBoxDescription.Text = Helper.Product.ProductDescription;
                imageProductPhoto.DataContext = Helper.Product;

                comboBoxCategory.Text = Helper.Product.Category.CategoryName;
                comboBoxManufacturer.Text = Helper.Product.Manufacturer.ManufacturerName;
                comboBoxUnit.Text = Helper.Product.Unit.UnitName;

            }
            
        }
    }
}
