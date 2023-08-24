using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp
{
    public static class CashHandler
    {
        private static bool PayoutProcessed = false;
        private static bool NoteDespensed = false;
        private static bool CoinProcessed = false;
        private static bool CoinDispensed = false;

        //Just to save the 
        public static void SubmitCashTransaction(string transactionDirection, string cashType, int returnableAmount, int transactionAmount)
        {
            try
            {
                //List<DbParameter> prams = new List<DbParameter>();
                //prams.Add(new DbParameter("CompanyId", ParameterDirection.Input, Convert.ToInt32(ConfigurationManager.AppSettings["COMPANY_ID"])));
                //prams.Add(new DbParameter("MachineId", ParameterDirection.Input, Convert.ToInt32(ConfigurationManager.AppSettings["MACHINE_ID"])));
                //prams.Add(new DbParameter("TransactionType", ParameterDirection.Input, "Cash"));
                //prams.Add(new DbParameter("TransactionDirection", ParameterDirection.Input, transactionDirection));
                //prams.Add(new DbParameter("SaleNo", ParameterDirection.Input, Global.General.SALE_NO));
                //prams.Add(new DbParameter("CashType", ParameterDirection.Input, cashType));
                //prams.Add(new DbParameter("TransactionAmount", ParameterDirection.Input, transactionAmount));
                //new DbManager().ExecuteNonQuery("spCashTransactionUpdate", prams);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while saving cash insertion Info");
            }
        }

        /// <summary>
        /// Used to get currently in amount by any mean. Example: "50|PayCard" , "30|Cash" use Common.Key and Common.Value Method to get Amount and type separatly
        /// </summary>
        /// <returns></returns>
        public static string GetCashIn()
        {
            int nvCash = Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
            if (nvCash > 0)
            {
                Global.General.CashIn = nvCash.ToString() + "|" + "Cash";
            }
            //if (Global.PayCard.TotalAmount > 0)
            //{
            //    Global.General.CashIn = Global.PayCard.TotalAmount + "|" + "PayCard";
            //}
            //if (nvCash > 0 && Global.PayCard.TotalAmount > 0)
            //{
            //    Global.General.CashIn = Global.PayCard.TotalAmount + "|" + "PayCard";
            //}
            return Global.General.CashIn;
        }

        public static int GetCashInAmount()
        {
            return Convert.ToInt32(ReadWrite.Read(Global.Actions.AddToAmount.ToString()));
        }
        //public static int GetPayCardInAmount()
        //{
        //    return Convert.ToInt32(Global.PayCard.TotalAmount);
        //}

        /// <summary>
        /// Used to get returnable amount with type. Example: "50|PayCard" , "30|Cash" use Common.Key and Common.Value Method to get Amount and type separatly
        /// </summary>
        /// <returns></returns>
        public static string ReturnableCashWithType()
        {
            string ci = GetCashIn();
            return ci;
        }



        /// <summary>
        /// This method will pay only one note of denomination set intially, Default is 50.
        /// </summary>
        private static void PayNote()
        {
            while (!PayoutProcessed)
            {
                // do nothing wait for the payout command to execute
            }
            //if (PayoutProcessed && NoteDespensed)
            //{
            //    CASH_RETURNED_AMOUNT += STACKED_NOTE_VALUE;
            //    IS_NOTE_RETURNED = true;
            //    //new TransactionHandler().SubmitCashTransaction(CASH_OUT, "Note", 0, STACKED_NOTE_VALUE);
            //}
            //else if (!NoteDespensed)
            //{
            //    // no coins was dispensed
            //    IS_NOTE_RETURNED = false;
            //}
        }
    }

    }
