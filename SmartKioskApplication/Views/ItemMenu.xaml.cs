using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Drawing.Printing;
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
using SmartKioskApp.ViewModels;
using System.Printing;
using PrinterUtility;
using Path = System.IO.Path;

namespace SmartKioskApp.Views
{
    /// <summary>
    /// Interaction logic for ItemMenu.xaml
    /// </summary>
    public partial class ItemMenu : Window
    {
        public bool clickedCat1 = false;
        public Models.Menu _item { get; set; }
        int Qty = 0;
        public int sec;
        public static List<Models.Cart> carts = new List<Models.Cart>();
        public static List<Models.OrderSummary> orders = new List<Models.OrderSummary>();
        public static List<Models.Menu> menus = new List<Models.Menu>();

        public int itemPrice = 0;
        public static string OrderNo = "";
        public static string OrderDateTime;
        public static int QRdueAmount;
        public static int CashDueAmount;

        public static bool PaymentCompleted = false;
        public static bool HasQuarter = false;
        public static bool HasHalf = false;


        SqlCommand cmd = null;
        private string sql = null;
        //public static string ConnectionString = "Integrated Security=SSPI;" + "Initial Catalog=LocDBKiosk;" + "Data Source=localhost;";
        public static string ConnectionString = ConfigurationManager.AppSettings["LocalCon"].ToString();
        private SqlConnection conn = null;
        SqlDataReader reader;

        MenuViewModel menuViewModel = new MenuViewModel();
        public bool HasPortion = false;
        public ItemMenu()
        {
            try
            {
                InitializeComponent();
                tBtnCat1.IsChecked = true;
                tBtnCat2.IsChecked = false;
                tBtnCat3.IsChecked = false;
                tBtnCat3.Opacity = 0.4;
                tBtnCat2.Opacity = 0.4;
                tBtnCat1.Opacity = 2;

                menuViewModel.LoadCategory();
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);

                var uri = new Uri("pack://application:,,,/Icons/cart.jpg");
                imgCart.Source = new BitmapImage(uri);


                menuViewModel.CountCategories();
                DisplayCategories();
                DisplayMenuItems();

            }
            catch (Exception ex)
            {

            }
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

        private void btnCategory1(object sender, RoutedEventArgs e)
        {


            try
            {
                scrol.ScrollToTop();
                scrol.UpdateLayout();
                if (tBtnCat1.IsChecked == true)
                {
                    tBtnCat3.Opacity = 0.4;
                    tBtnCat2.Opacity = 0.4;
                    tBtnCat1.Opacity = 2;
                    tBtnCat2.IsChecked = false;
                    tBtnCat3.IsChecked = false;

                }
                if (tBtnCat1.IsChecked == false)
                {
                    tBtnCat1.IsChecked = true;
                    tBtnCat3.Opacity = 0.4;
                    tBtnCat2.Opacity = 0.4;
                    tBtnCat1.Opacity = 2;
                    tBtnCat2.IsChecked = false;
                    tBtnCat3.IsChecked = false;
                }

                menuViewModel.LoadCategory();
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);

                var uri = new Uri("pack://application:,,,/Icons/cart.jpg");
                imgCart.Source = new BitmapImage(uri);

                menuViewModel.CountCategories();
                DisplayCategories();
                DisplayMenuItems();



            }
            catch (Exception ex)
            {

            }
        }

