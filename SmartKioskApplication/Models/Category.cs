using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartKioskApp.Models
{
    public class CategoryModel
    {
    }
    public class Category : INotifyPropertyChanged
    {
        public string catName;

        public string CatName
        {
            get
            {
                return catName;
            }

            set
            {
                if (catName != value)
                {
                    catName = value;
                    RaisePropertyChanged("CatName");
                }
            }
        }

        public byte[] catIcon;

        public byte[] CatIcon
        {
            get { return catIcon; }
            set
            {
                if (catIcon != value)
                {
                    catIcon = value;
                    RaisePropertyChanged("CatIcon");
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
