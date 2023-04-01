using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Management;
using Microsoft.Win32;

namespace AirforceProtect
{


    /// <summary>
    /// MainW.xaml 的交互逻辑
    /// </summary>
    public partial class MainW : Window
    {
        public static bool ShowWaring = false;
        public static string ShowWaringInfor = "";
        public static string ShowWaringName = "";
        public static string ShowWaringPath = "";
        private NotifyIcon notifyIcon;
        public List<路径data> LJs = new List<路径data>();
        public List<FileMD5> FilelistMD5 = new List<FileMD5>();
        public List<FileMD5> FileScanMD5 = new List<FileMD5>();
        public List<FileMD5> FileScanMD5Difrent = new List<FileMD5>();
        public List<FileMD5> FileScanMD5Add = new List<FileMD5>();
        public List<FileMD5> FileScanMD5Lost = new List<FileMD5>();
        BinaryFormatter bf = new BinaryFormatter();
        public string ExePath;
        bool protectexe = false;
        bool protectdll = false;
        bool protectxml = false;
        bool protectconfig = false;
        bool protectOnlyRead = false;
        bool protectDriver = false;
        bool 温度监控提示 = false;
        SetData st = new SetData();
        FileMD5[] FileGrid;
        //扫描变量
        int Scannum = 0;
        int 新增num = 0;
        int 缺失num = 0;
        int 存在差异num = 0;
        int ThreadNum = 0;
        List<Thread> pool = new List<Thread>();
        //网络连接
        public List<NetConns> nets = new List<NetConns>();
        Waring w = new Waring();

        public MainW()
        {
            try
            {
                InitializeComponent();
                this.notifyIcon = new NotifyIcon();
                this.notifyIcon.BalloonTipText = "AP系统监控中......";
                this.notifyIcon.ShowBalloonTip(2000);
                this.notifyIcon.Text = "AP系统监控中......";
                this.notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.Windows.Forms.Application.ExecutablePath);
                this.notifyIcon.Visible = false;
                //打开菜单项
                System.Windows.Forms.MenuItem open = new System.Windows.Forms.MenuItem("Open");
                open.Click += new EventHandler(Show);
                //退出菜单项
                System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("Exit");
                exit.Click += new EventHandler(Close);
                //关联托盘控件
                System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { open, exit };
                notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

                this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler((o, e) =>
                {
                    if (e.Button == MouseButtons.Left) this.Show(o, e);
                });
                ExePath = System.Environment.CurrentDirectory;

                FileStream fs = new FileStream(ExePath + "\\SetData.dx", FileMode.Open);
                st = (SetData)bf.Deserialize(fs);
                fs.Flush();
                fs.Close();

                protectexe = st.protectexe;
                protectdll = st.protectdll; ;
                protectxml = st.protectxml;
                protectconfig = st.protectconfig;
                protectOnlyRead = st.protectOnlyRead;
                protectDriver = st.protectDriver;


                FileStream fs2 = new FileStream(ExePath + "\\PathData.dx", FileMode.Open);
                LJs = (List<路径data>)bf.Deserialize(fs2);
                fs2.Flush();
                fs2.Close();
                try
                {
                    if (protectDriver)
                    {
                        //实时 文件监控
                        FileMonitor flm = new FileMonitor();
                        for (int i = 0; i < LJs.Count; i++)
                        {
                            flm.WatcherStrat(LJs[i].文件夹路径, "*.exe");
                            flm.WatcherStrat(LJs[i].文件夹路径, "*.dll");
                        }

                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message);
                }
                //警告提示
                w.info = ShowWaringInfor;
                w.Show();
                //建立监听线程
                Thread t = new Thread(new ThreadStart(Twaring));
                t.Start();
            }
            catch(Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }
        }
        private void Twaring()
        {
            while (true)
            {
                Thread.Sleep(500);
                if (ShowWaring)
                {

                    信息提示Show(ShowWaringInfor);
                    w.name = ShowWaringName;
                    w.path = ShowWaringPath;
                    Dispatcher.BeginInvoke(new Action(() => w.Test.Text = ShowWaringInfor));
                    var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
                    Dispatcher.BeginInvoke(new Action(() => w.Left = desktopWorkingArea.Right - w.Width));
                    Dispatcher.BeginInvoke(new Action(() => w.Top = desktopWorkingArea.Bottom - w.Height));
                    ShowWaring = false;
                    Dispatcher.BeginInvoke(new Action(() => w.选择()));
                    Dispatcher.BeginInvoke(new Action(() => 信息提示Show(ShowWaringInfor)));


                }
            }
        }
        private void Show(object sender, EventArgs e)
        {
            this.Visibility = System.Windows.Visibility.Visible;
            this.ShowInTaskbar = true;
            this.Activate();
        }

        private void Hide(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.Visibility = System.Windows.Visibility.Hidden;
        }

        private void Close(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
            System.Environment.Exit(0);
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            this.notifyIcon.Visible = true;
        }

