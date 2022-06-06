using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartKioskApp.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Login login = new Login { Name = "mansoorbaloch", Password = 13141 };

        //private string ConnectionString = "Integrated Security=False;" + "Initial Catalog=;" + "Data Source=localhost;" + "User Id =Kiosk;" +
        //    "Password =sa123;";
        //private string ConnectionString = "server = DESKTOP-2QQTON3; " + "database = master;" + "integrated security = SSPI;";
        //private string ConnectionString = "Integrated Security=SSPI;" + "Initial Catalog=;" + "Data Source=localhost;";
        //private string ConnectionString1 = "Integrated Security=SSPI;" + "Initial Catalog=LocDBKiosk;" + "Data Source=DESKTOP-2QQTON3;";

        public static string ConnectionString = ConfigurationManager.AppSettings["LocalCon"].ToString();

        private SqlDataReader reader = null;
        private SqlConnection conn = null;
        private SqlCommand cmd = null;
        private string sql = null;
        string txtDatabase = "LocDBKiosk";
        private string result;
        public const string url = "http://203.135.63.93/api/KioskMangement/GetCupcakeMenus";
        public MainWindow()
        {
            InitializeComponent();
            //CreateDatabaseIfNotExists(ConnectionString, txtDatabase);

            //CreateCartTable(ConnectionString);
            //CreateOrdersTable(ConnectionString);
            //CreateMenuTable(ConnectionString);
            //StartPureNV();
            //create database and related tables -code first
            //try
            //{
            //    using (var ctx = new KioskContext())
            //    {
            //        //var stud = new Models.Menus() ;
            //        var stud = new Menu() { };
            //        var cat = new Category() { };
            //        var order = new Order() { };


            //        ctx.Menu.Add(stud);
            //        ctx.Categories.Add(cat); 
            //        ctx.Orders.Add(order);

            //        ctx.SaveChanges();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
            var uri = new Uri("pack://application:,,,/Icons/Kiosk logo.png");
            kiosklogo.Source = new BitmapImage(uri);

            if (ReadWrite.Read(Global.Actions.todaysDate.ToString()) == Convert.ToString(0))
            {
                ReadWrite.Write(DateTime.Now.ToString("MM/dd/yyyy"), Global.Actions.todaysDate.ToString());
            }
            if (ReadWrite.Read(Global.Actions.todaysDate.ToString()) != DateTime.Now.ToString("MM/dd/yyyy"))
            {
                ReadWrite.Write("0", Global.Actions.SaleNo.ToString());
            }

            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
            ReadWrite.Write("Stop", Global.Actions.Enabled.ToString());

            ReadInsertMenu();
        }
        public async void ReadInsertMenu()
        {
            DataTable responseObj = new DataTable();

            List<myMenu> list1 = new List<myMenu>();
            try
            {
                using (var client = new HttpClient())
                {
                    using (var httpRequest = new HttpRequestMessage(new HttpMethod("GET"), url))
                    {
                        //var base64authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes("place_your_toke_here"));
                        //httpRequest.Headers.TryAddWithoutValidation("Authorization", $"Basic {base64authorization}");

                        //httpRequest.Accept = "application/json";
                        //httpRequest.ContentType = "application/json";

                        var response = await client.SendAsync(httpRequest);

                        HttpContent responseContent = response.Content;

                        using (var reader = new StreamReader(await responseContent.ReadAsStreamAsync()))
                        {
                            result = await reader.ReadToEndAsync();
                            result = result.TrimStart('\"');
                            result = result.TrimEnd();
                            result = result.TrimEnd('\"');
                            result = result.Replace("\\", "");

                            var obj = JsonConvert.DeserializeObject<myMenu[]>(result);
                            
                            list1 = obj.ToList();
                            Console.WriteLine("", list1.Count);
                        }
                    }
                }
                
            }

            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }
            try
            {
                //set the active status of all items to false
                using (SqlConnection conn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("Update  tblMenu set IsAvailable = 'False'", conn))
                    {
                        conn.Open();
                        int rowsAffeced = cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
                for (int i = 0; i < list1.Count; i++)
                {
                    using (SqlConnection conn = new SqlConnection(ConnectionString))
                    {
                        //updates the active flag of only the active menu items
                        using (SqlCommand cmd = new SqlCommand("Update  tblMenu set IsAvailable = 'True' where LiveMenuId = @MenuId ", conn))
                        {
                            cmd.Parameters.AddWithValue("@MenuId", list1[i].MenuId);
                            cmd.Parameters.AddWithValue("@IsAvailable", list1[i].IsAvailable);
                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            conn.Close();
                        }

                        //inserts menu items in the DB if the item already doesn't exists

                        using (SqlCommand cmd = new SqlCommand("if exists( select * from tblmenu where livemenuid = @LiveMenuId) begin (select '') end else begin INSERT INTO tblMenu" +
                            "(LiveMenuId, ItemName, Category, PriceQP, PriceHP, PriceSP, ItemImage, HasPortion, IsAvailable ) VALUES (@LiveMenuId, @ItemName,  @Category, @PriceQP, @PriceHP, @PriceSP, @ItemImage, @HasPortion, @IsAvailable ) end", conn))
                        //using (SqlCommand cmd = new SqlCommand("INSERT INTO tblMenu (LiveMenuId, ItemName, Category, PriceQP, PriceHP, PriceSP, ItemImage, HasPortion, IsAvailable ) VALUES (@LiveMenuId, @ItemName,  @Category, @PriceQP, @PriceHP, @PriceSP, @ItemImage, @HasPortion, @IsAvailable )", conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            cmd.Parameters.AddWithValue("@ItemName", list1[i].ItemName);
                            cmd.Parameters.AddWithValue("@Category", list1[i].Category);
                            cmd.Parameters.AddWithValue("@LiveMenuId", list1[i].MenuId);
                            cmd.Parameters.AddWithValue("@PriceQP", list1[i].PriceQp);
                            cmd.Parameters.AddWithValue("@PriceHP", list1[i].PriceHp);
                            cmd.Parameters.AddWithValue("@PriceSP", list1[i].PriceSp);
                            cmd.Parameters.AddWithValue("@ItemImage", list1[i].ItemImage);
                            cmd.Parameters.AddWithValue("@HasPortion", list1[i].HasPortion);
                            cmd.Parameters.AddWithValue("@IsAvailable", list1[i].IsAvailable);

                            conn.Open();
                            int rowsAffected = cmd.ExecuteNonQuery();
                            conn.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
        public class myMenu
        {
            public int MenuId { get; set; }
            public string ItemName { get; set; }
            public string Category { get; set; }
            public int PriceQp { get; set; }
            public int PriceHp { get; set; }
            public int PriceSp { get; set; }
            public byte[] ItemImage { get; set; }
            public bool HasPortion { get; set; }
            public bool IsAvailable { get; set; }
            //public List<myMenu> MyMenu { get; set; }
        }

        //public void StartPureNV()
        //{
        //    ////check if Note Validator Application is running
        //    if (Process.GetProcessesByName("PureNV").Length > 0)
        //    {
        //        // Is running
        //    }
        //    else
        //    {
        //        Process.Start(ConfigurationManager.AppSettings["NvApp"].ToString());


        //    }
        //    Thread.Sleep(2500);
        //    ReadWrite.Write("Stop", Global.Actions.Enabled.ToString());
        //}
        public void CreateDatabaseIfNotExists(string connectionString, string dbName)
        {
            SqlCommand cmd = null;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (cmd = new SqlCommand($"If(db_id(N'{dbName}') IS NULL) CREATE DATABASE [{dbName}]", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void CreateCartTable(string ConnectionString)
        {
            ConnectionString = "Integrated Security=SSPI;" +
            "Initial Catalog=LocDBKiosk;" +
            "Data Source=localhost;";

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                bool fixTableExists = CheckSQLTable(sqlCon, "tblCart");
                //Create New Table If table is not available
                if (!fixTableExists)
                {
                    conn = new SqlConnection(ConnectionString);
                    //Open the connection
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                    sql = "CREATE TABLE tblCart" +
                    "(ID INTEGER IDENTITY(1,1) CONSTRAINT PKeyCart PRIMARY KEY," +
                    "OrderNo CHAR(50), ItemName CHAR(50), Portion CHAR(50),  Price int NULL,  Quantity int NULL)";

                    cmd = new SqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    conn.Close();
                }

            }


        }
        public static bool CheckSQLTable(SqlConnection sqlCon, string tableName)
        {
            bool checkTable;
            string sqlCmdExTableCheck = @"SELECT count(*) as IsExists FROM dbo.sysobjects where id = object_id('[dbo].[" + tableName + "]')";

            using (SqlCommand sqlCmd = new SqlCommand(sqlCmdExTableCheck, sqlCon))
            {
                sqlCon.Open();
                try
                {
                    checkTable = ((int)sqlCmd.ExecuteScalar() == 1);
                    {
                        sqlCon.Close();
                        return checkTable;
                    }
                }
                catch
                {
                    checkTable = false;
                    sqlCon.Close();
                    return checkTable;
                }
            }
        }
        public void CreateMenuTable(string ConnectionString)
        {

            //ConnectionString = "Integrated Security=SSPI;" +
            //"Initial Catalog=LocDBKiosk;" +
            //"Data Source=localhost;";

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                bool fixTableExists = CheckSQLTable(sqlCon, "tblMenu");
                //Create New Table If table is not available
                if (!fixTableExists)
                {
                    conn = new SqlConnection(ConnectionString);
                    // Open the connection  
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                    try
                    {
                        sql = "CREATE TABLE tblMenu" +
                        "(MenuId INTEGER IDENTITY(1,1) CONSTRAINT PKeyMenu PRIMARY KEY," +
                        "ItemName CHAR(50), Category CHAR(50), PriceQP int NULL, PriceHP int NULL, PriceSP int NULL, ItemImage image NULL)";
                        cmd = new SqlCommand(sql, conn);

                        cmd.ExecuteNonQuery();
                        // Adding records the table  
                        sql = "INSERT INTO tblMenu(ItemName, Category , PriceQP , PriceHP , PriceSP, ItemImage ) " +
                            "SELECT 'Chicken Biryani', 'lunch', 200, 250, 300, BulkColumn FROM Openrowset(Bulk 'F:\\Mansoor Baloch\\Kiosk\\Menu Photos\\biryani.PNG', Single_Blob) as img";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "INSERT INTO tblMenu(ItemName, Category , PriceQP , PriceHP , PriceSP, ItemImage ) " +
                            "SELECT 'Chicken Karahi', 'lunch', 100, 150, 200, BulkColumn FROM Openrowset(Bulk 'F:\\Mansoor Baloch\\Kiosk\\Menu Photos\\karahi.jpg', Single_Blob) as img";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "INSERT INTO tblMenu(ItemName, Category , PriceQP , PriceHP , PriceSP, ItemImage ) " +
                            "SELECT 'Mix Sabzi', 'lunch', 100, 120, 140, BulkColumn FROM Openrowset(Bulk 'F:\\Mansoor Baloch\\Kiosk\\Menu Photos\\mix sabzi.PNG', Single_Blob) as img";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "INSERT INTO tblMenu(ItemName, Category , PriceQP , PriceHP , PriceSP, ItemImage ) " +
                            "SELECT 'Pulao', 'lunch', 220, 230, 240, BulkColumn FROM Openrowset(Bulk 'F:\\Mansoor Baloch\\Kiosk\\Menu Photos\\mix sabzi.PNG', Single_Blob) as img";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "INSERT INTO tblMenu(ItemName, Category , PriceQP , PriceHP , PriceSP, ItemImage ) " +
                            "SELECT 'Aaloo Qeema', 'lunch', 300, 320, 340, BulkColumn FROM Openrowset(Bulk 'F:\\Mansoor Baloch\\Kiosk\\Menu Photos\\biryani.PNG', Single_Blob) as img";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();

                        sql = "INSERT INTO tblMenu(ItemName, Category , PriceQP , PriceHP , PriceSP, ItemImage ) " +
                            "SELECT 'Haleem', 'lunch', 100, 120, 140, BulkColumn FROM Openrowset(Bulk 'F:\\Mansoor Baloch\\Kiosk\\Menu Photos\\karahi.jpg', Single_Blob) as img";
                        cmd = new SqlCommand(sql, conn);
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        conn.Close();

                    }
                    catch (SqlException ae)
                    {

                    }
                }
            }
        }
        public void CreateOrdersTable(string ConnectionString)
        {

            ConnectionString = "Integrated Security=SSPI;" +
            "Initial Catalog=LocDBKiosk;" +
            "Data Source=localhost;";

            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                bool fixTableExists = CheckSQLTable(sqlCon, "tblTransactionSummary");
                //Create New Table If table is not available
                if (!fixTableExists)
                {
                    conn = new SqlConnection(ConnectionString);
                    // Open the connection  
                    if (conn.State == ConnectionState.Open)
                        conn.Close();
                    conn.ConnectionString = ConnectionString;
                    conn.Open();
                    try
                    {
                        sql = "CREATE TABLE tblTransactionSummary" +
                        "(ID INTEGER IDENTITY(1,1) CONSTRAINT PKeyOrders PRIMARY KEY," +
                        "OrderNo CHAR(50),   InsertedAmount int NULL, DueAmount int NULL, RemainingAmount int NULL, TicketNumber int NULL )";
                        cmd = new SqlCommand(sql, conn);

                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        conn.Close();
                    }
                    catch (SqlException ae)
                    {

                    }
                }
            }

        }
        private void btnLogin(object sender, RoutedEventArgs e)
        {
            try
            {

                this.Hide();

                ItemMenu menus = new ItemMenu();
                menus.Show();


            }
            catch (Exception ex)
            {

            }
        }
    }

    public class Login
    {
        public string Name { get; set; }
        public double Password { get; set; }

    }
    public class KioskContext : DbContext
    {
        public KioskContext() : base("name=KioskDBConnectionString")
        {

        }

        public DbSet<Menu> Menu { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }


    }
    public class Menu
    {
        public int MenuID { get; set; }
        public string ItemName { get; set; }
        public string Category { get; set; }

        public int PriceQP { get; set; }
        public int PriceMP { get; set; }
        public int PriceSP { get; set; }
        public byte[] ItemImage { get; set; }
        public bool HasPortion { get; set; }
        public bool IsAvailable { get; set; }
        //public Category Categories { get; set; }

    }
    public class Category
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public byte[] CategoryIcon { get; set; }
        public bool IsActive { get; set; }
        //public ICollection<Menu> Menu { get; set; }

    }
    public class Order
    {
        public int OrderID { get; set; }
        public string OrderNo { get; set; }
        public int InsertedAmount { get; set; }
        public int DueAmount { get; set; }
        public int RemainingAmount { get; set; }
        public int TicketNumber { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string PaymentType { get; set; }
    }


}

