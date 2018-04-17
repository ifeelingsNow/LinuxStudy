using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.IO.Ports;
using System.Windows;
using System.Diagnostics;
using System.Runtime.Serialization;  
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Net;
using System.Data;
using MySql.Data;
using MySql.Data.MySqlClient;

//using NPOI;
//using NPOI.HPSF;
//using NPOI.HSSF;
//using NPOI.SS.Formula.Eval;
//using NPOI.HSSF.UserModel;
//using NPOI.HSSF.Util;
//using NPOI.POIFS;
//using NPOI.SS.UserModel;
//using NPOI.Util;
//using NPOI.SS;
//using NPOI.DDF;
//using NPOI.SS.Util;

namespace CommonFunction
{
    public class ComFunc
    {
        public static Vertex Rotate2DPt(Vertex srcPt, double theta)
        {
            Vertex resPt = new Vertex();
            resPt.x = srcPt.x * Math.Cos(theta / 180 * Math.PI) - srcPt.y * Math.Sin(theta / 180 * Math.PI);
            resPt.y = srcPt.x * Math.Sin(theta / 180 * Math.PI) + srcPt.y * Math.Cos(theta / 180 * Math.PI);
            resPt.z = srcPt.z;
            return resPt;
        }
        public static string FindGapString(string oriStr)
        {
            if (oriStr == "")
            {
                return "";
            }
            int pos = oriStr.IndexOf('\t');
            if (pos != -1)
            {
                return "	";
            }
            pos = oriStr.IndexOf(",");
            if (pos != -1)
            {
                return ",";
            }
            pos = oriStr.IndexOf(" ");
            if (pos != -1)
            {
                return " ";
            }
            return "";
        }
        public static bool ResolveFileNumToList(string filePath, ref List<Vertex> dstList)
        {
            if (File.Exists(filePath) == false)
    	    {
                return false;
	        }
            dstList = new List<Vertex>();
            StreamReader sr = new StreamReader(filePath, Encoding.Default);
            string oneline = sr.ReadLine();
            string gapStr = FindGapString(oneline);
            int gap1 = oneline.IndexOf(gapStr);
            int gap2 = oneline.IndexOf(gapStr, gap1 + 1);
            double x = 0, y = 0, z = 0;
            x = Convert.ToDouble(oneline.Substring(0, gap1));
            if (gap2 == -1)//只有2D的数据
            {
                y = Convert.ToDouble(oneline.Substring(gap1 + 1, oneline.Length - gap1 - 1));
                z = 0;
            }
            else
            {
                y = Convert.ToDouble(oneline.Substring(gap1 + 1, gap2 - gap1 - 1));
                z = Convert.ToDouble(oneline.Substring(gap2 + 1, oneline.Length - gap2 - 1));
            }
            Vertex tp1 = new Vertex(x, y, z);
            dstList.Add(tp1);
            //继续添加点
            while ((oneline = sr.ReadLine()) != null)
            {
                gap1 = oneline.IndexOf(gapStr);
                gap2 = oneline.IndexOf(gapStr, gap1 + 1);
                x = Convert.ToDouble(oneline.Substring(0, gap1));
                if (gap2 == -1)//只有2D的数据
                {
                    y = Convert.ToDouble(oneline.Substring(gap1 + 1, oneline.Length - gap1 - 1));
                    z = 0;
                }
                else
                {
                    y = Convert.ToDouble(oneline.Substring(gap1 + 1, gap2 - gap1 - 1));
                    z = Convert.ToDouble(oneline.Substring(gap2 + 1, oneline.Length - gap2 - 1));
                }
                Vertex tp = new Vertex(x, y, z);
                dstList.Add(tp);                
            }
            sr.Close();
            return true;
        }
        public static List<Vertex> MirrorByYAxiaVertex2D(List<Vertex> oriList)
        {
            if (oriList.Count == 0)
            {
                return null;
            }
            List<Vertex> resList = new List<Vertex>();
            if (oriList.Count == 1)
            {
                Vertex tempPt = new Vertex(-oriList[0].x, 0, oriList[0].z);
                resList.Add(tempPt);
                return resList;
            }
            if (oriList[0].x == oriList[1].x)
            {
                return null;
            }
            int oriLen = oriList.Count;
            //先判断X的取值方向
            if (oriList[0].x < oriList[1].x)
            {
                //X正向取值
                //判断X的取值范围
                if (oriList[1].x > 0)
                {
                    //X取值为Y轴右侧,则先镜像再插入
                    for (int i = oriLen - 1; i > 0; i--)
                    {
                        Vertex tempPt = new Vertex(-oriList[i].x, 0, oriList[i].z);
                        resList.Add(tempPt);
                    }
                    for (int i = 0; i < oriLen; i++)
                    {
                        resList.Add(oriList[i]);                        
                    }
                }
                else if (oriList[0].x < 0)
                {
                    //x取值为Y轴左侧，先插入再镜像
                    for (int i = 0; i < oriLen; i++)
                    {
                        resList.Add(oriList[i]);                                                
                    }
                    //镜像
                    for (int i = oriLen - 2; i >= 0; i--)
                    {
                        Vertex tempPt = new Vertex(-oriList[i].x, 0, oriList[i].z);
                        resList.Add(tempPt);
                    }
                }
            }
            else
            {
                //X反向取值
                //全部在Y右边,则先复制再镜像
                if (oriList[0].x > 0)
                {
                    for (int i = 0; i < oriLen; i++)
                    {
                        resList.Add(oriList[i]);
                    }
                    //镜像
                    for (int i = oriLen - 2; i >= 0; i--)
                    {
                        Vertex tempPt = new Vertex(-oriList[i].x, 0, oriList[i].z);
                        resList.Add(tempPt);
                    }
                }
                //全部在Y左边，则先镜像再复制
                if (oriList[1].x < 0)
                {
                    //镜像
                    for (int i = oriLen - 1; i > 0; i--)
                    {
                        Vertex tempPt = new Vertex(-oriList[i].x, 0, oriList[i].z);
                        resList.Add(tempPt);                        
                    }
                    //复制
                    for (int i = 0; i < oriLen; i++)
                    {
                        resList.Add(oriList[i]);
                    }
                }
            }
            return resList;
        }
        public static bool MakeEndOneEnter(string dataTxt)
        {
            if (dataTxt == "")
            {
                return false;
            }
            if (!dataTxt.EndsWith("\r\n"))
            {
                dataTxt += "\r\n";
            }
            else
            {
                while (dataTxt.EndsWith("\r\n"))
                {
                    dataTxt = dataTxt.Remove(dataTxt.Length - 2, 2);
                }
                dataTxt += "\r\n";
            }
            return true;
        }
        public static string getOneLineGap(string oneLineStr)
        {
            int dotPos = oneLineStr.IndexOf(',');
            int gapPos = oneLineStr.IndexOf(' ');
            int tabPos = oneLineStr.IndexOf('\t');
            if (dotPos != -1)
            {
                return ",";
            }
            else
            {
                if (tabPos != -1)
                {
                    return "\t";
                }
                return " ";
            }
        }
        public static List<int> BuildRandomSequence(int low, int high)
        {
            int x = 0, tmp = 0;
            if (low > high)
            {
                tmp = low;
                low = high;
                high = tmp;
            }
            List<int> resList = new List<int>();
            Random ro = new Random();
            for (int i = low; i <= high; i++)
            {
                resList.Add(i);
            }
            for (int i = resList.Count - 1; i > 0; i--)
            {
                x = ro.Next() % (i + 1);
                tmp = resList[i];
                resList[i] = resList[x];
                resList[x] = tmp;
            }
            return resList;
        }
        public static T Clone<T>(T RealObject)   
        {   
            using (Stream objectStream = new MemoryStream())   
            {   
                IFormatter formatter = new BinaryFormatter();   
                formatter.Serialize(objectStream, RealObject);   
                objectStream.Seek(0, SeekOrigin.Begin);   
                return (T)formatter.Deserialize(objectStream);   
            }   
        }
        public static bool ChangeAssDataToFile(string filePath, int eachColLen)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }
            //FileStream fs = new FileStream(filePath, FileMode.Open);

