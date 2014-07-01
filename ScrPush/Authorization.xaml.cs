using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YDisk;

namespace ScrPush
{
    /// <summary>
    /// Логика взаимодействия для Authorization.xaml
    /// </summary>
    public partial class Authorization : Window
    {
        public Authorization()
        {
            InitializeComponent();
        }

        private void enter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                String login = loginBox.Text;
                String pass = passBox.Password;
                if (login != String.Empty && pass != String.Empty && login != "example@yandex.ru")
                {
                    String token = YDisk.YDisk.Login(loginBox.Text, passBox.Password);
                    Properties.Settings.Default.Veterok = token;
                    Properties.Settings.Default.Save();
                    ScrPush.App.EnabledItem(true);
                    this.Hide();
                    YDisk.YDisk.MkDir();
                    
                }
                else
                    MessageBox.Show("Не введен логин или пароль", "ScrPush");
            }
            catch
            {

            }
        }

        private void Close(Object sender, EventArgs e)
        {
            ScrPush.App.EnabledItem(true);
        }

        private void loginBox_GotFocus(Object sender, EventArgs e)
        {
            loginBox.Foreground = Brushes.Black;
            loginBox.FontStyle = FontStyles.Normal;
            loginBox.Clear();
            
        }
    }
}
