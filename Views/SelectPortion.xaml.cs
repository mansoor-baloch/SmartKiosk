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

                var uri = new Uri("pack://application:,,,/Icons/back.png");
                backIcon.Source = new BitmapImage(uri);

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
                var uri1 = new Uri("pack://application:,,,/Icons/quater portion.png");
                imgQuarter.Source = new BitmapImage(uri1);

                var uri2 = new Uri("pack://application:,,,/Icons/half portion.png");
                imgHalf.Source = new BitmapImage(uri2);

                var uri3 = new Uri("pack://application:,,,/Icons/full portion 1.png");
                imgStandard.Source = new BitmapImage(uri3);

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


        private void btnBack(object sender, RoutedEventArgs e)
        {
            IsBack = true;
            this.Close();

        }
    }
}
