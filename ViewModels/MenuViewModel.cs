using SmartKioskApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace SmartKioskApp.ViewModels
{
    class MenuViewModel
    {
        SqlCommand cmd = null;
        private string sql = null;
        //private string ConnectionString = "Integrated Security=SSPI;" + "Initial Catalog=LocDBKiosk;" + "Data Source=localhost;";
        public static string ConnectionString = ConfigurationManager.AppSettings["LocalCon"].ToString();
        private SqlConnection conn = null;
        SqlDataReader reader;
        public static int ItemsCount;
        public static int CategoriesCount;
        public static string CategoryName = "";
        public ObservableCollection<Category> Categories
        {
            get;
            set;
        }
        public ObservableCollection<Menu> Menus
        {
            get;
            set;
        }
        public ObservableCollection<Cart> myCart
        {
            get;
            set;
        }
        public ObservableCollection<OrderSummary> Orders
        {
            get;
            set;
        }
        public ObservableCollection<Icons> myIcons
        {
            get;
            set;
        }
        public ObservableCollection<Payment> Payments
        {
            get;
            set;
        }

        public void LoadMenu(string CategoryName )
        {
            //LoadIcons();
            ObservableCollection<Menu> menu1 = new ObservableCollection<Menu>();
            conn = new SqlConnection(ConnectionString);
            conn.ConnectionString = ConnectionString;
            conn.Open();
            sql = "select ItemName, Category, PriceQP, PriceHP, PriceSP, ItemImage, IsActive, HasPortion, LiveMenuId from tblMenu " +
            "inner join tblCategory on tblMenu.Category = tblCategory.CategoryName " +
            "where tblMenu.Category = '" + CategoryName + "' and IsActive = 1 and tblMenu.isAvailable = 1 ";
            cmd = new SqlCommand(sql, conn);
            reader = cmd.ExecuteReader();
            byte[] arr;
            var uri = new Uri("pack://application:,,,/Icons/placeholder.png");
            
            try
            {
                while (reader.Read())
                {
                    if (reader.GetValue(5) == DBNull.Value)
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(new BitmapImage(uri)));
                        using (MemoryStream ms = new MemoryStream())
                        {
                            encoder.Save(ms);
                            arr = ms.ToArray();
                        }
                    }
                    else
                    {
                        arr = (byte[])(reader.GetValue(5));
                    }

                    menu1.Add(new Menu { ItemName = Convert.ToString(reader.GetValue(0)).Trim(), PriceQP = Convert.ToInt32(reader.GetValue(2)), PriceMP = Convert.ToInt32(reader.GetValue(3)), PriceSP = Convert.ToInt32(reader.GetValue(4)), ItemImage = arr, HasPortion = Convert.ToBoolean(reader.GetValue(7)), LiveMenuId = Convert.ToInt32(reader.GetValue(8)) });
                }
            }
            catch (Exception ex)
            {

            }
            
            reader.Close();
            cmd.Dispose();
            conn.Close();

            Menus = menu1;

            CountItems(CategoryName);
        }
        public void LoadPaymentDetails(string OrderNo)
        {
            ObservableCollection<Payment> payments = new ObservableCollection<Payment>();
            try
            {
                conn = new SqlConnection(ConnectionString);
                conn.ConnectionString = ConnectionString;
                conn.Open();
                sql = "select  cashType, transactionAmount,   transactionDirection from tblSaleTransaction " +
                "where OrderNo = '" + OrderNo + "' ";
                cmd = new SqlCommand(sql, conn);
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    payments.Add(new Payment { PaymentType = reader.GetString(0), TransactionAmount = reader.GetInt32(1),  TransactionDirection = reader.GetString(2) });
                }
                reader.Close();
                cmd.Dispose();
                conn.Close();

                Payments = payments;
            }
            catch (Exception ex)
            {

            }
            

        }

        public void CountItems(string CatName)
        {
            try
            {
                conn = new SqlConnection(ConnectionString);
                conn.ConnectionString = ConnectionString;
                conn.Open();

                //Query for getting Count
                string QueryCnt = "select count(*) as ItemsCount from tblMenu where category = '" + CatName + "' and tblMenu.isAvailable = 1  ";

                //Execute Queries and save results into variables
                SqlCommand CmdCnt = conn.CreateCommand();
                CmdCnt.CommandText = QueryCnt;

                ItemsCount = (Int32)CmdCnt.ExecuteScalar();
                conn.Close();

            }
            catch (Exception ex)
            {

            }
        }
        public void CountCategories()
        {
            try
            {
                conn = new SqlConnection(ConnectionString);
                conn.ConnectionString = ConnectionString;
                conn.Open();

                //Query for getting Count
                string QueryCnt = "select count(distinct(CategoryName)) from tblCategory where isactive = 1";

                //Execute Queries and save results into variables
                SqlCommand CmdCnt = conn.CreateCommand();
                CmdCnt.CommandText = QueryCnt;

                CategoriesCount = (Int32)CmdCnt.ExecuteScalar();
                conn.Close();

            }
            catch (Exception ex)
            {

            }
        }
        public void LoadCategory()
        {
            ObservableCollection<Category> categories = new ObservableCollection<Category>();

            conn = new SqlConnection(ConnectionString);
            conn.ConnectionString = ConnectionString;
            conn.Open();
            sql = "select * from tblcategory where isactive = 1 order by CategoryName desc";
            cmd = new SqlCommand(sql, conn);
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                byte[] arr = (byte[])(reader.GetValue(2));
                categories.Add(new Category { CatName = Convert.ToString(reader.GetValue(1)).Trim(), CatIcon = arr });
            }
            reader.Close();
            cmd.Dispose();
            conn.Close();
            Categories = categories;
            
        }

        //public void LoadIcons()
        //{
        //    ObservableCollection<Icons> icons = new ObservableCollection<Icons>();
        //    conn = new SqlConnection(ConnectionString);
        //    conn.ConnectionString = ConnectionString;
        //    conn.Open();
        //    sql = "select * from tbl_icons ";
        //    cmd = new SqlCommand(sql, conn);
        //    reader = cmd.ExecuteReader();
        //    byte[] arr;
        //    while (reader.Read())
        //    {
        //        arr = (byte[])(reader.GetValue(2));
        //        icons.Add(new Icons { Description = Convert.ToString(reader.GetValue(1)).Trim(), Icon = arr });
        //    }
        //    reader.Close();
        //    cmd.Dispose();
        //    conn.Close();
        //    myIcons = icons;

        //}
        public void LoadCart()
        {
            ObservableCollection<Cart> cart = new ObservableCollection<Cart>();


            myCart = cart;
        }
        public void LoadOrderSummary()
        {
            ObservableCollection<OrderSummary> orderSummaries = new ObservableCollection<OrderSummary>();


            Orders = orderSummaries;
        }

    }
}
