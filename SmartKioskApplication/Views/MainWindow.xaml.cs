using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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

        public MainWindow()
        {
            InitializeComponent();
            //CreateDatabaseIfNotExists(ConnectionString, txtDatabase);

            //CreateCartTable(ConnectionString);
            //CreateOrdersTable(ConnectionString);
            //CreateMenuTable(ConnectionString);
            //StartPureNV();

            
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
        }

        public void StartPureNV()
        {
            ////check if Note Validator Application is running
            if (Process.GetProcessesByName("PureNV").Length > 0)
            {
                // Is running
            }
            else
            {
                Process.Start(ConfigurationManager.AppSettings["NvApp"].ToString());


            }
            Thread.Sleep(2500);
            ReadWrite.Write("Stop", Global.Actions.Enabled.ToString());
        }
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
   
}

