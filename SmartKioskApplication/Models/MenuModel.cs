using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp.Models
{
    class MenuModel
    {
    }
    public class Menu : INotifyPropertyChanged
    {
        public string itemName;
        public int priceQP;
        public int priceMP;
        public int priceSP;
        public bool hasPortion;
        public byte[] itemImage;

        public string ItemName
        {
            get
            {
                return itemName;
            }

            set
            {
                if (itemName != value)
                {
                    itemName = value;
                    RaisePropertyChanged("ItemName");
                }
            }
        }

        public int PriceQP
        {
            get { return priceQP; }
            set
            {
                if (priceQP != value)
                {
                    priceQP = value;
                    RaisePropertyChanged("PriceQP");
                }
            }
        }

        public int PriceMP
        {
            get { return priceMP; }
            set
            {
                if (priceMP != value)
                {
                    priceMP = value;
                    RaisePropertyChanged("PriceMP");
                }
            }
        }

        public int PriceSP
        {
            get { return priceSP; }
            set
            {
                if (priceSP != value)
                {
                    priceSP = value;
                    RaisePropertyChanged("PriceSP");
                }
            }
        }

        public byte[] ItemImage
        {
            get { return itemImage; }
            set
            {
                if (itemImage != value)
                {
                    itemImage = value;
                    RaisePropertyChanged("ItemImage");
                }
            }
        }
        
        public bool HasPortion
        {
            get { return hasPortion; }
            set
            {
                if (hasPortion != value)
                {
                    hasPortion = value;
                    RaisePropertyChanged("HasPortion");
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
