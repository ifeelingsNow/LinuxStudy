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
            int pos = -1;
            DateTime dt1 = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                bool test = Func.simpleFind(c_oriStr.Text, c_findStr.Text, ref pos);
            }
            DateTime dt2 = DateTime.Now;
            TimeSpan ts = (dt2 - dt1);
            MessageBox.Show(ts.Ticks.ToString());
            Trace.WriteLine("1");
        }

        private void Button_Click_KMP(object sender, RoutedEventArgs e)
        {
            //测试用时
            int pos = -1;
            int[] Mlist = Func.KMPMatchList(c_findStr.Text);
            DateTime dt1 = DateTime.Now;
            for (int i = 0; i < 100000; i++)
            {
                bool test = Func.KMPFind(c_oriStr.Text, c_findStr.Text, Mlist, ref pos);
            }
            DateTime dt2 = DateTime.Now;
            TimeSpan ts = (dt2 - dt1);
            MessageBox.Show(ts.Ticks.ToString());
            Trace.WriteLine("1");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            double a = 157000 / 36;
            double mCoef = 0.05 / 12;
            double sum = 0;
            for (int i = 0; i < 36; i++)
            {
                double curM = a + sum * mCoef;
                sum += curM;
            }
            double leftS = 157000;
            double allS = leftS;
            for (int i = 0; i < 36; i++)
            {
                allS += (allS * mCoef);
            }
            MessageBox.Show(allS.ToString());
            Trace.WriteLine("1");
        }
    }
}
