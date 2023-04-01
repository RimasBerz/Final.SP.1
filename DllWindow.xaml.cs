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
using System.Runtime;
using System.Runtime.InteropServices;

namespace SystemProgramming1
{
    /// <summary>
    /// Interaction logic for DllWindow.xaml
    /// </summary>
    public partial class DllWindow : Window
    {
        #region basis
        [DllImport("User32.dll")]
        public static extern int MessageBoxA(
            IntPtr hWnd,
            String lpText,
            String lpCaption,
            uint    uType);

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBoxW(
            IntPtr hWnd,
            String lpText,
            String lpCaption,
            uint uType);

        [DllImport("Kernel32.dll", EntryPoint = "Beep")]
        public static extern bool Sound(uint dwFreq, uint dwDuration);
        [DllImport("Kernel32.dll", EntryPoint = "CreateThread")]
        public static extern IntPtr CreateThread(
            IntPtr lpTHreadAttributes,
            uint   dwStackSize,
            ThreadMethod lpStartAddress,
            IntPtr lpParameter,
            uint   dwCreatingFlags,
            IntPtr lpThreadId);
        public delegate void ThreadMethod();
       
        public void SayHello()
        {
            Dispatcher.Invoke(() => SayhelloLabel.Content = "Hello");
            sayHElloHandle.Free();
        }
        #endregion
        #region MM Timer
        delegate void TimerMethod(uint uTImerID, uint uMsg, 
            ref uint dwUser, uint dw1, uint dw2);
        [DllImport("winmm.dll", EntryPoint = "timeSetEvent")]
        static extern uint TimeSetEvent(
            uint uDelay,
            uint uResolution,
            TimerMethod lpTimeProc,
            ref uint dwUser,
            uint eventType);
        [DllImport("winmm.dll", EntryPoint = "timeKillEvent")]
        static extern void TimerKillEvent(uint uTimerID);

        const uint TIME_ONESHOT = 0;
        const uint TIME_PERIODIC = 1;

        uint uDelay;
        uint uResolution;
        uint timerId;
        uint dwUser = 0;
        TimerMethod timerMethod = null!;
        GCHandle timerHandle;

        int ticks;
        void TimerTick(uint uTImerID, uint uMsg,
            ref uint dwUser, uint dw1, uint dw2)
        {
            ticks++;
            Dispatcher.Invoke(() => { StopT.Content = ticks.ToString();  });
        }
        #endregion
        public DllWindow()
        {
            InitializeComponent();
        }
        #region basis 
        private void MessageBoxA_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxA(
                System.IntPtr.Zero,
                "Message",
                "Title",
                1);
        }

        private void MessageBoxW_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxW(
              IntPtr.Zero,
              "Message",
              "Title",
              1);
        }

        private void MessageBoxC_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxW(
             IntPtr.Zero,
             "Повоторить попытку соединения",
             "Соединение не установлено",
             0x35);
        }
        private void ErrorAlert(String message)
        {
            MessageBoxW(IntPtr.Zero,
              message,
              "ErrorAlert",
              0x10);
        }

        private void MessageBoxD_Click(object sender, RoutedEventArgs e)
        {
            ErrorAlert("Ошибка");
        }
        private bool? ConfirmMessage(String message)
        {
            int res = MessageBoxW(IntPtr.Zero,
              message,
              "",
              0x46);
            return res switch
            {
                11 => true,
                4 => false,
                _ => null
            };
        }
        private bool? AskMessage(String message)
        {
            int res = MessageBoxW(IntPtr.Zero,
              message,
              "",
              0x46);
            return res switch
            {
                2 => true,
                3 => false,
                _ => null
            };
        }
        private void MessageBoxF_Click(object sender, RoutedEventArgs e)
        {
            ConfirmMessage("Процесс занимает много времени");
        }

        private void MessageBoxJ_Click(object sender, RoutedEventArgs e)
        {
            AskMessage(
            "Подтверждаете?");
        }

        private void MessageBoxBeep_Click(object sender, RoutedEventArgs e)
        {
            Sound(420, 250);
            Sound(290, 420);
            Sound(250, 120);
            Sound(480, 520);
            Sound(340, 150);
            Sound(90, 520);
            Sound(200, 250);
            Sound(200, 520);
        }
        GCHandle sayHElloHandle;
        private void SayHelloThread_Click(object sender, RoutedEventArgs e)
        {
            //CreateThread(IntPtr.Zero, 0, SayHello, IntPtr.Zero, 0, IntPtr.Zero);
            var sayHelloObject = new ThreadMethod(SayHello);
            var handle = GCHandle.Alloc(sayHelloObject);
            CreateThread(IntPtr.Zero, 0, sayHelloObject, IntPtr.Zero, 0, IntPtr.Zero);
        }
        #endregion

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            uDelay = 100;
            uResolution = 10;
            ticks = 0;
            timerMethod = new TimerMethod(TimerTick);
            timerHandle = GCHandle.Alloc(timerMethod);
            timerId = TimeSetEvent(uDelay, uResolution,timerMethod, ref dwUser, TIME_PERIODIC);
            if(timerId != 0)
            {
                StopTimer.IsEnabled= true;
                StartTimer.IsEnabled= false;
            }
        }

        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            TimerKillEvent(timerId);
            timerHandle.Free();
            StopTimer.IsEnabled = false;
            StartTimer.IsEnabled = true;
        }

        private void MessageBoxBeep2_Click(object sender, RoutedEventArgs e)
        {
            Sound(10, 120);
            Sound(30, 400);
            Sound(50, 250);
            Sound(450, 120);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (timerId != 0)
            {
                TimerKillEvent(timerId);
                timerHandle.Free();
            }
        }

        void TimerTick2(uint uTImerID, uint uMsg, ref uint dwUser, uint dw1, uint dw2)
        {
            ticks++;
            TimeSpan ts = TimeSpan.FromMilliseconds(ticks * uDelay);
            Dispatcher.Invoke(() =>
            {
                StopT.Content = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D2}",
                    ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
            });
        }
    }
}
