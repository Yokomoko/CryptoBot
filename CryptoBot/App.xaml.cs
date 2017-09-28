using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using BusinessLayer.BusinessLogic;
using CryptoBot.Code.Connection;
using Hardcodet.Wpf.TaskbarNotification;


namespace CryptoBot
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        

        private static readonly Timer timer = new Timer();
        private static readonly Timer scheduleTimer = new Timer();
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ScheduleHandler.LoadMasterScheduleFromFile();
            Task.Run(() => Populate());
            
            timer.Interval = 10000;
            timer.AutoReset = true;
            timer.Elapsed += TimerOnElapsed;
            scheduleTimer.Interval = CryptoBot.Properties.Settings.Default.CheckScheduledTime * 60000;
            scheduleTimer.AutoReset = true;
            scheduleTimer.Elapsed += ScheduleTimer_Elapsed;
            scheduleTimer.Start();
            timer.Start();
            
        }

        private void Tb_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Windows[0].WindowState = WindowState.Normal;
        }

        private void ScheduleTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => Connection.ProcessSchedules());
        }


        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Task.Run(() => Populate());
        }

        private void Populate()
        {
            if (CryptoBot.Properties.Settings.Default.InitialSetupCompleted)
            {
                try
                {
                    var task = new Task<Task>(async () =>
                                              {
                                                  await Code.Connection.Connection.Populate();
                                              });
                    task.Start();
                    task.Wait();
                    task.Result.Wait();
                }
                catch (Exception e)
                {
                    //  MessageBox.Show($"Error: {e.Message}");
                }
            }
        }
    }
}