        private void image_Copy_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)//图标鼠标进入
        {
            ((Image)sender).Height += 4;
            ((Image)sender).Width += 4;
        }

        private void image_Copy_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)//图标鼠标出
        {
            ((Image)sender).Height -= 4;
            ((Image)sender).Width -= 4;
        }

        private void image_Copy_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//文件扫描 图标鼠标点击
        {
            pad文件扫描.Visibility = Visibility;
            pad文件扫描.Focus();
            //进行文件扫描
            //检查是否设置了需要保护的文件夹

        }

        private void image_Copy2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//策略设置
        {
            pad策略设置.Visibility = Visibility;
            pad策略设置.Focus();
            set();
            dataGrid.ItemsSource = LJs;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)//选择保护的文件夹路径
        {
            string sPath = "";
            FolderBrowserDialog folder = new FolderBrowserDialog();
            folder.Description = "选择文件所在文件夹目录";  //定义在对话框上显示的文本
            if (folder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                sPath = folder.SelectedPath;
                path.Text = sPath;
                信息提示Show("选择文件夹路径成功！");
            }
            else
            {

            }

            //tccData Lc = new tccData();//特征向量
            //Lc.name = "坐忘";
            //List<int> vvv = new List<int>();
            //vvv.Add(2);
            //vvv.Add(25);
            //Lc.vctor = vvv;
            //LVctorData.Add(Lc);
            //FileStream fs = new FileStream(@ExePath + "//TZ.dll", FileMode.Create);
            //bf.Serialize(fs, LVctorData);
            //fs.Close();
            //OpenFileDialog opf = new OpenFileDialog();
            //opf.Title = "选择保护文件夹路径";

            //DialogResult rc = opf.ShowDialog();


        }
        private void 信息提示Show(string inf)
        {
            Thread t = new Thread(new ParameterizedThreadStart(信息提示));
            t.Start("" + inf);
        }
        private void 信息提示(object obj)
        {
            Dispatcher.BeginInvoke(new Action(() => infor.Content = "" + (string)obj));
            Thread.Sleep(100);
            Dispatcher.BeginInvoke(new Action(() => cav_infor.Margin = new Thickness(450, 204, 0, 0)));
            Thread.Sleep(100);
            Dispatcher.BeginInvoke(new Action(() => cav_infor.Opacity = 90));
            Thread.Sleep(800);
            for (int i = 0; i < 100; i++)
            {

                Dispatcher.BeginInvoke(new Action(() => cav_infor.Opacity = 1 - 0.01 * i));
                Thread.Sleep(10);
            }
            Dispatcher.BeginInvoke(new Action(() => cav_infor.Margin = new Thickness(-350, 204, 0, 0)));
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)//保护路径
        {
            if (path.Text.Length > 2)
            {
                bool Find = false;
                //检查路径是否和数据中有重复
                for (int i = 0; i < LJs.Count; i++)
                {
                    if (LJs[i].文件夹路径.Contains(path.Text))
                    {
                        信息提示Show("失败！存在相同路径!");
                        Find = true;
                    }
                }
                if (!Find)
                {
                    路径data lj = new 路径data();
                    lj.文件夹路径 = path.Text;
                    lj.备注 = "";
                    LJs.Add(lj);

                    FileStream fs = new FileStream(ExePath + "\\PathData.dx", FileMode.Create);
                    bf.Serialize(fs, LJs);
                    fs.Flush();
                    fs.Close();
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = LJs;

                    pad策略设置.Focus();
                    信息提示Show("添加路径成功!");
                }
                //或者存在包含关系
            }
        }

        private void dataGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void dataGrid_DataContextChanged(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(ExePath + "\\PathData.dx", FileMode.Create);
            bf.Serialize(fs, LJs);
            fs.Flush();
            fs.Close();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            FileStream fs = new FileStream(ExePath + "\\PathData.dx", FileMode.Create);
            bf.Serialize(fs, LJs);
            fs.Flush();
            fs.Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            LJs.Remove((路径data)dataGrid.SelectedItem);
            dataGrid.ItemsSource = null;
            dataGrid.ItemsSource = LJs;
        }

        private void btn1_Copy1_Click(object sender, RoutedEventArgs e)//
        {
            if (protectexe == false)
            {
                protectexe = true;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "√";
            }
            else
            {
                protectexe = false;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
            }
            save();
        }

        private void btn1_Copy2_Click(object sender, RoutedEventArgs e)
        {

            if (protectdll == false)
            {
                protectdll = true;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                ((System.Windows.Controls.Button)sender).Content = "√";
            }
            else
            {
                protectdll = false;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
            }
            save();
        }

        private void btn1_Copy4_Click(object sender, RoutedEventArgs e)
        {
            if (protectxml == false)
            {
                protectxml = true;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                ((System.Windows.Controls.Button)sender).Content = "√";
            }
            else
            {
                protectxml = false;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
            }
            save();
        }

        private void btn1_Copy6_Click(object sender, RoutedEventArgs e)
        {
            if (protectconfig == false)
            {
                protectconfig = true;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                ((System.Windows.Controls.Button)sender).Content = "√";
            }
            else
            {
                protectconfig = false;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
            }
            save();
        }

        private void btn1_Copy8_Click(object sender, RoutedEventArgs e)
        {
            if (protectOnlyRead == false)
            {
                protectOnlyRead = true;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                ((System.Windows.Controls.Button)sender).Content = "√";
            }
            else
            {
                protectOnlyRead = false;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
            }
            save();
        }

        private void btn1_Copy10_Click(object sender, RoutedEventArgs e)
        {
            if (protectDriver == false)
            {
                protectDriver = true;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                ((System.Windows.Controls.Button)sender).Content = "√";
                save();
                MessageBoxResult r = System.Windows.MessageBox.Show("开启实时监控成功！请重新启动软件！", "Success", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
                if (r == MessageBoxResult.OK)
                {
                    notifyIcon.Visible = false;
                    notifyIcon.Dispose();
                    System.Environment.Exit(0);
                }
            }
            else
            {
                protectDriver = false;
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
            }
            save();
        }

        private void btn1_Copy7_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("软件将会设置所保护的文件的属性为【只读属性】，config和XML文件不会受到本功能影响");
        }

        private void btn1_Copy9_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("软件将会注入驱动文件，实时拦截对保护文件的任何IO操作，防止病毒干染和破坏，达到Ring0级检测和防护");
        }

        private void set()
        {

            if (protectexe == true)
            {
                (btn1_Copy1).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                (btn1_Copy1).Content = "√";
            }
            else
            {
                (btn1_Copy1).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                (btn1_Copy1).Content = "X";
            }
            if (protectexe == true)
            {
                (btn1_Copy2).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                (btn1_Copy2).Content = "√";
            }
            else
            {
                (btn1_Copy2).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                (btn1_Copy2).Content = "X";
            }
            if (protectconfig == true)
            {
                (btn1_Copy4).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                (btn1_Copy4).Content = "√";
            }
            else
            {
                (btn1_Copy4).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                (btn1_Copy4).Content = "X";
            }
            if (protectxml == true)
            {
                (btn1_Copy6).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                (btn1_Copy6).Content = "√";
            }
            else
            {
                (btn1_Copy6).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                (btn1_Copy6).Content = "X";
            }
            if (protectOnlyRead == true)
            {
                (btn1_Copy8).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                (btn1_Copy8).Content = "√";
            }
            else
            {
                (btn1_Copy8).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                (btn1_Copy8).Content = "X";
            }
            if (protectDriver == true)
            {
                (btn1_Copy10).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));#FE9DF533
                (btn1_Copy10).Content = "√";
            }
            else
            {
                (btn1_Copy10).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                (btn1_Copy10).Content = "X";
            }
        }

        private void save()
        {
            SetData st = new SetData();
            st.protectexe = protectexe;
            st.protectdll = protectdll;
            st.protectconfig = protectconfig;
            st.protectxml = protectxml;
            st.protectOnlyRead = protectOnlyRead;
            st.protectDriver = protectDriver;

            FileStream fs = new FileStream(ExePath + "\\SetData.dx", FileMode.Create);
            bf.Serialize(fs, st);
            fs.Flush();
            fs.Close();
        }

        private void image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//网络监控
        {
            tab_网络监控.Visibility = Visibility;
            tab_网络监控.Focus();

        }

        private void btn1_Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("备份当前扫描的文件作为【标准比对文件】，并存储当前文件至软件【备份文件库】，名称可自定义，默认为当前扫描文件时的起始时间");
        }

        private void button_Copy4_Click(object sender, RoutedEventArgs e)//开始扫描
        {
            //将比对文件MD5表加载
            try
            {
                pool.Clear();
                probar.Value = 0;
                button_Copy4.IsEnabled = false;
                scan_num.Content = 0;
                scan_Add.Content = 0;
                scan_Lost.Content = 0;
                scan_Difrent.Content = 0;
                dataGridSacn.ItemsSource = null;
                Scannum = 0;
                新增num = 0;
                缺失num = 0;
                存在差异num = 0;
                FileScanMD5.Clear();
                FileScanMD5Add.Clear();
                FileScanMD5Difrent.Clear();
                FileScanMD5Lost.Clear();
                FileStream fs = new FileStream(ExePath + "\\MD5.LIST", FileMode.Open);
                FilelistMD5 = (List<FileMD5>)bf.Deserialize(fs);
                fs.Close();
                信息提示Show("比对文件MD5表加载成功！");
                FileScanMD5Lost = FilelistMD5;//差异寻找
                ThreadNum = 0;

                for (int i = 0; i < LJs.Count; i++)//从设置路径一个个开始
                {
                    //迭代扫描其中的文件和文件夹
                    //创建多线程 进行扫描
                    Thread t = new Thread(new ParameterizedThreadStart(Scan));
                    t.Start("" + LJs[i].文件夹路径);
                    pool.Add(t);
                }
                Thread t2 = new Thread(new ThreadStart(Pro));
                t2.Start();
                button_Copy4.IsEnabled = true;
            }
            catch (Exception ex)
            {
                信息提示Show("本次为首次扫描！");
                ThreadNum = 0;

                for (int i = 0; i < LJs.Count; i++)//从设置路径一个个开始
                {
                    //迭代扫描其中的文件和文件夹
                    //创建多线程 进行扫描
                    Thread t = new Thread(new ParameterizedThreadStart(Scan));
                    t.Start("" + LJs[i].文件夹路径);
                    pool.Add(t);
                }
                Thread t2 = new Thread(new ThreadStart(Pro));
                t2.Start();
                button_Copy4.IsEnabled = true;
            }
            //如果是首次扫描则提示：首次扫描，并自动存储当前文件为 基准文件MD5并备份

        }
        private void Pro()
        {
            Dispatcher.BeginInvoke(new Action(() => probar.Value = 0));
            while (ThreadNum != LJs.Count)
            {
                Thread.Sleep(100);
                ThreadNum = 0;
                for (int j = 0; j < pool.Count; j++)
                {
                    if (pool[j].ThreadState == ThreadState.Stopped)
                    {
                        ThreadNum++;
                    }
                }
                Dispatcher.BeginInvoke(new Action(() => probar.Value = 100 * (double)(ThreadNum) / (double)(LJs.Count)));
            }
            Thread.Sleep(2000);
            信息提示Show("扫描完成！");
            //开始显示
            for (int i = 0; i < FileScanMD5Lost.Count; i++)
            {
                FileScanMD5Lost[i].type = "^缺失";
            }
            //拼接 顺序 差异 缺失 新增
            int len = FileScanMD5Difrent.Count + FileScanMD5Add.Count + FileScanMD5Lost.Count;
            FileGrid = new FileMD5[len];
            FileScanMD5Difrent.CopyTo(FileGrid, 0);
            FileScanMD5Lost.CopyTo(FileGrid, FileScanMD5Difrent.Count);
            FileScanMD5Add.CopyTo(FileGrid, FileScanMD5Difrent.Count + FileScanMD5Lost.Count);
            Dispatcher.BeginInvoke(new Action(() => dataGridSacn.ItemsSource = null));
            Dispatcher.BeginInvoke(new Action(() => dataGridSacn.ItemsSource = FileGrid));
            Dispatcher.BeginInvoke(new Action(() => scan_Lost.Content = "" + FileScanMD5Lost.Count));
        }
        private void Scan(object path)
        {
            try
            {
                DirectoryInfo dri = new DirectoryInfo((string)path);
                FileInfo[] FileS = dri.GetFiles();
                //对文件进行比对
                for (int k = 0; k < FileS.Length; k++)//FileScanMD5
                {

                    if ((FileS[k].Name.Contains(".exe") && protectexe) || (FileS[k].Name.Contains(".dll") && protectdll) ||
                        (FileS[k].Name.Contains(".xml") && protectxml) || (FileS[k].Name.Contains(".config") && protectconfig) ||
                        (FileS[k].Name.Contains(".ini") && protectconfig))
                    {
                        string md5 = GetFileMD5(FileS[k].FullName);
                        FileMD5 fm5 = new FileMD5();
                        fm5.MD5 = md5;
                        fm5.FileName = FileS[k].Name;
                        fm5.Path = FileS[k].FullName;
                        FileScanMD5.Add(fm5);
                        //比较不同的
                        Cmp(fm5);
                        //找缺失
                        Del(fm5);
                        Scannum++;
                        Dispatcher.BeginInvoke(new Action(() => scan_num.Content = "" + Scannum));
                    }

                }

                //获取文件夹
                DirectoryInfo[] Ds = dri.GetDirectories();
                for (int i = 0; i < Ds.Length; i++)
                {
                    Scan(Ds[i].FullName);
                }

            }
            catch
            {

            }

        }
        private void Del(FileMD5 fm5)
        {
            bool lost = true;
            for (int i = 0; i < FileScanMD5Lost.Count; i++)
            {
                if (FileScanMD5Lost[i].Path == fm5.Path && FileScanMD5Lost[i].FileName == fm5.FileName)//发现不相同文件
                {
                    FileScanMD5Lost.RemoveAt(i);
                }
            }
        }
        private void Cmp(FileMD5 fm5)
        {
            bool add = false;
            for (int i = 0; i < FilelistMD5.Count; i++)
            {
                if (FilelistMD5[i].Path == fm5.Path && FilelistMD5[i].FileName == fm5.FileName && FilelistMD5[i].MD5 != fm5.MD5)//发现不相同文件
                {
                    存在差异num++;
                    Dispatcher.BeginInvoke(new Action(() => scan_Difrent.Content = "" + 存在差异num));
                    fm5.type = "*差异";
                    FileScanMD5Difrent.Add(fm5);
                }
                if (FilelistMD5[i].Path == fm5.Path)//发现不相同文件
                {
                    add = true;
                }

            }
            if (!add)
            {
                fm5.type = "+新增";
                新增num++;
                Dispatcher.BeginInvoke(new Action(() => scan_Add.Content = "" + 新增num));
                FileScanMD5Add.Add(fm5);
            }
        }
        public static string GetFileMD5(string filepath)
        {
            try
            {
                FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read);

                int bufferSize = 1048576;
                byte[] buff = new byte[bufferSize];
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                md5.Initialize();
                long offset = 0;
                while (offset < fs.Length)
                {
                    long readSize = bufferSize;
                    if (offset + readSize > fs.Length)
                        readSize = fs.Length - offset;
                    fs.Read(buff, 0, Convert.ToInt32(readSize));
                    if (offset + readSize < fs.Length)
                        md5.TransformBlock(buff, 0, Convert.ToInt32(readSize), buff, 0);
                    else
                        md5.TransformFinalBlock(buff, 0, Convert.ToInt32(readSize));
                    offset += bufferSize;
                }
                if (offset >= fs.Length)
                {
                    fs.Close();
                    byte[] result = md5.Hash;
                    md5.Clear();
                    StringBuilder sb = new StringBuilder(32);
                    for (int i = 0; i < result.Length; i++)
                        sb.Append(result[i].ToString("X2"));
                    return sb.ToString();
                }
                else
                {
                    fs.Close();
                    return null;
                }
            }
            catch
            {
                return "空文件";
            }

        }

        private void button_Copy5_Click(object sender, RoutedEventArgs e)//备份扫描文件 并存储MD5表
        {
            button_Copy5.IsEnabled = false;
            if (FileScanMD5.Count == 0)
            {
                System.Windows.MessageBox.Show("当前检测到的文件个数为0，请先扫描文件！");
                Dispatcher.BeginInvoke(new Action(() => button_Copy5.IsEnabled = true));
                return;
            }
            FileStream fs = new FileStream(ExePath + "\\MD5.LIST", FileMode.Create);
            bf.Serialize(fs, FileScanMD5);
            fs.Close();
            信息提示Show("文件MD5存储成功！");
            //备份文件
            Thread t = new Thread(new ThreadStart(Backdata));
            t.Start();

        }
        private void Backdata()//备份文件
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(ExePath + "\\BackData");
                di.Delete();
                di.Create();
            }
            catch (Exception ex)
            {

            }
            try
            {
                for (int i = 0; i < FileScanMD5.Count; i++)
                {
                    FileStream fs = new FileStream(FileScanMD5[i].Path, FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] buff = new byte[fs.Length];
                    fs.Read(buff, 0, buff.Length);
                    fs.Close();

                    string NewName = FileScanMD5[i].FileName.Replace("exe", "exeXH").Replace("dll", "dllXH").Replace("xml", "xmlXH").Replace("config", "configXH").Replace("ini", "iniXH");
                    SetFileAttributes_no(ExePath + "\\BackData\\" + NewName);
                    FileStream fs2 = new FileStream(ExePath + "\\BackData\\" + NewName, FileMode.Create);
                    fs2.Write(buff, 0, buff.Length);
                    fs2.Close();
                }
            }
            catch
            {

            }

            Thread.Sleep(3000);
            信息提示Show("备份 " + FileScanMD5.Count + " 个文件成功!");
            Dispatcher.BeginInvoke(new Action(() => button_Copy5.IsEnabled = true));
        }

        private void button_Copy6_Click(object sender, RoutedEventArgs e)//将文件设置为 只读
        {
            Thread t = new Thread(new ThreadStart(SetOnlyRead));
            t.Start();
        }
        private void SetOnlyRead()
        {
            int num = 0;
            for (int i = 0; i < FileScanMD5.Count; i++)
            {
                if (FileScanMD5[i].FileName.Contains(".exe") || FileScanMD5[i].FileName.Contains(".dll"))
                {
                    SetFileAttributes(FileScanMD5[i].Path);
                    num++;
                }

            }
            信息提示Show("锁定 " + num + " 个文件成功!");
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
        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            FileInfo d = new FileInfo(((FileMD5)dataGridSacn.SelectedItem).Path);
            try
            {
                System.Diagnostics.Process.Start(d.Directory.FullName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("未找到文件位置！");
            }

        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            FileInfo d = new FileInfo(((FileMD5)dataGridSacn.SelectedItem).Path);
            try
            {
                System.Diagnostics.Process.Start(d.FullName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("未找到文件！");
            }

        }

        private void image_Copy1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tab_备份恢复.Visibility = Visibility;
            tab_备份恢复.Focus();
        }

        private void button_Copy7_Click(object sender, RoutedEventArgs e)//开始恢复备份
        {
            if (FileScanMD5.Count == 0)
            {
                System.Windows.MessageBox.Show("恢复备份文件前，请先进行文件扫描！", "提示", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult r = System.Windows.MessageBox.Show("恢复备份文件前，请确保相关软件处于关闭状态！", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Exclamation);
            if (r == MessageBoxResult.OK)
            {
                textBlock.Text = "开始进行备份恢复操作..." + "\n";
                //统计
                textBlock.Text += "预计恢复文件：   差异文件：" + FileScanMD5Difrent.Count + "个    缺失文件：" + FileScanMD5Lost.Count + "个..." + "\n";
                textBlock.Text += "差异文件：" + "\n";
                for (int i = 0; i < +FileScanMD5Difrent.Count; i++)
                {
                    textBlock.Text += i + "  " + FileScanMD5Difrent[i].Path + "\n";
                }
                textBlock.Text += "------------------------------------------------------------" + "\n";
                textBlock.Text += "缺失文件：" + "\n";
                for (int i = 0; i < +FileScanMD5Lost.Count; i++)
                {
                    textBlock.Text += i + "  " + FileScanMD5Lost[i].Path + "\n";
                }
                textBlock.Text += "------------------------------------------------------------" + "\n";

                //对保护文件解锁
                textBlock.Text += "开始文件解锁操作..." + "\n";
                //还原 差异的文件
                Unlock(FileScanMD5Difrent);
                //还原 缺失的文件
                Unlock(FileScanMD5Lost);
                textBlock.Text += "文件解锁成功！" + "\n";
                //恢复文件
                oknum = 0;
                nonum = 0;
                RecoverData(FileScanMD5Difrent);
                RecoverData(FileScanMD5Lost);
                textBlock.Text += "文件恢复成功！" + "\n";
                Back_num.Content = oknum;
                Back_num_error.Content = nonum;
                textBlock.ScrollToEnd();
            }
        }
        private void Unlock(List<FileMD5> ScanMD5)//解锁函数
        {
            for (int i = 0; i < ScanMD5.Count; i++)
            {
                SetFileAttributes_no(ScanMD5[i].Path);
            }
        }
        int oknum = 0;
        int nonum = 0;
        private void RecoverData(List<FileMD5> ScanMD5)//恢复文件
        {
            for (int i = 0; i < ScanMD5.Count; i++)
            {
                try
                {
                    FileStream fs = new FileStream(ExePath + "\\BackData\\" + ScanMD5[i].FileName + "XH", FileMode.Open, FileAccess.Read, FileShare.Read);
                    byte[] buff = new byte[fs.Length];
                    fs.Read(buff, 0, buff.Length);
                    fs.Close();

                    FileStream fs2 = new FileStream(ScanMD5[i].Path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs2.Write(buff, 0, buff.Length);
                    fs2.Close();
                    oknum++;
                    textBlock.Text += "恢复" + ScanMD5[i].Path + "成功！" + "\n";

                    if (protectOnlyRead)
                    {
                        SetFileAttributes(ScanMD5[i].Path);//锁定
                        textBlock.Text += "锁定" + ScanMD5[i].Path + "成功！" + "\n";
                    }
                }
                catch (Exception ex)
                {
                    nonum++;
                    textBlock.Text += "【ERROR】恢复" + ScanMD5[i].Path + "失败！" + "\n";
                }

            }

        }

        private void button_Copy8_Click(object sender, RoutedEventArgs e)
        {
            //
            nets.Clear();
            信息提示Show("开始遍历网络连接...");
            ////搜索网络进程
            //IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            //TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            //foreach (TcpConnectionInformation t in connections)
            //{
            //    NetConns nt = new NetConns();
            //    nt.网络连接状态 = t.State.ToString();
            //    nt.本地端口 = t.LocalEndPoint.Port;
            //    nt.本地IP地址 = t.LocalEndPoint.Address.ToString();
            //    nt.远程端口 = t.RemoteEndPoint.Port;
            //    nt.远程IP地址 = t.RemoteEndPoint.Address.ToString();
            //    nets.Add(nt);

            //    dataGridSacnNet.ItemsSource = null;
            //    dataGridSacnNet.ItemsSource = nets;
            //    //Console.WriteLine("本地： " + t.LocalEndPoint.Address+ "本地Point： " + t.LocalEndPoint.Port + "===" +"远程: " + t.RemoteEndPoint.Address+ "状态：" + t.State);

            //}
            /////

            Thread t = new Thread(new ThreadStart(loadProcess));
            t.Priority = ThreadPriority.Highest;
            t.Start();
        }
        private void loadProcess()
        {
            Dispatcher.BeginInvoke(new Action(() => dataGridSacnNet.ItemsSource = null));

            //tcp
            foreach (var p in NetProcessAPI.GetAllTcpConnections())
            {
                var icon = ProcessAPI.GetIcon(p.owningPid, true);
                NetConns nt = new NetConns();
                System.Drawing.Bitmap bmp = (icon == null ? Properties.Resources.app : icon).ToBitmap();
                nt.ico = BitmapToBitmapImage(bmp);//(icon == null ? Properties.Resources.app : icon).ToBitmap()
                nt.应用程序名称 = ProcessAPI.GetProcessNameByPID(p.owningPid);
                nt.网络连接状态 = p.state.ToString();
                nt.本地IP地址 = p.LocalAddress.ToString();
                nt.本地端口 = p.LocalPort;
                nt.远程IP地址 = p.RemotePort.ToString();
                nt.远程端口 = p.RemotePort;
                nt.应用程序路径 = ProcessAPI.GetProcessPathByPID(p.owningPid);
                nt.PID = p.owningPid;
                nt.协议 = "TCP";
                nt.CPU使用率 = ProcessAPI.UsingProcess(p.owningPid);
                nets.Add(nt);
                //dataGridSacnNet.Items.Add(new object[] { icon == null ? Properties.Resources.app : icon, ProcessAPI.GetProcessNameByPID(p.owningPid), "TCP", p.LocalAddress.ToString(), p.LocalPort.ToString(), p.RemoteAddress.ToString(), p.RemotePort.ToString() });
            }

            foreach (var p in NetProcessAPI.GetAllUdpConnections())
            {
                var icon = ProcessAPI.GetIcon(p.owningPid, true);
                NetConns nt = new NetConns();
                System.Drawing.Bitmap bmp = (icon == null ? Properties.Resources.app : icon).ToBitmap();
                nt.ico = BitmapToBitmapImage(bmp);//(icon == null ? Properties.Resources.app : icon).ToBitmap()
                nt.应用程序名称 = ProcessAPI.GetProcessNameByPID(p.owningPid);
                nt.本地IP地址 = p.LocalAddress.ToString();
                nt.本地端口 = p.LocalPort;
                nt.应用程序路径 = ProcessAPI.GetProcessPathByPID(p.owningPid);
                nt.PID = p.owningPid;
                nt.协议 = "UDP";
                nets.Add(nt);
                //dataGridSacnNet.Items.Add(new object[] { icon == null ? Properties.Resources.app : icon, ProcessAPI.GetProcessNameByPID(p.owningPid), "TCP", p.LocalAddress.ToString(), p.LocalPort.ToString(), p.RemoteAddress.ToString(), p.RemotePort.ToString() });
            }
            Dispatcher.BeginInvoke(new Action(() => dataGridSacnNet.ItemsSource = nets));

            //udp
            //foreach (var p in NetProcessAPI.GetAllUdpConnections())
            //{
            //    var icon = ProcessAPI.GetIcon(p.owningPid, true);
            //    dataGridSacnNet.Items.Add(new object[] { icon == null ? Properties.Resources.app : icon, ProcessAPI.GetProcessNameByPID(p.owningPid), "UDP", p.LocalAddress.ToString(), p.LocalPort.ToString(), "-", "-" });
            //}
        }
        //转换器中二进制转化为BitmapImage  datagrid绑定仙石的
        private BitmapImage BitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = ms;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }
        private BitmapImage ShowSelectedIMG(byte[] img)

        {

            BitmapImage newBitmapImage = null;

            if (img != null)

            {

                System.IO.MemoryStream ms = new System.IO.MemoryStream(img);//img是从数据库中读取出来的字节数组

                ms.Seek(0, System.IO.SeekOrigin.Begin);



                newBitmapImage = new BitmapImage();

                newBitmapImage.BeginInit();

                newBitmapImage.StreamSource = ms;

                newBitmapImage.EndInit();

            }

            return newBitmapImage;

        }
        private void btn1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("1、LISTENING状态:FTP服务启动后首先处于侦听（LISTENING）状态。\n" + "2、ESTABLISHED状态:ESTABLISHED的意思是建立连接，表示两台机器正在通信。\n" + "3、CLOSE_WAIT对方主动关闭连接或者网络异常导致连接中断。\n" + "4、TIME_WAIT我方主动调用close()断开连接，收到对方确认后状态变为TIME_WAIT。\n");
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)//打开网络连接位置
        {
            try
            {
                FileInfo fi = new FileInfo(((NetConns)dataGridSacnNet.SelectedItem).应用程序路径);
                System.Diagnostics.Process.Start(fi.Directory.FullName);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("打开网络连接位置所属进程失败！" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)//终止网络连接exe
        {
            try
            {
                string re = ProcessAPI.KillProcessByPID(((NetConns)dataGridSacnNet.SelectedItem).PID);
                if (re == "Success")
                {
                    System.Windows.MessageBox.Show("终止进程成功！", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    信息提示Show("开始遍历网络连接...");
                    nets.Clear();
                    Thread t = new Thread(new ThreadStart(loadProcess));
                    t.Priority = ThreadPriority.Highest;
                    t.Start();
                }
                else
                {
                    System.Windows.MessageBox.Show("终止进程失败！拒绝访问", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("终止进程失败！拒绝访问" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void button_Copy8_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //信息提示Show("开始遍历网络连接...");
        }

        private void image_Copy4_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)//拦截设置
        {
            tab_高级工具.Visibility = Visibility;
            tab_高级工具.Focus();
        }
        private void GetCPUTemp()
        {
            try
            {
                bool su = true;
                while (su)
                {
                    string WD = ProcessAPI.GetCPU_WD();
                    if (WD == "")
                    {
                        su = false;
                        break;
                    }
                    Dispatcher.BeginInvoke(new Action(() => lab_CPUWD.Content = WD));
                    if (double.Parse(WD) > 60)
                    {
                        信息提示Show("CPU温度过高!!!" + WD + "C°");
                        //if (温度监控提示)
                        //{
                        Dispatcher.BeginInvoke(new Action(() => report.Text += "" + "CPU温度过高!!!" + WD + "C°" + "\n"));//report.ScrollToEnd();
                        Dispatcher.BeginInvoke(new Action(() => report.ScrollToEnd()));
                        //}
                    }
                    Thread.Sleep(10000);
                }

            }
            catch
            {

            }
        }

        private void TabItem_Loaded(object sender, RoutedEventArgs e)
        {
            Thread t = new Thread(new ThreadStart(GetCPUTemp));
            t.Start();
        }

        private void btn1_Copy12_Click(object sender, RoutedEventArgs e)
        {
            if (温度监控提示 == false)
            {
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(0xFE, 0x9D, 0xF5, 0x33));//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "√";
                温度监控提示 = true;
            }
            else
            {
                ((System.Windows.Controls.Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0x00, 0x5C, 0x99)); ;//new SolidColorBrush(Color.FromArgb( 100, 250, 230, 160));
                ((System.Windows.Controls.Button)sender).Content = "X";
                温度监控提示 = false;
            }
        }

        private void button_Copy9_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(ExePath + "\\CDAnalyzer\\CDAnalyzer.exe");
            }
            catch
            {

            }
        }

        private void button_Copy10_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                System.Diagnostics.Process.Start(ExePath + "\\WinDbg Preview1.1910.3003.0\\Microsoft.WinDbg_1.1910.3003.0_neutral__8wekyb3d8bbwe\\DbgX.Shell.exe");
            }
            catch (Exception ex)
            {

            }
        }
        //.exe木马专杀
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            信息提示Show("开始文件夹.EXE木马专杀...");
            Thread t = new Thread(new ThreadStart(KillHorseEXE));
            t.Start();

        }
        private void KillHorseEXE()
        {

            try
            {
                System.Security.PermissionSet Perm = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                Perm.Demand();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            System.Diagnostics.Process p = System.Diagnostics.Process.Start(ExePath + @"\Everything\Everything.exe");
            //检测 Mstray.exe这个进程
            System.Diagnostics.Process[] MstrayThread = System.Diagnostics.Process.GetProcessesByName("Mstary");
            string MstrayPath = "";
            if (MstrayThread.Length > 0)
            {
                Console.WriteLine(MstrayThread[0].ProcessName);
                MstrayPath = MstrayThread[0].MainModule.FileName;
                //Kill Mstray.exe这个进程
                MstrayThread[0].Kill();
                //删除 C:\Windows\Mstray.exe
                FileInfo fi = new FileInfo(MstrayPath);
                SetFileAttributes_no(MstrayPath);//解锁@"D:\HWBoxDock.exe"
                fi.Delete();
            }
            else
            {
                System.Windows.MessageBox.Show("未找到 Mstary.exe进程");
            }
            if (MstrayThread.Length == 0)
            {
                try
                {
                    //删除 C:\Windows\Mstray.exe
                    FileInfo fi = new FileInfo(@"C:\Windows\Mstray.exe");
                    SetFileAttributes_no(MstrayPath);//解锁
                    fi.Delete();
                }
                catch
                {
                    System.Windows.MessageBox.Show(@"删除 C:\Windows\Mstray.exe 失败！");
                }

            }

            //删除注册表RavTimeXP RavTimXP 键值
            /*
 Run键主要用于开机自动加载程序之用，也是是病毒最青睐的自启动之所，该键位置有两处： 
[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run] 
[HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\Run] 
其下的所有程序在每次启动登录时都会按顺序自动执行。 
还有一个不被注意的Run键，位于注册表的 
[HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run]和[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run]下，有的机器没有，不用担心。 
如果误删会导致使用该位置启动的软件开机时加载不了。当然误删之后可以新建该注册表. 


             */

            //RegistryKey key = Registry.LocalMachine;
            //RegistryKey software = key.OpenSubKey("software\\My", true);
            ////写入
            //software.SetValue("ProductID", "XH is Dog");
            ////读取
            //string info = "";
            //info = software.GetValue("ProductID").ToString();
            ////删除
            //software.DeleteValue("test");
            //关闭
            try
            {
                RegistryKey key = Registry.LocalMachine;
                RegistryKey software = key.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                software.DeleteValue("RavTimeXP");//病毒木马启动项
                software.DeleteValue("RavTimXP");
                software.Close();
                Dispatcher.BeginInvoke(new Action(() => report.Text += "删除注册表RavTimeXP RavTimXP 键值 成功！" + "\n"));
            }
            catch
            {
                Dispatcher.BeginInvoke(new Action(() => report.Text += "删除注册表RavTimeXP RavTimXP 键值 失败！键值可能已经删除！" + "\n"));
            }
            try
            {
                RegistryKey key = Registry.CurrentUser;
                RegistryKey software = key.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                software.DeleteValue("RavTimeXP");//病毒木马启动项
                software.DeleteValue("RavTimXP");
                software.Close();
                Dispatcher.BeginInvoke(new Action(() => report.Text += "删除注册表RavTimeXP RavTimXP 键值 成功！" + "\n"));
            }
            catch
            {
                Dispatcher.BeginInvoke(new Action(() => report.Text += "删除注册表RavTimeXP RavTimXP 键值 失败！键值可能已经删除！" + "\n"));
            }
            try
            {
                //删除 所有的.exe 大于240Mb  file.ProductName == "Xgtray" && file.OriginalFilename == "wukill.exe"
                EverythingAPI everyThingAPI = new EverythingAPI();
                System.Collections.IEnumerable results = everyThingAPI.Search(".exe", 100);
                foreach (var item in results)
                {
                    // Dispatcher.BeginInvoke(new Action(() => report.Text += "" + item + "\n"));


                    System.Diagnostics.FileVersionInfo file = System.Diagnostics.FileVersionInfo.GetVersionInfo("" + item);
                    //Console.WriteLine(file.ProductName);//产品名称
                    //Console.WriteLine(file.OriginalFilename);//原始文件名
                    if (file.ProductName == "Xgtray" && file.OriginalFilename == "wukill.exe")
                    {
                        //确定是 木马
                        SetFileAttributes_no("" + item);//解锁
                        FileInfo fi = new FileInfo("" + item);
                        fi.Delete();
                        Dispatcher.BeginInvoke(new Action(() => report.Text += "删除 木马" + item + " 成功！" + "\n"));
                    }


                }
                Dispatcher.BeginInvoke(new Action(() => report.ScrollToEnd()));
                Dispatcher.BeginInvoke(new Action(() => report.Text += "Success！" + "\n"));
                everyThingAPI.Reset();
            }
            catch
            {

            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Security.PermissionSet Perm = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
                Perm.Demand();
                System.Diagnostics.Process p = System.Diagnostics.Process.Start(ExePath + @"\Everything\Everything.exe");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message);
            }

            //删除 所有的 .temp文件
            try
            {
                EverythingAPI everyThingAPI = new EverythingAPI();
                System.Collections.IEnumerable results = everyThingAPI.Search("ext:temp", 1000);
                int num = 0;
                foreach (var item in results)
                {
                    num++;
                    // Dispatcher.BeginInvoke(new Action(() => report.Text += "" + item + "\n"));

                    try
                    {
                        SetFileAttributes_no("" + item);//解锁
                        FileInfo fi = new FileInfo("" + item);
                        fi.Delete();
                        Dispatcher.BeginInvoke(new Action(() => report.Text += "删除 木马" + item + " 成功！" + "\n"));
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.BeginInvoke(new Action(() => report.Text += ex.Message + "\n"));
                    }

                }
                if (num == 0)
                {
                    Dispatcher.BeginInvoke(new Action(() => report.Text += "未发现 .temp 木马病毒！" + "\n"));
                    信息提示Show("未发现 .temp 木马病毒！");
                }
            }
            catch
            {

            }

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            try
            {
                //检测 Mstray.exe这个进程
                System.Diagnostics.Process[] MstrayThread = System.Diagnostics.Process.GetProcessesByName("Sever");
                string MstrayPath = "";
                if (MstrayThread.Length > 0)
                {
                    Console.WriteLine(MstrayThread[0].ProcessName);
                    MstrayPath = MstrayThread[0].MainModule.FileName;
                    //Kill Mstray.exe这个进程
                    MstrayThread[0].Kill();
                    //删除 C:\Windows\Mstray.exe
                    FileInfo fi = new FileInfo(MstrayPath);
                    SetFileAttributes_no(MstrayPath);//解锁@"D:\HWBoxDock.exe"
                    fi.Delete();
                    Dispatcher.BeginInvoke(new Action(() => report.Text += "删除 Server木马成功！" + "\n"));
                }
                else
                {
                    Dispatcher.BeginInvoke(new Action(() => report.Text += "未发现 Server木马成功！" + "\n"));
                    信息提示Show("未发现 Server 木马病毒！");
                }
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(new Action(() => report.Text += ex.Message + "\n"));
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("预留功能!", "haha", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

}
