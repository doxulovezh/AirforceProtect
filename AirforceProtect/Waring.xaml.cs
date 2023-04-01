using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AirforceProtect
{
    /// <summary>
    /// Waring.xaml 的交互逻辑
    /// </summary>
    public partial class Waring : Window
    {
        public string info = "信息提示";
        public string name = "";
        public string path = "";
        public bool Find = false;
        public Waring()
        {
            InitializeComponent();

        }
        //var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
        //    this.Left = desktopWorkingArea.Right - this.Width;
        //    this.Top = desktopWorkingArea.Bottom - this.Height;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom + this.Height;
            this.Visibility = Visibility.Hidden;
        }
        private void image_Copy_Loaded(object sender, RoutedEventArgs e)//开始走进度条
        {
            //Thread t = new Thread(new ThreadStart(StartCheck));
            //t.Start();
            
        }
        public void 选择()
        {
            Thread t = new Thread(new ThreadStart(StartCheck));
            t.Start();
        }
        private void StartCheck()
        {
            Dispatcher.BeginInvoke(new Action(() =>  this.Visibility = Visibility.Visible));
            Find = true;
            //调 主界面
            int time = 15;
            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(1000);
                time--;
                Dispatcher.BeginInvoke(new Action(() => button_Copy1.Content="忽略("+ time+ ")"));
            }
            Thread.Sleep(100);
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            Dispatcher.BeginInvoke(new Action(() => this.Left = desktopWorkingArea.Right - this.Width));
            Dispatcher.BeginInvoke(new Action(() => this.Top = desktopWorkingArea.Bottom + this.Height));
            Dispatcher.BeginInvoke(new Action(() => this.Visibility = Visibility.Hidden));
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom + this.Height;
            this.Visibility = Visibility.Hidden;
            //恢复文件
            FileInfo fi2 = new FileInfo(path);
            string Filename = fi2.Name;
            
            try
            {
                SetFileAttributes(System.Environment.CurrentDirectory + "\\BackData\\" + Filename + "XH");//解锁
                FileStream fs = new FileStream(System.Environment.CurrentDirectory + "\\BackData\\" + Filename + "XH", FileMode.Open, FileAccess.Read, FileShare.Read);
                byte[] buff = new byte[fs.Length];
                fs.Read(buff, 0, buff.Length);
                fs.Close();
                Find = true;
                MainW.ShowWaring = false;
                FileStream fs2 = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs2.Write(buff, 0, buff.Length);
                fs2.Close();
                MainW.ShowWaring = false;
                SetFileAttributes(path);//锁定
                Console.WriteLine("阻止成功");
                Thread.Sleep(1000);
                MainW.ShowWaring = false;
                Find = false;
            }
            catch (Exception ex)
            {
                FileInfo fi = new FileInfo(path);
                if (fi.Attributes == FileAttributes.ReadOnly)
                {
                    MessageBox.Show("已阻止成功！");
                }
                else
                {
                    MessageBox.Show("阻止失败！" + ex.Message + "建议立即扫描文件！"+ Filename + "%%%"+path);
                }
               
                Find = false;
            }

        }
        private string getFileNameStr(string name)
        {
            char[] cc = name.ToCharArray();
            char[] c2 =new char[1000];
            int index = 0;
            for(int i = cc.Length - 1; i >= 0; i--)
            {
                if (cc[i] != '\\')
                {
                    c2[index]= cc[i];
                    index++;
                }
            }
            string ss = "";
            for(int j = index; j >=0; j--)
            {
                ss += c2[j];
            }
            return ss;
        }
        public void SetFileAttributes(string aFilePath)//改变文件属性
        {
            try
            {
                string sss = File.GetAttributes(aFilePath).ToString();
                File.SetAttributes(aFilePath, FileAttributes.ReadOnly);
            }
            catch
            {

            }
        }
        public void SetFileAttributes_no(string aFilePath)//改变文件属性
        {
            try
            {
                string sss = File.GetAttributes(aFilePath).ToString();
                File.SetAttributes(aFilePath, FileAttributes.Normal);
            }
            catch
            {

            }

        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom + this.Height;
            this.Visibility = Visibility.Hidden;
        }

        private void button_Copy_Click_1(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom + this.Height;
            this.Visibility = Visibility.Hidden;
        }
    }
}
