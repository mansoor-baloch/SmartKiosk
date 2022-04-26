using SmartKioskApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace SmartKioskApp.Views
{
    /// <summary>
    /// Interaction logic for SelectPortion.xaml
    /// </summary>
    public partial class SelectPortion : Window
    {
        public SelectPortion()
        {
            try
            {

                InitializeComponent();

                MenuViewModel menuViewModelObject = new MenuViewModel();
                menuViewModelObject.LoadCategory();
                menuViewModelObject.LoadIcons();
                backIcon.Source = BitmaSourceFromByteArray(menuViewModelObject.myIcons[6].Icon);
                
                txtQuarter.Visibility = Visibility.Visible;
                txtHalf.Visibility = Visibility.Visible;
                txtStandard.Visibility = Visibility.Visible;
                secHalf.Visibility = Visibility.Visible;
                secQuarter.Visibility = Visibility.Visible;
                priceQuarter.Visibility = Visibility.Visible;
                priceMedium.Visibility = Visibility.Visible;
                priceStandard.Visibility = Visibility.Visible;

                if (!ItemMenu.HasQuarter )
                {
                    secQuarter.Visibility = Visibility.Hidden;
                    txtQuarter.Visibility = Visibility.Hidden;
                    priceQuarter.Visibility = Visibility.Hidden;
                    secHalf.Margin = new Thickness(-180, 0, 100, 0);
                    secHalf.Width = 250;
                    txtHalf.Margin = new Thickness(-80, 0, 100, 0);
                    priceMedium.Margin = new Thickness(-140, 0, 100, 0);

                    secStandard.Margin = new Thickness(-80, 0, 30, 0);
                    secStandard.Width = 250;
                    txtStandard.Margin = new Thickness(-3, 0, 0, 0);
                    priceStandard.Margin = new Thickness(-70, 0, 0, 0);

                }
                if (!ItemMenu.HasHalf)
                {
                    secHalf.Visibility = Visibility.Hidden;
                    txtHalf.Visibility = Visibility.Hidden;
                    priceMedium.Visibility = Visibility.Hidden;
                }
                imgQuarter.Source = BitmaSourceFromByteArray(menuViewModelObject.myIcons[2].Icon);
                imgHalf.Source = BitmaSourceFromByteArray(menuViewModelObject.myIcons[3].Icon);
                imgStandard.Source = BitmaSourceFromByteArray(menuViewModelObject.myIcons[4].Icon);

                StartCloseTimer();

            }
            catch (Exception ex)
            {

            }

        }
        public bool quarterClicked = false;
        public bool mediumClicked = false;
        public bool standardClicked = false;
        public string portionClicked = "";
        public bool IsBack = false;
        public void btnQuarter(object sender, RoutedEventArgs e)
        {
            try
            {
                portionClicked = "Quarter";
                quarterClicked = true;

                this.Close();

            }
            catch (Exception ex)
            {

            }
        }
        private void btnMedium(object sender, RoutedEventArgs e)
        {
            portionClicked = "Half";
            mediumClicked = true;
            this.Close();
        }
        private void btnStandard(object sender, RoutedEventArgs e)
        {
            portionClicked = "Standard";
            standardClicked = true;
            this.Close();
        }
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(20d);
            timer.Tick += TimerTick;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Close();
            this.Close();
        }
        public static BitmapSource BitmaSourceFromByteArray(byte[] buffer)
        {
            var bitmap = new BitmapImage();

            using (var stream = new MemoryStream(buffer))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            bitmap.Freeze(); // optionally make it cross-thread accessible
            return bitmap;
        }

        private void btnBack(object sender, RoutedEventArgs e)
        {
            IsBack = true;
            this.Close();

        }
    }
}
