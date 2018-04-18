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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using StudyFunc;
using DailyFunc;

namespace KMP
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DateTime dt1 = DateTime.Parse("2015/2/16");
            DateTime dt2 = DateTime.Parse("2015/3/23");
            DateTime dt3 = DateTime.Parse("2015/4/2");
            int days = (dt2 - dt1).Days;
            string daysofweek = dt3.DayOfWeek.ToString();
            Trace.WriteLine("1");
        }

        private void Button_Click_KMP(object sender, RoutedEventArgs e)
        {
        }
    }
}
