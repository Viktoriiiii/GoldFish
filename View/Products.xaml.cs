using GoldFish.Classes;
using GoldFish.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO;
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
        bool isDeleted = false;
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
                textBoxArticle.IsEnabled = false;
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

        private void buttonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(textBoxArticle.Text) || String.IsNullOrEmpty(textBoxCost.Text)
                || String.IsNullOrEmpty(textBoxDescription.Text) || String.IsNullOrEmpty(textBoxMaxDicsount.Text)
                || String.IsNullOrEmpty(textBoxProductName.Text) || String.IsNullOrEmpty(textBoxOnStore.Text)
                || String.IsNullOrEmpty(textBoxDiscount.Text))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            Product productNew;
            if (Helper.Product != null)
            {
                ChangeProduct(Helper.Product);
                SaveChanges();
            }
            else
            {
                productNew = Helper.ContextFish.Product.Find(textBoxArticle.Text);
                if (productNew != null)
                {
                    MessageBox.Show("Такой артикул есть в БД");
                    return;
                }
                else
                {
                    productNew = new Product();
                    productNew = ChangeProduct(productNew);
                    Helper.ContextFish.Product.Add(productNew);
                    SaveChanges();
                } 
            }
        }

        private Product ChangeProduct(Product p)
        {
            p.ProductArticleNumber = textBoxArticle.Text;
            p.ProductName = textBoxProductName.Text;
            p.ProductDescription = textBoxDescription.Text;
            p.CategoryID = (int)comboBoxCategory.SelectedIndex + 1;
            p.ManufacturerID = (int)comboBoxManufacturer.SelectedIndex + 1;
            p.ProductCost = decimal.Parse(textBoxCost.Text);
            p.ProductDiscountAmount = int.Parse(textBoxDiscount.Text);
            p.ProductQuantityInStock = int.Parse(textBoxOnStore.Text);
            p.UnitID = (int)comboBoxUnit.SelectedIndex + 1;
            p.ProductMaxDiscountAmount = int.Parse(textBoxMaxDicsount.Text);
            
            if (!isDeleted)
            {
                BitmapImage bitmapImage = imageProductPhoto.Source as BitmapImage;
                MemoryStream memStream = new MemoryStream();
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(memStream);
                byte[] data = memStream.ToArray();
                p.ProductPhoto = data;
            }
            else { p.ProductPhoto = null; }          
            return p;
        }

        private void SaveChanges()
        {
            try
            {
                Helper.ContextFish.SaveChanges();
                MessageBox.Show("Изменения успешно сохранены");
            }
            catch (DbEntityValidationException ex)
            {
                MessageBox.Show("При обновлении данных в БД возникла ошибка");
                foreach (DbEntityValidationResult validationError in ex.EntityValidationErrors)
                {
                    MessageBox.Show("Object: " + validationError.Entry.Entity.ToString());
                    foreach (DbValidationError err in validationError.ValidationErrors)
                    {
                        MessageBox.Show(err.ErrorMessage + "");
                    }
                }
                return;
            }
            catch
            {
                MessageBox.Show("Неизвестная ошибка");
                return;
            }
            Helper.Product = null;
            Catalog catalog = new Catalog();
            catalog.Show();
            this.Close();
        }

        private void buttonChangeImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                isDeleted = false;
                string path = openFileDialog.FileName;
                imageProductPhoto.Source = new BitmapImage(new Uri(path, UriKind.Absolute)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            }
        }

        private void butonDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить изображение из базы данных?",
                    "Удаление изображения",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                isDeleted = true;
                imageProductPhoto.Source = new BitmapImage(new Uri("/Resources/logo.png", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            }
                
        }

        private void buttonDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить этот товар?",
                "Удаление товара",
                MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (string.IsNullOrEmpty(textBoxArticle.Text))
                {
                    MessageBox.Show("Товар для удаления не найден");
                    return;
                }
                var productDelete = Helper.ContextFish.Product.Find(textBoxArticle.Text);
                Helper.ContextFish.Product.Remove(productDelete);
                try
                {
                    Helper.ContextFish.SaveChanges();
                    MessageBox.Show("Товар успешно удален");
                    Helper.Product = null;
                    Catalog catalog = new Catalog();
                    catalog.Show();
                    this.Close();
                }
                catch
                {
                    MessageBox.Show("Товар нельзя удалить, т.к. он используется в заказах");
                    User u = Helper.User;
                    Product p = Helper.Product;
                    List<OrderProduct> list = Helper.ListOrderProduct;
                    Helper.ContextFish = new ContextFish();
                    Helper.Product = p;
                    Helper.User = u;
                    Helper.ListOrderProduct = list;
                }
            }            
        }
    }
}
