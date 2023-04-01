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
using System.Windows.Markup;
using System.Linq.Expressions;

namespace SystemProgramming1
{
    /// <summary>
    /// Interaction logic for MultiWindow.xaml
    /// </summary>
    public partial class MultiWindow : Window
    {
        private Random random = new();
        private CancellationTokenSource cts;

        public MultiWindow()
        {
            InitializeComponent();
        }
        #region 1
        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
        {
            sum = 100;
            progressBar1.Value = 0;
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent).Start();
            }
        }

        private void ButtonStop1_Click(object sender, RoutedEventArgs e)
        {

        }
        private double sum;
        private void plusPercent()
        {
            double val = sum;
            Thread.Sleep(random.Next(250, 300));
            double percent = 10;
            val *= 1 + percent / 100;
            sum = val;
            Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += sum + "\n";
                progressBar1.Value += 100.0 / 12;
            });
        }
        #endregion
        #region 2
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
        {
            sum2 = 100;
            progressBar2.Value = 0;
            for (int i = 0; i < 12; i++)
            {
                new Thread(plusPercent2).Start();
            }
        }

        private void ButtonStop2_Click(object sender, RoutedEventArgs e)
        {

        }
        private double sum2;
        private readonly object locker2 = new();
        private void plusPercent2()
        {
            double val;
            lock (locker2)
            {
                val = sum2;
                Thread.Sleep(random.Next(250, 300));
                double percent = 10;
                val *= 1 + percent / 100;
                sum2 = val;
            }
            Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += sum2 + "\n";
                progressBar2.Value += 100.0 / 12;
            });
        }
        #endregion
        #region 3
        //private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        //{
        //    sum3 = 100;
        //    progressBar3.Value = 0;
        //    for (int i = 0; i < 12; i++)
        //    {
        //        new Thread(plusPercent3).Start(i + 1);
        //    }
        //}

        //private void ButtonStop3_Click(object sender, RoutedEventArgs e)
        //{

        //}
        //private double sum3;
        //private readonly object locker3 = new();
        //private void plusPercent3(object? month)
        //{
        //    if (month is not int) return;

        //    double val;

        //    Thread.Sleep(random.Next(250, 300));
        //    double percent = 10 + (int)month;
        //    double factor = 1 + percent / 100;

        //    lock (locker3)
        //    {
        //        val = sum3;
        //        val *= factor;  
        //        sum3 = val;

        //    }
        //    Dispatcher.Invoke(() => {
        //        ConsoleBlock.Text += month + "---" + val + "\n";
        //        progressBar3.Value += 100.0 / 12;
        //    });
        //}
        private List<Thread> threads = new List<Thread>();
        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
        {
            sum3 = 100;
            progressBar3.Value = 0;
            cts = new CancellationTokenSource();
            for (int i = 0; i < 12; i++)
            {
                Thread thread = new Thread(plusPercent3);
                threads.Add(thread);
                thread.Start(new ThreadParams { Month = i + 1, Token = cts.Token });
            }
        }

        private void ButtonStop3_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel();
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            threads.Clear();
        }

        private double sum3;
        private readonly object locker3 = new();
        private void plusPercent3(object? data)
        {
            if (data is not ThreadParams threadParams) return;
            CancellationToken cancellationToken = threadParams.Token;

            double val;

            Thread.Sleep(random.Next(250, 300));
            double percent = 10 + threadParams.Month;
            double factor = 1 + percent / 100;

            lock (locker3)
            {
                if (cancellationToken.IsCancellationRequested) return;
                val = sum3;
                val *= factor;
                sum3 = val;
            }
            Dispatcher.Invoke(() =>
            {
                ConsoleBlock.Text += threadParams.Month + "---" + val + "\n";
                progressBar3.Value += 100.0 / 12;
            });
        }

        private class ThreadParams
        {
            public int Month { get; set; }
            public CancellationToken Token { get; set; }
        }

        #endregion
        //public int Count = 1;
        //private void ButtonStart4_Click(object sender, RoutedEventArgs e)
        //{
        //    Count++;
        //    sum4 = 100;
        //    progressBar4.Value = 0;
        //    cts = new();
        //    for (int i = 0; i < 12; i++)
        //    {
        //        new Thread(plusPercent4).Start(cts.Token);
        //    }
        //}

        //private void ButtonStop4_Click(object sender, RoutedEventArgs e)
        //{
        //    cts?.Cancel();
        //    Count--;
        //    if(Count == 2)
        //    {

        //    }
        //}
        //private double sum4;
        //private readonly object locker4 = new();
        //private void plusPercent4(object? token)
        //{
        //    if (token is not CancellationToken) return;
        //    CancellationToken cancellationToken =(CancellationToken)token;

        //    double val;
        //    lock (locker4)
        //    {

        //        if (cancellationToken.IsCancellationRequested) return;
        //        val = sum4;
        //        Thread.Sleep(random.Next(250, 300));
        //        double percent = 10;
        //        val *= 1 + percent / 100;
        //        sum4 = val;
        //    }
        //    Dispatcher.Invoke(() => {
        //        ConsoleBlock.Text += val + "\n";
        //        progressBar4.Value += 100.0 / 12;
        //    });
        //}
        #region 4
        private bool isThreadRunning = false;
        private Thread thread;

        private void ButtonStart4_Click(object sender, RoutedEventArgs e)
        {
            if (!isThreadRunning)
            {
                isThreadRunning = true;
                thread = new Thread(plusPercent4);
                thread.Start();
                ButtonStart4.IsEnabled = false;
            }
        }

        private void ButtonStop4_Click(object sender, RoutedEventArgs e)
        {
            if (isThreadRunning)
            {
                thread.Abort();
                isThreadRunning = false;
                ButtonStart4.IsEnabled = true;
                ButtonStart4.Content = "Resume";
            }
            else
            {
                isThreadRunning = true;
                thread = new Thread(plusPercent4);
                thread.Start();
                ButtonStart4.Content = "Start";
            }
        }

        private void plusPercent4()
        {
            try
            {
                // Some long running operation here
            }
            catch (ThreadAbortException)
            {
                // Thread was aborted
                Dispatcher.Invoke(() =>
                {
                    ButtonStart4.Content = "Resume";
                    ButtonStart4.IsEnabled = true;
                });
                Thread.ResetAbort();
            }
            isThreadRunning = false;
            Dispatcher.Invoke(() =>
            {
                ButtonStart4.IsEnabled = true;
                ButtonStart4.Content = "Start";
            });
        }
        #endregion
        #region Thread Pool
        CancellationTokenSource cts5;
        private void ButtonStart5_Click(object sender, RoutedEventArgs e)
        {
            cts5 = new CancellationTokenSource();
            for (int i = 0; i < 25; i++)
            {
                ThreadPool                     // Пул потоков (new не надо)
                    .QueueUserWorkItem(        // добавление новой задачи
                    plusPercent5,              // (она сразу ставится на исполнение - 
                    new ThreadData3            //  отдельный Start() не нужен)
                    {                          // Дополнительный аргумент также
                        Month = i,             // может быть только один и 
                        Token = cts5.Token     // передается в поток вторым параметром
                    });                        // при добавлении
            }                                  // 
        }
        private void ButtonStop5_Click(object sender, RoutedEventArgs e)
        {
            cts5?.Cancel();
        }
        private double sum5;
        private readonly object locker5 = new();     // объект для синхронизации
        private void plusPercent5(object? data)
        {

            var threadData = data as ThreadData3;
            if (threadData is null) return;
            double val;
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    Thread.Sleep(random.Next(250, 350));
                    // место для возможной отмены потока
                    threadData.Token.ThrowIfCancellationRequested();
                }
                double percent = 10 + threadData.Month;
                double factor = 1 + percent / 100;
                lock (locker5)
                {                                      // внутри блока
                    val = sum5;                        // остается часть рассчетов
                    val *= factor;                     // которую нельзя более
                    sum5 = val;                        // разделять
                }
                Dispatcher.Invoke(() =>
                {
                    ConsoleBlock.Text += threadData.Month + " " + percent + " " + val + "\n";
                    progressBar5.Value += 100.0 / 25;
                });

            }
            catch (OperationCanceledException)
            {
                Dispatcher.Invoke(() =>
               {
                   ConsoleBlock.Text += threadData.Month + " Canceled" + "\n";
                   progressBar5.Value += 100.0 / 25;
               });
            }

          
        }

        class ThreadData3   // комплексный тип для передачи нескольких данных в поток
        {
            public int Month { get; set; }
            public CancellationToken Token { get; set; }
        }
        #endregion
        private readonly object locker = new();
        private async void ButtonStart6_Click(object sender, RoutedEventArgs e)
        {
            cts = new CancellationTokenSource();
            for (int i = 0; i < 12; i++)
            {
                int month = i + 1;
                Task task = Task.Run(() => CalculateInterest(month, cts.Token));
                await Task.Delay(random.Next(250, 350)); // случайная задержка
            }
        }

        private void ButtonStop6_Click(object sender, RoutedEventArgs e)
        {
            cts?.Cancel();

        }
        private void CalculateInterest(int month, CancellationToken token)
        {
            try
            {
                double percent = 10 + month;
                double factor = 1 + percent / 100;
                double value;
                lock (locker)
                {
                    value = sum;
                    value *= factor;
                    sum = value;
                }
                Dispatcher.Invoke(() =>
                {
                    ConsoleBlock.Text += $"{month} {percent} {value}\n";
                    progressBar6.Value += 100.0 / 12;
                });
            }
            catch (OperationCanceledException)
            {
                Dispatcher.Invoke(() =>
                {
                    ConsoleBlock.Text += $"{month} Canceled\n";
                    progressBar6.Value += 100.0 / 12;
                });
            }
        }

        private void progressBar6_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}