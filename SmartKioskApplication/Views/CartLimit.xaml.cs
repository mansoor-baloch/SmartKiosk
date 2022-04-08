using System;
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
    /// Interaction logic for CartLimit.xaml
    /// </summary>
    public partial class CartLimit : Window
    {
        public CartLimit()
        {
            InitializeComponent();
            StartCloseTimer();
        }

        private void btnOK(object sender, RoutedEventArgs e)
        {
            this.Close();
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
