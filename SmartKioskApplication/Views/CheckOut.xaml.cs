using Newtonsoft.Json;
using SmartKioskApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
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
using static SmartKioskApp.Global;

namespace SmartKioskApp.Views
{
    /// <summary>
    /// Interaction logic for CheckOut.xaml
    /// </summary>
    public partial class CheckOut : Window
    {
        public bool PaymentConfirmed = false;

        bool TransactionCompleted = false;
        public static string PaymentMethod;
        bool InAction = false;
        string _method = "";
        public bool CloseThisScreen = false;
        int cashAmount = 0;
        public bool IsBack = false;
        public bool IsPaymentCancelled = false;
        string dateTime = "";
        public  bool QRPayConfirmed = false;
        public bool CardPayConfirmed = false;

        SqlCommand cmd = null;
        private string sql = null;
        private string ConnectionString = "Integrated Security=SSPI;" + "Initial Catalog=LocDBKiosk;" + "Data Source=localhost;";
        private SqlConnection conn = null;
        SqlDataReader reader;

        
        DispatcherTimer dispatcherTimer = new DispatcherTimer();
        DispatcherTimer dispatcherTimerQR = new DispatcherTimer();
        DispatcherTimer screenTimer = new DispatcherTimer();
        DispatcherTimer dispatcherTimerCard = new DispatcherTimer();
        private readonly DispatcherTimer timer = new DispatcherTimer();

        SerialPort _serialPort = new SerialPort();

