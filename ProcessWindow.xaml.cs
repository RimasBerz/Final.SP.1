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
using System.Diagnostics;
using System.Data.SqlTypes;
using System.Threading;
using Microsoft.Win32;

namespace SystemProgramming1
{
    public partial class ProcessWindow : Window
    {
        private Dictionary<string, List<Process>> processDict = new();
        public ProcessWindow()
        {
            InitializeComponent();
        }
        //private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        //{
        //    Process[] processes = Process.GetProcesses();
        //    Stopwatch sw = Stopwatch.StartNew();
        //    foreach (Process process in processes)
        //    {
        //        List<Process> list;
        //        if (processDict.ContainsKey(process.ProcessName))
        //        {
        //            list = processDict[process.ProcessName];
        //            // если нет исключения, то процесс с этим именем уже в словаре
        //            list.Add(process);
        //        }
        //        else   // исключение - если нет такого имени в словаре
        //        {
        //            list = new List<Process>();
        //            list.Add(process);
        //            processDict[process.ProcessName] = list;
        //        }
        //    }
        //    sw.Stop();
        //    timeElapsed.Content = sw.ElapsedTicks + "tck";
        //    foreach (var pair in processDict)
        //    {
        //        TreeViewItem node = new() { Header = pair.Key };

        //        foreach (Process process in pair.Value)
        //        {
        //            TreeViewItem subnode = new() { Header = process.Id };
        //            node.Items.Add(subnode);
        //        }

        //        treeView.Items.Add(node);
        //    }

        private void ShowProcesses_Click(object sender, RoutedEventArgs e)
        {
            ShowProcesses.IsEnabled = false;
            new Thread(UpdateProcesses).Start();
        }
        private void UpdateProcesses()
        { 
            Process[] processes = Process.GetProcesses();
            Stopwatch sw = Stopwatch.StartNew();
            foreach (Process process in processes)
            {
                List<Process> list;
                if (processDict.ContainsKey(process.ProcessName))
                {
                    list = processDict[process.ProcessName];
                    // если нет исключения, то процесс с этим именем уже в словаре
                    list.Add(process);
                }
                else   // исключение - если нет такого имени в словаре
                {
                    list = new List<Process>();
                    list.Add(process);
                    processDict[process.ProcessName] = list;
                }
            }
            sw.Stop();

            Dispatcher.Invoke(() => 
                {
                    timeElapsed.Content = sw.ElapsedTicks + "tck";
                    treeView.Items.Clear();
                    foreach (var pair in processDict)
                    {
                        TreeViewItem node = new() { Header = pair.Key };

                        foreach (Process process in pair.Value)
                        {
                            TreeViewItem subnode = new() { Header = process.Id };
                            node.Items.Add(subnode);
                        }

                        treeView.Items.Add(node);
                    }
                    ShowProcesses.IsEnabled = true;
                });
            /*
             foreach (Process process in processes)
            {
                TreeViewItem node = new TreeViewItem();
                node.Header = process.ProcessName;
                node.Tag = process;
                TreeViewItem subnode = new TreeViewItem();
                subnode.Header = process.Id;
                node.Items.Add(subnode);
                treeView.Items.Add(node);
            }*/
        }

        private void StartNotepadProcesses_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                notepadProcess = Process.Start("notepad.exe", openFileDialog.FileName);
                if (notepadProcess is not null)
                {
                    ShowProcesses2stop.IsEnabled = true;
                    StartNotepad.IsEnabled = false;
                }
            }
        }
        private Process notepadProcess;
        private void StopNotepadProcesses_Click(object sender, RoutedEventArgs e)
        {
            if (notepadProcess is not null)
            {
                notepadProcess.Kill();

                ShowProcesses2stop.IsEnabled = false;
                StartNotepad.IsEnabled = true;

                notepadProcess = null!;
            }
        }
        private Process notepadProcess2;
        private void StartBNotepadProcesses_Click(object sender, RoutedEventArgs e)
        {
            notepadProcess2 = Process.Start(@"C:\Program Files\Google\Chrome\Application\chrome.exe", @"https://youtu.be/dQw4w9WgXcQ");
            if (notepadProcess2 is not null)
            {
                ShowProcesses2stop.IsEnabled = true;
                StartNotepad.IsEnabled = false;
            }
        }
    }
}