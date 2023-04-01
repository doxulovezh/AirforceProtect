using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace AirforceProtect
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        MainW M;
        public MainWindow()
        {
            InitializeComponent();
            M = new MainW();
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        private void image_Copy_Loaded(object sender, RoutedEventArgs e)//开始走进度条
        {
            Thread t = new Thread(new ThreadStart(StartCheck));
            t.Start();
        }
        private void StartCheck()
        {
            for(int i = 0; i < 100; i++)
            {
                Thread.Sleep(10);
                Dispatcher.BeginInvoke(new Action(() => pro.Value+=1));
            }
            //调 主界面
            Dispatcher.BeginInvoke(new Action(() => this.Visibility = Visibility.Collapsed));
            Dispatcher.BeginInvoke(new Action(() => M.Show()));
          
            

        }
    }
}
