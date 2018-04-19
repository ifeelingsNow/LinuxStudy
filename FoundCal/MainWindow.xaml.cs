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
            //计算支付宝复利，每个月存入2000，5年以后的总收入
            double dayRate = Convert.ToDouble(c_rate.Text) / 100 / 365;
            double sFound = Convert.ToDouble(c_everyMonth.Text);
            int monthNum = Convert.ToInt16(c_month.Text);
            DateTime dts = DateTime.Parse("2018/5/1");
            DateTime dse = dts.AddYears(1);
            //DateTime dse = dts.AddYears(5);
            double sum = 0;
            for (; dts < dse; dts = dts.AddDays(1))
            {
                sum += (sum * dayRate);
                if (dts.Day == 10)
                {
                    sum += sFound;
                }
            }
            Trace.WriteLine("1");
        }

        private void Button_Click_KMP(object sender, RoutedEventArgs e)
        {
            DateTime dts = DateTime.Parse("2018/5/1");
            DateTime dse = dts.AddYears(1);
            //DateTime dse = dts.AddYears(5);
            double dayRate = Convert.ToDouble(c_rate.Text) / 100 / 365;
            double sum = 47991;
            for (; dts < dse; dts = dts.AddDays(1))
            {
                sum += (sum * dayRate);
            }
            double yearRate = (50000.0 - 47991.0) / 47991.0 * 100;
            Trace.WriteLine("1");
        }
    }
}
