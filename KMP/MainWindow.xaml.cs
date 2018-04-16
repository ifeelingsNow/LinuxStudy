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
            //string oristr = c_oriStr.Text;
            //string findstr = c_findStr.Text;
            //char getc = oristr[3];
            //int oriTag = 0, findTag = 0, lastOriTag = 0, resTag = -1;
            //int findLen = findstr.Length;
            //int oriLen = oristr.Length;
            //bool isFind = false;
            //for (oriTag = 0; oriTag < oriLen; oriTag++)
            //{
            //    lastOriTag = oriTag;
            //    for (findTag = 0; findTag < findLen; findTag++)
            //    {
            //        if (oristr[oriTag] == findstr[findTag])
            //        {
            //            oriTag++;
            //            if (findTag == findLen - 1)
            //            {
            //               resTag = lastOriTag;
            //               isFind = true;
            //            }
            //        }
            //        else
            //        {
            //            oriTag = lastOriTag;
            //            break;
            //        }
            //    }
            //    //表示已经找到了
            //    if (isFind == true)
            //    {
            //        MessageBox.Show(oristr.Substring(resTag, findLen));
            //        break; 
            //    }
            //}
            //测试用时
            int pos = -1;
            DateTime dt1 = DateTime.Now;
            for (int i = 0; i < 1000; i++)
            {
                Func.simpleFind(c_oriStr.Text, c_findStr.Text, ref pos);                
            }
            DateTime dt2 = DateTime.Now;
            TimeSpan ts = (dt2 - dt1);
            MessageBox.Show(ts.Ticks.ToString());
            Trace.WriteLine("1");
        }

        private void Button_Click_KMP(object sender, RoutedEventArgs e)
        {
            string findstr = "ABCDABD";
            //c_findStr.Text;
            //生成KMP构造匹配表
            int[] a = Func.KMPMatchList(findstr);
            Trace.WriteLine("1");
        }
    }
}
