using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp.Models
{
    class PaymentModel
    {
    }
    public class Payment : INotifyPropertyChanged
    {

        public int transactionAmount;
        public string paymentType;
        public string transactionDirection;
        public string paymentDateTime;
        
        public int TransactionAmount
        {
            get
            {
                return transactionAmount;
            }

            set
            {
                if (transactionAmount != value)
                {
                    transactionAmount = value;
                    RaisePropertyChanged("TransactionAmount");
                }
            }
        }

        public string PaymentType
        {
            get
            {
                return paymentType;
            }

            set
            {
                if (paymentType != value)
                {
                    paymentType = value;
                    RaisePropertyChanged("PaymentType");
                }
            }
        }


        public string TransactionDirection
        {
            get
            {
                return transactionDirection;
            }

            set
            {
                if (transactionDirection != value)
                {
                    transactionDirection = value;
                    RaisePropertyChanged("TransactionDirection");
                }
            }
        }


        public string PaymentDateTime
        {
            get
            {
                return paymentDateTime;
            }

            set
            {
                if (paymentDateTime != value)
                {
                    paymentDateTime = value;
                    RaisePropertyChanged("PaymentDateTime");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