        private int lastDay;
        public CheckOut()
        {
            InitializeComponent();

            MenuViewModel menuViewModel = new MenuViewModel();
            menuViewModel.LoadCategory();

            var uri = new Uri("pack://application:,,,/Icons/back.png");
            backIcon.Source = new BitmapImage(uri);

            Back.Visibility = Visibility.Visible;
            DueAmountBar.Visibility = Visibility.Hidden;
            CreditAmountBar.Visibility = Visibility.Hidden;
            imgNotesAccepted.Visibility = Visibility.Hidden;
            txtInstructNotes.Visibility = Visibility.Hidden;
            imgQRMethods.Visibility = Visibility.Hidden;
            txtQRTotal.Visibility = Visibility.Hidden;
            imgCardsAccepted.Visibility = Visibility.Hidden;

            var uri1 = new Uri("pack://application:,,,/Icons/cash pay.jpg");
            imgCashPay.Source = new BitmapImage(uri1);

            var uri2 = new Uri("pack://application:,,,/Icons/qr pay.png");
            imgQRPay.Source = new BitmapImage(uri2);

            var uri3 = new Uri("pack://application:,,,/Icons/Cash NOTES.png");
            imgNotesAccepted.Source = new BitmapImage(uri3);

            var uri4 = new Uri("pack://application:,,,/Icons/QR payment methods.png");
            imgQRMethods.Source = new BitmapImage(uri4);

            var uri5 = new Uri("pack://application:,,,/Icons/card pay.png");
            imgCardPay.Source = new BitmapImage(uri5);

            var uri6 = new Uri("pack://application:,,,/Icons/card payment methods.png");
            imgCardsAccepted.Source = new BitmapImage(uri6);


           
            screenTimer.Tick += new EventHandler(ScreenTimeOut_Tick);
            screenTimer.Interval = new TimeSpan(0, 0, 60);
            screenTimer.Start();

            this.timer.Interval = TimeSpan.FromSeconds(1);
            this.timer.Tick += this.timer_Tick;
            this.lastDay = DateTime.Now.Date.Day;
            this.timer.Start();

        }
        public void btnPayWithCash(object sender, EventArgs e)
        {

            //disable Note Validator
            ReadWrite.Write("Neutral", Global.Actions.Enabled.ToString());
            PaymentBar.Visibility = Visibility.Hidden;
            Back.Visibility = Visibility.Hidden;
            txtPayInstruct.Visibility = Visibility.Hidden;
            PaymentQR.Visibility = Visibility.Hidden;
            imgQRMethods.Visibility = Visibility.Hidden;
            txtQRTotal.Visibility = Visibility.Hidden;
            PaymentCard.Visibility = Visibility.Hidden;
            imgCardsAccepted.Visibility = Visibility.Hidden;
            txtTotalAmount1.Visibility = Visibility.Visible;
            txtPayInstruct1.Visibility = Visibility.Visible;
            txtPayInstruct3.Visibility = Visibility.Hidden;
            DueAmountBar.Visibility = Visibility.Visible;
            CreditAmountBar.Visibility = Visibility.Visible;
            imgNotesAccepted.Visibility = Visibility.Visible;
            txtInstructNotes.Visibility = Visibility.Visible;
            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
            Global.General.CreditAmount = 0;
            PaymentMethod = "Cash";
            if (dispatcherTimer.IsEnabled)
            {
                dispatcherTimer.Stop();
            }

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
            

        }
        public int counter = 0;
        private async void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            string _method = "cash";
            if (!InAction)
            {
                InAction = true;
                counter++;
                try
                {
                    string msg = ReadWrite.Read(Global.Actions.Rejection.ToString());
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        txtValidNote.Visibility = Visibility.Visible;
                        txtValidNote.Text = msg;
                        StartCloseTimer();

                        ReadWrite.Write("", Global.Actions.Rejection.ToString());
                        ReadWrite.Write("Neutral", Global.Actions.Enabled.ToString());
                        StartCloseTimer();
                    }
                    if (Global.NextAction == Global.ActionList.None)
                    {
                        Global.NextAction = Global.ActionList.CollectingMoney;

                    }
                    if (Global.NextAction == Global.ActionList.CollectingMoney)
                    {
                        Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                        if (_method == "cash")
                        {
                            if (cashAmount != Global.General.CreditAmount)
                            {
                                screenTimer.Stop();
                                screenTimer.Start();
                                
                            }

                            cashAmount = Global.General.CreditAmount;
                            ItemMenu.orders[0].InsertedAmount = Global.General.CreditAmount;

                            if (CashHandler.GetCashInAmount() >= Global.cartTotalAmount)
                            {
                                Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                ItemMenu.orders[0].InsertedAmount = Global.General.CreditAmount;
                                Global.NextAction = Global.ActionList.StartDispensing;
                                
                            }
                            else
                            {
                                Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                ItemMenu.orders[0].InsertedAmount = Global.General.CreditAmount;
                                txtCreditAmount.DataContext = ItemMenu.orders[0];

                                await txtCreditAmount.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    //if (ItemMenu.orders[0].InsertedAmount > 0)
                                    //{
                                    //    Back.Visibility = Visibility.Hidden;
                                    //}
                                    txtCreditAmount.Content = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                }));
                                //if (counter <= 40)
                                //{
                                //    InAction = false;
                                //}
                            }
                        }
                    }
                    if (Global.NextAction == Global.ActionList.StartDispensing)
                    {
                        ReadWrite.Write("Stop", Global.Actions.Enabled.ToString());
                        Global.General.CashInserted = Global.General.CreditAmount;
                        dispatcherTimer.Stop();
                        PaymentConfirmed = true;
                    }
                    if (CloseThisScreen)
                    {
                        screenTimer.Stop();
                        dispatcherTimer.Stop();
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Global.General.CreditAmount = 0;
                    Global.NextAction = Global.ActionList.None;
                    InAction = false;
                    
                }
            }
            if (PaymentConfirmed)
            {

                Close();
                this.Close();

            }
        }
        public async void ScreenTimeOut_Tick(object sender, EventArgs e)
        {
            try
            {
                screenTimer.Stop();
                ReadWrite.Write("Stop", Global.Actions.Enabled.ToString());
                this.Close();
                CloseThisScreen = true;
                dispatcherTimer.Stop();
                dispatcherTimerCard.Stop();
                dispatcherTimerQR.Stop();
                dispatcherTimer = null;
                screenTimer = null;
                if (counter > 0)
                {

                }
                _serialPort.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        
        public static int GenNewTicketNo()
        {
            int TicketNo = Convert.ToInt32(ReadWrite.Read(Global.Actions.SaleNo.ToString()));
            TicketNo++;
            ReadWrite.Write(TicketNo.ToString(), Global.Actions.SaleNo.ToString());
            return TicketNo;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now.Date.Day != this.lastDay)
            {
                this.lastDay = DateTime.Now.Date.Day;
                ReadWrite.Write("0", Global.Actions.SaleNo.ToString());
            }
        }



        private void btnBack(object sender, RoutedEventArgs e)
        {
            IsBack = true;
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            screenTimer.Stop();
            dispatcherTimer.Stop();
            dispatcherTimer = null;
            screenTimer = null;
            
            ReadWrite.Write("Stop", Global.Actions.Enabled.ToString());
            this.Hide();
        }
        private void btnPayWithCard(object sender, RoutedEventArgs e)
        {
            CardPayConfirmed = false;
            _method = "Card";
            Back.Visibility = Visibility.Visible;
            dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            PaymentBar.Visibility = Visibility.Hidden;
            txtPayInstruct.Visibility = Visibility.Hidden;
            PaymentQR.Visibility = Visibility.Hidden;
            imgNotesAccepted.Visibility = Visibility.Hidden;
            PaymentCard.Visibility = Visibility.Hidden;
            txtInstructNotes.Visibility = Visibility.Hidden;

            DueAmountBar.Visibility = Visibility.Visible;
            txtQRTotal.Visibility = Visibility.Visible;
            txtTotalAmount1.Visibility = Visibility.Hidden;
            txtQRTotal.Text = ItemMenu.QRdueAmount.ToString();
            imgQRMethods.Visibility = Visibility.Hidden;
            imgCardsAccepted.Visibility = Visibility.Visible;
            txtPayInstruct2.Visibility = Visibility.Hidden;
            txtPayInstruct1.Visibility = Visibility.Hidden;
            txtPayInstruct3.Visibility = Visibility.Visible;
            CreditAmountBar.Visibility = Visibility.Visible;
            QRcode.Visibility = Visibility.Hidden;
            txtInstructNotes.Visibility = Visibility.Hidden;
            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
            PaymentMethod = "Card";
            _serialPort.Close();
            dispatcherTimerCard.Tick += new EventHandler(dispatcherTimerCard_Tick);
            dispatcherTimerCard.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimerCard.Start();
        }

        private async void dispatcherTimerCard_Tick(object sender, EventArgs e)
        {
            if (!InAction)
            {
                InAction = true;
                try
                {
                    if (Global.NextAction == Global.ActionList.None)
                    {
                        Global.NextAction = Global.ActionList.CollectingMoney;
                    }
                    if (Global.NextAction == Global.ActionList.CollectingMoney)
                    {
                        _ = TransactionSales(ItemMenu.QRdueAmount.ToString());
                        if (ReadWrite.PaidAmount > 0)
                        {
                            ReadWrite.Write(ReadWrite.PaidAmount.ToString(), Global.Actions.AddToAmount.ToString());
                        }
                        Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                        if (_method == "Card")
                        {
                            if (ReadWrite.PaidAmount != Global.General.CreditAmount)
                            {
                                screenTimer.Stop();
                                screenTimer.Start();
                            }
                            cashAmount = Global.General.CreditAmount;
                            if (CashHandler.GetCashInAmount() >= Global.cartTotalAmount)
                            {
                                Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                Console.WriteLine("Transaction in process!");
                                Global.NextAction = Global.ActionList.StartDispensing;
                                screenTimer.Stop();
                                dispatcherTimer.Stop();
                                Close();
                            }
                            else
                            {
                                Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                ItemMenu.orders[0].InsertedAmount = Global.General.CreditAmount;

                            }

                        }
                    }

                    if (Global.NextAction == Global.ActionList.StartDispensing)
                    {
                        dispatcherTimerCard.Stop();
                        CardPayConfirmed = true;
                    }
                    if (CloseThisScreen)
                    {
                        screenTimer.Stop();
                        dispatcherTimer.Stop();
                        Close();
                        _serialPort.Close();
                        CloseThisScreen = false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Global.General.CreditAmount = 0;
                    Global.General.CashInserted = 0;
                    Global.NextAction = Global.ActionList.None;
                    InAction = false;
                }
            }
            if (CardPayConfirmed)
            {

                Close();
                this.Close();
            }
        }
        public  async Task TransactionSales(string amount)
        {


            _serialPort = new SerialPort();
            _serialPort.PortName = Global.PORT_NUM;
            _serialPort.BaudRate = Global.BAUD_RATE;
            _serialPort.ReadTimeout = 65000;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;

            string ItemResult = "";
            string message = "";
            string message1 = "";
            string message2 = "";

            //convert amount into the 12 digit amount 

            amount = "0000" + new string('0', 12 - amount.Length) + amount ;
            Console.WriteLine(amount);
            if (!_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Open();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            
            try
            {

                //Clear POS Serial Port
                _serialPort.Write("");
                //Sending amount to POS machine signal
                _serialPort.Write(amount);
                // Thread.Sleep(3000);
                if (!(message.ToLower().Contains("response") || message.ToLower().Contains("cancelled")))
                {
                    message += _serialPort.ReadExisting();
                    Console.WriteLine("read: " + message);
                }
                Console.WriteLine("Pos read: " + message);
                if (message.ToLower().Contains("date"))
                {
                    //Global.VMPosMachine.Timeout = false;
                    message2 = "Date=";
                    message1 = message.Substring(message.IndexOf(message2) + message2.Length, message.IndexOf('\n'));
                    message2 = "Time=";
                    message1 = message.Substring(message.IndexOf(message2) + message2.Length, message.IndexOf('\n'));
                    message2 = "TID=";
                    message1 = message.Substring(message.IndexOf(message2) + message2.Length, message.IndexOf('\n'));
                    message2 = "MID=";
                    message1 = message.Substring(message.IndexOf(message2) + message2.Length, message.IndexOf('\n'));
                    message2 = "Total Amount=";
                    message1 = message.Substring(message.IndexOf(message2) + message2.Length, message.IndexOf('\n'));
                    //message2 ="Response=";
                    //message1 = message.Substring(message.IndexOf(message2) + message2.Length, message.IndexOf('\n')-1);
                    //Console.WriteLine("Response:" + message1);
                    CardPayConfirmed = true;
                    //break;
                }
                if (message.ToLower().Contains("cancelled"))
                {
                    //Global.VMPosMachine.Timeout = true;
                    ItemResult = "Timeout";
                    //break;
                }
            }
            catch (Exception ex)
            {
                //Global.VMPosMachine.Timeout = false;
                ItemResult = "Timeout";
                //break;
            }

            _serialPort.Close();
            Console.WriteLine( message);
        }

        private void btnPayWithQR(object sender, RoutedEventArgs e)
        {
            _ = FunGenerateQR();
            _method = "QR";
            QRPayConfirmed = false;
            Back.Visibility = Visibility.Visible;
            dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            PaymentBar.Visibility = Visibility.Hidden;
            txtPayInstruct.Visibility = Visibility.Hidden;
            PaymentQR.Visibility = Visibility.Hidden;
            imgNotesAccepted.Visibility = Visibility.Hidden;
            PaymentCard.Visibility = Visibility.Hidden;

            DueAmountBar.Visibility = Visibility.Visible;
            txtQRTotal.Visibility = Visibility.Visible;
            txtTotalAmount1.Visibility = Visibility.Hidden;
            txtPayInstruct3.Visibility = Visibility.Hidden;
            imgCardsAccepted.Visibility = Visibility.Hidden;
            txtQRTotal.Text = ItemMenu.QRdueAmount.ToString();
            imgQRMethods.Visibility = Visibility.Visible;
            txtPayInstruct2.Visibility = Visibility.Visible;
            CreditAmountBar.Visibility = Visibility.Visible;
            QRcode.Visibility = Visibility.Visible;
            txtInstructNotes.Visibility = Visibility.Hidden;
            ReadWrite.Write("0", Global.Actions.AddToAmount.ToString());
            PaymentMethod = "QR";
            dispatcherTimerQR.Tick += new EventHandler(dispatcherTimerQR_Tick);
            dispatcherTimerQR.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimerQR.Start();
        }
        private async Task<bool> FunGenerateQR()
        {
            string @UserId = "DigitalTrans_QR";
            string @Password = "Alf@D!g!t@l*1";
            string @TransactionId = "DT123";
            string @CompanyId = "DigiTrans";
            string @ProductId = "DT456";
            string @OrderId = "DT789";
            int @Amount = ItemMenu.QRdueAmount;
            string @MerchantPAN = ConfigurationManager.AppSettings["PAN"].ToString();
            //string @MerchantPAN = "5333387519489734";
            string @DataHash = "";
            // Hash calculation  ///
            string ConcatenateStringofRequest = @UserId + "+" + @Password + "+" + @TransactionId + "+" + @CompanyId + "+" + @ProductId + "+" + @OrderId + "+" + @Amount + "+" + @MerchantPAN;
            byte[] key = Encoding.ASCII.GetBytes("88moQHxKJbtvFzk4yqF7CAwgeGyMYVakbSNPjPOKWIQYB6xgikjqf5caaQiVqgQ");
            HMACSHA256 myhmacsha1 = new HMACSHA256(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(ConcatenateStringofRequest);
            MemoryStream stream = new MemoryStream(byteArray);
            @DataHash = myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
            Console.WriteLine("Hitting Api .......!");
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://payments.bankalfalah.com/mVisaAcq/AlfaPay.svc/GenerateQRString");
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var requestBody = new
                    {
                        UserId = @UserId,
                        Password = @Password,
                        TransactionId = @TransactionId,
                        CompanyId = @CompanyId,
                        ProductId = @ProductId,
                        OrderId = @OrderId,
                        Amount = @Amount,
                        MerchantPAN = @MerchantPAN,
                        DataHash = @DataHash
                    };
                    HttpResponseMessage apiResponse = await client.PostAsJsonAsync(client.BaseAddress, requestBody);
                    _ResponseData _responseData = new _ResponseData();

                    Console.WriteLine("Response API .......!\r\n");
                    Console.WriteLine(apiResponse + "\r\n");
                    if (apiResponse.IsSuccessStatusCode)
                    {
                        var documentResponse = await apiResponse.Content.ReadAsStringAsync();
                        _responseData = JsonConvert.DeserializeObject<_ResponseData>(documentResponse);
                        Console.WriteLine(_responseData);
                        Console.WriteLine("URL: {0}", _responseData.URL);
                        Console.WriteLine("ResponseCode: {0}", _responseData.ResponseCode);
                        Console.WriteLine("Response Description: {0}", _responseData.ResponseDesc);
                        Console.WriteLine("QR-Code Image string: {0}", _responseData.QRCode);


                        //byte[] arr = Encoding.ASCII.GetBytes(_responseData.QRCode);
                        byte[] img = Convert.FromBase64String(_responseData.QRCode);
                        QRcode.Source = ItemMenu.BitmaSourceFromByteArray(img);

                    }
                    //  creditamount = @Amount;

                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async void dispatcherTimerQR_Tick(object sender, EventArgs e)
        {
            if (!InAction )
            {
                InAction = true;
                try
                {
                    if (Global.NextAction == Global.ActionList.None)
                    {
                        Global.NextAction = Global.ActionList.CollectingMoney;
                    }
                    if (Global.NextAction == Global.ActionList.CollectingMoney)
                    {
                        //dateTime = "2022-04-25 ";
                        _ = ReadWrite.ReadQRAmountAPIAsync(dateTime, ConfigurationManager.AppSettings["PAN"].ToString());
                        if (ReadWrite.PaidAmount > 0)
                        {
                            ReadWrite.Write(ReadWrite.PaidAmount.ToString(), Global.Actions.AddToAmount.ToString());
                        } 
                        Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                        if (_method == "QR")
                        {
                            if (ReadWrite.PaidAmount != Global.General.CreditAmount)
                            {
                                screenTimer.Stop();
                                screenTimer.Start();
                            }
                            cashAmount = Global.General.CreditAmount;
                            if (CashHandler.GetCashInAmount() >= Global.cartTotalAmount)
                            {
                                Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                Console.WriteLine("Transaction in process!");
                                Global.NextAction = Global.ActionList.StartDispensing;
                                screenTimer.Stop();
                                dispatcherTimer.Stop();
                                Close();
                            }
                            else
                            {
                                Global.General.CreditAmount = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
                                ItemMenu.orders[0].InsertedAmount = Global.General.CreditAmount;

                            }

                        }
                    }

                    if (Global.NextAction == Global.ActionList.StartDispensing)
                    {
                        dispatcherTimerQR.Stop();
                        QRPayConfirmed = true;
                    }
                    if (CloseThisScreen)
                    {
                        screenTimer.Stop();
                        dispatcherTimer.Stop();
                        Close();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Global.General.CreditAmount = 0;
                    Global.General.CashInserted = 0;
                    Global.NextAction = Global.ActionList.None;
                    InAction = false;
                }
            }
            if (QRPayConfirmed)
            {

                Close();
                this.Close();
            }
        }
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(2d);
            timer.Tick += TimerTick;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;


            string msg = ReadWrite.Read(Global.Actions.Rejection.ToString());
            txtValidNote.Text = msg;
        }
    }
}