            StreamReader smR = new StreamReader(filePath);
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Txt Files|*.txt";
            saveFileDialog1.Title = "Select a Save File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                string newFilePath = saveFileDialog1.FileName;
                //System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                FileStream fs = new FileStream(newFilePath, FileMode.Create);
                StreamWriter swW = new StreamWriter(fs, Encoding.Default);
                char[] eachLine = new char[eachColLen];
                int i = 0;
                string oneLine = "";
                while (smR.Read(eachLine, 0, eachColLen) != 0)
                {
                    if (i == 9)
                    {
                        oneLine += new string(eachLine);
                        oneLine += "\r\n";
                        swW.Write(oneLine);
                        oneLine = "";
                        i = 0;
                    }
                    else
                    {
                        oneLine += new string(eachLine);
                        i++;
                    }
                }
                swW.Close();
                fs.Close();
                MessageBox.Show("成功保存至" + newFilePath, "提示");
            }
            smR.Close();
            return true;
        }
        public static bool InputFile(ref string filePath, ref string fileExt, string fileFilter = "Txt Files|*.txt|nc Files|*.nc|All Files|*.*", string openStr = "打开文件")
        {
            if (filePath == "")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = fileFilter;
                //openFileDialog1.Filter = "Txt Files|*.txt|nc Files|*.nc|All Files|*.*";
                openFileDialog1.Title = openStr;
                if (openFileDialog1.ShowDialog() == true)
                {
                    filePath = openFileDialog1.FileName;
                }
                else
                {
                    return false;
                }
            }
            else if (File.Exists(filePath) == false)
            {
                MessageBox.Show("文件名不合法");
                return false;
            }
            //得到文件后缀名
            int dotPos = filePath.LastIndexOf('.');
            fileExt = filePath.Substring(dotPos + 1);
            return true;
        }
        public static void savestringToFile(string strData, string filePath = "")
        {
            if (strData.Length == 0)
            {
                return;
            }
            if (filePath == "")
            {
                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "Txt Files|*.txt";
                saveFileDialog1.Title = "Select a Save File";
                saveFileDialog1.ShowDialog();
                if (saveFileDialog1.FileName != "")
                {
                    string newFilePath = saveFileDialog1.FileName;
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    sw.Write(strData);
                    string showTxt = "已将文件保存至" + newFilePath;
                    MessageBox.Show(showTxt);
                    sw.Close();
                    fs.Close();
                }                
            }
            else if (File.Exists(filePath) == false)
            {
                MessageBox.Show("文件名不合法");
                return;
            }
            else if (File.Exists(filePath) == true)
            {
                System.IO.FileStream fs = new FileStream(filePath, FileMode.Append);
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                sw.Write(strData);
                sw.Close();
                fs.Close();
            }
        }
        public static void Save1DPtToTxtByCroup(List<double> srcList, int eachLineCount)
        {
            if (srcList.Count == 0 || eachLineCount <=0)
            {
                return;
            }
            int ptNum = srcList.Count;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "cmp Files|*.cmp";
            saveFileDialog1.Title = "保存CMP文件";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                string filePath = saveFileDialog1.FileName;
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                string temp;
                int enterTag = eachLineCount - 1;
                for (int i = 0; i < ptNum; i++)
                {
                    if (i % eachLineCount == enterTag)
                    {
                        temp = string.Format("{0:f8}\r\n", srcList[i]);
                    }
                    else
                    {
                        temp = string.Format("{0:f8} ", srcList[i]);
                    }
                    sw.Write(temp);
                }
                string showTxt = "已将文件保存至" + filePath;
                MessageBox.Show(showTxt);
                sw.Close();
                fs.Close();
            }
        }
        public static void Save1DPtToTxt(List<int> srcList)
        {
            if (srcList.Count == 0)
            {
                return;
            }
            int ptNum = srcList.Count;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Txt Files|*.txt";
            saveFileDialog1.Title = "Select a Cursor File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                string filePath = saveFileDialog1.FileName;
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                for (int i = 0; i < ptNum; i++)
                {
                    string temp = string.Format("{0}\r\n", srcList[i]);
                    sw.Write(temp);
                }
                string showTxt = "已将文件保存至" + filePath;
                MessageBox.Show(showTxt);
                sw.Close();
                fs.Close();
            }        		
        }
        public static void Save1DValueToTxt<T>(List<T> srcList)
        {
            if (srcList.Count == 0)
            {
                return;
            }
            int ptNum = srcList.Count;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Txt Files|*.txt";
            saveFileDialog1.Title = "Select a Cursor File";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                string filePath = saveFileDialog1.FileName;
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                for (int i = 0; i < ptNum; i++)
                {
                    string temp = string.Format("{0}\r\n", srcList[i]);
                    sw.Write(temp);
                }
                string showTxt = "已将文件保存至" + filePath;
                MessageBox.Show(showTxt);
                sw.Close();
                fs.Close();
            }
        }
        public static void Save3DPtToTxt(List<Vertex> srcList)
        {
            if (srcList.Count == 0)
            {
                return;
            }
            int ptNum = srcList.Count;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "Txt Files|*.txt";
            saveFileDialog1.Title = "保存文件";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                //如果列表长度大于3000，则分几次保存，否则一次保存
                string filePath = saveFileDialog1.FileName;
                if (ptNum < 200000)
                {
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    //FileStream fs = new FileStream(filePath, FileMode.CreateNew);
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    for (int i = 0; i < ptNum; i++)
                    {
                        string temp = string.Format("{0:f8} {1:f8} {2:f8}\r\n", srcList[i].x, srcList[i].y, srcList[i].z);
                        sw.Write(temp);
                    }
                    string showTxt = "已将文件保存至" + filePath;
                    MessageBox.Show(showTxt);
                    sw.Close();
                    fs.Close();
                }
                else
                {
                    int ptArrLen = (int)(ptNum / 200000), ptTag = 0;
                    if (ptArrLen * 200000 != ptNum)
                    {
                        ptArrLen++;
                    }
                    int[] ptArr = new int[ptArrLen];
                    for (int i = 0; i < ptArrLen - 1; i++)
                    {
                        ptArr[i] = 200000;
                        ptTag += 200000;
                    }
                    ptArr[ptArrLen - 1] = ptNum - ptTag;
                    ptTag = 0;
                    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                    for (int j = 0; j < ptArr[0]; j++)
                    {
                        string temp = string.Format("{0:f8},{1:f8},{2:f8}\r\n", srcList[ptTag].x, srcList[ptTag].y, srcList[ptTag].z);
                        sw.Write(temp);
                        ptTag++;
                    }
                    sw.Close();
                    fs.Close();
                    for (int i = 1; i < ptArrLen; i++)
                    {
                        //System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                        fs = new FileStream(filePath, FileMode.Append);
                        sw = new StreamWriter(fs, Encoding.Default);
                        for (int j = 0; j < ptArr[i]; j++)
                        {
                            string temp = string.Format("{0:f8},{1:f8},{2:f8}\r\n", srcList[ptTag].x, srcList[ptTag].y, srcList[ptTag].z);
                            sw.Write(temp);
                            ptTag++;
                        }
                        sw.Close();
                        fs.Close();
                    }
                    string showTxt = "已将文件保存至" + filePath;
                    MessageBox.Show(showTxt);
                }
                
            }
        }
        public static bool GetCsvPt(ref string filePath, List<Vertex> resList, List<int> BadIdx, CsvInfo fileInfo)
        {
            if (filePath == "")
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Txt Files|*.txt|all files(*.*)|*.*";
                openFileDialog1.Title = "Select a Cursor File";
                if (openFileDialog1.ShowDialog() == true)
                {
                    filePath = openFileDialog1.FileName;
                }
                else
                {
                    return false;
                }
                //清空resList列表
                resList.Clear();
                BadIdx.Clear();
            }
            else if (File.Exists(filePath) == false)
            {
                return false;
            }
            //首先找到x-pixels =1025, y-pixels =1025
            System.IO.StreamReader sr = new System.IO.StreamReader(filePath);
            int rowTag = 0, xPix = 0, ListTag = 0, zTag = 0;
            double xRsl = 0, yRsl = 0, zRsl = 0;
            string curStr = sr.ReadLine();
            while (curStr != "Start of Data:" && sr.Peek() != -1)
            {
                //if (String.Compare(curStr, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) == 0)
                if (curStr.IndexOf("x-pixels") >= 0)
                {
                    //找到X有多少个点
                    int firEqual = curStr.IndexOf("=");
                    int secEqual = curStr.IndexOf("=", firEqual + 1);
                    int firstDot = curStr.IndexOf(",");
                    xPix = Convert.ToInt32(curStr.Substring(firEqual + 1, firstDot - firEqual - 1));
                    //yPix = Convert.ToInt32(curStr.Substring(secEqual + 1, curStr.Length - secEqual - 1));
                }
                //x-resolution =2.8541(um), y-resolution =2.8541(um), z-resolution =0.001(um)
                if (curStr.IndexOf("x-resolution") >= 0)
                {
                    //找到X有多少个点
                    int firEqual = curStr.IndexOf("=");
                    int secEqual = curStr.IndexOf("=", firEqual + 1);
                    int thirdEqual = curStr.IndexOf("=", secEqual + 1);
                    int firLB = curStr.IndexOf("(");
                    int firRB = curStr.IndexOf(")");
                    int secLB = curStr.IndexOf("(", firLB + 1);
                    int thirdLB = curStr.IndexOf("(", secLB + 1);
                    //int firRB = curStr.IndexOf(")");
                    string unitXYZ = curStr.Substring(firLB + 1, firRB - firLB - 1);
                    if (unitXYZ == "um")
                    {
                        xRsl = Convert.ToDouble(curStr.Substring(firEqual + 1, firLB - firEqual - 1)) * 0.001;
                        yRsl = Convert.ToDouble(curStr.Substring(secEqual + 1, secLB - secEqual - 1)) * 0.001;
                        zRsl = Convert.ToDouble(curStr.Substring(thirdEqual + 1, thirdLB - thirdEqual - 1)) * 0.001;
                    }
                    else if (unitXYZ == "mm")
                    {
                        xRsl = Convert.ToDouble(curStr.Substring(firEqual + 1, firLB - firEqual - 1));
                        yRsl = Convert.ToDouble(curStr.Substring(secEqual + 1, secLB - secEqual - 1));
                        zRsl = Convert.ToDouble(curStr.Substring(thirdEqual + 1, thirdLB - thirdEqual - 1));
                    }
                }
                curStr = sr.ReadLine();
                Trace.WriteLine("1");
            }
            while (sr.Peek() != -1)
            {
                curStr = sr.ReadLine();
                //循环读取
                int dotPos = -1, lastDotPos = -1, yTag = 0;
                dotPos = curStr.IndexOf(",");
                while (dotPos != -1)
                {
                    Vertex tempPt = new Vertex();
                    if (curStr.Substring(lastDotPos + 1, dotPos - lastDotPos - 1) == "BAD")
                    {
                        tempPt.x = rowTag * xRsl;
                        tempPt.y = yTag * yRsl;
                        tempPt.z = 0;
                        resList.Add(tempPt);
                        BadIdx.Add(ListTag);
                        ListTag++;
                    }
                    else
                    {
                        tempPt.z = Convert.ToDouble(curStr.Substring(lastDotPos + 1, dotPos - lastDotPos - 1));
                        zTag++;
                        if (zTag == 1)
                        {
                            fileInfo.zMin = tempPt.z;
                            fileInfo.zMax = tempPt.z;
                        }
                        else
                        {
                            if (fileInfo.zMin > tempPt.z)
                            {
                                fileInfo.zMin = tempPt.z;                                
                            }
                            if (fileInfo.zMax < tempPt.z)
                            {
                                fileInfo.zMax = tempPt.z;
                            }
                        }
                        tempPt.x = rowTag * xRsl;
                        tempPt.y = yTag * yRsl;
                        resList.Add(tempPt);
                        ListTag++;
                    }
                    if (yTag == (xPix - 1)/2 && rowTag == (xPix - 1)/2)
                    {
                        fileInfo.midPt.x = rowTag * xRsl;
                        fileInfo.midPt.y = yTag * yRsl;
                        if (curStr.Substring(lastDotPos + 1, dotPos - lastDotPos - 1) == "BAD")
                        {
                            tempPt.z = 0;
                        }
                        else
                        {
                            fileInfo.midPt.z = tempPt.z;
                        }
                    }
                    lastDotPos = dotPos;
                    dotPos = curStr.IndexOf(",", lastDotPos + 1);
                    yTag++;
                }
                //最后一个数要插入
                Vertex lastPtTemp = new Vertex();
                if (curStr.Substring(lastDotPos + 1, curStr.Length - lastDotPos - 1) == "BAD")
                {
                    lastPtTemp.z = 0;
                    lastPtTemp.x = rowTag * xRsl;
                    lastPtTemp.y = yTag * yRsl;
                    resList.Add(lastPtTemp);
                    BadIdx.Add(ListTag);
                    ListTag++;
                }
                else
                {
                    lastPtTemp.z = Convert.ToDouble(curStr.Substring(lastDotPos + 1, curStr.Length - lastDotPos - 1));
                    lastPtTemp.x = rowTag * xRsl;
                    lastPtTemp.y = yTag * yRsl;
                    zTag++;
                    if (zTag == 1)
                    {
                        fileInfo.zMin = lastPtTemp.z;
                        fileInfo.zMax = lastPtTemp.z;
                    }
                    else
                    {
                        if (fileInfo.zMin > lastPtTemp.z)
                        {
                            fileInfo.zMin = lastPtTemp.z;
                        }
                        if (fileInfo.zMax < lastPtTemp.z)
                        {
                            fileInfo.zMax = lastPtTemp.z;
                        }
                    }
                    resList.Add(lastPtTemp);
                    ListTag++;
                }
                rowTag++;
            }
						sr.Close();
            fileInfo.ptNum = xPix * xPix;
            fileInfo.xResolution = xRsl;
            fileInfo.yResolution = yRsl;
            fileInfo.zResolution = zRsl;
            return true;
        }

        public static bool offsetCsvPt(CsvInfo fileInfo, List<Vertex> srcList)
        {
            if (fileInfo.ptNum == 0 || srcList.Count == 0)
            {
                return false;
            }
            //找到X,Y平移值
            foreach(Vertex VPt in srcList)
            {
                VPt.x -= fileInfo.midPt.x;
                VPt.y -= fileInfo.midPt.y;
            }
            return true;
        }
        public static bool arrCsvPt(List<Vertex> srcList, ref int ArrNum, double ArrCycle, List<Vertex> OutputList)
        {
            if (srcList.Count == 0)
            {
                return false;
            }
            if (ArrNum <= 0)
            {
                return false;
            }
            if (ArrCycle <= 0)
            {
                return false;
            }
            if (ArrNum % 2 == 1)
            {
                ArrNum++;
            }
            int beginArr = -ArrNum/ 2;
            int endArr = -beginArr;
            for (int i = beginArr; i < endArr; i++)
            {
                foreach(Vertex VT in srcList)
                {
                    //VT.x += i * ArrCycle;
                    Vertex tempPt = new Vertex(VT.x + i*ArrCycle + ArrCycle/2, VT.y, VT.z);
                    OutputList.Add(tempPt);
                }
                Trace.WriteLine("1");
            }
            return true;
        }

        public static void AddPar(double x, double y, ref double res)
        {
            res = x + y;
        }
        public static string QueueToPathData(Queue<double> srcQ, double dltX)
        {
            string strRes = "M ", tempStr;
            int qLen = srcQ.Count;
            for (int i = 0; i < qLen - 1; i++)
            {
                tempStr = string.Format("{0},{1} L ", (i * dltX).ToString(), srcQ.ElementAt(i).ToString());
                strRes += tempStr;
            }
            tempStr = string.Format("{0},{1}", ((qLen - 1) * dltX).ToString(), srcQ.ElementAt(qLen - 1).ToString());
            strRes += tempStr;
            return strRes;
        }
        public static string QueueFloatToPathData(Queue<float> srcQ, double dltX)
        {
            string strRes = "M ", tempStr;
            int qLen = srcQ.Count;
            for (int i = 0; i < qLen - 1; i++)
            {
                tempStr = string.Format("{0},{1} L ", (i * dltX).ToString(), srcQ.ElementAt(i).ToString());
                strRes += tempStr;
            }
            tempStr = string.Format("{0},{1}", ((qLen - 1) * dltX).ToString(), srcQ.ElementAt(qLen - 1).ToString());
            strRes += tempStr;
            return strRes;
        }
        public static string QueueInt16ToPathData(Queue<Int16> srcQ, double dltX)
        {
            if (srcQ.Count == 0)
            {
                return "";
            }
            string strRes = "M ", tempStr;
            int qLen = srcQ.Count;
            for (int i = 0; i < qLen - 1; i++)
            {
                tempStr = string.Format("{0},{1} L ", (i * dltX).ToString(), srcQ.ElementAt(i).ToString());
                strRes += tempStr;
            }
            tempStr = string.Format("{0},{1}", ((qLen - 1) * dltX).ToString(), srcQ.ElementAt(qLen - 1).ToString());
            strRes += tempStr;
            return strRes;
        }
        public static float[] GetDataFromPort(byte[] getData)
        {
            int dataLen = getData.Length;
            int ValueCount = dataLen / sizeof(float);
            int flen = sizeof(float);
            if (dataLen % 4 != 0)
            {
                float[] errList = new float[1];
                return errList;
            }
            float[] fList = new float[ValueCount];
            for (int i = 0; i < ValueCount; i++)
            {
                byte[] tempBtye = new byte[4];
                for (int j = 0; j < flen; j++)
                {
                    tempBtye[j] = getData[i * flen + j];
                }
                fList[i] = BitConverter.ToSingle(tempBtye, 0);
            }
            return fList;
        }

        public static void SaveToExcelFile(MemoryStream ms, string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                byte[] data = ms.ToArray();
                fs.Write(data, 0, data.Length);
                fs.Flush();
                data = null;
            }
        }

        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }
        public static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }
        public static bool isTel(string strInput)
        {
            return Regex.IsMatch(strInput, @"\d{3}-\d{8}|\d{4}-\d{7}");
        }
    }
    public class FTPFunc
    {
        public static void UploadFile(FileInfo fInfo, string targetDir, string hostname, string username, string password)
        {
            //string target;
            //if (targetDir.Trim() == "")
            //{
            //    return;
            //}
            //target = Guid.NewGuid().ToString();
            ////使用临时文件名
            //string URI = "FTP://" + hostname + "/" + targetDir + "/" + target;
            //System.Net.FtpWebRequest ftp = GetRequest(URI, username, password)
        }
    }
    public class DiaCut
    {
        public struct Lens_MdlAndMst
        {
            //MICROSTRUCTURE 参数
            public double d1;
            public double l1;
            public double m1;
            public double a1;
            public double a1min;
            public double a1max;
            public double RRuStructMax1;

            //MODULATION 参数
            public double d2;
            public double l2;
            public double m2;
            public double a2;
            public double a2min;
            public double a2max;
            public double RRuStructMax2;
        }
        public static double LinkFunctionWithPar(double phi, double rr, Lens_MdlAndMst LensPar)
        {
            double Y, Z, Z2;
            double xt, y, z;
            double r;

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////// MICROSTRUCTURE PARAMETERS // /////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            double d = LensPar.d1;
            double L = LensPar.l1;
            double m = LensPar.m1;
            double a = LensPar.a1;
            double a1min = LensPar.a1min;
            double a1max = LensPar.a1max;
            double RRuStructMax = LensPar.RRuStructMax1;
            double t = (1.0 / RRuStructMax) * Math.Log(a1min / a1max);
            double a1 = a1max * Math.Exp(((1 - a) * rr + a * rr * Math.Abs(Math.Cos(phi))) * t);   // !!!!! MODIF ATTTENUATION X  !!!!!!!!!!
            double A = 64 * a1 * (2.0 * m + L - 2.0) / (d * d * d);
            double B = 16 * a1 * (3.0 - 2.0 * L - 2.0 * m) / (d * d);
            double C = 4.0 * L * a1 / d;
            double F = (A / 2.0) * Math.Pow((d / 4), 4.0) + (2.0 * B / 3.0) * Math.Pow((d / 4.0), 3.0) + C * Math.Pow((d / 4.0), 2.0);

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////MODULATION PARAMETERS //////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            double d2 = LensPar.d2;
            double L2 = LensPar.l2;
            double m2 = LensPar.m2;
            double a2min = LensPar.a2min;
            double a2max = LensPar.a2max;
            double RRuStructMax2 = LensPar.RRuStructMax2;
            double t2 = (1.0 / RRuStructMax2) * Math.Log(a2min / a2max);
            double a2 = a2max * Math.Exp((rr * Math.Abs(Math.Sin(phi))) * t2);
            double A2 = 64 * a2 * (2.0 * m2 + L2 - 2.0) / (d2 * d2 * d2);
            double B2 = 16 * a2 * (3.0 - 2.0 * L2 - 2.0 * m2) / (d2 * d2);
            double C2 = 4.0 * L2 * a2 / d2;
            double F2 = (A2 / 2.0) * Math.Pow((d2 / 4), 4.0) + (2.0 * B2 / 3.0) * Math.Pow((d2 / 4.0), 3.0) + C2 * Math.Pow((d2 / 4.0), 2.0);
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            y = rr * Math.Cos(phi);
            z = rr * Math.Sin(phi);
            y = y - d / 2.0;
            z = z - d / 2.0;
            Y = Math.Abs(y) - Math.Floor(Math.Abs(y) / d) * d - d / 2.0;
            Z = Math.Abs(z) - Math.Floor(Math.Abs(z) / d) * d - d / 2.0;
            r = Math.Sqrt(Y * Y + Z * Z);


            Z2 = Math.Abs(z - Math.Floor(z / d2) * d2 - d2 / 2);

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////     EQUATIONS        ///////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            if (Math.Abs(z + d / 2.0) > 10)
            {
                //MICROSTRUCTURE
                if (r < (d / 4.0))
                {
                    xt = (A / 4.0) * Math.Pow(r, 4.0) + (B / 3.0) * Math.Pow(r, 3.0) + (C / 2.0) * Math.Pow(r, 2.0) - F;
                }
                else
                {
                    if (r < (d / 2.0))
                    {
                        xt = -(A / 4.0) * Math.Pow((d / 2.0 - r), 4.0) - (B / 3.0) * Math.Pow((d / 2.0 - r), 3.0) - (C / 2.0) * Math.Pow((d / 2.0 - r), 2.0);
                    }
                    else
                    {
                        xt = 0;
                    }
                }
            }
            else
            {
                //MODULATION		
                if (Z2 < (d2 / 4.0))
                {
                    xt = (A2 / 4.0) * Math.Pow(Z2, 4.0) + (B2 / 3.0) * Math.Pow(Z2, 3.0) + (C2 / 2.0) * Math.Pow(Z2, 2.0) - F2;
                }
                else
                {
                    xt = -(A2 / 4.0) * Math.Pow((d2 / 2.0 - Z2), 4.0) - (B2 / 3.0) * Math.Pow((d2 / 2.0 - Z2), 3.0) - (C2 / 2.0) * Math.Pow((d2 / 2.0 - Z2), 2.0);
                }
            }
            return (xt * 1E3); /* output in microns */
        }

        public static double LinkFunction(double phi, double rr)
        {
            double Y, Z, Z2;
            double xt, y, z;
            double r;

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////// MICROSTRUCTURE PARAMETERS // /////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            double d = 0.800;
            double L = 0.75;
            double m = 0.50;
            double a = 0.500;
            double a1min = 0.003;
            double a1max = 0.100;
            double RRuStructMax = 35.0;
            double t = (1.0 / RRuStructMax) * Math.Log(a1min / a1max);
            double a1 = a1max * Math.Exp(((1 - a) * rr + a * rr * Math.Abs(Math.Cos(phi))) * t);   // !!!!! MODIF ATTTENUATION X  !!!!!!!!!!
            double A = 64 * a1 * (2.0 * m + L - 2.0) / (d * d * d);
            double B = 16 * a1 * (3.0 - 2.0 * L - 2.0 * m) / (d * d);
            double C = 4.0 * L * a1 / d;
            double F = (A / 2.0) * Math.Pow((d / 4), 4.0) + (2.0 * B / 3.0) * Math.Pow((d / 4.0), 3.0) + C * Math.Pow((d / 4.0), 2.0);

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            ///////////////////////////////MODULATION PARAMETERS //////////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////
            double d2 = 0.800;
            double L2 = 0.1;
            double m2 = 0.98;
            double a2min = 0.005;
            double a2max = 0.03;
            double RRuStructMax2 = 10.0;
            double t2 = (1.0 / RRuStructMax2) * Math.Log(a2min / a2max);
            double a2 = a2max * Math.Exp((rr * Math.Abs(Math.Sin(phi))) * t2);
            double A2 = 64 * a2 * (2.0 * m2 + L2 - 2.0) / (d2 * d2 * d2);
            double B2 = 16 * a2 * (3.0 - 2.0 * L2 - 2.0 * m2) / (d2 * d2);
            double C2 = 4.0 * L2 * a2 / d2;
            double F2 = (A2 / 2.0) * Math.Pow((d2 / 4), 4.0) + (2.0 * B2 / 3.0) * Math.Pow((d2 / 4.0), 3.0) + C2 * Math.Pow((d2 / 4.0), 2.0);
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            y = rr * Math.Cos(phi);
            z = rr * Math.Sin(phi);
            y = y - d / 2.0;
            z = z - d / 2.0;
            Y = Math.Abs(y) - Math.Floor(Math.Abs(y) / d) * d - d / 2.0;
            Z = Math.Abs(z) - Math.Floor(Math.Abs(z) / d) * d - d / 2.0;
            r = Math.Sqrt(Y * Y + Z * Z);


            Z2 = Math.Abs(z - Math.Floor(z / d2) * d2 - d2 / 2);

            ///////////////////////////////////////////////////////////////////////////////////////////////////
            //////////////////////////////////     EQUATIONS        ///////////////////////////////////////////
            ///////////////////////////////////////////////////////////////////////////////////////////////////

            if (Math.Abs(z + d / 2.0) > 10)
            {
                //MICROSTRUCTURE
                if (r < (d / 4.0))
                {
                    xt = (A / 4.0) * Math.Pow(r, 4.0) + (B / 3.0) * Math.Pow(r, 3.0) + (C / 2.0) * Math.Pow(r, 2.0) - F;
                }
                else
                {
                    if (r < (d / 2.0))
                    {
                        xt = -(A / 4.0) * Math.Pow((d / 2.0 - r), 4.0) - (B / 3.0) * Math.Pow((d / 2.0 - r), 3.0) - (C / 2.0) * Math.Pow((d / 2.0 - r), 2.0);
                    }
                    else
                    {
                        xt = 0;
                    }
                }
            }
            else
            {
                //MODULATION		
                if (Z2 < (d2 / 4.0))
                {
                    xt = (A2 / 4.0) * Math.Pow(Z2, 4.0) + (B2 / 3.0) * Math.Pow(Z2, 3.0) + (C2 / 2.0) * Math.Pow(Z2, 2.0) - F2;
                }
                else
                {
                    xt = -(A2 / 4.0) * Math.Pow((d2 / 2.0 - Z2), 4.0) - (B2 / 3.0) * Math.Pow((d2 / 2.0 - Z2), 3.0) - (C2 / 2.0) * Math.Pow((d2 / 2.0 - Z2), 2.0);
                }
            }
            return (xt * 1E3); /* output in microns */
        }
        public static List<Vertex> SetCXZROIValue(List<Vertex> srcList, Rect givenRC, double zValue)
        {
            List<Vertex> newList = new List<Vertex>();
            for (int i = 0; i < srcList.Count; i++)
            {
                double x = srcList[i].y * Math.Cos(srcList[i].x * Math.PI / 180);
                double y = srcList[i].y * Math.Sin(srcList[i].x * Math.PI / 180);
                bool isX1 = (x > givenRC.X || Math.Abs(x - givenRC.X) < 1e-9);
                bool isX2 = (x < (givenRC.X + givenRC.Width) || Math.Abs(x - givenRC.X - givenRC.Width) < 1e-9);
                bool isY1 = (y > givenRC.Y || Math.Abs(y - givenRC.Y) < 1e-9);
                bool isY2 = (y < (givenRC.Y + givenRC.Height) || Math.Abs(y - givenRC.Y - givenRC.Height) < 1e-9);
                if (isX1 && isX2 && isY1 && isY2)
                {
                    newList.Add(srcList[i]);
                }
                else
                {
                    Vertex tp = new Vertex(srcList[i].x, srcList[i].y, zValue);
                    newList.Add(tp);
                }
            }
            return newList;
        }
        public static List<Vertex> GetHexArrPtList(double hexLen, double width, double height)
        {
            List<Vertex> allList = new List<Vertex>();
            if (hexLen <=0 || width <= 0 || height <= 0)
            {
                return allList;
            }
            double xCur = hexLen / 2, yCur = 0;
            while (yCur < height || Math.Abs(yCur - height) < 1e-9)
            {
                while (xCur < width || Math.Abs(xCur - width) < 1e-9)
                {
                    Vertex tp = new Vertex(xCur, yCur, 0);
                    allList.Add(tp);
                    xCur += hexLen;
                    Vertex tp1 = new Vertex(xCur, yCur, 0);
                    allList.Add(tp1);
                    xCur += (hexLen * 2);
                }
                xCur = 0;
                yCur += hexLen /2 * Math.Sqrt(3);
                while (xCur < width || Math.Abs(xCur - width) < 1e-9)
                {
                    Vertex tp = new Vertex(xCur, yCur, 0);
                    allList.Add(tp);
                    xCur += (hexLen * 2);
                    Vertex tp1 = new Vertex(xCur, yCur, 0);
                    allList.Add(tp1);
                    xCur += hexLen;                    
                }
                xCur = hexLen / 2;
                yCur += hexLen / 2 * Math.Sqrt(3);
            }
            return allList;
        }
        public static bool HexIntoPiece(List<Vertex>srcList, int nPiece)
        {
            if (srcList == null)
            {
                return false;
            }
            if (srcList.Count == 0 || nPiece <= 0)
            {
                return false;
            }
            int listLen = srcList.Count;
            for (int i = 0; i < listLen; i++)
            {
                double theta = GetThetaValue(srcList[i].x, srcList[i].y);
                int thetaTag = 0;
                if (theta < 2 * Math.PI - Math.PI / nPiece)
                {
                    thetaTag = (int)((theta + Math.PI / nPiece) / (2 * Math.PI / nPiece));
                }
                srcList[i].z = -2 * Math.PI / nPiece * thetaTag;
            }
            return true;
        }
        public static double GetThetaValue(double x, double y)
        {
            if (x == 0 && y == 0)
            {
                return 0;
            }
            double Theta = Math.Acos(x / Math.Sqrt(x * x + y * y));
            //第三四象限
            if (y < 0)
            {
                return Math.PI * 2 - Theta;
            }
            return Theta;
        }
        public static List<Vertex> GetOneThetaPt(double R, int n, double xPrec, double yPrec)
        {
            List<Vertex> ptList = new List<Vertex>();
            double Theta = Math.PI / n ;//在<= -Theta 到 <Theta范围之内的点
            double ATheta = Theta * 180 / Math.PI;
            double xExt = R, xCur = xPrec;
            double yExt = R * Math.Sin(Theta), yCur = 0;
            while (yCur <= R)
            {
                while (xCur <= R)
                {
                    //计算角度
                    double rCur = Math.Sqrt(xCur * xCur + yCur * yCur);
                    double tCur = Math.Asin(yCur / rCur);
                    //在单个范围之内
                    if (tCur >= - Theta && tCur < Theta)
                    {
                        if (rCur <= R)
                        {
                            Vertex tpPt = new Vertex(xCur, yCur, 0);
                            ptList.Add(tpPt);
                        }
                    }
                    xCur += xPrec;
                }
                xCur = 0;
                yCur += yPrec;
            }
            return ptList;
        }
        public static List<List<Vertex>> GetEllipsePt(double a, double b, int n, double xPrec, double yPrec)
        {
            List<List<Vertex>> allPtList = new List<List<Vertex>>();
            for (int i = 0; i < n; i++)
            {
                List<Vertex> ptList = new List<Vertex>();
                allPtList.Add(ptList);
            }
            //先生成所有点
            List<Vertex> allPt = new List<Vertex>();
            double xCur = -a, yCur = -b;
            while (yCur <= b)
            {
                while (xCur <= a)
                {
                    //Z值在取值范围之内
                    if ((1 - xCur * xCur / a / a - yCur * yCur / b / b) >= 0)
                    {
                        //计算角度
                        double Theta = GetThetaValue(xCur, yCur);
                        int thetaTag = 0;
                        if (Theta < 2 * Math.PI - Math.PI / n)
                        {
                            thetaTag = (int)((Theta + Math.PI / n) / (2 * Math.PI / n));
                        }
                        Vertex tpPt = new Vertex(xCur, yCur, -2 * Math.PI / n * thetaTag);
                        allPtList[thetaTag].Add(tpPt);
                        allPt.Add(tpPt);
                    }
                    xCur += xPrec;
                }
                xCur -= xPrec;
                yCur += yPrec;
                if (yCur > b)
                {
                    break;
                }
                while (xCur > -a)
                {
                    if ((1 - xCur * xCur / a / a - yCur * yCur / b / b) >= 0)
                    {
                        //计算角度
                        double Theta = GetThetaValue(xCur, yCur);
                        int thetaTag = 0;
                        if (Theta < 2 * Math.PI - Math.PI / n)
                        {
                            thetaTag = (int)((Theta + Math.PI / n) / (2 * Math.PI / n));
                        }
                        Vertex tpPt = new Vertex(xCur, yCur, -2 * Math.PI / n * thetaTag);
                        allPtList[thetaTag].Add(tpPt);
                        allPt.Add(tpPt);
                    }
                    xCur -= xPrec;
                }
                //最后一个-R点
                xCur = -a;
                if ((1 - xCur * xCur / a / a - yCur * yCur / b / b) >= 0)
                {
                    //计算角度
                    double Theta = GetThetaValue(xCur, yCur);
                    int thetaTag = 0;
                    if (Theta < 2 * Math.PI - Math.PI / n)
                    {
                        thetaTag = (int)((Theta + Math.PI / n) / (2 * Math.PI / n));
                    }
                    Vertex tpPt = new Vertex(xCur, yCur, -2 * Math.PI / n * thetaTag);
                    allPtList[thetaTag].Add(tpPt);
                    allPt.Add(tpPt);
                }
                yCur += yPrec;
            }
            return allPtList;
        }
        public static List<List<Vertex>> GetThetaPt(double R, int n, double xPrec, double yPrec)
        {
            List<List<Vertex>> allPtList = new List<List<Vertex>>();
            for (int i = 0; i < n; i++)
            {
                List<Vertex> ptList = new List<Vertex>();
                allPtList.Add(ptList);
            }
            //先生成所有点
            List<Vertex> allPt = new List<Vertex>();
            double xCur = -R, yCur = -R;
            while (yCur < R || Math.Abs(yCur - R) < 1e-9)
            {
                while (xCur < R || Math.Abs(xCur - R) < 1e-9)
                {
                    if (Math.Sqrt(xCur* xCur + yCur * yCur) <= R)
                    {
                        //计算角度
                        double Theta = GetThetaValue(xCur, yCur);
                        int thetaTag = 0;
                        if (Theta < 2 * Math.PI - Math.PI / n)
                        {
                            thetaTag = (int)((Theta + Math.PI / n) / (2 * Math.PI / n));
                        }
                        Vertex tpPt = new Vertex(xCur, yCur, -2 * Math.PI / n * thetaTag);
                        allPtList[thetaTag].Add(tpPt);
                        allPt.Add(tpPt);
                    }
                    xCur += xPrec;
                }
                xCur -= xPrec;
                yCur += yPrec;
                if (yCur > R)
                {
                    break;
                }
                while (xCur > -R)
                {
                    if (Math.Sqrt(xCur * xCur + yCur * yCur) <= R)
                    {
                        //计算角度
                        double Theta = GetThetaValue(xCur, yCur);
                        int thetaTag = 0;
                        if (Theta < 2 * Math.PI - Math.PI / n)
                        {
                            thetaTag = (int)((Theta + Math.PI / n) / (2 * Math.PI / n));
                        }
                        Vertex tpPt = new Vertex(xCur, yCur, -2 * Math.PI / n * thetaTag);
                        allPtList[thetaTag].Add(tpPt);
                        allPt.Add(tpPt);
                    }
                    xCur -= xPrec;
                }
                //最后一个-R点
                xCur = -R;
                if (Math.Sqrt(xCur * xCur + yCur * yCur) <= R)
                {
                    //计算角度
                    double Theta = GetThetaValue(xCur, yCur);
                    int thetaTag = 0;
                    if (Theta < 2 * Math.PI - Math.PI / n)
                    {
                        thetaTag = (int)((Theta + Math.PI / n) / (2 * Math.PI / n));
                    }
                    Vertex tpPt = new Vertex(xCur, yCur, -2 * Math.PI / n * thetaTag);
                    allPtList[thetaTag].Add(tpPt);
                    allPt.Add(tpPt);
                }
                yCur += yPrec;
            }
            //ComFunc.Save3DPtToTxt(allPt);
            ////保存文件对话框
            //SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            //saveFileDialog1.Filter = "Txt Files|*.txt";
            //saveFileDialog1.Title = "保存txt文件";
            //saveFileDialog1.ShowDialog();
            //if (saveFileDialog1.FileName != "")
            //{
            //    string filePath = saveFileDialog1.FileName;
            //    System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
            //    StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            //    int ptLen = allPtList[0].Count;
            //    for (int i = 0; i < ptLen; i++)
            //    {
            //        string oneLineStr = allPtList[0][i].x.ToString("F8") + " " + allPtList[0][i].y.ToString("F8") + "\r\n";
            //        sw.Write(oneLineStr);
            //    }
            //    string showTxt = "已将文件保存至" + filePath;
            //    MessageBox.Show(showTxt);
            //    sw.Close();
            //    fs.Close();
            //}
            return allPtList;
        }
        public static List<List<Vertex>> Get9RecPt(double R, double xPrec, double yPrec, double midLen)
        {
            //此功能是基本写死了
            List<List<Vertex>> allPtList = new List<List<Vertex>>();
            for (int i = 0; i < 9; i++)
            {
                List<Vertex> ptList = new List<Vertex>();
                allPtList.Add(ptList);
            }
            //先生成所有点
            List<Vertex> allPt = new List<Vertex>();
            double xCur = -R, yCur = -R;
            double xSt = -R, ySt = -R;
            int yTimes = (int)(R / yPrec * 2) + 1;
            int xTimes = (int)(R / xPrec * 2) + 1;
            for (int i = 0; i < yTimes; i += 2)
            {
                yCur = ySt + yPrec * i;
                for (int j = 0; j < xTimes; j++)
                {
                    xCur = xSt + xPrec * j;
                    if (Math.Sqrt(xCur * xCur + yCur * yCur) < R || Math.Abs(Math.Sqrt(xCur * xCur + yCur * yCur) - R) < 1e-9)
                    {
                        Vertex tp = new Vertex(xCur, yCur);
                        allPt.Add(tp);
                    }
                }
                //如果xcur!=xSt,即不在边界
                if (Math.Abs(xCur + xSt) > 1e-9)
                {
                    xCur = -xSt;
                    if (Math.Sqrt(xCur * xCur + yCur * yCur) < R || Math.Abs(Math.Sqrt(xCur * xCur + yCur * yCur) - R) < 1e-9)
                    {
                        Vertex tp = new Vertex(xCur, yCur);
                        allPt.Add(tp);
                    }
                }

                yCur = ySt + yPrec * (i + 1);
                for (int j = 0; j < xTimes; j++)
                {
                    xCur = -xSt - xPrec * j;
                    if (Math.Sqrt(xCur * xCur + yCur * yCur) < R || Math.Abs(Math.Sqrt(xCur * xCur + yCur * yCur) - R) < 1e-9)
                    {
                        Vertex tp = new Vertex(xCur, yCur);
                        allPt.Add(tp);
                    }
                }
                //如果x不在边界
                if (Math.Abs(xCur - xSt) > 1e-9)
                {
                    xCur = xSt;
                    if (Math.Sqrt(xCur * xCur + yCur * yCur) < R || Math.Abs(Math.Sqrt(xCur * xCur + yCur * yCur) - R) < 1e-9)
                    {
                        Vertex tp = new Vertex(xCur, yCur);
                        allPt.Add(tp);
                    }
                }
            }
            //将所有的点归类
            int ptLen = allPt.Count;
            DateTime dt1 = DateTime.Now;
            for (int i = 0; i < ptLen; i++)
            {
                //判断是否是九宫格最中间的区域 -midlen/2 <= x <= midlen/2 和 -midlen/2 <= y <= midlen/2
                if ((allPt[i].x > -midLen / 2 || Math.Abs(allPt[i].x + midLen / 2) < 1e-9) &&
                    (allPt[i].x < midLen / 2 || Math.Abs(allPt[i].x - midLen / 2) < 1e-9) &&
                    (allPt[i].y > -midLen / 2 || Math.Abs(allPt[i].y + midLen / 2) < 1e-9) &&
                    (allPt[i].y < midLen / 2 || Math.Abs(allPt[i].y - midLen / 2) < 1e-9))
                {
                    allPtList[0].Add(allPt[i]);
                    allPt[i].z = 1;
                    //allPt.Remove(allPt[i]);
                    //i--;
                    //ptLen--;
                }
            }
            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格最右边的区域 x > midlen/2 和 -midlen/2 < y < midlen/2
                    if (allPt[i].x > midLen / 2 && allPt[i].y > -midLen / 2 && allPt[i].y < midLen / 2)
                    {
                        allPtList[1].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格右上角的区域 x > midlen/2 和 y > midlen/2
                    if (allPt[i].x > midLen / 2 && allPt[i].y > midLen / 2)
                    {
                        allPtList[2].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格中间上面的区域 -midlen/2 < x < midlen/2 和 y > midlen/2
                    if (allPt[i].x > -midLen / 2 && allPt[i].x < midLen / 2 && allPt[i].y > midLen / 2)
                    {
                        allPtList[3].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格左上角的区域 x < - midlen/2 和 y > midlen/2
                    if (allPt[i].x < -midLen / 2 && allPt[i].y > midLen / 2)
                    {
                        allPtList[4].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格最左边的区域 x < - midlen/2 和 -midlen/2 < y < midlen/2
                    if (allPt[i].x < -midLen / 2 && allPt[i].y > -midLen / 2 && allPt[i].y < midLen / 2)
                    {
                        allPtList[5].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格左下角的区域 x < - midlen/2 和 y < -midlen/2
                    if (allPt[i].x < -midLen / 2 && allPt[i].y < -midLen / 2)
                    {
                        allPtList[6].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格中间下面的区域 -midlen/2 < x < midlen/2 和 y < -midlen/2
                    if (allPt[i].x > -midLen / 2 && allPt[i].x < midLen / 2 && allPt[i].y < -midLen / 2)
                    {
                        allPtList[7].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }

            for (int i = 0; i < ptLen; i++)
            {
                if (Math.Abs(allPt[i].z - 1) > 1e-9)
                {
                    //判断是否是为九宫格右下角的区域 x > midlen/2 和 y < -midlen/2
                    if (allPt[i].x > midLen / 2 && allPt[i].y < -midLen / 2)
                    {
                        allPtList[8].Add(allPt[i]);
                        allPt[i].z = 1;
                    }
                }
            }
            for (int i = 0; i < allPtList.Count; i++)
            {
                StreamWriter sw = new StreamWriter(@"E:\SVN\Baidu\CSharp\ConvexCurveFly2Step\VisualPole\Debug\list" + i.ToString() + ".txt");
                for (int j = 0; j < allPtList[i].Count; j++)
                {
                    string oneline = allPtList[i][j].x.ToString("F8") + " " + allPtList[i][j].y.ToString("F8") + " " + i.ToString();
                    sw.WriteLine(oneline);
                }
                sw.Close();
            }
            DateTime dt2 = DateTime.Now;
            TimeSpan spT = dt2 - dt1;
            MessageBox.Show(spT.ToString());
            return allPtList;
        }
        public static List<Vertex> CXZToXYZ(List<Vertex> srcList, double zScale = 1.0, double zOffset = 0.0)
        {
            List<Vertex> xyzList = new List<Vertex>();
            if (srcList.Count == 0)
            {
                return xyzList;                
            }
            foreach (Vertex VT in srcList)
            {
                Vertex tempPt = new Vertex();
                tempPt.x = VT.y * Math.Cos(VT.x * Math.PI / 180);
                tempPt.y = VT.y * Math.Sin(VT.x * Math.PI / 180);
                tempPt.z = VT.z * zScale + zOffset;
                xyzList.Add(tempPt);
            }
            return xyzList;
        }
        public static bool NcFilePhrase(string filePath, List<Vertex> zList)
        {
            try
            {
                StreamReader sr = new System.IO.StreamReader(filePath);
                string line = "";
                zList.Clear();
                while ((line = sr.ReadLine()) != null)
                {
                    //找到了点数据
                    if (line.IndexOf('C') == 0)
                    {
                        //int cPos = line.IndexOf('C');
                        int xPos = line.IndexOf('X');
                        int zPos = line.IndexOf('Z');
                        Vertex tempVT = new Vertex();
                        double x, y, z;
                        bool pxRes = double.TryParse(line.Substring(1,xPos - 1), out x);
                        bool pyRes = double.TryParse(line.Substring(xPos + 1, zPos - xPos- 1), out y);
                        bool pzRes = double.TryParse(line.Substring(zPos + 1), out z);
                        if (pzRes == true && pxRes == true && pyRes == true)
                        {
                            tempVT.x = x;
                            tempVT.y = y;
                            tempVT.z = z;
                            zList.Add(tempVT);
                        }
                    }
                }
                sr.Close();
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show(err.Message);
                return false;
            }
        }
        //public static bool NCCXZFilePhrase(string filePath, List<Vertex> zList)
        //{

        //}
        public static bool XYZFilePhrase(string filePath, List<Vertex> zList)
        {
            try
            {
                StreamReader sr = new System.IO.StreamReader(filePath);
                string line = "";
                zList.Clear();
                line = sr.ReadLine();
                //确定分隔符号
                string gapStr = ComFunc.FindGapString(line);
                int pos1 = line.IndexOf(gapStr);
                int pos2 = line.IndexOf(gapStr, pos1 + 1);
                double x, y, z;
                if (pos2 != -1)//3D数据
                {
                    bool pxRes = double.TryParse(line.Substring(0, pos1), out x);
                    bool pyRes = double.TryParse(line.Substring(pos1 + 1, pos2 - pos1 - 1), out y);
                    bool pzRes = double.TryParse(line.Substring(pos2 + 1), out z);
                    if (pxRes == true && pyRes == true && pzRes == true)
                    {
                        Vertex tempVT = new Vertex(x, y, z);
                        zList.Add(tempVT);
                    }
                }
                while ((line = sr.ReadLine()) != null)
                {
                    pos1 = line.IndexOf(gapStr);
                    pos2 = line.IndexOf(gapStr, pos1 + 1);
                    bool pxRes = double.TryParse(line.Substring(0, pos1), out x);
                    bool pyRes = double.TryParse(line.Substring(pos1 + 1, pos2 - pos1 - 1), out y);
                    bool pzRes = double.TryParse(line.Substring(pos2 + 1), out z);
                    if (pxRes == true && pyRes == true && pzRes == true)
                    {
                        Vertex tempVT = new Vertex(x, y, z);
                        zList.Add(tempVT);
                    }
                }
                sr.Close();
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show(err.Message);
                return false;
            }
        }
        public static List<Vertex> GetSlimList(List<Vertex> oriList, int slimF)
        {
            List<Vertex> allList = new List<Vertex>();
            if (oriList.Count == 0 || slimF <= 0)
            {
                return allList;
            }
            int listLen = oriList.Count;
            int x1 = 0, x2 = 0;
            double platZ = oriList[0].z;
            //将2个特征的X1，X2找出
            for (int i = 0; i < listLen; i++)
            {
                if (oriList[i].z != platZ)
                {
                    x1 = i;
                    break;
                }
            }
            for (int i = x1 + 1; i < listLen; i++)
            {
                if (oriList[i].z == platZ)
                {
                    x2 = i;
                    break;
                }
            }
            for (int i = 0; i < x1 - 1; i+= slimF)
            {
                allList.Add(oriList[i]);
            }
            for (int i = x1 - 1; i <= x2; i++)
            {
                allList.Add(oriList[i]);
            }
            for (int i = x2 + 1; i < listLen; i+= slimF)
            {
                allList.Add(oriList[i]);
            }
            return allList;
        }
        public static List<Vertex> GetArrList(int arrN, double arrT, List<Vertex> oriList)
        {
            List<Vertex> allList = new List<Vertex>();
            if (arrN <= 0 || arrT <=0)
            {
                return allList;
            }
            int listLen = oriList.Count;
            bool isXPlus = true;//表示X的取值方向
            if (oriList[0].x > oriList[1].x)
            {
                isXPlus = false;
            }
            if (arrN % 2 == 0)
            {
                //偶数个阵列，则原列表平移半个周期
                for (int i = 0; i < listLen; i++)
                {
                    oriList[i].x += arrT / 2;
                }
                //阵列
                int beginN = -arrN / 2;
                if (isXPlus)
                {
                    for (int i = beginN; i < -beginN; i++)
                    {
                        for (int j = 0; j < listLen; j++)
                        {
                            Vertex tp = new Vertex(oriList[j].x + arrT * i, oriList[j].y, oriList[j].z);
                            allList.Add(tp);
                        }
                    }                    
                }
                else
                {
                    for (int i = -beginN - 1; i >= beginN; i--)
                    {
                        for (int j = 0; j < listLen; j++)
                        {
                            Vertex tp = new Vertex(oriList[j].x + arrT * i, oriList[j].y, oriList[j].z);
                            allList.Add(tp);
                        }
                    }
                }
            }
            else
            {
                int beginN = -arrN / 2;
                if (isXPlus)
                {
                    for (int i = beginN; i <= -beginN; i++)
                    {
                        for (int j = 0; j < listLen; j++)
                        {
                            Vertex tp = new Vertex(oriList[j].x + arrT * i, oriList[j].y, oriList[j].z);
                            allList.Add(tp);
                        }
                    }                    
                }
                else
                {
                    for (int i = -beginN; i >= beginN; i--)
                    {
                        for (int j = 0; j < listLen; j++)
                        {
                            Vertex tp = new Vertex(oriList[j].x + arrT * i, oriList[j].y, oriList[j].z);
                            allList.Add(tp);
                        }
                    }
                }
            }
            //将重复的X删去
            if (allList.Count == 0)
            {
                return allList;
            }
            listLen = allList.Count;
            List<Vertex> backList = new List<Vertex>();
            Vertex cTp = allList[0];
            for (int i = 1; i < listLen - 1; i++)
            {
                if (allList[i].x != cTp.x)
                {
                    backList.Add(allList[i]);
                }
                cTp.x = allList[i].x;
                cTp.y = allList[i].y;
                cTp.z = allList[i].z;
            }
            return backList;
        }
        public static List<Vertex> GetMirrorList(List<Vertex> oriList)
        {
            List<Vertex> allList = new List<Vertex>();
            //bool hasZero = true;
            int oriListType = 0;
            if (oriList.Count == 0)
            {
                return allList;
            }
            if (oriList.Count == 1 && oriList[0].x == 0)
            {
                allList.Add(oriList[0]);
                return allList;
            }
            if (oriList.Count == 1)
            {
                Vertex temp = new Vertex(-oriList[0].x, oriList[0].y, oriList[0].z);
                if (oriList[0].x > 0)
                {
                    allList.Add(temp);
                    allList.Add(oriList[0]);
                    return allList;
                }
                else
                {
                    allList.Add(oriList[0]);
                    allList.Add(temp);
                    return allList;
                }
            }
            //判断有零还是无零
            int listLen = oriList.Count;
            //if (allList[0].x != 0 && allList[listLen - 1].x != 0)
            //{
            //    hasZero = false;
            //}
            //else
            //{
            //    hasZero = true;
            //}
            //判断原列表的类型，一共四种
            if (oriList[0].x < 0 && oriList[1].x > oriList[0].x)
            {
                oriListType = 0;
            }
            else if (oriList[0].x == 0 && oriList[1].x > oriList[0].x)
            {
                oriListType = 1;
            }
            else if (oriList[0].x > 0 && oriList[1].x < oriList[0].x)
            {
                oriListType = 2;
            }
            else if (oriList[0].x == 0 && oriList[1].x < oriList[0].x)
            {
                oriListType = 3;
            }
            switch (oriListType)
            {
                case 0:
                    {
                        for (int i = 0; i < listLen; i++)
                        {
                            allList.Add(oriList[i]);
                        }
                        for (int i = listLen - 2; i >= 0; i--)
                        {
                            Vertex tp = new Vertex(-oriList[i].x, oriList[i].y, oriList[i].z);
                            allList.Add(tp);
                        }
                        break;
                    }
                case 1:
                    {
                        for (int i = listLen - 1; i > 0; i--)
                        {
                            Vertex tp = new Vertex(-oriList[i].x, oriList[i].y, oriList[i].z);
                            allList.Add(tp);
                        }
                        for (int i = 0; i < listLen; i++)
                        {
                            allList.Add(oriList[i]);
                        }
                        break;
                    }
                case 2:
                    {
                        for (int i = 0; i < listLen; i++)
                        {
                            allList.Add(oriList[i]);
                        }
                        for (int i = listLen - 2; i >= 0; i--)
                        {
                            Vertex tp = new Vertex(-oriList[i].x, oriList[i].y, oriList[i].z);
                            allList.Add(tp);
                        }
                        break;
                    }
                case 3:
                    {
                        for (int i = listLen - 1; i > 0; i--)
                        {
                            Vertex tp = new Vertex(-oriList[i].x, oriList[i].y, oriList[i].z);
                            allList.Add(tp);
                        }
                        for (int i = 0; i < listLen; i++)
                        {
                            allList.Add(oriList[i]);
                        }
                        break;
                    }
            }
            return allList;
        }
        public static bool NcXZFilePhrase(string filePath, List<Vertex> zList)
        {
            try
            {
                StreamReader sr = new System.IO.StreamReader(filePath);
                string line = "";
                zList.Clear();
                while ((line = sr.ReadLine()) != null)
                {
                    string tpLine = line;
                    //删除空格
                    tpLine = tpLine.Replace(" ", "");
                    //找到了点数据
                    if (tpLine.IndexOf('X') == 0)
                    {
                        //int cPos = line.IndexOf('C');
                        int zPos = line.IndexOf('Z');
                        Vertex tempVT = new Vertex();
                        double x, z;
                        bool pxRes = double.TryParse(line.Substring(1, zPos - 1), out x);
                        bool pzRes = double.TryParse(line.Substring(zPos + 1), out z);
                        if (pzRes == true && pxRes == true)
                        {
                            tempVT.x = x;
                            tempVT.z = z;
                            zList.Add(tempVT);
                        }
                    }
                }
                sr.Close();
                return true;
            }
            catch (Exception)
            {
                //MessageBox.Show(err.Message);
                return false;
            }
        }
        public static bool MakePrfFile(List<Vertex> zList)
        {
            if (zList.Count == 0)
            {
                return false;
            }
            //计算X精度
            double xPrec = Math.Abs(zList[1].x - zList[0].x);
            //Z精度统一为1e-9
            string headTxt = "1 2\r\n" +
                             "Measur 0.00000000000e+000 PRF\r\n" +
                             "CX M " + zList.Count.ToString("e11") + " MM 1.00000000000e+000 D\r\n" +
                             "CZ M " + zList.Count.ToString("e11") + " MM 1.00000000000e-009 L\r\n" +
                             "EOR\r\n" +
                             "STYLUS_RADIUS 0.00000000000e+000 MM\r\n" +
                             "SPACING CX " + xPrec.ToString("e11") + "\r\n" + 
                             "MAP 1.000000e+000 CZ CZ 1.00000000000e+000 1.00000000000e+000\r\n" + 
                             "MAP 2.000000e+000 CZ CX 1.00000000000e+000 0.00000000000e+000\r\n" +
                             "COMMENT TH_DEVELOPMENT MEASUREMENT_CONDITIONS_STYLUS_TIP_RADIUS 2.00000000000e-003\r\n" +
                             "COMMENT TH_DEVELOPMENT MEASUREMENT_CONDITIONS_STYLUS_ARM_LENGTH 6.00000000000e+001\r\n" + 
                             "COMMENT TH_DEVELOPMENT MEASUREMENT_CONDITIONS_GAUGE_RANGE 1.25000000000e+001\r\n" + 
                             "COMMENT TH_DEVELOPMENT MEASUREMENT_CONDITIONS_CALIB_CONSTS_WERE_LINEAR 1.00000000000e+000\r\n" +
                             "EOR\r\n";
            //保存文件对话框
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "prf Files|*.prf";
            saveFileDialog1.Title = "保存PRF文件";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                string filePath = saveFileDialog1.FileName;
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                string temp = "";
                //写入文件头
                sw.Write(headTxt);
                int ptNum = zList.Count;
                for (int i = 0; i < ptNum; i++)
                {
                    temp = (zList[i].z * (1e9)).ToString("f0") + "\r\n";
                    sw.Write(temp);
                }
                sw.WriteLine("EOR");
                sw.WriteLine("EOF");
                string showTxt = "已将文件保存至" + filePath;
                MessageBox.Show(showTxt);
                sw.Close();
                fs.Close();
            }
            return true;
        }
        public static bool MakeModeFile(List<Vertex> zList)
        {
            if (zList.Count == 0)
            {
                return false;
            }
            //计算X长度
            double xLen = 0, maxX = zList[0].x, minX = zList[0].x;
            for (int i = 0; i < zList.Count; i++)
            {
                if (zList[i].x > maxX)
                {
                    maxX = zList[i].x;
                }
                if (zList[i].x < minX)
                {
                    minX = zList[i].x;
                }
            }
            xLen = maxX - minX;
            //文件头
            string headTxt = "1 2\r\n" + 
                             "Aspheric_Utility 1.000000e+000 MOD\r\n" +
                             "CX A " + zList.Count.ToString("e6") + " MM 1.000000e+000 D\r\n" +
                             "CZ A " + zList.Count.ToString("e6") + " MM 1.000000e+000 D\r\n" + 
                             "EOR\r\n" +
                             "FILTER_MODE NO_FILTER\r\n" + 
                             "FORM ASPHERIC\r\n" +
                             "ASSESSMENT_LENGTH " + xLen.ToString("e6") + " MM\r\n" +
                             "UMBER_MOD_POINTS " + zList.Count.ToString("e6") + "\r\n" +
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -2.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -3.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -4.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -5.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -6.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -7.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -8.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -9.000000e+000\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.000000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.100000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.200000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.300000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.400000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.500000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.600000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.700000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.800000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -1.900000e+001\r\n" + 
                             "ASPHERIC_COEFF 0.000000e+000 MM -2.000000e+001\r\n" + 
                             "ASPHERIC_RADIUS 9.100000e+000 MM\r\n" + 
                             "ASPHERIC_K 0.000000e+000\r\n" + 
                             "EOR\r\n";
            //保存文件对话框
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "mod Files|*.mod";
            saveFileDialog1.Title = "保存MOD文件";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                // Saves the Image via a FileStream created by the OpenFile method.
                string filePath = saveFileDialog1.FileName;
                System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                StreamWriter sw = new StreamWriter(fs, Encoding.Default);
                string temp = "";
                //写入文件头
                sw.Write(headTxt);
                int ptNum = zList.Count;
                for (int i = 0; i < ptNum; i++)
                {
                    temp = zList[i].x.ToString("e6") + "\r\n";
                    sw.Write(temp);
                }
                for (int i = 0; i < ptNum; i++)
                {
                    temp = zList[i].z.ToString("e6") + "\r\n";
                    sw.Write(temp);
                }
                sw.WriteLine("EOR");
                sw.WriteLine("EOF");
                string showTxt = "已将文件保存至" + filePath;
                MessageBox.Show(showTxt);
                sw.Close();
                fs.Close();
            }

            return true;
        }
        public static bool MakeFNLNcFile(fnlSegments fSeg, string filePath, string numData, ref string MainTxt)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string fileP = filePath.Substring(0, findTag);
            if (!Directory.Exists(fileP))
            {
                return false;
            }
            double xLead = 0;
            string cycStr = "";
            if (fSeg.SpdDir == "顺时针")
            {
                xLead = fSeg.PartSize + fSeg.LeadInX;
                cycStr = "M03";
            }
            else
            {
                xLead = -fSeg.PartSize - fSeg.LeadInX;
                cycStr = "M04";
            }
            string HeadStr = "( Cigit Version - Version 1.40 )\r\n"
                        + "( CREATED : " + DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.ToString() + " )\r\n"//时间日期 
                        + "( ========== PART SURFACE INFO ========== )\r\n"
                        + "( NUMBER OF SEGMENTS: 1 )\r\n"
                        + "( SEGMENT 1: Line )"
                        + "( Width = " + fSeg.PartSize.ToString() + " mm )\r\n"
                        + "( Height = 0 mm )\r\n"
                        + "( --- RESET 5XX VARIABLES --- )\r\n"
                        + "#547 = 0\r\n"
                        + "\r\n"
                        + "( ========== SECTION - COMMANDS ========== )\r\n"
                        + "#501 = " + fSeg.TotalLoops.ToString() + "                      ( NUMBER OF LOOPS )\r\n"
                        + "#502 = " + fSeg.CutDepth.ToString() + "                    ( DEPTH OF CUT )\r\n"
                        + "#503 = " + fSeg.FeedRate.ToString() + "                      ( FEEDRATE )\r\n"
                        + "#504 = " + fSeg.SpindleSpd.ToString() + "				( SPINDLE SPEED )\r\n"
                        + "\r\n"
                        + "#506 = 0                      ( LOOP COUNT )\r\n"
                        + "#509 = 0                      ( FINISH CUT COUNT 1 )\r\n"
                        + "#547 = 0				( CUT OFFSET VARIABLE )\r\n"
                        + "\r\n"
                        + "G71 G01 G18 G40 G63 G90 G94 " + fSeg.WorkCoord +  "\r\n"
                        + "T0000                    ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                        + "\r\n"
                        + fSeg.CutCpst + "\r\n"
                        + "#547 = #547 - #502		( SUBTRACTS DEPTH OF CUT FROM CUT OFFSET VARIABLE)\r\n"
                        + "G52 Z[#547]                   ( INCREMENT LOCAL COORDINATE SYSTEM SETTING BY CUT OFFSET VARIABLE )\r\n"
                        + "G01 X" + xLead.ToString() + " F200                ( PARKING POSITION - X )\r\n"
                        + "Z" + fSeg.LeadInZ.ToString() + "                        ( PARKING POSITION - Z )\r\n"
                        + "\r\n"
                        + "Y0.0              ( SET Y AXIS TO ZERO )\r\n"
                        + cycStr + "S[#504]\r\n"
                        + "\r\n"
                        + "X" + xLead.ToString() + " Z" + fSeg.HZ.ToString() + " F100     ( FROM PARKING POSITION )\r\n"
                        + "( LEAD IN BLOCKS )\r\n"
                        + fSeg.JetNo + "\r\n"
                        + "X" + xLead.ToString() + " Z" + fSeg.HZ.ToString() + " F[#503]\r\n"
                        + "( CUTTING BLOCKS )\r\n";

            string EndStr = "( LEAD OUT BLOCKS )\r\n"
                        + "G04P.1\r\n"
                        + "Z" + fSeg.LeadOutZ.ToString() + "\r\n"
                        + "M29\r\n"
                        + "G94 G01 Z" + fSeg.LeadOutZ.ToString() + " F200                ( PARKING POSITION - Z )\r\n"
                        + "X" + xLead.ToString() + "                         ( PARKING POSITION - X )\r\n"
                        + "\r\n"
                        + "#506 = #506 + 1\r\n"
                        + "END 1\r\n"
                        + "G52 Z0.0 			( SHIFT THE LOCAL COORDINATE SYSTEM TO 0.0)\r\n"
                        + "G94 G01 Z" + fSeg.LeadOutZ.ToString() + " F200                ( PARKING POSITION - Z )\r\n"
                        + "X" + xLead.ToString() + "                         ( PARKING POSITION - X )\r\n"
                        + "\r\n"
                        + "M30                  ( RESET PROGRAM )\r\n";
            System.IO.FileStream fs = new FileStream(filePath, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(HeadStr);
            sw.Write(numData);
            sw.Write(EndStr);
            sw.Close();
            fs.Close();
            return true;

        }
        public static bool MakeMainNcFile(CutSegments hcs, CutSegments cs, string filePath, ref string MainTxt)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string fileP = filePath.Substring(0, findTag);
            if (!Directory.Exists(fileP))
            {
                return false;
            }
            MainTxt = "( ============ SECTION - HEADER ====================== )\r\n( Cigit Version : 2.7 Build : 55 )\r\n( CREATED : "
                                + DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.ToString() + " )\r\n"//时间日期                               
                                + "( OPERATOR : Cigit Operator )\r\n"
                                + "( SCRIPT: FTS Post.mpyx - MODIFIED: 2016/5/12 )\r\n\r\n"
                                + "( ============ SECTION - INITIALIZATION ============== )\r\n"
                                + "( ***** RESET ALL 5XX VARIABLES ******* )\r\n"
                                + "#500 = 500\r\n"
                                + "WHILE [#500LT599] DO 1\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "#[#500] = 0\r\n"
                                + "END1\r\n"
                                + "#599 = 0\r\n"
                                + "#500 = 0\r\n"
                                + "M79							( DISABLE C AXIS MODE )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - VARIABLE DECLARATION ======== )\r\n"
                                + "( ***** PROGRAM VARIABLES ******* )\r\n"
                                + "#500 = 0                        ( PASS COUNTER )\r\n"
                                + "#501 = 0.00                     ( USER INPUT - OVERALL LENGTH )\r\n"
                                + "#503 = 0.00                     ( USER INPUT - ZORIGIN=FLAT )\r\n"
                                + "#504 = 0.0                      ( USER INPUT - TOOL NOSE RADIUS FOR MACHINE COMP )\r\n"
                                + "#505 = 0.01536108				( POSITIVE FREEFORM DEPARTURE )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "                      ( MIST NUMBER )\r\n"//喷气号
                                + "#588 = -0.00340770				( NEGATIVE FREEFORM DEPARTURE )\r\n"
                                + "\r\n"
                                + "( ***** SEMI-FINISH CONSTANTS *** )\r\n"
                                + "#530 = " + hcs.SpdSpeed.ToString() + "	    			( RPM )\r\n"
                                + "#531 = " + hcs.TotalLoops.ToString() + "						( # OF CUTS )\r\n"
                                + "#532 = " + hcs.DepthOfCut.ToString() + "			( DOC )\r\n"
                                + "#533 = " + hcs.FeedRate.ToString() + "			( FEEDRATEConstantSpeed_G94 )\r\n"
                                + "#534 = 1					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( ***** FINISH CUT CONSTANTS **** )\r\n"
                                + "#540 = " + cs.SpdSpeed.ToString() + "	    			( RPM )\r\n"
                                + "#541 = " + cs.TotalLoops.ToString() + "						( # OF CUTS )\r\n"
                                + "#542 = " + cs.DepthOfCut.ToString() + "			( DOC )\r\n"
                                + "#543 = " + cs.FeedRate.ToString() + "			( FEEDRATEConstantSpeed_G94 )\r\n"
                                + "#544 = 1					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "( #550 = TOTAL Z-STOCK REMOVAL )\r\n"
                                + "( #551 = TOTAL X-STOCK REMOVAL )\r\n"
                                + "( #552 = CURRENT DOC )\r\n"
                                + "( #553 = CURRENT FEEDRATE )\r\n"
                                + "( #554 = CURRENT NFTS STATUS )\r\n"
                                + "( #555 = CURRENT SPINDLE SPEED )\r\n"
                                + "\r\n"
                                + "( M60 : REFERENCE ON  - X AND C )\r\n"
                                + "( M61 : REFERENCE OFF - X AND C )\r\n"
                                + "( M62 : FTSN ON-TRACKING, FLOATING )\r\n"
                                + "( M63 : W ON AND REFERENCE )\r\n"
                                + "( M64 : FTS ON,NON TRACKING,HOLD ZERO POSITION )\r\n"
                                + "( M65 : FTS ON,TRACKING, ACTIVE )\r\n"
                                + "( M66 : SYNC WITH FTS READY )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - COMMANDS ==================== )\r\n"
                                + "\r\n"
                                + "G01 G71 G90 G40 G18 " + cs.WorkCoord
                                + " T0000					    ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "G94				    	( FEED PER MINUTE )\r\n"
                                + "G04 P.4					( DWELL )\r\n"
                                + "M01 						( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "#550 = #501 + [#521*#522] + [#531*#532] + [#541*#542] + #503 + #504 - #505  ( TOTAL STOCK REMOVAL )\r\n"
                                + "\r\n"
                                + " ( ----- SEMI-FINISH CUT --- )\r\n"
                                + "\r\n"
                                + "#550 = #550 + #505   ( RETRACT TOOL BY FREEFORM DEPARTURE DISTANCE  )\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - SEMI-FINISH CUT )\r\n"
                                + "IF[#500EQ#531] GOTO31\r\n"
                                + "\r\n"
                                + cs.CutCpst + "          ( ACTIVATE TOOL OFFSET )\r\n"
                                + "M05            ( ENSURE SPINDLE IS STOPPED COMPLETELY )\r\n"
                                + "M80            ( ORIENT C TO 0 )\r\n"
                                + "G09 C0.0       ( USER INPUT )\r\n"
                                + "G92 C0.0\r\n"
                                + "G04 P1         ( DWELL FOR SETTLING )\r\n"
                                + "X-0 C0 F500    ( MOVE X AND C TO START POSITION )\r\n"
                                + "G04 P5         ( DWELL FOR STOP )\r\n"
                                + "M60            ( REF X AND C                BIT 1 ON, DRY CONTACT ON )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "M61            ( END REF X- AND C           BIT 1 OFF, DRY CONTACT OFF )\r\n"
                                + "G4P1           ( DWELL )\r\n"
                                + "( M63            ( W ON AND REFERENCE         BIT 2 ON, BIT 3 OFF ) )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "M66            ( SYNC WITH FTS READY        WAIT FOR INPUT 1 TO TOGGLE )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "M79            ( EXIT C-AXIS MODE )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "WHILE[#500LT#531]DO2\r\n"
                                + cs.CutCpst + "          ( ACTIVATE TOOL OFFSET )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "                      ( MIST NUMBER )\r\n"//喷气号
                                + "#552 = #532			( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #533			( ASSIGN FEED RATE )\r\n"
                                + "#554 = #534			( SET NFTS TO OFF )\r\n"
                                + "#555 = #530			( ASSIGN SPINDLE SPEED )\r\n"
                                + "M98(" + hcs.NCFilePath + ")\r\n"//半精车
                                + "#500 = #500 + 1\r\n"
                                + "END 2\r\n"
                                + "N31\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0\r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "( ----- FINISH CUT -------- )\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - FINISH CUT )\r\n"
                                + "IF[#500EQ#541] GOTO41\r\n"
                                + "\r\n"
                                + cs.CutCpst + "          ( ACTIVATE TOOL OFFSET )\r\n"
                                + "M05            ( ENSURE SPINDLE IS STOPPED COMPLETELY )\r\n"
                                + "M80            ( ORIENT C TO 0 )\r\n"
                                + "G09 C0.0       ( USER INPUT )\r\n"
                                + "G92 C0.0\r\n"
                                + "G04 P1         ( DWELL FOR SETTLING )\r\n"
                                + "X-0 C0 F500    ( MOVE X AND C TO START POSITION )\r\n"
                                + "G04 P5         ( DWELL FOR STOP )\r\n"
                                + "M60            ( REF X AND C                BIT 1 ON, DRY CONTACT ON )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "M61            ( END REF X- AND C           BIT 1 OFF, DRY CONTACT OFF )\r\n"
                                + "G4P1           ( DWELL )\r\n"
                                + "( M63            ( W ON AND REFERENCE         BIT 2 ON, BIT 3 OFF ) )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "M66            ( SYNC WITH FTS READY        WAIT FOR INPUT 1 TO TOGGLE )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "M79            ( EXIT C-AXIS MODE )\r\n"
                                + "G04 P1         ( DWELL )\r\n"
                                + "WHILE[#500LT#541]DO3\r\n"
                                + cs.CutCpst + "          ( ACTIVATE TOOL OFFSET )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "                      ( MIST NUMBER )\r\n"//喷气号
                                + "#552 = #542			( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #543			( ASSIGN FEED RATE )\r\n"
                                + "#554 = #544			( SET NFTS TO OFF )\r\n"
                                + "#555 = #540			( ASSIGN SPINDLE SPEED )\r\n"
                                + "M98(" + cs.NCFilePath + ")\r\n"//精车
                                + "#500 = #500 + 1\r\n"
                                + "END 3\r\n"
                                + "\r\n"
                                + "N41\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0 \r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "( ============ SECTION - FOOTER ====================== )\r\n"
                                + "\r\n"
                                + "T0000					( CANCEL ALL TOOL OFFSETS )\r\n"
                                + "M05						( STOP SPINDLE )\r\n"
                                + "M30						( RESET PROGRAM )\r\n"
                                + "\r\n";
            System.IO.FileStream fs = new FileStream(filePath,FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(MainTxt);
            sw.Close();
            fs.Close();
            return true;
        }
        public static bool MakeMainRecNcFile(RecCutSegments ccs, RecCutSegments hcs, RecCutSegments cs, string filePath)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string fileP = filePath.Substring(0, findTag);
            string MainTxt = "";
            if (!Directory.Exists(fileP))
            {
                return false;
            }
            MainTxt = "( ============ SECTION - HEADER ====================== )\r\n"
                                + "( Cigit Version : 2.7 Build : 55 )\r\n"
                                + "( CREATED : "
                                + DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.ToString() + " )\r\n"//时间日期                               
                                + "( OPERATOR : Cigit Operator ZhuGuoDong)\r\n"
                                + "( SCRIPT: SSS Post.mpyx - MODIFIED: 2016/5/12 )\r\n\r\n"
                                + "( ============ SECTION - INITIALIZATION ============== )\r\n"
                                + "( ***** RESET ALL 5XX VARIABLES ******* )\r\n"
                                + "#500 = 500\r\n"
                                + "WHILE [#500LT599] DO 1\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "#[#500] = 0\r\n"
                                + "END1\r\n"
                                + "#599 = 0\r\n"
                                + "#500 = 0\r\n"
                                + "M79							( DISABLE C AXIS MODE )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - VARIABLE DECLARATION ======== )\r\n"
                                + "\r\n"
                                + "( ***** PROGRAM VARIABLES ******* )\r\n"
                                + "#500 = 0                        ( PASS COUNTER )\r\n"
                                + "#501 = 0.00                     ( USER INPUT - OVERALL LENGTH )\r\n"
                                + "#503 = 0.00                     ( USER INPUT - ZORIGIN=FLAT )\r\n"
                                + "#504 = 0.0                      ( USER INPUT - TOOL NOSE RADIUS FOR MACHINE COMP )\r\n"
                                + "#505 = 0              ( POSITIVE FREEFORM DEPARTURE )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "                       ( MIST NUMBER )\r\n"
                                + "#588 = 0              ( NEGATIVE FREEFORM DEPARTURE )\r\n" //喷气号
                                + "\r\n"
                                + "( ***** ROUGH-FINISH CONSTANTS *** )\r\n"
                                + "#521 = " + ccs.TotalLoops.ToString() + "					( # OF CUTS )\r\n"
                                + "#522 = " + ccs.DepthOfCut.ToString("F8") + "			( DOC )\r\n"
                                + "#523 = 10					( FEEDRATE-G94 USER INPUT )\r\n"
                                + "#524 = 0					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( ***** SEMI-FINISH CONSTANTS *** )\r\n"
                                + "#531 = " + hcs.TotalLoops.ToString() + "						( # OF CUTS )\r\n"
                                + "#532 = " + hcs.DepthOfCut.ToString("F8") + "			( DOC )\r\n"
                                + "#533 = 10					( FEEDRATE-G94 USER INPUT )\r\n"
                                + "#534 = 0					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( ***** FINISH CUT CONSTANTS **** )\r\n"
                                + "#541 = " + cs.TotalLoops.ToString() + "						( # OF CUTS )\r\n"
                                + "#542 = " + cs.DepthOfCut.ToString("F8") + "			( DOC )\r\n"
                                + "#543 = 10					( FEEDRATE-G94 USER INPUT )\r\n"
                                + "#544 = 0					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( #550 = TOTAL Z-STOCK REMOVAL )\r\n"
                                + "( #551 = TOTAL X-STOCK REMOVAL )\r\n"
                                + "( #552 = CURRENT DOC )\r\n"
                                + "( #553 = CURRENT FEEDRATE )\r\n"
                                + "( #554 = CURRENT NFTS STATUS )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - COMMANDS ==================== )\r\n"
                                + "\r\n"
                                + "G01 G71 G90 G40 G18 " + cs.WorkCoord + "\r\n"
                                + "T0000					( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "G94					( FEED PER MINUTE )\r\n"
                                + "G04 P.4					( DWELL )\r\n"
                                + "M80                                 ( C AXIS ORIENTATION )\r\n"
                                + "G09 C0.0                            ( USER INPUT )\r\n"
                                + "G92 C0.0                            ( SET COORD SYS )\r\n"
                                + "M01 						( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "#550 = #501 + [#521*#522] + [#531*#532] + [#541*#542] + #503 + #504 - #505     ( TOTAL STOCK REMOVAL )\r\n"
                                + "\r\n"
                                + "( ----- ROUGH-FINISH CUT --- )\r\n"
                                + "\r\n"
                                + "#550 = #550 + #505   ( RETRACT TOOL BY FREEFORM DEPARTURE DISTANCE  )\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - SEMI-FINISH CUT )\r\n"
                                + "IF[#500EQ#521] GOTO21\r\n"
                                + "\r\n"
                                + "WHILE[#500LT#521]DO1\r\n"
                                + ccs.CutCpst + "               ( ACTIVATE TOOL OFFSET )\r\n"
                                + "#507 = " + ccs.JetNo.Replace("M", "") + "           ( MIST NUMBER )\r\n"
                                + "#552 = #522      ( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #523      ( ASSIGN FEED RATE )\r\n"
                                + "#554 = #524      ( SET NFTS TO OFF )\r\n"
                                + "M98(" + ccs.NCFilePath + ")\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "END 1\r\n"
                                + "\r\n"
                                + "N21\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0\r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "( ----- SEMI-FINISH CUT --- )\r\n"
                                + "\r\n"
                                + "#550 = #550 + #505   ( RETRACT TOOL BY FREEFORM DEPARTURE DISTANCE  )\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - SEMI-FINISH CUT )\r\n"
                                + "IF[#500EQ#531] GOTO31\r\n"
                                + "\r\n"
                                + "WHILE[#500LT#531]DO2\r\n"
                                + hcs.CutCpst + "               ( ACTIVATE TOOL OFFSET )\r\n"
                                + "#507 = " + hcs.JetNo.Replace("M", "") + "           ( MIST NUMBER )\r\n"
                                + "#552 = #532      ( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #533      ( ASSIGN FEED RATE )\r\n"
                                + "#554 = #534      ( SET NFTS TO OFF )\r\n"
                                + "M98(" + hcs.NCFilePath + ")\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "END 2\r\n"
                                + "\r\n"
                                + "N31\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0\r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "( ----- FINISH CUT -------- )\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - FINISH CUT )\r\n"
                                + "IF[#500EQ#541] GOTO41\r\n"
                                + "\r\n"
                                + "WHILE[#500LT#541]DO3\r\n"
                                + cs.CutCpst + "               ( ACTIVATE TOOL OFFSET  )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "           ( MIST NUMBER )\r\n"
                                + "#552 = #542      ( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #543      ( ASSIGN FEED RATE )\r\n"
                                + "#554 = #544      ( SET NFTS TO OFF )\r\n"
                                + "M98(" + cs.NCFilePath + ")\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "END 3\r\n"
                                + "\r\n"
                                + "N41\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0 \r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "( ============ SECTION - FOOTER ====================== )\r\n"
                                + "\r\n"
                                + "T0000      ( CANCEL ALL TOOL OFFSETS )\r\n"
                                + "M30      ( RESET PROGRAM )\r\n";
            System.IO.FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(MainTxt);
            sw.Close();
            fs.Close();
            return true;
        }
        public static bool MakeRecDataNCHE(RecCutSegments sg, double firstX, double Y, double firstZ, ref string headTxt, ref string endTxt)
        {
            headTxt = "G94 	   		( FEED RATE IN mm/min & NFTS OFF )\r\n"
                      + "#550 = #550 - #552					( CURRENT CUTTING OFFSET )\r\n"
                      + "G52 Z[#550]							( SET COORDINATE SYSTEM OFFSET )\r\n"
                      + "G01 X" + firstX.ToString("F8") + " Y" + Y.ToString("F8") + " Z" + (firstZ + sg.RPOfs).ToString("F8") + " F400				( PARKING POSITION - X )\r\n"
                      + "( LEAD IN BLOCKS )\r\n"
                      + "M[#507]							( MIST ON )\r\n";
            endTxt = "G04 P1             ( DWELL )\r\n"
                      + "M29                ( MIST OFF )\r\n"
                      + "( PARKING POSITION )\r\n"
                      + "G94 G01 Z5 F200\r\n"
                      + "X" + firstX.ToString("F8") + "\r\n"
                      + "( =============== SECTION - FOOTER =================== )\r\n"
                      + "M99 									( RETURN TO MAIN )\r\n";
            return true;
        }
        public static bool MakeRecDataNc(RecCutSegments ccs, RecCutSegments hcs, RecCutSegments cs,
            string ccDataStr, string hcDataStr, string cDataStr, double[] firstX, double[] firstZ, double Y, string filePath)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string ccsFileP = filePath.Substring(0, findTag);
            string hcsFileP = ccsFileP, csFileP = ccsFileP;
            string headTxt = "", endTxt = "";//前后STR
            if (!Directory.Exists(ccsFileP))
            {
                return false;
            }
            ccsFileP += ("\\" + ccs.NCFilePath);
            hcsFileP += ("\\" + hcs.NCFilePath);
            csFileP += ("\\" + cs.NCFilePath);

            //初车数据文件
            MakeRecDataNCHE(ccs, firstX[0], Y, firstZ[0], ref headTxt, ref endTxt);
            StreamWriter sw = new StreamWriter(ccsFileP, false, Encoding.Default);
            sw.Write(headTxt);
            //中间数据段
            //将中间数据段最后以回车结束
            ComFunc.MakeEndOneEnter(ccDataStr);
            sw.Write(ccDataStr);
            //结尾
            sw.Write(endTxt);
            sw.Close();

            //半精车数据文件
            MakeRecDataNCHE(hcs, firstX[1], Y, firstZ[1], ref headTxt, ref endTxt);
            sw = new StreamWriter(hcsFileP, false, Encoding.Default);
            sw.Write(headTxt);
            //中间数据段
            //将中间数据段最后以回车结束
            ComFunc.MakeEndOneEnter(hcDataStr);
            sw.Write(hcDataStr);
            //结尾
            sw.Write(endTxt);
            sw.Close();

            //精车数据文件
            MakeRecDataNCHE(cs, firstX[2], Y, firstZ[2], ref headTxt, ref endTxt);
            sw = new StreamWriter(csFileP, false, Encoding.Default);
            sw.Write(headTxt);
            //中间数据段
            //将中间数据段最后以回车结束
            ComFunc.MakeEndOneEnter(cDataStr);
            sw.Write(cDataStr);
            //结尾
            sw.Write(endTxt);
            sw.Close();
            return true;
        }
        public static bool MakeMainSSSNcFile(SSSCutSegments ccs, SSSCutSegments hcs, SSSCutSegments cs, string filePath)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string fileP = filePath.Substring(0, findTag);
            string MainTxt = "";
            if (!Directory.Exists(fileP))
            {
                return false;
            }
            MainTxt = "( ============ SECTION - HEADER ====================== )\r\n"
                                + "( Cigit Version : 2.7 Build : 55 )\r\n"
                                + "( CREATED : "
                                + DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.ToString() + " )\r\n"//时间日期                               
                                + "( OPERATOR : Cigit Operator ZhuGuoDong)\r\n"
                                + "( SCRIPT: SSS Post.mpyx - MODIFIED: 2015/5/14 )\r\n\r\n"
                                + "( ============ SECTION - INITIALIZATION ============== )\r\n"
                                + "( ***** RESET ALL 5XX VARIABLES ******* )\r\n"
                                + "#500 = 500\r\n"
                                + "WHILE [#500LT599] DO 1\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "#[#500] = 0\r\n"
                                + "END1\r\n"
                                + "#599 = 0\r\n"
                                + "#500 = 0\r\n"
                                + "M79							( DISABLE C AXIS MODE )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - VARIABLE DECLARATION ======== )\r\n"
                                + "\r\n"
                                + "( ***** PROGRAM VARIABLES ******* )\r\n"
                                + "#500 = 0                        ( PASS COUNTER )\r\n"
                                + "#501 = 0.00                     ( USER INPUT - OVERALL LENGTH )\r\n"
                                + "#503 = 0.00                     ( USER INPUT - ZORIGIN=FLAT )\r\n"
                                + "#504 = 0.0                      ( USER INPUT - TOOL NOSE RADIUS FOR MACHINE COMP )\r\n"
                                + "#505 = 0.03514524              ( POSITIVE FREEFORM DEPARTURE )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "                       ( MIST NUMBER )\r\n"//喷气号
                                + "#588 = -0.01218929              ( NEGATIVE FREEFORM DEPARTURE )\r\n"
                                + "\r\n"
                                + "( ***** ROUGH CUT CONSTANTS ***** )\r\n"
                                + "#521 = " + ccs.TotalLoops.ToString() + "					( # OF CUTS )\r\n"
                                + "#522 = " + ccs.DepthOfCut.ToString() + "			( DOC )\r\n"
                                + "#523 = 10.0					( FEEDRATE-G94 )\r\n"
                                + "#524 = 0					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( ***** SEMI-FINISH CONSTANTS *** )\r\n"
                                + "#531 = " + hcs.TotalLoops.ToString() + "						( # OF CUTS )\r\n"
                                + "#532 = " + hcs.DepthOfCut.ToString() + "			( DOC )\r\n"
                                + "#533 = 10					( FEEDRATE-G94 USER INPUT )\r\n"
                                + "#534 = 0					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( ***** FINISH CUT CONSTANTS **** )\r\n"
                                + "#541 = " + cs.TotalLoops.ToString() + "						( # OF CUTS )\r\n"
                                + "#542 = " + cs.DepthOfCut.ToString() + "			( DOC )\r\n"
                                + "#543 = 10					( FEEDRATE-G94 USER INPUT )\r\n"
                                + "#544 = 0					( 0 = NFTS OFF, 1 = NFTS ON )\r\n"
                                + "\r\n"
                                + "( #550 = TOTAL Z-STOCK REMOVAL )\r\n"
                                + "( #551 = TOTAL X-STOCK REMOVAL )\r\n"
                                + "( #552 = CURRENT DOC )\r\n"
                                + "( #553 = CURRENT FEEDRATE )\r\n"
                                + "( #554 = CURRENT NFTS STATUS )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - COMMANDS ==================== )\r\n"
                                + "\r\n"
                                + "G01 G71 G90 G40 G18 " + cs.WorkCoord + "\r\n"
                                + "T0000					( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "G94					( FEED PER MINUTE )\r\n"
                                + "G04 P.4					( DWELL )              ( PARKING POSITION - NO TOOL ACTIVE )\r\n"
                                + "M01 						( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "#550 = #501 + [#521*#522] + [#531*#532] + [#541*#542] + #503 + #504 - #505     ( TOTAL STOCK REMOVAL )\r\n"
                                + "\r\n"
                                + "( ----- ROUGH CUT --------- )\r\n"
                                + "\r\n"
                                + "G04 P.4					( DWELL )\r\n"
                                + "M64					    	( NFTS OFF )\r\n"
                                + "\r\n"
                                + "#550 = #550 + #505   ( RETRACT TOOL BY FREEFORM DEPARTURE DISTANCE  )\r\n"
                                + "( CALL SUB ROUTINE - ROUGH CUT )\r\n"
                                + "IF[#500EQ#521] GOTO21\r\n"
                                + "M80                                 ( C AXIS ORIENTATION )\r\n"
                                + "G09 C0.0                            ( USER INPUT )\r\n"
                                + "G92 C0.0\r\n"
                                + "WHILE[#500LT#521]DO2\r\n"
                                + cs.CutCpst + "               ( ACTIVATE TOOL OFFSET )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "                      ( MIST NUMBER )\r\n"//喷气号\r\n"
                                + "#552 = #522			( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #523			( ASSIGN FEED RATE  )\r\n"
                                + "#554 = #524			( SET NFTS TO OFF  )\r\n"
                                + "M98(" + ccs.NCFilePath + ")\r\n"
                                + "#500 = #500 + 1\r\n"
                                + "END 2\r\n"
                                + "\r\n"
                                + "N21\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )              ( PARKING POSITION - NO TOOL ACTIVE )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "( ----- SEMI-FINISH CUT --- )\r\n"
                                + "\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - SEMI-FINISH CUT )\r\n"
                                + "IF[#500EQ#531] GOTO31\r\n"
                                + "\r\n"
                                + "M80                                 ( C AXIS ORIENTATION )\r\n"
                                + "G09 C0.0                            ( USER INPUT )\r\n"
                                + "G92 C0.0                            ( SET COORD SYS )\r\n"
                                + "WHILE[#500LT#531]DO3\r\n"
                                + cs.CutCpst + "               ( ACTIVATE TOOL OFFSET )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "           ( MIST NUMBER )\r\n"//喷气号
                                + "#552 = #532			( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #533			( ASSIGN FEED RATE )\r\n"
                                + "#554 = #534			( SET NFTS TO OFF )\r\n"
                                + "M98(" + hcs.NCFilePath + ")\r\n"//半精车
                                + "#500 = #500 + 1\r\n"
                                + "END 3\r\n"
                                + "\r\n"
                                + "N31\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0\r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "M79                        ( DISABLE C AXIS MODE )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )              ( PARKING POSITION - NO TOOL ACTIVE )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "( ----- FINISH CUT -------- )\r\n"
                                + "\r\n"
                                + "( CALL SUB ROUTINE - FINISH CUT )\r\n"
                                + "IF[#500EQ#541] GOTO41\r\n"
                                + "\r\n"
                                + "M80                                 ( C AXIS ORIENTATION )\r\n"
                                + "G09 C0.0                            ( USER INPUT )\r\n"
                                + "G92 C0.0                            ( SET COORD SYS )\r\n"
                                + "WHILE[#500LT#541]DO1\r\n"
                                + cs.CutCpst + "               ( ACTIVATE TOOL OFFSET  )\r\n"
                                + "#507 = " + cs.JetNo.Replace("M", "") + "           ( MIST NUMBER )\r\n"//喷气号
                                + "#552 = #542			( ASSIGN DEPTH OF CUT )\r\n"
                                + "#553 = #543			( ASSIGN FEED RATE )\r\n"
                                + "#554 = #544			( SET NFTS TO OFF )\r\n"
                                + "M98(" + cs.NCFilePath + ")\r\n"//精车
                                + "#500 = #500 + 1\r\n"
                                + "END 1\r\n"
                                + "\r\n"
                                + "N41\r\n"
                                + "\r\n"
                                + "#500 = 0                   ( RESET PASS COUNTER )\r\n"
                                + "#554 = 0 \r\n"
                                + "G52                        ( RESET G52 OFFSETS )\r\n"
                                + "M79                        ( DISABLE C AXIS MODE )\r\n"
                                + "G04 P.4                    ( DWELL )\r\n"
                                + "T0000                      ( DEACTIVATE ALL TOOL OFFSETS )\r\n"
                                + "G53 Z0.0 F200              ( PARKING POSITION - NO TOOL ACTIVE )\r\n"
                                + "M01                        ( OPTIONAL STOP )\r\n"
                                + "\r\n"
                                + "( ============ SECTION - FOOTER ====================== )\r\n"
                                + "\r\n"
                                + "T0000					( CANCEL ALL TOOL OFFSETS )\r\n"
                                + "M05						( STOP SPINDLE )\r\n"
                                + "M30						( RESET PROGRAM )\r\n"
                                + "\r\n";
            System.IO.FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            sw.Write(MainTxt);
            sw.Close();
            fs.Close();
            return true;
        }
        public static void MakeSSSFileHE(SSSCutSegments sg, double firstZ, ref string headStr, ref string endStr)
        {
            headStr = "G94 	   		( FEED RATE IN mm/min & NFTS OFF )\r\n"
                                + "#550 = #550 - #552					( CURRENT CUTTING OFFSET )\r\n"
                                + "G52 Z[#550]							( SET COORDINATE SYSTEM OFFSET )\r\n"
                                + "G01 X" + sg.CutXStart.ToString("F6") + " F200				( PARKING POSITION - X )\r\n"
                                + "Z" + sg.RPOfs.ToString("F6") + "							( PARKING POSITION - Z )\r\n"
                                + "\r\n"
                                + "( LEAD IN BLOCKS )\r\n"
                                + "G01 C0.0 X18.1 F200\r\n"
                                + "Z" + (0.1 + firstZ).ToString("F8") + "\r\n"
                                + "Y0.0							( SET Y AXIS TO 0 )\r\n"
                                + "M[#507]							( MIST ON )\r\n"
                                + "G01 G93 F" + sg.CutTime.ToString("F6") + "\r\n";

            endStr = "C0.0\r\n"
                    + "G04 P1             ( DWELL )\r\n"
                    + "M29                ( MIST OFF )\r\n"
                    + "( PARKING POSITION )\r\n"
                    + "G94 G01 Z" + sg.RPOfs.ToString("F8") + " F200\r\n"
                    + "X" + sg.CutXStart.ToString("F8") + "\r\n"
                    + "( =============== SECTION - FOOTER =================== )\r\n"
                    + "M99\r\n";
        }
        public static bool MakeSSSDataNc(SSSCutSegments ccs, SSSCutSegments hcs, SSSCutSegments cs,
            string ccDataPath, string hcDataPath, string cDataPath, double firstZ, string filePath)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string ccsFileP = filePath.Substring(0, findTag);
            string hcsFileP = ccsFileP, csFileP = ccsFileP;
            string headTxt = "", endTxt = "";//前后STR
            if (!Directory.Exists(ccsFileP))
            {
                return false;
            }
            ccsFileP += ("\\" + ccs.NCFilePath);
            hcsFileP += ("\\" + hcs.NCFilePath);
            csFileP += ("\\" + cs.NCFilePath);

            //初车数据文件
            MakeSSSFileHE(ccs, firstZ, ref headTxt, ref endTxt);
            StreamWriter sw = new StreamWriter(ccsFileP, false, Encoding.Default);
            sw.Write(headTxt);
            //读取中间数据段,写入文件
            StreamReader sr = new StreamReader(ccDataPath, Encoding.Default);
            string tpOneLine = "";
            while ((tpOneLine = sr.ReadLine()) != null)
            {
                sw.WriteLine(tpOneLine);
            }
            sr.Close();
            //结尾
            sw.Write(endTxt);
            sw.Close();

            //半精车数据文件
            MakeSSSFileHE(hcs, firstZ, ref headTxt, ref endTxt);
            sw = new StreamWriter(hcsFileP, false, Encoding.Default);
            sw.Write(headTxt);
            //读取中间数据段,写入文件
            sr = new StreamReader(hcDataPath, Encoding.Default);
            tpOneLine = "";
            while ((tpOneLine = sr.ReadLine()) != null)
            {
                sw.WriteLine(tpOneLine);
            }
            sr.Close();
            //结尾
            sw.Write(endTxt);
            sw.Close();

            //精车数据文件
            MakeSSSFileHE(cs, firstZ, ref headTxt, ref endTxt);
            sw = new StreamWriter(csFileP, false, Encoding.Default);
            sw.Write(headTxt);
            //读取中间数据段,写入文件
            sr = new StreamReader(cDataPath, Encoding.Default);
            tpOneLine = "";
            while ((tpOneLine = sr.ReadLine()) != null)
            {
                sw.WriteLine(tpOneLine);
            }
            sr.Close();
            //结尾
            sw.Write(endTxt);
            sw.Close();
            return true;
        }
        
        public static bool MakeSFNc(CutSegments hcs, string filePath, ref string SFTxt)
        {
            int findTag = filePath.LastIndexOf("\\");
            if (findTag == -1)
            {
                return false;
            }
            string csFileP = filePath.Substring(0, findTag);
            if (!Directory.Exists(csFileP))
            {
                return false;
            }
            //csFileP += ("\\" + hcs.NCFilePath.Replace("-FtsSF", "-FtsFC"));
            string SpdDir = "";
            if (hcs.SpdDir == "逆时针")
            {
                SpdDir = "M04";
            }
            else
            {
                SpdDir = "M03";
            }
            SFTxt = "( Cigit Version : 2.7 Build : 55 )\r\n( CREATED : "
                                + DateTime.Now.DayOfWeek.ToString() + " " + DateTime.Now.ToString() + " )\r\n"//时间日期                               
                                + "( OPERATOR : Cigit Operator )\r\n"
                                + "( SCRIPT: FTS Post.mpyx - MODIFIED: 2016/5/12 )\r\n"
                                + "( Aperture Type : Circle )\r\n"
                                + "( Outer Aperture : " + (hcs.CutXStart * 2).ToString() + "\r\n"
                                + "( X Start and X End : " + hcs.CutXStart.ToString() + " mm, " + hcs.CutXEnd.ToString() + " mm )\r\n"
                                + "( LeadIn and LeadOut Distance, Increment : " + hcs.LeadInDis.ToString() + " mm, " + hcs.LeadOutDis.ToString() + " mm )\r\n"
                                + "( ========== SECTION - COMMANDS ========== )\r\n"
                                + "G94 M64				( FEED RATE IN mm/min & NFTS OFF )\r\n"
                                + "#550 = #550 - #552					( CURRENT CUTTING OFFSET )\r\n"
                                + "G52 Z[#550]							( SET COORDINATE SYSTEM OFFSET )\r\n"
                                + "G01 X" + (hcs.CutXStart + hcs.LeadInDis).ToString("F8") + " F200				( PARKING POSITION - X )\r\n"
                                + "Z" + hcs.RPOfs.ToString() + "						( PARKING POSITION - Z )\r\n"
                                + "\r\n"
                                + "M64         ( FTS ON, NONTRACKING        BIT 2 OFF, BIT 3 ON           )\r\n"
                                + "G04 P1        ( DWELL                                                    )\r\n"
                                + SpdDir + " S[#555]\r\n"
                                + "G01 X" + (hcs.CutXStart + hcs.LeadInDis).ToString("F8") + " F200				( PARKING POSITION - X )\r\n"
                                + "Z" + hcs.RPOfs.ToString("F8") + "						( PARKING POSITION - Z )\r\n"
                                + "( LEAD IN BLOCKS )\r\n"
                                + "X" + (hcs.CutXStart + hcs.LeadInDis).ToString("F8") +　" F200\r\n"
                                + "Z" + hcs.ZDepth.ToString() + "\r\n"
                                + "Y0.0							( SET Y AXIS TO 0 )\r\n"
                                + "M[#507]							( MIST ON )\r\n"
                                + "M65		( TRACKING ON )\r\n"
                                + "( CUTTING BLOCKS )\r\n"
                                + "X" + hcs.CutXStart.ToString("F8") + "Z0.00000000 F[#553]\r\n";
            //中间数据段
            string dataStr = "";
            double XSt = hcs.CutXStart;
            double deltaX = hcs.SurIncre;
            if (hcs.SpdDir == "逆时针")
            {
                XSt = -XSt;
                while (XSt < hcs.CutXEnd && Math.Abs(XSt - hcs.CutXEnd) > 1e-9)
                {
                    dataStr += ("X" + XSt.ToString("F8") + " Z0.00000000\r\n");
                    XSt += deltaX;
                }
                //最后一个点
                XSt = hcs.CutXEnd;
                dataStr += ("X" + XSt.ToString("F8") + " Z0.00000000\r\n");
            }
            else
            {
                deltaX = -deltaX;
                while (XSt > hcs.CutXEnd && Math.Abs(XSt - hcs.CutXEnd) > 1e-9)
                {
                    dataStr += ("X" + XSt.ToString("F8") + " Z0.00000000\r\n");
                    XSt += deltaX;
                }
                //最后一个点
                XSt = hcs.CutXEnd;
                dataStr += ("X" + XSt.ToString("F8") + " Z0.00000000\r\n");
            }
            string endStr =     "( LEAD OUT BLOCKS )\r\n"
                                + "X" + hcs.CutXEnd.ToString("F8") + " Z" + hcs.ZDepth.ToString() + "\r\n"
                                + "M62		( TRACKING OFF )\r\n"
                                + "X" + hcs.CutXEnd.ToString("F8") + " Z" + hcs.ZDepth.ToString() + " F[#553]\r\n"
                                + "G04P.4                   ( DWELL )\r\n"
                                + "M29                      ( MIST OFF )\r\n"
                                + "( PARKING POSITION )\r\n"
                                + "G94 G01 Z" + hcs.RPOfs.ToString("F8") + " F200\r\n"
                                + "X" + (hcs.CutXStart + hcs.LeadOutDis).ToString("F8") + "\r\n"
                                + "( =============== SECTION - FOOTER =================== )\r\n"
                                + "M64\r\n"
                                + "M99 									( RETURN TO MAIN )\r\n";
            //SFTxt += dataStr;
            //SFTxt += endStr;
            //System.IO.FileStream fs = new FileStream(filePath, FileMode.Truncate, FileAccess.ReadWrite);
            //System.IO.FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(filePath, false);
            sw.Write(SFTxt);
            sw.Write(dataStr);
            sw.Write(endStr);
            sw.Close();
            return true;
        }

        public static bool GetZValue(Point pt1, Point pt2, double xValue, ref double zValue)
        {
            zValue = 0;
            //斜率不存在
            if (pt2.X == pt1.X)
            {
                if (xValue != pt1.X)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            //计算斜率
            double k = (pt2.Y - pt1.Y) / (pt2.X - pt1.X);
            zValue = k * (xValue - pt1.X) + pt1.Y;
            return true;
        }
    }
    public class FitFunc
    {
        public static bool FittingCircle(List<Vertex> ptList, ref Vertex circleInfo)
        {
            Vertex resCircle = new Vertex(0, 0, 0);
            if (ptList.Count < 3)
            {
                return false;
            }
            double X1 = 0, X2 = 0, X3 = 0;
            double Y1 = 0, Y2 = 0, Y3 = 0;
            double X1Y1 = 0, X1Y2 = 0, X2Y1 = 0;
            int listLen = ptList.Count;
            for (int i = 0; i < listLen; i++)
            {
                X1 = X1 + ptList[i].x;
                Y1 = Y1 + ptList[i].y;
                X2 = X2 + ptList[i].x * ptList[i].x;
                Y2 = Y2 + ptList[i].y * ptList[i].y;
                X3 = X3 + ptList[i].x * ptList[i].x * ptList[i].x;
                Y3 = Y3 + ptList[i].y * ptList[i].y * ptList[i].y;
                X1Y1 = X1Y1 + ptList[i].x * ptList[i].y;
                X1Y2 = X1Y2 + ptList[i].x * ptList[i].y * ptList[i].y;
                X2Y1 = X2Y1 + ptList[i].x * ptList[i].x * ptList[i].y;
            }
            double C, D, E, G, H, N;
            double a, b, c;
            N = ptList.Count;
            C = N * X2 - X1 * X1;
            D = N * X1Y1 - X1 * Y1;
            E = N * X3 + N * X1Y2 - (X2 + Y2) * X1;
            G = N * Y2 - Y1 * Y1;
            H = N * X2Y1 + N * Y3 - (X2 + Y2) * Y1;
            a = (H * D - E * G) / (C * G - D * D);
            b = (H * C - E * D) / (D * D - G * C);
            c = -(a * X1 + b * Y1 + X2 + Y2) / N;
            circleInfo.x = a / (-2);
            circleInfo.y = b / (-2);
            circleInfo.z = Math.Sqrt(a * a + b * b - 4 * c) / 2;
            return true;
        }
        public static bool FittingCircle(Queue<Vertex> ptList, ref Vertex circleInfo)
        {
            Vertex resCircle = new Vertex(0, 0, 0);
            if (ptList.Count < 3)
            {
                return false;
            }
            double X1 = 0, X2 = 0, X3 = 0;
            double Y1 = 0, Y2 = 0, Y3 = 0;
            double X1Y1 = 0, X1Y2 = 0, X2Y1 = 0;
            int listLen = ptList.Count;
            foreach(Vertex tp in ptList)
            {
                X1 = X1 + tp.x;
                Y1 = Y1 + tp.y;
                X2 = X2 + tp.x * tp.x;
                Y2 = Y2 + tp.y * tp.y;
                X3 = X3 + tp.x * tp.x * tp.x;
                Y3 = Y3 + tp.y * tp.y * tp.y;
                X1Y1 = X1Y1 + tp.x * tp.y;
                X1Y2 = X1Y2 + tp.x * tp.y * tp.y;
                X2Y1 = X2Y1 + tp.x * tp.x * tp.y;
            }
            //for (int i = 0; i < listLen; i++)
            //{
            //    X1 = X1 + ptList[i].x;
            //    Y1 = Y1 + ptList[i].y;
            //    X2 = X2 + ptList[i].x * ptList[i].x;
            //    Y2 = Y2 + ptList[i].y * ptList[i].y;
            //    X3 = X3 + ptList[i].x * ptList[i].x * ptList[i].x;
            //    Y3 = Y3 + ptList[i].y * ptList[i].y * ptList[i].y;
            //    X1Y1 = X1Y1 + ptList[i].x * ptList[i].y;
            //    X1Y2 = X1Y2 + ptList[i].x * ptList[i].y * ptList[i].y;
            //    X2Y1 = X2Y1 + ptList[i].x * ptList[i].x * ptList[i].y;
            //}
            double C, D, E, G, H, N;
            double a, b, c;
            N = ptList.Count;
            C = N * X2 - X1 * X1;
            D = N * X1Y1 - X1 * Y1;
            E = N * X3 + N * X1Y2 - (X2 + Y2) * X1;
            G = N * Y2 - Y1 * Y1;
            H = N * X2Y1 + N * Y3 - (X2 + Y2) * Y1;
            a = (H * D - E * G) / (C * G - D * D);
            b = (H * C - E * D) / (D * D - G * C);
            c = -(a * X1 + b * Y1 + X2 + Y2) / N;
            circleInfo.x = a / (-2);
            circleInfo.y = b / (-2);
            circleInfo.z = Math.Sqrt(a * a + b * b - 4 * c) / 2;
            return true;
        }
    }
    public class portsFunc
    {
        //private SerialPort sp = null;
        public static bool CheckCom(List<string> comList)
        {
            comList.Clear();
            //查找10个串口
            for (int i = 0; i < 15; i++)
            {
                try
                {
                    SerialPort spCheck = new SerialPort("COM" + (i + 1).ToString());
                    spCheck.Open();
                    spCheck.Close();
                    comList.Add("COM" + (i + 1).ToString());
                }
                catch (Exception)
                {
                    continue;
                }
            }
            if (comList.Count != 0)
            {
                return true;                
            }
            return false;
        }
        public static bool GetUSBPort(ref string portName)
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length <= 1)
            {
                return false;
            }
            //string registName = @"\Device\Serial3";
            string registName = @"\Device\Serial2";
            RegistryKey hkml = Registry.LocalMachine;
            //RegistryKey hardware = hkml.OpenSubKey("HARDWARE", true);
            RegistryKey hardware = hkml.OpenSubKey("HARDWARE");
            RegistryKey divicemap = hardware.OpenSubKey("DEVICEMAP");
            RegistryKey serialcomM = divicemap.OpenSubKey("SERIALCOMM");
            try
            {
                portName = serialcomM.GetValue(registName).ToString();

            }
            catch (Exception)
            {
                return false;
            }
            if (portName.IndexOf("COM") != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool OpenCom(SerialPort sp)
        {
            try
            {
                //SerialPort sp = new SerialPort(comName);
                sp.Open();
                return true;
            }
            catch (Exception err)
            {
                string errType = err.GetType().ToString();
                switch (errType)
                {
                    case "System.UnauthorizedAccessException":
                        MessageBox.Show("端口已被别的应用程序所占", "错误");
                        break;
                    case "System.ArgumentOutOfRangeException":
                        MessageBox.Show("ArgumentOutOfRangeException", "错误");
                        break;
                    case "System.ArgumentException":
                        MessageBox.Show("ArgumentException", "错误");
                        break;
                    case "System.IO.IOException":
                        MessageBox.Show("该端口无效", "错误");
                        break;
                    case "System.InvalidOperationException":
                        MessageBox.Show("InvalidOperationException", "错误");
                        break;
                    default:
                        break;
                }
                return false;
            }
        }
        public static bool GetComData(string comName, byte[] getData)
        {
            SerialPort sp = new SerialPort(comName);
            bool test = sp.IsOpen;
            if(sp.IsOpen)
            {
                getData = new byte[sp.BytesToRead];
                sp.Read(getData, 0, getData.Length);//读取所接收到的数据
                return true;
            }
            else
            {
                try
                {
                    SerialPort port =new SerialPort(comName);
                    port.Open();
                    port.Close();
                    MessageBox.Show("串口存在但是没打开");
                }
                catch (Exception)
                {
                    MessageBox.Show("串口不存在");
                }
                return false;
            }
        }
    }
    public class DBInfo
    {
        private string _DBServer;//数据库所在服务器名称
        private string _UserId;//用户ID
        private string _Psw;//用户密码
        private string _DBName;//数据库
        private string _TBName;
        public DBInfo(string dbServer, string user, string psw, string dbName, string dbTableName)
        {
            _DBServer = dbServer;
            _UserId = user;
            _Psw = psw;
            _DBName = dbName;
            _TBName = dbTableName;
        }
        public DBInfo()
        {
            _DBServer = "";
            _UserId = "";
            _Psw = "";
            _DBName = "";
            _TBName = "";
        }
        public string DBServer
        {
            get
            {
                return _DBServer;
            }
            set
            {
                if (value != this._DBServer)
                {
                    this._DBServer = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string UserId
        {
            get
            {
                return _UserId;
            }
            set
            {
                if (value != this._UserId)
                {
                    this._UserId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Psw
        {
            get
            {
                return _Psw;
            }
            set
            {
                if (value != this._Psw)
                {
                    this._Psw = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string DBName
        {
            get
            {
                return _DBName;
            }
            set
            {
                if (value != this._DBName)
                {
                    this._DBName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string TBName
        {
            get
            {
                return _TBName;
            }
            set
            {
                if (value != this._TBName)
                {
                    this._TBName = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class DBFunc
    {
        //初始化连接数据库字符串
        public static string InitConString(DBInfo dInfo)
        {
            string constr = "server=" + dInfo.DBServer + ";User Id=" + dInfo.UserId + ";password=" + dInfo.Psw + ";Database=" + dInfo.DBName;
            return constr;
        }
        public static bool AddFCToDB(DBInfo dinfo, CutSegments hsSeg, CutSegments sSeg)
        {
            //连接数据库
            string constr = "server=" + dinfo.DBServer + ";User Id=" + dinfo.UserId + ";password=" + dinfo.Psw + ";Database=" + dinfo.DBName;
            MySqlConnection mycon = new MySqlConnection(constr);
            mycon.Open();
            //先查询到所有字段名
            //string sqlstr = "insert into " + dinfo.TBName + "";
            string sqlstr = "select column_name from information_schema.columns where table_name='" + dinfo.TBName + "' and COLUMN_NAME not like '%id%' and COLUMN_NAME not like '%date%';";
            MySqlCommand cmd = new MySqlCommand(sqlstr, mycon);
            List<string> names = new List<string>();
            try 
            {	        
                //调用 MySqlCommand  的 ExecuteReader 方法
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int len = reader.FieldCount;
                    names.Add(reader.GetString(0));
                    //string s2 = reader.GetString(1);
                    //Console.WriteLine(reader.GetInt32(0) + "," + myreader.GetString(1) + "," + myreader.GetString(2));
                    Trace.WriteLine("1");
                }
                //将DataReader关闭
                reader.Close();
                Trace.WriteLine("2");
            }
            catch
            {
                //关闭连接，抛出异常
                mycon.Close();
                throw;
            }
            //写入参数
            //得到所有字段名称串起来
            string colAll = "";
            for (int i = 0; i < names.Count; i++)
            {
                colAll += (names[i] + ",");
            }
            colAll = colAll.Substring(0, colAll.Length - 1);
            //初始化本次插入操作信息
            string initStr = "'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " 新记录', '说明:', '熊欣',"
                                + "'" + hsSeg.WorkCoord + "','" + hsSeg.CutCpst + "','" + hsSeg.SpdDir + "','" + hsSeg.JetNo //此行是半精车和精车通用的属性
                                 + "','" + hsSeg.SpdSpeed + "','" + hsSeg.FeedRate + "','" + hsSeg.SurIncre //半精车单独属性
                                 + "','" + hsSeg.CutXStart + "','" + hsSeg.CutXEnd + "','" + hsSeg.LeadInDis + "','" + hsSeg.LeadOutDis + "','" + hsSeg.RPOfs + "','" + hsSeg.RPFdr // 又是通用属性
                                  + "','" + hsSeg.TotalLoops + "','" + hsSeg.DepthOfCut//单独属性
                                   + "','" + hsSeg.ZDepth + "','" + hsSeg.CutR//共同属性
                                    + "','" + hsSeg.NCFilePath//单独属性
                                    + "','" + sSeg.SpdSpeed + "','" + sSeg.FeedRate + "','" + sSeg.SurIncre + "','" + sSeg.TotalLoops
                                    + "','" + sSeg.DepthOfCut + "','" + sSeg.NCFilePath + "'";
            sqlstr = "insert into " + dinfo.TBName + "(" + colAll + ") VALUES (" + initStr + ")";
            //cmd.Parameters.Clear();
            //cmd.CommandText = sqlstr;
            //cmd.Connection = mycon;
            MySqlCommand cmd1 = new MySqlCommand(sqlstr, mycon);
            int rowsReturned = cmd1.ExecuteNonQuery();
            mycon.Close();
            return true;
        }
    /// <summary>
    /// 准备执行一个命令
    /// </summary>
    /// <param name="cmd">sql命令</param>
    /// <param name="conn">OleDb连接</param>
    /// <param name="trans">OleDb事务</param>
    /// <param name="cmdType">命令类型例如 存储过程或者文本</param>
    /// <param name="cmdText">命令文本,例如:Select * from Products</param>
    /// <param name="cmdParms">执行命令的参数</param>
    private static void PrepareCommand(MySqlCommand cmd, MySqlConnection conn, MySqlTransaction trans, CommandType cmdType, string cmdText, MySqlParameter[] cmdParms)
    {

        if (conn.State != ConnectionState.Open)
            conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = cmdText;

        if (trans != null)
            cmd.Transaction = trans;

        cmd.CommandType = cmdType;

        if (cmdParms != null)
        {
            foreach (MySqlParameter parm in cmdParms)
                cmd.Parameters.Add(parm);
        }
    }

}

    [Serializable]
    public class Vertex
    {
        public double x { get; set; }
        public double y { get; set; }
        public double z { get; set; }

        public Vertex()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public Vertex(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Vertex(double _x, double _y)
        {
            x = _x;
            y = _y;
            z = 0;
        }
        //public static bool operator == (Vertex v1, Vertex v2)
        //{
        //    return (v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);
        //}
        //public static bool operator != (Vertex v1, Vertex v2)
        //{
        //    return !(v1.x == v2.x && v1.y == v2.y && v1.z == v2.z);
        //}

    }

    public class CsvInfo
    {
        public int ptNum { get; set; }
        public double xResolution { get; set; }
        public double yResolution { get; set; }
        public double zResolution { get; set; }
        public double zMin { get; set; }
        public double zMax { get; set; }
        public Vertex midPt { get; set; }
        public CsvInfo()
        {
            ptNum = 0;
            xResolution = 0;
            yResolution = 0;
            zResolution = 0;
            zMin = 0;
            zMax = 0;
            midPt = new Vertex();
        }
        public CsvInfo(int _ptNum, double _xRsl, double _yRsl, double _zRsl, double _zMin, double _zMax, Vertex _midPt)
        {
            ptNum = _ptNum;
            xResolution = _xRsl;
            yResolution = _yRsl;
            zResolution = _zRsl;
            midPt = new Vertex();
            midPt.x = _midPt.x;
            midPt.y = _midPt.y;
            midPt.z = _midPt.z;
            zMin = _zMin;
            zMax = _zMax;
        }
    }

    public sealed class Arrow : Shape
    {
        #region Dependency Properties

        public static readonly DependencyProperty X1Property = DependencyProperty.Register("X1", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y1Property = DependencyProperty.Register("Y1", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty X2Property = DependencyProperty.Register("X2", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty Y2Property = DependencyProperty.Register("Y2", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadWidthProperty = DependencyProperty.Register("HeadWidth", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
        public static readonly DependencyProperty HeadHeightProperty = DependencyProperty.Register("HeadHeight", typeof(double), typeof(Arrow), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

        #endregion

        #region CLR Properties

        [TypeConverter(typeof(LengthConverter))]
        public double X1
        {
            get { return (double)base.GetValue(X1Property); }
            set { base.SetValue(X1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y1
        {
            get { return (double)base.GetValue(Y1Property); }
            set { base.SetValue(Y1Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double X2
        {
            get { return (double)base.GetValue(X2Property); }
            set { base.SetValue(X2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double Y2
        {
            get { return (double)base.GetValue(Y2Property); }
            set { base.SetValue(Y2Property, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadWidth
        {
            get { return (double)base.GetValue(HeadWidthProperty); }
            set { base.SetValue(HeadWidthProperty, value); }
        }

        [TypeConverter(typeof(LengthConverter))]
        public double HeadHeight
        {
            get { return (double)base.GetValue(HeadHeightProperty); }
            set { base.SetValue(HeadHeightProperty, value); }
        }

        #endregion

        #region Overrides

        protected override Geometry DefiningGeometry
        {
            get
            {
                // Create a StreamGeometry for describing the shape
                StreamGeometry geometry = new StreamGeometry();
                geometry.FillRule = FillRule.EvenOdd;

                using (StreamGeometryContext context = geometry.Open())
                {
                    InternalDrawArrowGeometry(context);
                }

                // Freeze the geometry for performance benefits
                geometry.Freeze();

                return geometry;
            }
        }

        #endregion

        #region Privates

        private void InternalDrawArrowGeometry(StreamGeometryContext context)
        {
            double theta = Math.Atan2(Y1 - Y2, X1 - X2);
            double sint = Math.Sin(theta);
            double cost = Math.Cos(theta);

            Point pt1 = new Point(X1, this.Y1);
            Point pt2 = new Point(X2, this.Y2);

            Point pt3 = new Point(
                X2 + (HeadWidth * cost - HeadHeight * sint),
                Y2 + (HeadWidth * sint + HeadHeight * cost));

            Point pt4 = new Point(
                X2 + (HeadWidth * cost + HeadHeight * sint),
                Y2 - (HeadHeight * cost - HeadWidth * sint));

            context.BeginFigure(pt1, true, false);
            context.LineTo(pt2, true, true);
            context.LineTo(pt3, true, true);
            context.LineTo(pt2, true, true);
            context.LineTo(pt4, true, true);
        }

        #endregion
    }
    public class fnlSegments : INotifyPropertyChanged
    {
        private string _WorkCoord;//工作坐标系
        private string _CutCpst;//刀补号
        private string _SpdDir;//主轴旋转方向
        private string _JetNo;//喷气号
        private double _PartSize;//工件半径
        private int _TotalLoops;//循环次数
        private double _CutDepth;//切深
        private double _FeedRate;//进给速度
        private double _SpindleSpd;//主轴转速
        private double _LeadInX;//Xleadin
        private double _LeadInZ;//Zleadin
        private double _LeadOutZ;//Zleadout
        private double _HZ;//等等Z
        private string _NCFilePath;
        
        public double PartSize
        {
            get
            {
                return _PartSize;
            }
            set
            {
                if (value != this._PartSize)
                {
                    this._PartSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public int TotalLoops
        {
            get
            {
                return _TotalLoops;
            }
            set
            {
                if (value != this._TotalLoops)
                {
                    this._TotalLoops = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double SpindleSpd
        {
            get
            {
                return _SpindleSpd;
            }
            set
            {
                if (value != this._SpindleSpd)
                {
                    this._SpindleSpd = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double FeedRate
        {
            get
            {
                return _FeedRate;
            }
            set
            {
                if (value != this._FeedRate)
                {
                    this._FeedRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double CutDepth
        {
            get
            {
                return _CutDepth;
            }
            set
            {
                if (value != this._CutDepth)
                {
                    this._CutDepth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string WorkCoord
        {
            get
            {
                return _WorkCoord;
            }
            set
            {
                if (value != this._WorkCoord)
                {
                    this._WorkCoord = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CutCpst
        {
            get
            {
                return _CutCpst;
            }
            set
            {
                if (value != this._CutCpst)
                {
                    this._CutCpst = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SpdDir
        {
            get
            {
                return _SpdDir;
            }
            set
            {
                if (value != this._SpdDir)
                {
                    this._SpdDir = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string JetNo
        {
            get
            {
                return _JetNo;
            }
            set
            {
                if (value != this._JetNo)
                {
                    this._JetNo = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double LeadInX
        {
            get
            {
                return _LeadInX;
            }
            set
            {
                if (value != this._LeadInX)
                {
                    this._LeadInX = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double LeadInZ
        {
            get
            {
                return _LeadInZ;
            }
            set
            {
                if (value != this._LeadInZ)
                {
                    this._LeadInZ = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double LeadOutZ
        {
            get
            {
                return _LeadOutZ;
            }
            set
            {
                if (value != this._LeadOutZ)
                {
                    this._LeadOutZ = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        public double HZ
        {
            get
            {
                return _HZ;
            }
            set
            {
                if (value != this._HZ)
                {
                    this._HZ = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string NCFilePath
        {
            get
            {
                return _NCFilePath;
            }
            set
            {
                if (value != this._NCFilePath)
                {
                    this._NCFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public fnlSegments()
        {
            _WorkCoord = "G59";
            _CutCpst = "T0101";
            _SpdDir = "顺时针";
            _JetNo = "M26";
            _PartSize = 40.0;
            _TotalLoops = 1;
            _CutDepth = 0.0;
            _FeedRate = 10.0;
            _SpindleSpd = 2000.0;
            _LeadInX = 1.0;
            _LeadInZ = 5.0;
            _LeadOutZ = 0.5;
            _HZ = 0.0043999;
            _NCFilePath = "Untitled.NC";
        }
        public fnlSegments(string _workCoord, string _cutCpst, string _spdDir, string _jetNo,
                            double _portSize, int _totalLoops, double _depthOfCut, double _feedRate, double _spindleSpd, double _leadInX, double _leadInZ,
                            double _leadOutZ, double _hs, string _ncFilePath)
        {
            _WorkCoord = _workCoord;
            _CutCpst = _cutCpst;
            _SpdDir = _spdDir;
            _JetNo = _jetNo;
            _PartSize = _portSize;
            _TotalLoops = _totalLoops;
            _CutDepth = _depthOfCut;
            _FeedRate = _feedRate;
            _SpindleSpd = _spindleSpd;
            _LeadInX = _leadInX;
            _LeadInZ = _leadInZ;
            _LeadOutZ = _leadOutZ;
            _HZ = _hs;
            _NCFilePath = _ncFilePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
 
    }
    public class CutSegments : INotifyPropertyChanged
    {
        private string _WorkCoord;
        private string _CutCpst;
        private string _SpdDir;
        private string _JetNo;
        private int _SpdSpeed;
        private double _FeedRate;
        private double _SurIncre;
        private double _CutXStart;
        private double _CutXEnd;
        private double _LeadInDis;
        private double _LeadOutDis;
        private int _RPOfs;
        private int _RPFdr;
        private int _TotalLoops;
        private double _DepthOfCut;
        private double _ZDepth;
        private double _CutR;
        private string _NCFilePath;
        public string WorkCoord
        {
            get
            {
                return _WorkCoord;
            }
            set
            {
                if (value != this._WorkCoord)
                {
                    this._WorkCoord = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CutCpst
        {
            get
            {
                return _CutCpst;
            }
            set
            {
                if (value != this._CutCpst)
                {
                    this._CutCpst = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SpdDir
        {
            get
            {
                return _SpdDir;
            }
            set
            {
                if (value != this._SpdDir)
                {
                    this._SpdDir = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string JetNo
        {
            get
            {
                return _JetNo;
            }
            set
            {
                if (value != this._JetNo)
                {
                    this._JetNo = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int SpdSpeed
        {
            get
            {
                return _SpdSpeed;
            }
            set
            {
                if (value != this._SpdSpeed)
                {
                    this._SpdSpeed = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double FeedRate
        {
            get
            {
                return _FeedRate;
            }
            set
            {
                if (value != this._FeedRate)
                {
                    this._FeedRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double SurIncre
        {
            get
            {
                return _SurIncre;
            }
            set
            {
                if (value != this._SurIncre)
                {
                    this._SurIncre = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double CutXStart
        {
            get
            {
                return _CutXStart;
            }
            set
            {
                if (value != this._CutXStart)
                {
                    this._CutXStart = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double CutXEnd
        {
            get
            {
                return _CutXEnd;
            }
            set
            {
                if (value != this._CutXEnd)
                {
                    this._CutXEnd = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double LeadInDis
        {
            get
            {
                return _LeadInDis;
            }
            set
            {
                if (value != this._LeadInDis)
                {
                    this._LeadInDis = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double LeadOutDis
        {
            get
            {
                return _LeadOutDis;
            }
            set
            {
                if (value != this._LeadOutDis)
                {
                    this._LeadOutDis = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int RPOfs
        {
            get
            {
                return _RPOfs;
            }
            set
            {
                if (value != this._RPOfs)
                {
                    this._RPOfs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int RPFdr
        {
            get
            {
                return _RPFdr;
            }
            set
            {
                if (value != this._RPFdr)
                {
                    this._RPFdr = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int TotalLoops
        {
            get
            {
                return _TotalLoops;
            }
            set
            {
                if (value != this._TotalLoops)
                {
                    this._TotalLoops = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double DepthOfCut
        {
            get
            {
                return _DepthOfCut;
            }
            set
            {
                if (value != this._DepthOfCut)
                {
                    this._DepthOfCut = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double ZDepth
        {
            get
            {
                return _ZDepth;
            }
            set
            {
                if (value != this._ZDepth)
                {
                    this._ZDepth = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double CutR
        {
            get
            {
                return _CutR;
            }
            set
            {
                if (value != this._CutR)
                {
                    this._CutR = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string NCFilePath
        {
            get
            {
                return _NCFilePath;
            }
            set
            {
                if (value != this._NCFilePath)
                {
                    this._NCFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CutSegments()
        {
            _WorkCoord = "G59";
            //_WorkCoord = 0;
            _CutCpst = "T0101";
            _SpdDir = "顺时针";
            _JetNo = "M26";
            _SpdSpeed = 2000;
            _FeedRate = 10.0;
            _SurIncre = 0.005;
            _CutXStart = 25;
            _CutXEnd = 0;
            _LeadInDis = 0.1;
            _LeadOutDis = 0.1;
            _RPOfs = 9;
            _RPFdr = 200;
            _TotalLoops = 1;
            _DepthOfCut = 0;
            _ZDepth = 0.25;
            _CutR = 0.4;
            _NCFilePath = "Untitled-FtsSF.NC";
        }
        public CutSegments(string _workCoord, string _cutCpst, string _spdDir, string _jetNo,
                            int _spdSpeed, double _feedRate, double _surIncre, double _cutXStart, double _cutXEnd,
                            double _leadInDis, double _leadOutDis, int _rpofs, int _rpfdr, int _totalLoops, double _depthOfCut, double _zdepth, double _cutr, string _ncFilePath)
        {
            _WorkCoord = _workCoord;
            _CutCpst = _cutCpst;
            _SpdDir = _spdDir;
            _JetNo = _jetNo;
            _SpdSpeed = _spdSpeed;
            _FeedRate = _feedRate;
            _SurIncre = _surIncre;
            _CutXStart = _cutXStart;
            _CutXEnd = _cutXEnd;
            _LeadInDis = _leadInDis;
            _LeadOutDis = _leadOutDis;
            _RPOfs = _rpofs;
            _RPFdr = _rpfdr;
            _TotalLoops = _totalLoops;
            _DepthOfCut = _depthOfCut;
            _ZDepth = _zdepth;
            _CutR = _cutr;
            _NCFilePath = _ncFilePath;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
    public class SSSCutSegments : INotifyPropertyChanged
    {
        private string _WorkCoord;//主轴旋转方向
        private string _CutCpst;//刀补号
        private string _JetNo;//喷气号
        private double _CutXStart;//起始位置
        private int _RPOfs;//安全距离
        private int _TotalLoops;//加工循环次数
        private double _DepthOfCut;//每次切深
        private double _CutTime;//每次加工时间
        private string _NCFilePath;//NC文件
        public string WorkCoord
        {
            get
            {
                return _WorkCoord;
            }
            set
            {
                if (value != this._WorkCoord)
                {
                    this._WorkCoord = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CutCpst
        {
            get
            {
                return _CutCpst;
            }
            set
            {
                if (value != this._CutCpst)
                {
                    this._CutCpst = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string JetNo
        {
            get
            {
                return _JetNo;
            }
            set
            {
                if (value != this._JetNo)
                {
                    this._JetNo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double CutXStart
        {
            get
            {
                return _CutXStart;
            }
            set
            {
                if (value != this._CutXStart)
                {
                    this._CutXStart = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int RPOfs
        {
            get
            {
                return _RPOfs;
            }
            set
            {
                if (value != this._RPOfs)
                {
                    this._RPOfs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int TotalLoops
        {
            get
            {
                return _TotalLoops;
            }
            set
            {
                if (value != this._TotalLoops)
                {
                    this._TotalLoops = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double DepthOfCut
        {
            get
            {
                return _DepthOfCut;
            }
            set
            {
                if (value != this._DepthOfCut)
                {
                    this._DepthOfCut = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double CutTime
        {
            get
            {
                return _CutTime;
            }
            set
            {
                if (value != this._CutTime)
                {
                    this._CutTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string NCFilePath
        {
            get
            {
                return _NCFilePath;
            }
            set
            {
                if (value != this._NCFilePath)
                {
                    this._NCFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public SSSCutSegments()
        {
            _WorkCoord = "G59";
            _CutCpst = "T0101";
            _JetNo = "M26";
            _CutXStart = 25;
            _RPOfs = 9;
            _TotalLoops = 1;
            _DepthOfCut = 0.1;
            _CutTime = 0.001;
            _NCFilePath = "Untitled-SSSSF.NC";
        }
        public SSSCutSegments(string _workCoord, string _cutCpst, string _jetNo, double _cutXStart, 
                               int _rpofs, int _totalLoops, double _depthOfCut, double _cutTime, string _ncFilePath)
        {
            _WorkCoord = _workCoord;
            _CutCpst = _cutCpst;
            _JetNo = _jetNo;
            _CutXStart = _cutXStart;
            _RPOfs = _rpofs;
            _TotalLoops = _totalLoops;
            _DepthOfCut = _depthOfCut;
            _CutTime = _cutTime;
            _NCFilePath = _ncFilePath;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
    public class RecCutSegments : INotifyPropertyChanged
    {
        private string _WorkCoord;//工作坐标系
        private string _CutCpst;//刀补号
        private string _JetNo;//喷气号
        private double _RPOfs;//进刀安全距离
        private int _TotalLoops;//加工循环次数
        private double _DepthOfCut;//每次切深
        //private double _CutTime;//每次加工时间
        private string _NCFilePath;//NC文件
        public string WorkCoord
        {
            get
            {
                return _WorkCoord;
            }
            set
            {
                if (value != this._WorkCoord)
                {
                    this._WorkCoord = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string CutCpst
        {
            get
            {
                return _CutCpst;
            }
            set
            {
                if (value != this._CutCpst)
                {
                    this._CutCpst = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string JetNo
        {
            get
            {
                return _JetNo;
            }
            set
            {
                if (value != this._JetNo)
                {
                    this._JetNo = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double RPOfs
        {
            get
            {
                return _RPOfs;
            }
            set
            {
                if (value != this._RPOfs)
                {
                    this._RPOfs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public int TotalLoops
        {
            get
            {
                return _TotalLoops;
            }
            set
            {
                if (value != this._TotalLoops)
                {
                    this._TotalLoops = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double DepthOfCut
        {
            get
            {
                return _DepthOfCut;
            }
            set
            {
                if (value != this._DepthOfCut)
                {
                    this._DepthOfCut = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string NCFilePath
        {
            get
            {
                return _NCFilePath;
            }
            set
            {
                if (value != this._NCFilePath)
                {
                    this._NCFilePath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public RecCutSegments()
        {
            _WorkCoord = "G59";
            _CutCpst = "T0101";
            _JetNo = "M26";
            _RPOfs = 2;
            _TotalLoops = 1;
            _DepthOfCut = 0.1;
            //_CutTime = 0.001;
            _NCFilePath = "Untitled-RecSF.NC";
        }
        public RecCutSegments(string _workCoord, string _cutCpst, string _jetNo,
                               double _rpofs, int _totalLoops, double _depthOfCut, string _ncFilePath)
        {
            _WorkCoord = _workCoord;
            _CutCpst = _cutCpst;
            _JetNo = _jetNo;
            _RPOfs = _rpofs;
            //_CutTime = _cutTime;
            _TotalLoops = _totalLoops;
            _DepthOfCut = _depthOfCut;
            _NCFilePath = _ncFilePath;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
    public class NetFunc
    {
        public static bool IsStrIpAddress(string ipStr)
        {
            IPAddress ip;
            if (IPAddress.TryParse(ipStr, out ip))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
