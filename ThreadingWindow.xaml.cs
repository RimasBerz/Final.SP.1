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
using System.Windows.Shapes;

namespace SystemProgramming1
{
    /// <summary>
    /// Interaction logic for ThreadingWindow.xaml
    /// </summary>
    public partial class ThreadingWindow : Window
    {
            public ThreadingWindow()
            {
                InitializeComponent();
            }
        #region part 1 - зависание
        private void ButtonStart1_Click(object sender, RoutedEventArgs e)
            {
                Start1();
            }

            private void ButtonStop1_Click(object sender, RoutedEventArgs e)
            {

            }

            private void Start1()
            {
                for (int i = 0; i < 10; i++)
                {
                    progressBar1.Value = (i + 1) * 10;
                    ConsoleBlock.Text += i.ToString() + "\n";  // Console.WriteLine(i);
                    Thread.Sleep(300);
                }
            }
        #endregion
        #region part 2 - исключения
        private void ButtonStart2_Click(object sender, RoutedEventArgs e)
            {
                new Thread(Start1).Start();  //  - исключение (изменения из другого потока)
            }

            private void ButtonStop2_Click(object sender, RoutedEventArgs e)
            {

            }
        #endregion
        #region part 3 - Решение

        private void ButtonStart3_Click(object sender, RoutedEventArgs e)
            {
                new Thread(Start3).Start();
            }
        private bool isStopped;
            private void ButtonStop3_Click(object sender, RoutedEventArgs e)
            {
            isStopped = true;
            }
            private void Start3()
            {
                for (int i = 0; i < 10 && !isStopped; i++)
                {
                    this.Dispatcher.Invoke(() =>  // делегирование работы потоку окна
                    {
                        progressBar3.Value = (i + 1) * 10;
                        ConsoleBlock.Text += i.ToString() + "\n";  // Console.WriteLine(i);
                    });

                    Thread.Sleep(300);
                }
            }
        #endregion 
        #region part 4 - взаимодействие потоков
        private bool isStopped4;
        private int savedProgress4 = 0;
        private int savedIndex4;
        public int Count = 0;
        private void ButtonStart4_Click(object sender, RoutedEventArgs e)
        {
            isStopped4 = false;
            new Thread(Start4).Start( 0);
            if(savedIndex4 == 0)
            {
                ConsoleBlock.Text = "";
            }
            Count = +1;
        }

        private void ButtonStop4_Click(object sender, RoutedEventArgs e)
        {
            Count = -1;
        }
        private void Start4(object? startIndex)
        {
            while (Count == 1)
            {
                if (startIndex is int startForm)
                {
                    for (int i = startForm; i < 10; i++)
                    {
                        if (isStopped4)
                        {
                            savedIndex4 = i;
                            break;
                        }
                        this.Dispatcher.Invoke(() =>  // делегирование работы потоку окна
                        {
                            progressBar4.Value = (i + 1) * 10;
                            ConsoleBlock.Text += i.ToString() + "\n";  // Console.WriteLine(i);
                        });

                        Thread.Sleep(300);
                    }
                }
            }
            
        }
        #endregion
    }
}