        private void btnCategory2(object sender, RoutedEventArgs e)
        {

            try
            {
                scrol.ScrollToTop();
                scrol.UpdateLayout();
                if (tBtnCat2.IsChecked == true)
                {
                    tBtnCat3.Opacity = 0.4;
                    tBtnCat1.Opacity = 0.4;
                    tBtnCat2.Opacity = 2;
                    tBtnCat1.IsChecked = false;
                    tBtnCat3.IsChecked = false;
                }
                if (tBtnCat2.IsChecked == false)
                {
                    tBtnCat2.IsChecked = true;
                    tBtnCat3.Opacity = 0.4;
                    tBtnCat1.Opacity = 0.4;
                    tBtnCat2.Opacity = 2;
                    tBtnCat1.IsChecked = false;
                    tBtnCat3.IsChecked = false;
                }

                menuViewModel.LoadCategory();
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);

                var uri = new Uri("pack://application:,,,/Icons/cart.jpg");
                imgCart.Source = new BitmapImage(uri);
                menuViewModel.CountCategories();
                DisplayCategories();

                DisplayMenuItems();
            }
            catch (Exception ex)
            {

            }
        }

        public void btnCategory3(object sender, RoutedEventArgs e)
        {
            try
            {
                scrol.ScrollToTop();
                scrol.UpdateLayout();
                if (tBtnCat3.IsChecked == true)
                {
                    tBtnCat1.Opacity = 0.4;
                    tBtnCat2.Opacity = 0.4;
                    tBtnCat3.Opacity = 2;
                    tBtnCat2.IsChecked = false;
                    tBtnCat1.IsChecked = false;
                }
                if (tBtnCat3.IsChecked == false)
                {
                    tBtnCat3.IsChecked = true;
                    tBtnCat1.Opacity = 0.4;
                    tBtnCat2.Opacity = 0.4;
                    tBtnCat3.Opacity = 2;
                    tBtnCat2.IsChecked = false;
                    tBtnCat1.IsChecked = false;
                }


                menuViewModel.LoadCategory();
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);

                var uri = new Uri("pack://application:,,,/Icons/cart.jpg");
                imgCart.Source = new BitmapImage(uri);
                menuViewModel.CountCategories();

                DisplayCategories();
                DisplayMenuItems();


            }
            catch (Exception ex)
            {

            }

        }
        public void DisplayCategories()
        {
            try
            {
                menuViewModel.LoadCategory();
                if (MenuViewModel.CategoriesCount >= 3)
                {
                    txtCat1.DataContext = menuViewModel.Categories[0];
                    txtCat2.DataContext = menuViewModel.Categories[1];
                    txtCat3.DataContext = menuViewModel.Categories[2];

                    imgCat1.Source = BitmaSourceFromByteArray(menuViewModel.Categories[0].CatIcon);
                    imgCat2.Source = BitmaSourceFromByteArray(menuViewModel.Categories[1].CatIcon);
                    imgCat3.Source = BitmaSourceFromByteArray(menuViewModel.Categories[2].CatIcon);
                    txtCategory1.Visibility = Visibility.Visible;
                    txtCategory2.Visibility = Visibility.Visible;
                    txtCategory3.Visibility = Visibility.Visible;
                }
                else if (MenuViewModel.CategoriesCount == 2)
                {
                    txtCat1.DataContext = menuViewModel.Categories[0];
                    txtCat2.DataContext = menuViewModel.Categories[1];

                    imgCat1.Source = BitmaSourceFromByteArray(menuViewModel.Categories[0].CatIcon);
                    imgCat2.Source = BitmaSourceFromByteArray(menuViewModel.Categories[1].CatIcon);
                    txtCategory1.Visibility = Visibility.Visible;
                    txtCategory2.Visibility = Visibility.Visible;
                    txtCategory3.Visibility = Visibility.Hidden;
                }
                else if (MenuViewModel.CategoriesCount == 1)
                {
                    txtCat1.DataContext = menuViewModel.Categories[0];
                    imgCat1.Source = BitmaSourceFromByteArray(menuViewModel.Categories[0].CatIcon);
                    txtCategory1.Visibility = Visibility.Visible;
                    txtCategory2.Visibility = Visibility.Hidden;
                    txtCategory3.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {

            }
            
        }
       
        private void btnCheckout(object sender, RoutedEventArgs e)

        {
            try
            {
                bool PayAgain = false;
                PreviewOrder previewOrder = new PreviewOrder();
                MenuViewModel menuViewModel = new MenuViewModel();

                previewOrder.prevOrder.ItemsSource = carts;
                
                
                do
                {
                    orders.Add(new Models.OrderSummary { DueAmount = 0, InsertedAmount = 0, RemainingAmount = 0 });
                    ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                    CheckOut checkOut = new CheckOut();
                    Global.cartTotalAmount = Convert.ToInt32(CalTotalAmount());
                    previewOrder.txtTotalAmount.DataContext = orders[0];
                    CalTotalPrice();
                    previewOrder.ShowDialog();
                    //this.Opacity = 1.5;
                    PayAgain = false;
                    //generate a new order number and store the cart data in DB
                    if (previewOrder.checkedOut)
                    {
                        ReadWrite.Write(DateTime.Now.ToString("MM/dd/yyyy"), Global.Actions.todaysDate.ToString());
                        ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                        checkOut.txtTotalAmount.DataContext = orders[0];
                        checkOut.txtTotalAmount1.DataContext = orders[0];
                        QRdueAmount = orders[0].DueAmount + Convert.ToInt32(orders[0].DueAmount * 0.05);
                        checkOut.txtDueAmount.Text = QRdueAmount.ToString();
                        checkOut.txtTotalAmnt.Text = QRdueAmount.ToString();
                        checkOut.txtCreditAmount.DataContext = orders[0];
                        checkOut.txtQRTotal.Text = QRdueAmount.ToString();

                        GenerateOrderNo();
                        orders[0].OrderNo = OrderNo;
                        ReadWrite.Write(OrderNo.ToString(), Global.Actions.OrderNo.ToString());
                        //InsertOrdersTable();
                        checkOut.ShowDialog();
                        
                        if (checkOut.PaymentConfirmed)
                        {
                            PaymentCompleted = true;
                            ItemMenu.orders[0].InsertedAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                            checkOut.txtCreditAmount.DataContext = orders[0];
                            ItemMenu.orders[0].TicketNumber = CheckOut.GenNewTicketNo();
                            ItemMenu.orders[0].RemainingAmount = ItemMenu.orders[0].InsertedAmount - ItemMenu.orders[0].DueAmount;
                            ItemMenu.InsertOrdersTable();

                            InsertCartTable();

                            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                            PrintReceipt();
                            CashDueAmount = orders[0].DueAmount;
                            CancelOrder();
                            HideDisplayButtons();
                            OrderReceipt orderReceipt = new OrderReceipt();
                            orderReceipt.txtRemainAmount.DataContext = orders[0];
                            orderReceipt.txtTicketNo.DataContext = orders[0];
                            orderReceipt.DataContext = orders[0];
                            orderReceipt.ShowDialog();
                            PostData.CreateJSON();
                            orders[0].DueAmount = 0;
                            PaymentCompleted = false;
                        }
                        if (checkOut.QRPayConfirmed)
                        {
                            PaymentCompleted = true;
                            ItemMenu.orders[0].TicketNumber = CheckOut.GenNewTicketNo();
                            ItemMenu.orders[0].InsertedAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                            ItemMenu.orders[0].DueAmount = QRdueAmount;
                            ItemMenu.InsertOrdersTable();
                            InsertCartTable();

                            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                            PostData.CreateJSON();
                            OrderReceipt orderReceipt = new OrderReceipt();
                            orderReceipt.txtTicketNo.DataContext = orders[0];
                            PrintReceipt();
                            CancelOrder();
                            HideDisplayButtons();
                            orderReceipt.ShowDialog();
                            checkOut.CloseThisScreen = false;
                            orders[0].DueAmount = 0;
                            PaymentCompleted = false;
                        }
                        if (checkOut.CardPayConfirmed)
                        {
                            PaymentCompleted = true;
                            ItemMenu.orders[0].TicketNumber = CheckOut.GenNewTicketNo();
                            ItemMenu.orders[0].InsertedAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                            ItemMenu.orders[0].DueAmount = QRdueAmount;
                            ItemMenu.InsertOrdersTable();
                            InsertCartTable();

                            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                            PostData.CreateJSON();
                            OrderReceipt orderReceipt = new OrderReceipt();
                            orderReceipt.txtTicketNo.DataContext = orders[0];
                            PrintReceipt();
                            CancelOrder();
                            HideDisplayButtons();
                            orderReceipt.ShowDialog();
                            checkOut.CloseThisScreen = false;
                            orders[0].DueAmount = 0;
                            PaymentCompleted = false;
                        }
                        if (checkOut.CloseThisScreen)
                        {
                            PaymentCompleted = false;
                            ItemMenu.orders[0].InsertedAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                            checkOut.txtCreditAmount.DataContext = orders[0];
                            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());

                            CashDueAmount = orders[0].DueAmount;
                            if (ItemMenu.orders[0].InsertedAmount > 0)
                            {

                                ItemMenu.orders[0].TicketNumber = CheckOut.GenNewTicketNo();
                                ItemMenu.orders[0].RemainingAmount = ItemMenu.orders[0].DueAmount - ItemMenu.orders[0].InsertedAmount;
                                ItemMenu.InsertOrdersTable();
                                InsertCartTable();
                                PrintReceipt();
                                CancelOrder();
                                HideDisplayButtons();
                                OrderReceipt orderReceipt = new OrderReceipt();
                                orderReceipt.txtRemainAmount.DataContext = orders[0];
                                orderReceipt.txtTicketNo.DataContext = orders[0];
                                orderReceipt.DataContext = orders[0];
                                orderReceipt.ShowDialog();
                                PostData.CreateJSON();
                            }
                            CancelOrder();
                            HideDisplayButtons();
                            checkOut.CloseThisScreen = false;
                            orders[0].DueAmount = 0;
                        }
                        if (checkOut.IsBack)
                        {
                            orders[0].DueAmount = 0;
                            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                            PayAgain = true;
                            Global.General.CreditAmount = 0;
                        }
                        if (!PayAgain)
                        {
                            
                            orders[0].insertedAmount = 0;
                            orders[0].remainingAmount = 0;
                            QRdueAmount = 0;
                            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                            orders.Clear();
                        }
                    }
                    if (previewOrder.addAnotherItem)
                    {
                        orders[0].DueAmount = 0;
                        checkOut.Close();
                        ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
                        
                    }
                    if (previewOrder.OrderCanelled)
                    {
                        checkOut.Close();
                        CancelOrder();
                    }
                } while (PayAgain);
                
            }
            catch (Exception ex)
            {

            }

        }
        public void InsertCartTable()
        {
            conn = new SqlConnection(ConnectionString);
            conn.ConnectionString = ConnectionString;
            for (int i = 0; i < carts.Count; i++)
            {
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO tblCart (OrderNo, ItemName, Portion, Price ,  Quantity, DateTime ) VALUES (@OrderNo, @ItemName, @Portion, @Price ,  @Quantity, @DateTime )", conn))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.AddWithValue("@OrderNo", orders[0].OrderNo);
                        cmd.Parameters.AddWithValue("@ItemName", carts[i].Name);
                        cmd.Parameters.AddWithValue("@Portion", carts[i].Portion);
                        cmd.Parameters.AddWithValue("@Price", carts[i].UnitPrice);
                        cmd.Parameters.AddWithValue("@Quantity", carts[i].Quantity);
                        cmd.Parameters.AddWithValue("@DateTime", DateTime.Now);
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }

        }
        public static void InsertOrdersTable()
        {
            OrderDateTime = Convert.ToString(DateTime.Now);
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO tblTransactionSummary (OrderNo, InsertedAmount , DueAmount,  RemainingAmount, TicketNumber, OrderDateTime, PaymentType) VALUES (@OrderNo,  @InsertedAmount, @DueAmount, @RemainingAmount, @TicketNumber, @OrderDateTime, @PaymentType)", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@OrderNo", orders[0].OrderNo);
                    cmd.Parameters.AddWithValue("@InsertedAmount", orders[0].InsertedAmount);
                    cmd.Parameters.AddWithValue("@DueAmount", orders[0].DueAmount);
                    cmd.Parameters.AddWithValue("@RemainingAmount", orders[0].RemainingAmount);
                    cmd.Parameters.AddWithValue("@TicketNumber", orders[0].TicketNumber);
                    cmd.Parameters.AddWithValue("@OrderDateTime", Convert.ToDateTime( OrderDateTime));
                    cmd.Parameters.AddWithValue("@PaymentType", CheckOut.PaymentMethod);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public int CalTotalAmount()
        {

            for (int i = 0; i < carts.Count; i++)
            {
                orders[0].DueAmount = (Convert.ToInt32(carts[i].UnitPrice) * carts[i].Quantity) + orders[0].DueAmount;
            }
            return orders[0].DueAmount;
        }
        public void GenerateOrderNo()
        {

            //string todaysDate = DateTime.Now.ToString("yyyyMMddHHmm");
            //OrderNo = "SK-" + todaysDate;

            TimeSpan t = DateTime.Now - new DateTime(2021, 1, 1);
            int secondsSinceEpoch = (int)t.TotalSeconds;
            OrderNo = "SK01-" + secondsSinceEpoch;

            //OrderNo = "SK-" + ((DateTime)todaysDate.Date).ToString(@"yyyyMMdd");
        }

        public void CheckPortion(int sec)
        {
            if (menuViewModel.Menus[sec].PriceQP == 0 )
            {
                HasQuarter = false;
            }
            if (menuViewModel.Menus[sec].PriceMP == 0)
            {
                HasHalf = false;
            }
            if (!menuViewModel.Menus[sec].HasPortion)
            {
                HasPortion = true;
            }
            
        }
        bool itemExists = false;
        public void btnSec1(object sender, RoutedEventArgs e)
        {
            tBtnSec1.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(0); 
                SelectAnItem(0);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(0);
                SelectAnItem(0);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(0);
                SelectAnItem(0);
            }
        }

        private void btnSec2(object sender, RoutedEventArgs e)
        {
            tBtnSec2.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(1);
                SelectAnItem(1);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(1);
                SelectAnItem(1);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(1);
                SelectAnItem(1);
            }
        }

        private void btnSec3(object sender, RoutedEventArgs e)
        {
            tBtnSec3.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(2);
                SelectAnItem(2);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(2);
                SelectAnItem(2);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(2);
                SelectAnItem(2);
            }
        }
        MenuViewModel cartViewModel = new MenuViewModel();
        private void btnSec4(object sender, RoutedEventArgs e)
        {
            tBtnSec4.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(3);
                SelectAnItem(3);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(3);
                SelectAnItem(3);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(3);
                SelectAnItem(3);
            }
        }

        private void btnSec5(object sender, RoutedEventArgs e)
        {
            tBtnSec5.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(4);
                SelectAnItem(4);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(4);
                SelectAnItem(4);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(4);
                SelectAnItem(4);
            }
        }

        private void btnSec6(object sender, RoutedEventArgs e)
        {
            tBtnSec6.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(5);
                SelectAnItem(5);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(5);
                SelectAnItem(5);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(5);
                SelectAnItem(5);
            }
        }
        private void btnSec7(object sender, RoutedEventArgs e)
        {
            tBtnSec7.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(6);
                SelectAnItem(6);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(6);
                SelectAnItem(6);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(6);
                SelectAnItem(6);
            }
        }
        private void btnSec8(object sender, RoutedEventArgs e)
        {
            tBtnSec8.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(7);
                SelectAnItem(7);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(7);
                SelectAnItem(7);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(7);
                SelectAnItem(7);
            }
        }

        private void btnSec9(object sender, RoutedEventArgs e)
        {
            tBtnSec9.IsChecked = false;
            HasPortion = false;
            HasQuarter = true;
            HasHalf = true;
            if (tBtnCat1.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[0].CatName);
                CheckPortion(8);
                SelectAnItem(8);
            }
            else if (tBtnCat2.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[1].CatName);
                CheckPortion(8);
                SelectAnItem(8);
            }
            else if (tBtnCat3.IsChecked == true)
            {
                menuViewModel.LoadMenu(menuViewModel.Categories[2].CatName);
                CheckPortion(8);
                SelectAnItem(8);
            }
        }
        public void SelectAnItem(int sec)
        {
            SelectPortion selPortion = new SelectPortion();
            menuViewModel.CountCategories();
            selPortion.priceQuarter.DataContext = menuViewModel.Menus[sec];
            selPortion.priceMedium.DataContext = menuViewModel.Menus[sec];
            selPortion.priceStandard.DataContext = menuViewModel.Menus[sec];
            MenuViewModel cartViewModel = new MenuViewModel();
            CartLimit cartLimit = new CartLimit();

            cartViewModel.LoadCart();

            if (!HasPortion)
            {
                selPortion.ShowDialog();
            }
            else
            {
                if (carts.Count >= 1 && carts.Count < 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }
                    }
                    if (!itemExists)
                    {
                        carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceSP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                        itemExists = false;
                    }
                }
                else if (carts.Count == 0) //insert by checking if cart is empty
                {
                    carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceSP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                }
                else if (carts.Count == 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }

                    }
                    if (!itemExists)
                    {
                        itemExists = false;
                        cartLimit.ShowDialog();
                    }
                }
                else
                {
                    cartLimit.ShowDialog();
                }
                HasPortion = false;
            }

          if (selPortion.portionClicked == "Quarter")
            {
                if (carts.Count >= 1 && carts.Count < 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }
                    }
                    if (!itemExists)
                    {
                        carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceQP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                        itemExists = false;
                    }
                }
                else if (carts.Count == 0) //insert by checking if cart is empty
                {
                    carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceQP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                }
                else if (carts.Count == 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }

                    }
                    if (!itemExists)
                    {
                        itemExists = false;
                        cartLimit.ShowDialog();
                    }
                }
                else
                {
                    cartLimit.ShowDialog();
                }
            }
            else if (selPortion.portionClicked == "Half")
            {
                if (carts.Count >= 1 && carts.Count < 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }
                    }
                    if (!itemExists)
                    {
                        carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceMP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                        itemExists = false;
                    }

                }
                else if (carts.Count == 0)
                {
                    carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceMP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                }
                else if (carts.Count == 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }

                    }
                    if (!itemExists)
                    {

                        cartLimit.ShowDialog();
                    }
                }
                else
                {
                    cartLimit.ShowDialog();
                }

            }
            else if (selPortion.portionClicked == "Standard")
            {
                if (carts.Count >= 1 && carts.Count < 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }

                    }
                    if (!itemExists)
                    {
                        carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceSP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                        itemExists = false;
                    }
                }
                else if (carts.Count == 0)
                {
                    carts.Add(new Models.Cart { Name = menuViewModel.Menus[sec].itemName, UnitPrice = Convert.ToString(menuViewModel.Menus[sec].priceSP), Quantity = Qty + 1, Portion = selPortion.portionClicked });
                }
                else if (carts.Count == 8)
                {
                    itemExists = false;
                    for (int j = 0; j < carts.Count; j++)
                    {
                        if (menuViewModel.Menus[sec].itemName == carts[j].Name && selPortion.portionClicked == carts[j].Portion)
                        {
                            carts[j].Quantity++;
                            itemExists = true;
                            return;
                        }

                    }
                    if (!itemExists)
                    {
                        //itemExists = false;
                        cartLimit.ShowDialog();
                    }
                }
                else
                {
                    cartLimit.ShowDialog();
                }

            }

            DisplayCartItems();
            DisplayCartBtns();
        }

        public void CalTotalPrice()
        {
            for (int i = 0; i < carts.Count; i++)
            {

                carts[i].TotalPrice = Convert.ToString((Convert.ToInt32(carts[i].UnitPrice) * carts[i].Quantity));

            }

        }
        public void DisplayCartBtns()
        {
            if (carts.Count > 0)
            {
                Checkout.Visibility = Visibility.Visible;
                Cancel.Visibility = Visibility.Visible;
            }

            //cartBorder.Visibility = Visibility.Visible;


        }
        public void HideDisplayButtons()
        {
            Checkout.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;
        }
        public void DisplayMenuItems()
        {
            if (MenuViewModel.ItemsCount >= 9)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                txtItem4.DataContext = menuViewModel.Menus[3];
                txtItem5.DataContext = menuViewModel.Menus[4];
                txtItem6.DataContext = menuViewModel.Menus[5];
                txtItem7.DataContext = menuViewModel.Menus[6];
                txtItem8.DataContext = menuViewModel.Menus[7];
                txtItem9.DataContext = menuViewModel.Menus[8];
                //CheckItemImage(9);
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                imgItem4.Source = BitmaSourceFromByteArray(menuViewModel.Menus[3].ItemImage);
                imgItem5.Source = BitmaSourceFromByteArray(menuViewModel.Menus[4].ItemImage);
                imgItem6.Source = BitmaSourceFromByteArray(menuViewModel.Menus[5].ItemImage);
                imgItem7.Source = BitmaSourceFromByteArray(menuViewModel.Menus[6].ItemImage);
                imgItem8.Source = BitmaSourceFromByteArray(menuViewModel.Menus[7].ItemImage);
                imgItem9.Source = BitmaSourceFromByteArray(menuViewModel.Menus[8].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Visible;
                sec4.Visibility = Visibility.Visible;
                txtItem5.Visibility = Visibility.Visible;
                sec5.Visibility = Visibility.Visible;
                txtItem6.Visibility = Visibility.Visible;
                sec6.Visibility = Visibility.Visible;
                txtItem7.Visibility = Visibility.Visible;
                sec7.Visibility = Visibility.Visible;
                txtItem8.Visibility = Visibility.Visible;
                sec8.Visibility = Visibility.Visible;
                txtItem9.Visibility = Visibility.Visible;
                sec9.Visibility = Visibility.Visible;
            }
            if (MenuViewModel.ItemsCount == 8)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                txtItem4.DataContext = menuViewModel.Menus[3];
                txtItem5.DataContext = menuViewModel.Menus[4];
                txtItem6.DataContext = menuViewModel.Menus[5];
                txtItem7.DataContext = menuViewModel.Menus[6];
                txtItem8.DataContext = menuViewModel.Menus[7];
                //CheckItemImage(8);
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                imgItem4.Source = BitmaSourceFromByteArray(menuViewModel.Menus[3].ItemImage);
                imgItem5.Source = BitmaSourceFromByteArray(menuViewModel.Menus[4].ItemImage);
                imgItem6.Source = BitmaSourceFromByteArray(menuViewModel.Menus[5].ItemImage);
                imgItem7.Source = BitmaSourceFromByteArray(menuViewModel.Menus[6].ItemImage);
                imgItem8.Source = BitmaSourceFromByteArray(menuViewModel.Menus[7].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Visible;
                sec4.Visibility = Visibility.Visible;
                txtItem5.Visibility = Visibility.Visible;
                sec5.Visibility = Visibility.Visible;
                txtItem6.Visibility = Visibility.Visible;
                sec6.Visibility = Visibility.Visible;
                txtItem7.Visibility = Visibility.Visible;
                sec7.Visibility = Visibility.Visible;
                txtItem8.Visibility = Visibility.Visible;
                sec8.Visibility = Visibility.Visible;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;
            }
            else if (MenuViewModel.ItemsCount == 7)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                txtItem4.DataContext = menuViewModel.Menus[3];
                txtItem5.DataContext = menuViewModel.Menus[4];
                txtItem6.DataContext = menuViewModel.Menus[5];
                txtItem7.DataContext = menuViewModel.Menus[6];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                imgItem4.Source = BitmaSourceFromByteArray(menuViewModel.Menus[3].ItemImage);
                imgItem5.Source = BitmaSourceFromByteArray(menuViewModel.Menus[4].ItemImage);
                imgItem6.Source = BitmaSourceFromByteArray(menuViewModel.Menus[5].ItemImage);
                imgItem7.Source = BitmaSourceFromByteArray(menuViewModel.Menus[6].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Visible;
                sec4.Visibility = Visibility.Visible;
                txtItem5.Visibility = Visibility.Visible;
                sec5.Visibility = Visibility.Visible;
                txtItem6.Visibility = Visibility.Visible;
                sec6.Visibility = Visibility.Visible;
                txtItem7.Visibility = Visibility.Visible;
                sec7.Visibility = Visibility.Visible;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;
            }
            else if (MenuViewModel.ItemsCount == 6)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                txtItem4.DataContext = menuViewModel.Menus[3];
                txtItem5.DataContext = menuViewModel.Menus[4];
                txtItem6.DataContext = menuViewModel.Menus[5];
                //txtPrice1.DataContext = menuViewModel.Menus[0];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                imgItem4.Source = BitmaSourceFromByteArray(menuViewModel.Menus[3].ItemImage);
                imgItem5.Source = BitmaSourceFromByteArray(menuViewModel.Menus[4].ItemImage);
                imgItem6.Source = BitmaSourceFromByteArray(menuViewModel.Menus[5].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Visible;
                sec4.Visibility = Visibility.Visible;
                txtItem5.Visibility = Visibility.Visible;
                sec5.Visibility = Visibility.Visible;
                txtItem6.Visibility = Visibility.Visible;
                sec6.Visibility = Visibility.Visible;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;
            }
            else if (MenuViewModel.ItemsCount == 5)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                txtItem4.DataContext = menuViewModel.Menus[3];
                txtItem5.DataContext = menuViewModel.Menus[4];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                imgItem4.Source = BitmaSourceFromByteArray(menuViewModel.Menus[3].ItemImage);
                imgItem5.Source = BitmaSourceFromByteArray(menuViewModel.Menus[4].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Visible;
                sec4.Visibility = Visibility.Visible;
                txtItem5.Visibility = Visibility.Visible;
                sec5.Visibility = Visibility.Visible;
                txtItem6.Visibility = Visibility.Hidden;
                sec6.Visibility = Visibility.Hidden;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;
            }
            else if (MenuViewModel.ItemsCount == 4)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                txtItem4.DataContext = menuViewModel.Menus[3];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                imgItem4.Source = BitmaSourceFromByteArray(menuViewModel.Menus[3].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Visible;
                sec4.Visibility = Visibility.Visible;
                txtItem5.Visibility = Visibility.Hidden;
                sec5.Visibility = Visibility.Hidden;
                txtItem6.Visibility = Visibility.Hidden;
                sec6.Visibility = Visibility.Hidden;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;
            }
            else if (MenuViewModel.ItemsCount == 3)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                txtItem3.DataContext = menuViewModel.Menus[2];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                imgItem3.Source = BitmaSourceFromByteArray(menuViewModel.Menus[2].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Visible;
                sec3.Visibility = Visibility.Visible;
                txtItem4.Visibility = Visibility.Hidden;
                sec4.Visibility = Visibility.Hidden;
                txtItem5.Visibility = Visibility.Hidden;
                sec5.Visibility = Visibility.Hidden;
                txtItem6.Visibility = Visibility.Hidden;
                sec6.Visibility = Visibility.Hidden;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;

            }
            else if (MenuViewModel.ItemsCount == 2)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                txtItem2.DataContext = menuViewModel.Menus[1];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                imgItem2.Source = BitmaSourceFromByteArray(menuViewModel.Menus[1].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Visible;
                sec2.Visibility = Visibility.Visible;
                txtItem3.Visibility = Visibility.Hidden;
                sec3.Visibility = Visibility.Hidden;
                txtItem4.Visibility = Visibility.Hidden;
                sec4.Visibility = Visibility.Hidden;
                txtItem5.Visibility = Visibility.Hidden;
                sec5.Visibility = Visibility.Hidden;
                txtItem6.Visibility = Visibility.Hidden;
                sec6.Visibility = Visibility.Hidden;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;

            }
            else if (MenuViewModel.ItemsCount == 1)
            {
                txtItem1.DataContext = menuViewModel.Menus[0];
                imgItem1.Source = BitmaSourceFromByteArray(menuViewModel.Menus[0].ItemImage);
                txtItem1.Visibility = Visibility.Visible;
                sec1.Visibility = Visibility.Visible;
                txtItem2.Visibility = Visibility.Hidden;
                sec2.Visibility = Visibility.Hidden;
                txtItem3.Visibility = Visibility.Hidden;
                sec3.Visibility = Visibility.Hidden;
                txtItem4.Visibility = Visibility.Hidden;
                sec4.Visibility = Visibility.Hidden;
                txtItem5.Visibility = Visibility.Hidden;
                sec5.Visibility = Visibility.Hidden;
                txtItem6.Visibility = Visibility.Hidden;
                sec6.Visibility = Visibility.Hidden;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;

            }
            else if (MenuViewModel.ItemsCount == 0)
            {
                txtItem1.Visibility = Visibility.Hidden;
                sec1.Visibility = Visibility.Hidden;
                txtItem2.Visibility = Visibility.Hidden;
                sec2.Visibility = Visibility.Hidden;
                txtItem3.Visibility = Visibility.Hidden;
                sec3.Visibility = Visibility.Hidden;
                txtItem4.Visibility = Visibility.Hidden;
                sec4.Visibility = Visibility.Hidden;
                txtItem5.Visibility = Visibility.Hidden;
                sec5.Visibility = Visibility.Hidden;
                txtItem6.Visibility = Visibility.Hidden;
                sec6.Visibility = Visibility.Hidden;
                txtItem7.Visibility = Visibility.Hidden;
                sec7.Visibility = Visibility.Hidden;
                txtItem8.Visibility = Visibility.Hidden;
                sec8.Visibility = Visibility.Hidden;
                txtItem9.Visibility = Visibility.Hidden;
                sec9.Visibility = Visibility.Hidden;

            }

        }
        public void DisplayCartItems()
        {
            MenuViewModel menuViewModel = new MenuViewModel();
            menuViewModel.LoadCategory();

            var uri = new Uri("pack://application:,,,/Icons/Editing-Delete-icon.png");
            try
            {
                int CartCount = carts.Count();
                for (int i = 0; i < CartCount; i++)
                {
                    if (i == 0)
                    {

                        cartItem1.DataContext = carts[i];
                        delIcon1.Source = new BitmapImage(uri);
                        btnIncDec1.Visibility = Visibility.Visible;
                        Quantity1.DataContext = carts[i];
                        txtPortion1.DataContext = carts[i];

                        cartItem1.Visibility = Visibility.Visible;
                        delIcon1.Visibility = Visibility.Visible;
                        Del1.Visibility = Visibility.Visible;
                        Quantity1.Visibility = Visibility.Visible;
                        txtPortion1.Visibility = Visibility.Visible;
                    }
                    else if (i == 1)
                    {
                        cartItem2.DataContext = carts[i];
                        delIcon2.Source = new BitmapImage(uri);
                        btnIncDec2.Visibility = Visibility.Visible;
                        Quantity2.DataContext = carts[i];
                        txtPortion2.DataContext = carts[i];

                        cartItem2.Visibility = Visibility.Visible;
                        delIcon2.Visibility = Visibility.Visible;
                        Del2.Visibility = Visibility.Visible;
                        Quantity2.Visibility = Visibility.Visible;
                        txtPortion2.Visibility = Visibility.Visible;
                    }
                    else if (i == 2)
                    {
                        cartItem3.DataContext = carts[i];
                        delIcon3.Source = new BitmapImage(uri);
                        Quantity3.DataContext = carts[i];
                        txtPortion3.DataContext = carts[i];

                        cartItem3.Visibility = Visibility.Visible;
                        delIcon3.Visibility = Visibility.Visible;
                        Del3.Visibility = Visibility.Visible;
                        btnIncDec3.Visibility = Visibility.Visible;
                        Quantity3.Visibility = Visibility.Visible;
                        txtPortion3.Visibility = Visibility.Visible;
                    }
                    else if (i == 3)
                    {
                        cartItem4.DataContext = carts[i];
                        delIcon4.Source = new BitmapImage(uri);
                        btnIncDec4.Visibility = Visibility.Visible;
                        Quantity4.DataContext = carts[i];
                        txtPortion4.DataContext = carts[i];

                        cartItem4.Visibility = Visibility.Visible;
                        delIcon4.Visibility = Visibility.Visible;
                        Del4.Visibility = Visibility.Visible;
                        btnIncDec4.Visibility = Visibility.Visible;
                        Quantity4.Visibility = Visibility.Visible;
                        txtPortion4.Visibility = Visibility.Visible;
                    }
                    else if (i == 4)
                    {
                        cartItem5.DataContext = carts[i];
                        delIcon5.Source = new BitmapImage(uri);
                        btnIncDec5.Visibility = Visibility.Visible;
                        Quantity5.DataContext = carts[i];
                        txtPortion5.DataContext = carts[i];

                        cartItem5.Visibility = Visibility.Visible;
                        delIcon5.Visibility = Visibility.Visible;
                        Del5.Visibility = Visibility.Visible;
                        btnIncDec5.Visibility = Visibility.Visible;
                        Quantity5.Visibility = Visibility.Visible;
                        txtPortion5.Visibility = Visibility.Visible;
                    }
                    else if (i == 5)
                    {
                        cartItem6.DataContext = carts[i];
                        delIcon6.Source = new BitmapImage(uri);
                        btnIncDec6.Visibility = Visibility.Visible;
                        Quantity6.DataContext = carts[i];
                        txtPortion6.DataContext = carts[i];

                        cartItem6.Visibility = Visibility.Visible;
                        delIcon6.Visibility = Visibility.Visible;
                        Del6.Visibility = Visibility.Visible;
                        btnIncDec6.Visibility = Visibility.Visible;
                        Quantity6.Visibility = Visibility.Visible;
                        txtPortion6.Visibility = Visibility.Visible;
                    }
                    else if (i == 6)
                    {
                        cartItem7.DataContext = carts[i];
                        delIcon7.Source = new BitmapImage(uri);
                        btnIncDec7.Visibility = Visibility.Visible;
                        Quantity7.DataContext = carts[i];
                        txtPortion7.DataContext = carts[i];

                        cartItem7.Visibility = Visibility.Visible;
                        delIcon7.Visibility = Visibility.Visible;
                        Del7.Visibility = Visibility.Visible;
                        btnIncDec7.Visibility = Visibility.Visible;
                        Quantity7.Visibility = Visibility.Visible;
                        txtPortion7.Visibility = Visibility.Visible;
                    }
                    else if (i == 7)
                    {
                        cartItem8.DataContext = carts[i];
                        delIcon8.Source = new BitmapImage(uri);
                        btnIncDec8.Visibility = Visibility.Visible;
                        Quantity8.DataContext = carts[i];
                        txtPortion8.DataContext = carts[i];

                        cartItem8.Visibility = Visibility.Visible;
                        delIcon8.Visibility = Visibility.Visible;
                        Del8.Visibility = Visibility.Visible;
                        btnIncDec8.Visibility = Visibility.Visible;
                        Quantity8.Visibility = Visibility.Visible;
                        txtPortion8.Visibility = Visibility.Visible;
                    }


                }
            }
            catch (Exception ex)
            {

            }

        }
        public void CartRefreshed()
        {
            MenuViewModel menuViewModel = new MenuViewModel();
            menuViewModel.LoadCategory();

            var uri = new Uri("pack://application:,,,/Icons/Editing-Delete-icon.png");
            try
            {
                int CartCount = carts.Count();
                switch (CartCount)
                {
                    case 0:
                        cartItem1.Visibility = Visibility.Hidden;
                        delIcon1.Visibility = Visibility.Hidden;
                        Del1.Visibility = Visibility.Hidden;
                        btnIncDec1.Visibility = Visibility.Hidden;
                        Quantity1.Visibility = Visibility.Hidden;
                        txtPortion1.Visibility = Visibility.Hidden;

                        Cancel.Visibility = Visibility.Hidden;
                        Checkout.Visibility = Visibility.Hidden;
                        break;

                }

                for (int i = 0; i < CartCount; i++)
                {
                    switch (i)
                    {
                        case 0:
                            cartItem1.DataContext = carts[i];
                            delIcon1.Source = new BitmapImage(uri);
                            Del1.Visibility = Visibility.Visible;
                            btnIncDec1.Visibility = Visibility.Visible;
                            Quantity1.DataContext = carts[i];
                            txtPortion1.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem2.Visibility = Visibility.Hidden;
                                delIcon2.Visibility = Visibility.Hidden;
                                Del2.Visibility = Visibility.Hidden;
                                btnIncDec2.Visibility = Visibility.Hidden;
                                Quantity2.Visibility = Visibility.Hidden;
                                txtPortion2.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 1:
                            cartItem2.DataContext = carts[i];
                            delIcon2.Source = new BitmapImage(uri);
                            btnIncDec2.Visibility = Visibility.Visible;
                            Del2.Visibility = Visibility.Visible;
                            Quantity2.DataContext = carts[i];
                            txtPortion2.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem3.Visibility = Visibility.Hidden;
                                delIcon3.Visibility = Visibility.Hidden;
                                Del3.Visibility = Visibility.Hidden;
                                btnIncDec3.Visibility = Visibility.Hidden;
                                Quantity3.Visibility = Visibility.Hidden;
                                txtPortion3.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 2:
                            cartItem3.DataContext = carts[i];
                            delIcon3.Source = new BitmapImage(uri);
                            btnIncDec3.Visibility = Visibility.Visible;
                            Del3.Visibility = Visibility.Visible;
                            Quantity3.DataContext = carts[i];
                            txtPortion3.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem4.Visibility = Visibility.Hidden;
                                delIcon4.Visibility = Visibility.Hidden;
                                Del4.Visibility = Visibility.Hidden;
                                btnIncDec4.Visibility = Visibility.Hidden;
                                Quantity4.Visibility = Visibility.Hidden;
                                txtPortion4.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 3:
                            cartItem4.DataContext = carts[i];
                            delIcon4.Source = new BitmapImage(uri);
                            btnIncDec4.Visibility = Visibility.Visible;
                            Del4.Visibility = Visibility.Visible;
                            Quantity4.DataContext = carts[i];
                            txtPortion4.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem5.Visibility = Visibility.Hidden;
                                delIcon5.Visibility = Visibility.Hidden;
                                Del5.Visibility = Visibility.Hidden;
                                btnIncDec5.Visibility = Visibility.Hidden;
                                Quantity5.Visibility = Visibility.Hidden;
                                txtPortion5.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 4:
                            cartItem5.DataContext = carts[i];
                            delIcon5.Source = new BitmapImage(uri);
                            btnIncDec5.Visibility = Visibility.Visible;
                            Del5.Visibility = Visibility.Visible;
                            Quantity5.DataContext = carts[i];
                            txtPortion5.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem6.Visibility = Visibility.Hidden;
                                delIcon6.Visibility = Visibility.Hidden;
                                Del6.Visibility = Visibility.Hidden;
                                btnIncDec6.Visibility = Visibility.Hidden;
                                Quantity6.Visibility = Visibility.Hidden;
                                txtPortion6.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 5:
                            cartItem6.DataContext = carts[i];
                            delIcon6.Source = new BitmapImage(uri);
                            btnIncDec6.Visibility = Visibility.Visible;
                            Del6.Visibility = Visibility.Visible;
                            Quantity6.DataContext = carts[i];
                            txtPortion6.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem7.Visibility = Visibility.Hidden;
                                delIcon7.Visibility = Visibility.Hidden;
                                Del7.Visibility = Visibility.Hidden;
                                btnIncDec7.Visibility = Visibility.Hidden;
                                Quantity7.Visibility = Visibility.Hidden;
                                txtPortion7.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 6:
                            cartItem7.DataContext = carts[i];
                            delIcon7.Source = new BitmapImage(uri);
                            btnIncDec7.Visibility = Visibility.Visible;
                            Del7.Visibility = Visibility.Visible;
                            Quantity7.DataContext = carts[i];
                            txtPortion7.DataContext = carts[i];

                            if (i == CartCount - 1)
                            {
                                cartItem8.Visibility = Visibility.Hidden;
                                delIcon8.Visibility = Visibility.Hidden;
                                btnIncDec8.Visibility = Visibility.Hidden;
                                Del8.Visibility = Visibility.Hidden;
                                Quantity8.Visibility = Visibility.Hidden;
                                txtPortion8.Visibility = Visibility.Hidden;
                            }
                            break;
                        case 7:
                            cartItem8.DataContext = carts[i];
                            delIcon8.Source = new BitmapImage(uri);
                            btnIncDec8.Visibility = Visibility.Visible;
                            Del8.Visibility = Visibility.Visible;
                            Quantity8.DataContext = carts[i];
                            txtPortion8.DataContext = carts[i];

                            break;
                    }


                }
            }
            catch (Exception ex)
            {

            }

        }

        private void btnDec1(object sender, RoutedEventArgs e)
        {
            if (carts[0].Quantity > 1)
            {
                Dec1.DataContext = carts[0];
                carts[0].Quantity--;
            }
        }


        private void btnDec2(object sender, RoutedEventArgs e)
        {
            if (carts[1].Quantity > 1)
            {
                Dec2.DataContext = carts[1];
                carts[1].Quantity--;
            }
        }

        private void btnDec3(object sender, RoutedEventArgs e)
        {
            if (carts[2].Quantity > 1)
            {
                Dec3.DataContext = carts[2];
                carts[2].Quantity--;
            }
        }

        private void btnDec4(object sender, RoutedEventArgs e)
        {
            if (carts[3].Quantity > 1)
            {
                Dec4.DataContext = carts[3];
                carts[3].Quantity--;
            }
        }

        private void btnDec5(object sender, RoutedEventArgs e)
        {
            if (carts[4].Quantity > 1)
            {
                Dec5.DataContext = carts[4];
                carts[4].Quantity--;
            }
        }

        private void btnDec6(object sender, RoutedEventArgs e)
        {
            if (carts[5].Quantity > 1)
            {
                Dec6.DataContext = carts[5];
                carts[5].Quantity--;
            }
        }

        private void btnDec7(object sender, RoutedEventArgs e)
        {
            if (carts[6].Quantity > 1)
            {
                Dec7.DataContext = carts[6];
                carts[6].Quantity--;
            }
        }

        private void btnDec8(object sender, RoutedEventArgs e)
        {
            if (carts[7].Quantity > 1)
            {
                Dec8.DataContext = carts[7];
                carts[7].Quantity--;
            }
        }

        private void btnInc1(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc1.DataContext = carts[0];
                carts[0].Quantity++;
            }
        }

        private void btnInc2(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc2.DataContext = carts[1];
                carts[1].Quantity++;
            }
        }

        private void btnInc3(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc3.DataContext = carts[2];
                carts[2].Quantity++;
            }
        }

        private void btnInc4(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc4.DataContext = carts[3];
                carts[3].Quantity++;
            }
        }

        private void btnInc5(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc5.DataContext = carts[4];
                carts[4].Quantity++;
            }
        }

        private void btnInc6(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc6.DataContext = carts[5];
                carts[5].Quantity++;
            }
        }

        private void btnInc7(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc7.DataContext = carts[6];
                carts[6].Quantity++;
            }
        }

        private void btnInc8(object sender, RoutedEventArgs e)
        {
            if (carts.Count >= 1)
            {
                Inc8.DataContext = carts[7];
                carts[7].Quantity++;
            }
        }

        private void btnDel1(object sender, RoutedEventArgs e)
        {
            Del1.DataContext = carts[0];
            carts.Remove(carts[0]);
            CartRefreshed();
            DisplayCartBtns();
        }

        private void btnDel2(object sender, RoutedEventArgs e)
        {

            Del2.DataContext = carts[1];
            carts.Remove(carts[1]);
            CartRefreshed();
            DisplayCartBtns();


        }


        private void btnDel3(object sender, RoutedEventArgs e)
        {
            Del3.DataContext = carts[2];
            carts.Remove(carts[2]);
            CartRefreshed();
            DisplayCartBtns();
        }
        private void btnDel4(object sender, RoutedEventArgs e)
        {
            Del4.DataContext = carts[3];
            carts.Remove(carts[3]);
            CartRefreshed();
            DisplayCartBtns();
        }
        private void btnDel5(object sender, RoutedEventArgs e)
        {
            Del5.DataContext = carts[4];
            carts.Remove(carts[4]);
            CartRefreshed();
            DisplayCartBtns();
        }
        private void btnDel6(object sender, RoutedEventArgs e)
        {
            Del6.DataContext = carts[5];
            carts.Remove(carts[5]);
            CartRefreshed();
            DisplayCartBtns();
        }
        private void btnDel7(object sender, RoutedEventArgs e)
        {
            Del7.DataContext = carts[6];
            carts.Remove(carts[6]);
            CartRefreshed();
            DisplayCartBtns();
        }
        private void btnDel8(object sender, RoutedEventArgs e)
        {
            Del8.DataContext = carts[7];
            carts.Remove(carts[7]);
            CartRefreshed();
            DisplayCartBtns();
        }
        public void btnCancel(object sender, RoutedEventArgs e)
        {
            orders.Add(new Models.OrderSummary { DueAmount = 0, InsertedAmount = 0, RemainingAmount = 0 });
            //Global.cartTotalAmount = Convert.ToInt32(CalTotalAmount());
            CancelWarning cancelWarning = new CancelWarning();
            //this.Opacity = 0.9;
            cancelWarning.ShowDialog();
            //this.Opacity = 1.5;
            if (cancelWarning.isCancelled)
            {
                CancelOrder();
                HideDisplayButtons();
                cancelWarning.Close();
            }


        }
        public void CancelOrder()
        {
            carts.Clear();
            if (orders[0].DueAmount > 0)
            {
                orders[0].DueAmount = 0;
            }
            Checkout.Visibility = Visibility.Hidden;
            Cancel.Visibility = Visibility.Hidden;

            cartItem1.Visibility = Visibility.Hidden;
            delIcon1.Visibility = Visibility.Hidden;
            Del1.Visibility = Visibility.Hidden;
            Quantity1.Visibility = Visibility.Hidden;
            txtPortion1.Visibility = Visibility.Hidden;
            btnIncDec1.Visibility = Visibility.Hidden;

            cartItem2.Visibility = Visibility.Hidden;
            delIcon2.Visibility = Visibility.Hidden;
            Del2.Visibility = Visibility.Hidden;
            Quantity2.Visibility = Visibility.Hidden;
            txtPortion2.Visibility = Visibility.Hidden;
            btnIncDec2.Visibility = Visibility.Hidden;

            cartItem3.Visibility = Visibility.Hidden;
            delIcon3.Visibility = Visibility.Hidden;
            Del3.Visibility = Visibility.Hidden;
            Quantity3.Visibility = Visibility.Hidden;
            txtPortion3.Visibility = Visibility.Hidden;
            btnIncDec3.Visibility = Visibility.Hidden;

            cartItem4.Visibility = Visibility.Hidden;
            delIcon4.Visibility = Visibility.Hidden;
            Del4.Visibility = Visibility.Hidden;
            Quantity4.Visibility = Visibility.Hidden;
            txtPortion4.Visibility = Visibility.Hidden;
            btnIncDec4.Visibility = Visibility.Hidden;

            cartItem5.Visibility = Visibility.Hidden;
            delIcon5.Visibility = Visibility.Hidden;
            Del5.Visibility = Visibility.Hidden;
            Quantity5.Visibility = Visibility.Hidden;
            txtPortion5.Visibility = Visibility.Hidden;
            btnIncDec5.Visibility = Visibility.Hidden;

            cartItem6.Visibility = Visibility.Hidden;
            delIcon6.Visibility = Visibility.Hidden;
            Del6.Visibility = Visibility.Hidden;
            Quantity6.Visibility = Visibility.Hidden;
            txtPortion7.Visibility = Visibility.Hidden;
            btnIncDec6.Visibility = Visibility.Hidden;

            cartItem7.Visibility = Visibility.Hidden;
            delIcon7.Visibility = Visibility.Hidden;
            Del7.Visibility = Visibility.Hidden;
            Quantity7.Visibility = Visibility.Hidden;
            txtPortion7.Visibility = Visibility.Hidden;
            btnIncDec7.Visibility = Visibility.Hidden;

            cartItem8.Visibility = Visibility.Hidden;
            delIcon8.Visibility = Visibility.Hidden;
            Del8.Visibility = Visibility.Hidden;
            Quantity8.Visibility = Visibility.Hidden;
            txtPortion8.Visibility = Visibility.Hidden;
            btnIncDec8.Visibility = Visibility.Hidden;
        }
        public void PrintReceipt()
        {
            try
            {
                PrintDialog printDlg = new PrintDialog();
                //FlowDocument doc = new FlowDocument(new Paragraph(new Run("Hello world")));
                FlowDocument doc = CreateFlowDocument();
                doc.Name = "FlowDoc";
                IDocumentPaginatorSource idpSource = doc;
                printDlg.PrintDocument(idpSource.DocumentPaginator, "Hello WPF Printing.");
            }
            catch (Exception ex)
            {

            }
        }
        public FlowDocument CreateFlowDocument()
        {
            int i ;
            int ItemsSold = 0;
            // Create a FlowDocument  
            FlowDocument doc = new FlowDocument();

            Section sec1 = new Section();
            Paragraph p2 = new Paragraph();
            Bold bld1 = new Bold();
            bld1.Inlines.Add(new Run("Pepsi Dining Center, LUMS"));
            p2.Inlines.Add(bld1);
            sec1.Blocks.Add(p2);
            doc.Blocks.Add(sec1);
            sec1.FontSize = 15;

            // Create a Section  
            Section sec = new Section();
            // Create first Paragraph  
            Paragraph p1 = new Paragraph();  
            Bold bld = new Bold();
            bld.Inlines.Add(new Run(Convert.ToString(orders[0].TicketNumber)));
            p1.TextAlignment = TextAlignment.Justify;
            p1.Margin = new Thickness(90, 0, 80, 0);
            
            // Add Bold, Italic, Underline to Paragraph  
            p1.Inlines.Add(bld);
            sec.FontSize = 60;
            // Add Paragraph to Section  
            sec.Blocks.Add(p1);
            // Add Section to FlowDocument  
            doc.Blocks.Add(sec);

            Section section5 = new Section();
            Paragraph paragraph5 = new Paragraph();
            paragraph5.FontSize = 11;
            String txtDateTime = OrderDateTime;
            String txtOrderNo = OrderNo;
            paragraph5.Inlines.Add(String.Concat(txtOrderNo + "         " + txtDateTime));
            section5.Blocks.Add(paragraph5);
            doc.Blocks.Add(section5);

            //create a table
            Table table1 = new Table();
            TableRow tableRow = new TableRow();
            TableColumn tableColumn = new TableColumn();
            doc.Blocks.Add(table1);

            // Create 5 columns and add them to the table's Columns collection.

            table1.Columns.Add(new TableColumn());
            table1.Columns[0].Width = new GridLength(84);

            table1.Columns.Add(new TableColumn());
            table1.Columns[1].Width = new GridLength(42);

            table1.Columns.Add(new TableColumn());
            table1.Columns[2].Width = new GridLength(28);

            table1.Columns.Add(new TableColumn());
            table1.Columns[3].Width = new GridLength(25);

            table1.Columns.Add(new TableColumn());
            table1.Columns[4].Width = new GridLength(33);

            // Create and add an empty TableRowGroup Rows.
            table1.RowGroups.Add(new TableRowGroup());

            // Add the table head row.
            table1.RowGroups[0].Rows.Add(new TableRow());

            //Configure the table head row
            TableRow currentRow = table1.RowGroups[0].Rows[0];
            currentRow.Background = System.Windows.Media.Brushes.Navy;
            currentRow.Foreground = System.Windows.Media.Brushes.White;
            currentRow.FontSize = 11;

            // Add the header row with content,
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Description"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Size"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Rate"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Qty"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Total"))));

            table1.FontSize = 10; table1.FontSize = 10;
            for (i = 0; i < carts.Count; i++)
            {
                table1.RowGroups[0].Rows.Add(new TableRow());
                currentRow = table1.RowGroups[0].Rows[i + 1];
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(carts[i].Name))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(carts[i].Portion))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(carts[i].UnitPrice))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(Convert.ToString(carts[i].Quantity)))));
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(carts[i].TotalPrice))));
            }

            Section section = new Section();
            Paragraph paragraph = new Paragraph();
            String line = "______________________";
            paragraph.Inlines.Add(line);
            section.Blocks.Add(paragraph);
            doc.Blocks.Add(section);


            Section section4 = new Section();
            Paragraph paragraph4 = new Paragraph();

            if (!PaymentCompleted)
            {
                Section section6 = new Section();
                Paragraph paragraph6 = new Paragraph();
                String txtRemaining = "Payment Status:     Incomplete";
                Bold bold1 = new Bold();
                bold1.Inlines.Add(txtRemaining);
                paragraph6.Margin = new Thickness(10, 0, 0, 20);
                paragraph6.Inlines.Add(bold1);
                paragraph6.FontSize = 13;
                section6.Blocks.Add(paragraph6);
                doc.Blocks.Add(section6);
            }
            else
            {
                Section section6 = new Section();
                Paragraph paragraph6 = new Paragraph();
                String txtRemaining = "Payment Status:       Complete";
                Bold bold1 = new Bold();
                bold1.Inlines.Add(txtRemaining);
                paragraph6.Margin = new Thickness(10, 0, 0, 20);
                paragraph6.Inlines.Add(bold1);
                paragraph6.FontSize = 13;
                section6.Blocks.Add(paragraph6);
                doc.Blocks.Add(section6);
            }

            Section section8 = new Section();
            Paragraph paragraph8 = new Paragraph();
            for (i = 0; i < carts.Count; i++)
            {
                ItemsSold = carts[i].Quantity + ItemsSold;
            }
            String txtItemsSold = "Items Sold:                  " + Convert.ToString(ItemsSold);
            paragraph8.Margin = new Thickness(42, 0, 0, 0);
            paragraph8.Inlines.Add(txtItemsSold);
            paragraph8.FontSize = 11;
            section8.Blocks.Add(paragraph8);
            doc.Blocks.Add(section8);

            Section section7 = new Section();
            Paragraph paragraph7 = new Paragraph();
            String txtPayMethod = "Payment Method:      " + CheckOut.PaymentMethod;
            paragraph7.Inlines.Add(txtPayMethod);
            paragraph7.Margin = new Thickness(42, 0, 0, 0);
            paragraph7.FontSize = 11;
            section7.Blocks.Add(paragraph7);
            doc.Blocks.Add(section7);

            if (CheckOut.PaymentMethod == "QR")
            {

                Section section1 = new Section();
                Paragraph paragraph1 = new Paragraph();
                String txtDue = "Sale Subtotal:             Rs. " + Convert.ToString(QRdueAmount);
                paragraph1.Margin = new Thickness(42, 0, 0, 0);
                paragraph1.Inlines.Add(txtDue);
                paragraph1.FontSize = 11;
                section1.Blocks.Add(paragraph1);
                doc.Blocks.Add(section1);
            }
            else
            {
                Section section1 = new Section();
                Paragraph paragraph1 = new Paragraph();
                String txtDue = "Sale Subtotal:             Rs. " + Convert.ToString(orders[0].DueAmount);
                paragraph1.Margin = new Thickness(42, 0, 0, 0);
                paragraph1.Inlines.Add(txtDue);
                paragraph1.FontSize = 11;
                section1.Blocks.Add(paragraph1);
                doc.Blocks.Add(section1);
            }


            paragraph4.Margin = new Thickness(42, 0, 0, 0);
            String txtPaid =  "Paid Amount:             Rs. " + Convert.ToString(orders[0].InsertedAmount);
            paragraph4.Inlines.Add(txtPaid);
            paragraph4.FontSize = 11;
            section4.Blocks.Add(paragraph4);
            doc.Blocks.Add(section4);

            if (PaymentCompleted)
            {
                Section section2 = new Section();
                Paragraph paragraph2 = new Paragraph();
                String txtRemaining = "Balance:                       Rs. " + Convert.ToString(orders[0].RemainingAmount);
                paragraph2.Inlines.Add(txtRemaining);
                paragraph2.Margin = new Thickness(42, 0, 0, 0);
                paragraph2.FontSize = 11;
                section2.Blocks.Add(paragraph2);
                doc.Blocks.Add(section2);
            }
            else
            {
                Section section2 = new Section();
                Paragraph paragraph2 = new Paragraph();
                String txtRemaining = "Pending Amount:      Rs. " + Convert.ToString(orders[0].RemainingAmount);
                paragraph2.Inlines.Add(txtRemaining);
                paragraph2.Margin = new Thickness(42, 0, 0, 0);
                paragraph2.FontSize = 11;
                section2.Blocks.Add(paragraph2);
                doc.Blocks.Add(section2);
            }


            

            Section section3 = new Section();
            section3.FontSize = 12;
            Paragraph paragraph3 = new Paragraph();
            Bold bold = new Bold();
            bold.Inlines.Add("Powered by: www.vendingc.com");
            paragraph3.Inlines.Add(bold);
            
            paragraph3.Margin = new Thickness(15, 10, 5, 0);
            section3.Blocks.Add(paragraph3);
            doc.Blocks.Add(section3);

            return doc;
        }


       
    }
}
