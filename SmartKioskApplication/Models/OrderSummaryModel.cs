using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp.Models
{
    class OrderSummaryModel
    {
    }
    public class OrderSummary : INotifyPropertyChanged
    {

        public string orderNo;

        public string OrderNo
        {
            get
            {
                return orderNo;
            }

            set
            {
                if (orderNo != value)
                {
                    orderNo = value;
                    RaisePropertyChanged("OrderNo");
                }
            }
        }
        public int dueAmount;

        public int DueAmount
        {
            get
            {
                return dueAmount;
            }

            set
            {
                if (dueAmount != value)
                {
                    dueAmount = value;
                    RaisePropertyChanged("TotalAmount");
                }
            }
        }
        public int insertedAmount;

        public int InsertedAmount
        {
            get
            {
                return insertedAmount;
            }

            set
            {
                if (insertedAmount != value)
                {
                    insertedAmount = value;
                    RaisePropertyChanged("TotalAmount");
                }
            }
        }
        public int remainingAmount;

        public int RemainingAmount
        {
            get
            {
                return remainingAmount;
            }

            set
            {
                if (remainingAmount != value)
                {
                    remainingAmount = value;
                    RaisePropertyChanged("RemainingAmount");
                }
            }
        }
        public int ticketNumber;

        public int TicketNumber
        {
            get
            {
                return ticketNumber;
            }

            set
            {
                if (ticketNumber != value)
                {
                    ticketNumber = value;
                    RaisePropertyChanged("TicketNumber");
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
