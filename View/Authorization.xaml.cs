using GoldFish.Classes;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace GoldFish.View
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        private string text = String.Empty;
        int attempt = 0;
        DispatcherTimer dispatcherTimer;

        public Authorization()
        {
            InitializeComponent();
        }

        private void buttonGuest_Click(object sender, RoutedEventArgs e)
        {
            Catalog window = new Catalog();
            window.Show();
            this.Close();
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonAuthorize_Click(object sender, RoutedEventArgs e)
        {
            string login = textBoxLogin.Text;
            string password = textBoxPassword.Password;

            Helper.User = Helper.ContextFish.User
                .Where(x => x.UserLogin == login && x.UserPassword == password)
                .FirstOrDefault();

            if (Helper.User != null && textBoxCaptcha.Text == this.text)
            {
                MessageBox.Show("Авторизация пройдена");
                MessageBox.Show($"Вы зашли под ролью {Helper.User.Role.RoleName}");
                Catalog window = new Catalog();
                window.Show();
                this.Close();

            }
            else if (attempt == 0)
            {
                MessageBox.Show("Пользователь не найден");
                MessageBox.Show("Для следующей попытки входа необходимо ввести капчу");
                CreateImageCatpcha();
                buttonOtherPicture.Visibility = Visibility.Visible;
                imageCaptcha.Visibility = Visibility.Visible;
                textBoxCaptcha.Visibility = Visibility.Visible;
                attempt++;
            }
            else
            {
                MessageBox.Show("Данные введены неверно. Вы заблокированы в системе на 10 секунд");
                this.IsEnabled = false;
                dispatcherTimer = new DispatcherTimer();
                dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
                dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
                dispatcherTimer.Start();
                CreateImageCatpcha();
            }
        }

        private void Authorization_Loaded(object sender, RoutedEventArgs e)
        {
            Helper.ContextFish = new Models.ContextFish();
            textBoxLogin.Text = "petrov@namecomp.ru";
            textBoxPassword.Password = "uzWC67";
        }

        private void CreateImageCatpcha()
        {
            imageCaptcha.Source = Captcha.CreateCaptcha();
            text = Captcha.GetText();
            textBoxCaptcha.Text = text;
        }

        private void buttonOtherPicture_Click(object sender, RoutedEventArgs e)
        {
            CreateImageCatpcha();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            dispatcherTimer.Stop();
            MessageBox.Show("Время вышло");
            this.IsEnabled = true;
        }
    }
}
