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
using System.Runtime.InteropServices;
using System.Windows.Threading;

namespace MouseClickSender
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        //This is a replacement for Cursor.Position in WinForms
        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        //This simulates a left mouse click
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private DispatcherTimer timer;
        TimeSpan targetSpan;
        TimeSpan timerCounter;

        private void TryStart()
        {
            int targetX, targetY;
            if (!int.TryParse(tbX.Text, out targetX))
            {
                throw new Exception("TARGET X CANNOT BE PARSED TO int");
            }

            if (!int.TryParse(tbY.Text, out targetY))
            {
                throw new Exception("TARGET Y CANNOT BE PARSED TO int");
            }

            int hh, mm, ss;

            if(!int.TryParse(tbHH.Text, out hh))
            {
                throw new Exception("HH CANNOT BE PARSED TO int");
            }

            if (!int.TryParse(tbMM.Text, out mm))
            {
                throw new Exception("MM CANNOT BE PARSED TO int");
            }

            if (!int.TryParse(tbSS.Text, out ss))
            {
                throw new Exception("SS CANNOT BE PARSED TO int");
            }

            if (hh < 0 || mm < 0 || ss < 0 || hh >= 60 || mm >= 60 || ss >= 60
                || (hh==0&&mm==0&&ss==0))
            {
                throw new Exception("CHECK HH MM SS RANGE");
            }
            
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0,0,1);

            targetSpan=new TimeSpan(hh, mm, ss);
            timerCounter = new TimeSpan(0, 0, 0);

            timer.Tick += (s, e) =>
            {
                timerCounter = timerCounter.Add(new TimeSpan(0, 0, 1));

                var diff = targetSpan.Subtract(timerCounter);
                lblNextClickIn.Content = string.Format("{0:d2}:{1:d2}:{2:d2}", diff.Hours, diff.Minutes, diff.Seconds);

                if (timerCounter.CompareTo(targetSpan) == 0)
                {
                    LeftMouseClick(targetX, targetY);
                    timerCounter = new TimeSpan(0, 0, 0);
                }
            };

            timer.Start();
        }

        private void cbIsEnabled_Click(object sender, RoutedEventArgs e)
        {
            if(cbIsEnabled.IsChecked == true)
            {
                try
                {
                    TryStart();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    cbIsEnabled.IsChecked= false;
                }
            }
            else
            {
                timer.Stop();
                lblNextClickIn.Content = "--:--:--";
            }
        }
    }
}
