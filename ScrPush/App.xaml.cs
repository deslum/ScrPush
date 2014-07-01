using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Documents;
using System.Data;
using System.Linq;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using Hook;
using YDisk;


using Menu_Item = System.Windows.Forms.ContextMenu;
using MI = System.Windows.Forms.MenuItem;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace ScrPush
{
    public partial class App : Application
    {
        public static NotifyIcon icon = new NotifyIcon();
        static Hook.Hook hook = new Hook.Hook();
        private static Menu_Item m_menu;
        private static ImageCodecInfo jgpEncoder;
        static Settings setdlg = new Settings();

        static String getDate()
        {
            DateTime now = DateTime.Now;
            return now.Day +""+ now.Month +""+ now.Year +""+ now.Hour +""+ now.Minute +""+ now.Second;
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


        public static void ScrPrint()
        {
            if (YDisk.YDisk.GetToken() != String.Empty)
            {
                jgpEncoder = GetEncoder(ImageFormat.Jpeg);
                System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
                EncoderParameters myEncoderParameters = new EncoderParameters(1);
                EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;
                int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height + 20;
                using (Bitmap btm = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(btm))
                    {
                        string s = "ScrPush beta http://www.deslum.com";
                        Font drawFont = new Font("Times New Roman", 18);
                        SolidBrush drawBrush = new SolidBrush(System.Drawing.Color.GreenYellow);

                        g.CopyFromScreen(System.Windows.Forms.Screen.PrimaryScreen.Bounds.X,
                            System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y, 0, 0, btm.Size, CopyPixelOperation.SourceCopy);
                        g.DrawString(s, drawFont, drawBrush, new System.Drawing.Point(width - 360, btm.Size.Height - 25));
                    }
                    String date = getDate() + ".jpeg";
                    btm.Save(setdlg.textBox1.Text +'\\'+ date, jgpEncoder, myEncoderParameters);
                    YDisk.YDisk.Put(setdlg.textBox1.Text +'\\' + date);
                    String lnk = YDisk.YDisk.getLink(date);
                    icon.BalloonTipText = lnk;
                    icon.BalloonTipTitle = "Ссылка на файл";
                    icon.ShowBalloonTip(10);
                    Clipboard.SetDataObject(lnk);
                    
                }
            }
            else
                MessageBox.Show("Пожалуйста зарегистрируйтесь", "ScrPush");
            
        }

        public static void EnabledItem(bool b)
        {
            m_menu.MenuItems[0].Enabled = b;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            bool onlyInstance;
            Mutex mtx = new Mutex(true, "ScrPush",out onlyInstance);
            if (onlyInstance)
            {
                hook.SetHook();
                icon.Icon = ScrPush.Properties.Resources.Trico;
                icon.Visible = true;
                m_menu = new Menu_Item();
                
                m_menu.MenuItems.Add(0, new MI("Регистрация", new System.EventHandler(Registration_Click)));
                m_menu.MenuItems.Add(1, new MI("Сделать снимок", new System.EventHandler(ScreenShot_Click)));
                m_menu.MenuItems.Add(2, new MI("Настройки", new System.EventHandler(Settings_Click)));
                m_menu.MenuItems.Add(3, new MI("-"));
                m_menu.MenuItems.Add(4, new MI("Домашняя страница", new System.EventHandler(Site_Click)));
                m_menu.MenuItems.Add(5, new MI("О программе", new System.EventHandler(About_Click)));
                m_menu.MenuItems.Add(6, new MI("Выход", new System.EventHandler(Exit_Click)));
                icon.ContextMenu = m_menu;
                String token = ScrPush.Properties.Settings.Default.Veterok;
                if (token == String.Empty)
                {
                    var result = MessageBox.Show("У Вас есть учетная запись Яндекса?", "ScrPush", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        Process.Start("https://disk.yandex.ru/invite/?hash=GUNNFOVX");
                        Environment.Exit(0);
                    }
                    else
                    {
                            Authorization auth = new Authorization();
                            auth.Show();
                            EnabledItem(false);
                    }

                }
                else
                {
                    YDisk.YDisk.SetToken(token);           
                    String answer = "OK";
                    if (answer != String.Empty)
                    {
                        icon.BalloonTipText = "Добро пожаловать ";
                        icon.BalloonTipTitle = "ScrPush";
                        icon.ShowBalloonTip(2);
                    }
                    
                    base.OnStartup(e);
                    
                }
            }
            else
            {
                MessageBox.Show("Приложение уже запущено","ScrPush");
                Environment.Exit(0);
            }
        }

        protected void Registration_Click(Object sender, System.EventArgs e)
        {
            Authorization auth = new Authorization();
            auth.Show();
            EnabledItem(false);
        }

        protected void ScreenShot_Click(Object sender, System.EventArgs e)
        {
            ScrPrint();
        }
        protected void Settings_Click(Object sender, System.EventArgs e)
        {
            setdlg.Show();
        }

        protected void Site_Click(Object sender, System.EventArgs e)
        {
            Process.Start("http://deslum.com/");
        }

        protected void About_Click(Object sender, System.EventArgs e)
        {


           About about = new About();

            about.Show();
        }

        private void Exit_Click(Object sender, EventArgs e)
        {
            Environment.Exit(0);
        }


        protected void icon_Click(Object sender, System.EventArgs e)
        {
            
        }
    }
}
