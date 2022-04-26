using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp.Models
{
    class CartModel
    {
    }
    public class Cart : INotifyPropertyChanged
    {

        public string name;
        public string orderNo;
        public string portion;
        public int quantity;
        public string unitPrice;
        public string totalPrice;

        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                if (name != value)
                {
                    name = value;
                    RaisePropertyChanged("Name");
                }
            }
        }

        public string Portion
        {
            get
            {
                return portion;
            }

            set
            {
                if (portion != value)
                {
                    portion = value;
                    RaisePropertyChanged("Portion");
                }
            }
        }

        
        public int Quantity
        {
            get
            {
                return quantity;
            }

            set
            {
                if (quantity != value)
                {
                    quantity = value;
                    RaisePropertyChanged("Quantity");
                }
            }
        }

        
        public string UnitPrice
        {
            get
            {
                return unitPrice;
            }

            set
            {
                if (unitPrice != value)
                {
                    unitPrice = value;
                    RaisePropertyChanged("UnitPrice");
                }
            }
        }
        
        public string TotalPrice
        {
            get
            {
                return totalPrice;
            }

            set
            {
                if (totalPrice != value)
                {
                    totalPrice = value;
                    RaisePropertyChanged("TotalPrice");
                }
            }
        }

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
                    RaisePropertyChanged("Name");
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
