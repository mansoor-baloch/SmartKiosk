using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SmartKioskApp.ViewModels
{
    class PostData
    {
        
        public static void CreateJSON()
        {
            MenuViewModel menuViewModel = new MenuViewModel();
            try
            {
                Orders orders1 = new Orders();
                orders1.OrderNo = Views.ItemMenu.orders[0].OrderNo;
                orders1.TicketNo = Views.ItemMenu.orders[0].TicketNumber;
                orders1.purchaseAmount = Views.ItemMenu.CashDueAmount;
                orders1.PaidAmount = Views.ItemMenu.orders[0].InsertedAmount;
                orders1.RemainingAmount = Views.ItemMenu.orders[0].RemainingAmount;
                orders1.machineId = "PDC-1";
                orders1.OrderDateTime = Views.ItemMenu.OrderDateTime;
                orders1.isCompleted = Views.ItemMenu.PaymentCompleted;
                orders1.paymentType = Views.CheckOut.PaymentMethod;
                
                for (int i = 0; i < Views.ItemMenu.carts.Count; i++)
                {

                    CartDetails cartDetails = new CartDetails();

                    cartDetails.Portion = Views.ItemMenu.carts[i].Portion;
                    cartDetails.Quantity = Views.ItemMenu.carts[i].Quantity;

                    cartDetails.selectedDateTime = Views.ItemMenu.OrderDateTime;
                    cartDetails.purchasedItemId = Views.ItemMenu.carts[i].LiveMenuId;


                    orders1.tblOrderDetails.Add(cartDetails);
                }
                menuViewModel.LoadPaymentDetails(Views.ItemMenu.orders[0].OrderNo);
                for (int i = 0; i < menuViewModel.Payments.Count; i++)
                {
                    PaymentDetails paymentDetails= new PaymentDetails();

                    paymentDetails.TransactionAmount = menuViewModel.Payments[i].TransactionAmount;
                    paymentDetails.CashType = menuViewModel.Payments[i].PaymentType;
                    paymentDetails.TransactionDirection = menuViewModel.Payments[i].TransactionDirection;
                    paymentDetails.PaymentDateTime = Views.ItemMenu.OrderDateTime;


                    orders1.tblPaymentDetails.Add(paymentDetails);
                }
                var serialized = JsonConvert.SerializeObject( orders1);
                ReadWrite.PostDataToLDB(serialized);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        
    }
    public class Orders
    {
        public Orders()
        {
            tblOrderDetails = new List<CartDetails>();
            tblPaymentDetails = new List<PaymentDetails>();
        }

        public string OrderNo { get; set; }
        public int TicketNo { get; set; }
        public int purchaseAmount { get; set; }
        public int PaidAmount { get; set; }
        public int RemainingAmount { get; set; }
        public string machineId { get; set; }
        public string OrderDateTime { get; set; }
        public bool isCompleted { get; set; }
        public string completedDateTime { get; set; }
        public string completedBy { get; set; }
        public string paymentType { get; set; }

        public List<CartDetails> tblOrderDetails { get; set; }
        public List<PaymentDetails> tblPaymentDetails { get; set; }


    }
    public class CartDetails
    {
        public string Name { get; set; }
        public string OrderNo { get; set; }
        public string Portion { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; }
        public string selectedDateTime { get; set; }
        public int purchasedItemId { get; set; }
    }
    public class PaymentDetails
    {
        public string MachineId { get; set; }
        public string OrderNo { get; set; }
        public string  CashType{ get; set; }
        public int TransactionAmount { get; set; }
        public string PaymentDateTime { get; set; }
        public string TransactionDirection { get; set; }

    }
}
