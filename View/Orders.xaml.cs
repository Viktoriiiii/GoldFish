using GoldFish.Classes;
using GoldFish.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Word = Microsoft.Office.Interop.Word;


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
            if (!buttonGetCheck.IsEnabled)
            {
                Helper.ListOrderProduct = null;
                Helper.Order = null;
            }
            Catalog window = new Catalog();
            window.Show();
            this.Close();
        }

        private void windowOrders_Loaded(object sender, RoutedEventArgs e)
        {
            if (Helper.User != null)
                labelNameClient.Content = $"{Helper.User.UserFullName}";
            ShowOrder();            
        }

        private void ShowOrder()
        {
            dataGridOrderProduct.ItemsSource = Helper.ListOrderProduct.ToList();
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
                Helper.Order = newOrder;
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
            buttonGetCheck.IsEnabled = true;
            buttonPlaceAnOrder.IsEnabled = false;
        }

        private void buttonGetCheck_Click(object sender, RoutedEventArgs e)
        {
            if (Helper.ListOrderProduct.Count == 0)
            {
                MessageBox.Show("Нет товаров для формирования заказа");
                return;
            }

            Word.Application wordApp = new Word.Application();
            Word.Document wordDoc = wordApp.Documents.Add();

            Word.Paragraph wordParagraphTitle = wordDoc.Paragraphs.Add();
            Word.Range wordRangeTitel = wordParagraphTitle.Range;
            wordRangeTitel.Text = $"        Талон заказа № { Helper.Order.OrderCodeForGetOrder}";
            wordRangeTitel.Font.Bold = 1;
            wordRangeTitel.Font.Size = 20;
            Word.InlineShape wordShape = wordDoc.InlineShapes.
                        AddPicture(AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Resources\logo.png", Type.Missing, Type.Missing, wordRangeTitel);
            wordShape.Width = 50;
            wordShape.Height = 50;
            wordRangeTitel.ParagraphFormat.Alignment =
                                       Word.WdParagraphAlignment.wdAlignParagraphCenter;
            wordRangeTitel.InsertParagraphAfter();

            Word.Paragraph wordParagraphDesc = wordDoc.Paragraphs.Add();
            Word.Range wordRangeDesc = wordParagraphDesc.Range;
            wordRangeDesc.Text = $"Дата заказа: {Helper.Order.OrderDate.ToShortDateString()} {Helper.Order.OrderTime:hh\\:mm\\:ss}{Environment.NewLine}";
            wordRangeDesc.Text += $"Номер заказа: {Helper.Order.OrderID} {Environment.NewLine}";
            wordRangeDesc.Font.Bold = 0;
            wordRangeDesc.Font.Size = 12;
            wordRangeDesc.ParagraphFormat.Alignment =
                                Word.WdParagraphAlignment.wdAlignParagraphLeft;

            Word.Paragraph wordParagraphTable = wordDoc.Paragraphs.Add();
            Word.Range wordRangeTable = wordParagraphTable.Range;
            Word.Table wordTable = wordDoc.Tables.Add(wordRangeTable, Helper.ListOrderProduct.Count + 1, 2);
            wordTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleDouble;
            wordTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;

            Word.Range wordRangeCell;
            wordRangeCell = wordTable.Cell(1, 1).Range;
            wordRangeCell.Text = "Товар";
            wordRangeCell = wordTable.Cell(1, 2).Range;
            wordRangeCell.Text = Environment.NewLine + "Количество";
            wordTable.Rows[1].Range.Bold = 1;
            wordTable.Rows[1].Range.Font.Size = 14;
            wordTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            int indexTab = 2;            

            foreach(var d in Helper.ListOrderProduct)
            {
                wordRangeCell = wordTable.Cell(indexTab, 1).Range;
                wordRangeCell.Text = d.Product.ProductName;
                wordRangeCell = wordTable.Cell(indexTab, 2).Range;
                wordRangeCell.Text = d.OrderProductCount.ToString();
                indexTab++;
            }

            wordRangeTable.InsertParagraphAfter();

            Word.Paragraph wordParagraphResult = wordDoc.Paragraphs.Add();
            Word.Range wordRangeResult = wordParagraphResult.Range;

            wordRangeResult.Text = $"{textBoxDescriptionOrder.Text} {Environment.NewLine}";

            wordRangeResult.Text += $"Пункт выдачи: {comboBoxPoint.Text}{Environment.NewLine}";
            wordRangeResult.Text += $"Дата получения: {Helper.Order.OrderDeliveryDate.ToLongDateString()} {Environment.NewLine}";
            wordRangeResult.ParagraphFormat.Alignment =
                                          Word.WdParagraphAlignment.wdAlignParagraphLeft;
            wordRangeResult.InsertParagraphAfter();

            string fullNameFile = $"{AppDomain.CurrentDomain.BaseDirectory}{Helper.Order.OrderCodeForGetOrder}.pdf";
            wordDoc.Saved = true;
            if (File.Exists(fullNameFile))
            {
                File.Delete(fullNameFile);
            }
            wordDoc.SaveAs(fullNameFile, Word.WdExportFormat.wdExportFormatPDF);
            wordDoc.Close(true, null, null);
            MessageBox.Show("Талон создан успешно");
            wordApp.Quit();

            releaseObject(wordDoc);	
            releaseObject(wordApp);

            Helper.ListOrderProduct = null;
            Helper.Order = null;
            this.Close();
        }

        public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Не могу освободить объект " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }

        private void buttonDeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            int index = dataGridOrderProduct.SelectedIndex;
            Helper.ListOrderProduct.RemoveAt(index);
            listDescriptionProduct.Items.Clear();
            if (Helper.ListOrderProduct.Count == 0)
            {
                MessageBox.Show("Удален последний товар из заказа. Выберите пожалуйста товары снова.");
                Catalog catalog = new Catalog();
                catalog.Show();
                this.Close();
            }
            else
                ShowOrder();
        }

        private void buttonAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOrderProduct.SelectedItem is OrderProduct orderProduct)
            {
                orderProduct.OrderProductCount++;
                ShowOrder();
            }
        }

        private void buttonReduceProduct_Click(object sender, RoutedEventArgs e)
        {
            if (dataGridOrderProduct.SelectedItem is OrderProduct orderProduct)
            {
                orderProduct.OrderProductCount--;
                if (orderProduct.OrderProductCount == 0)
                {
                    MessageBox.Show("Для удаления товара нажмите на кнопку удаления.");
                    orderProduct.OrderProductCount = 1;
                }
                ShowOrder();
            }
        }
    }
}
