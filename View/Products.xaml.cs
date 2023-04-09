using GoldFish.Classes;
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
    /// Логика взаимодействия для Products.xaml
    /// </summary>
    public partial class Products : Window
    {
        public Products()
        {
            InitializeComponent();
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
            }
            else
            {
                labelTitle.Content = "Редактирование товара";
            }
        }
    }
}
