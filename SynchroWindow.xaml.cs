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
using System.Threading;
namespace SystemProgramming1
{
    /// <summary>
    /// Interaction logic for SynchroWindow.xaml
    /// </summary>
    public partial class SynchroWindow : Window
    {
        public SynchroWindow()
        {
            InitializeComponent();
        }
        private volatile bool stopAll = false;

        private void ButtonStopAll_Click(object sender, RoutedEventArgs e)
        {
            stopAll = true;
        }

        #region 1. lock
        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            stopAll = false;
            for (int i = 1; i < 5; i++)
            {
                new Thread(doWork1).Start(i);
            }
        }
        private readonly object locker = new();
        private void doWork1(object? state)
        {
            lock (locker)
            {
                while (!stopAll) 
                {
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
                }
            }
        }
        #endregion
        #region 2. Monitor
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            stopAll = false;
            for (int i = 1; i < 5; i++)
            {
                new Thread(doWork2).Start(i);
            }
        }
        private object monitor = new();
        private void doWork2(object? state)
        {
            try
            {
                while (!stopAll)
                {
                    Monitor.Enter(monitor);
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
                }
            }
            finally
            {
                Monitor.Exit(monitor);
            }
        }
        #endregion
        #region 3. Mutex
        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        {
            stopAll = false;
            for (int i = 1; i < 5; i++)
            {
                new Thread(doWork3).Start(i);
            }
        }
        private Mutex mutex = new();
        private void doWork3(object? state)
        {
            try
            {
                mutex.WaitOne();
                while (!stopAll) 
                {
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
        #endregion
        #region 4. EventWaitHandler
        private void ButtonStart4_Click(object sender, RoutedEventArgs e)
        {
            stopAll = false;
            for (int i = 1; i < 5; i++)
            {
                new Thread(doWork4).Start(i);
            }
            gates.Set();
        }
        private EventWaitHandle gates = new AutoResetEvent(true);
        private void doWork4(object? state)
        {
            gates.WaitOne();
            while (!stopAll)
            {
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");

                gates.Set();
            }
        }
        #endregion

        #region 5. Semaphore



        private void ButtonStart5_Click(object sender, RoutedEventArgs e)
        {
            stopAll = false;
            for (int i = 1; i < 5; i++)
            {
                new Thread(doWork5).Start(i);
            }
            semaphore.Release(2);
            Task.Run(async () =>
            {
                await Task.Delay(200);
                semaphore.Release(1);
            });
        }
        private Semaphore semaphore = new(0, 3);
        private void doWork5(object? state)
        {
                semaphore.WaitOne();
            try
            {
                while (!stopAll) 
                {
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
                }
            }
            finally
            {
                semaphore.Release(1);
            }
        }

        #endregion

        #region 6. SemaphoreSlim

        private void ButtonStart6_Click(object sender, RoutedEventArgs e)
        {
            stopAll = false;
            for (int i = 1; i < 5; i++)
            {
                new Thread(doWork6).Start(i);
            }
            semaphoreSlim.Release(2);
            Task.Run(async () =>
            {
                await Task.Delay(200);
                semaphoreSlim.Release(1);
            });
        }

        private readonly SemaphoreSlim semaphoreSlim = new(0, 3);
        private void doWork6(object? state)
        {
            semaphoreSlim.Wait();
            try
            {
                while (!stopAll)
                {
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " start\n");
                    Thread.Sleep(1000);
                    Dispatcher.Invoke(() => ConsoleBlock.Text += state?.ToString() + " finish\n");
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        #endregion
    }
}
