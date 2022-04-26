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

namespace SmartKioskApp.Views
{
    /// <summary>
    /// Interaction logic for PreviewOrder.xaml
    /// </summary>
    public partial class PreviewOrder : Window
    {
        public bool checkedOut = false;
        public bool addAnotherItem = false;
        public bool OrderCanelled = false;

        public PreviewOrder()
        {
            InitializeComponent();
            //Cart cart = new Cart();
            //prevOrder.ItemsSource = cart.LoadCollectionData();
            //ItemMenu itemMenu = new ItemMenu();
            MenuViewModel cartViewModel = new MenuViewModel();
            //prevOrder.ItemsSource = cartViewModel.myCart;
            Models.Cart cart = new Models.Cart();
            cartViewModel.LoadCart();
            cartViewModel.LoadCategory();
            cartViewModel.LoadIcons();

            itemName.Visibility = Visibility.Visible;
            itemPortion.Visibility = Visibility.Visible;
            itemQuantity.Visibility = Visibility.Visible;
            itemPrice.Visibility = Visibility.Visible;
            itemTotalPrice.Visibility = Visibility.Visible;

            imgUpdateCart.Source = ItemMenu.BitmaSourceFromByteArray(cartViewModel.myIcons[7].Icon);

        }

        private void btnCheckOut(object sender, RoutedEventArgs e)
        {
            try
            {
                checkedOut = true;
                this.Hide();

            }
            catch (Exception ex)
            {

            }
        }

        public void btnAddAnother(object sender, RoutedEventArgs e)
        {
            checkedOut = false;
            addAnotherItem = true;
            this.Close();

        }
        public void btnCancelOrder(object sender, RoutedEventArgs e)
        {
            CancelWarning cancelWarning = new CancelWarning();
            //this.Opacity = 0.9;
            cancelWarning.ShowDialog();
            //this.Opacity = 1.5;
            if (cancelWarning.isCancelled)
            {
                OrderCanelled = true;
                this.Close();
                checkedOut = false;

            }


        }
        
    }
}

