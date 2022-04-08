﻿using System;
using System.Collections.Generic;
using System.Linq;
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

namespace SmartKioskApp.Views
{
    /// <summary>
    /// Interaction logic for OrderReceipt.xaml
    /// </summary>
    public partial class OrderReceipt : Window
    {
        public OrderReceipt()
        {
            InitializeComponent();

            if (ItemMenu.orders[0].RemainingAmount != 0 && ItemMenu.PaymentCompleted)
            {
                txtInstruct2.Visibility = Visibility.Visible;
                txtInstruct3.Visibility = Visibility.Visible;
            }
            
            StartCloseTimer();
        }
        private void StartCloseTimer()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(10d);
            timer.Tick += TimerTick;
            timer.Start();
        }
        private void TimerTick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            timer.Tick -= TimerTick;
            Close();
            this.Close();
        }

    }
}
