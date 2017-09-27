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
            Task.Run(() => Populate());
            timer.Interval = 10000;
            timer.AutoReset = true;
            timer.Elapsed += TimerOnElapsed;
            scheduleTimer.Interval = CryptoBot.Properties.Settings.Default.CheckScheduledTime * 60000;
            scheduleTimer.AutoReset = true;
            scheduleTimer.Elapsed += ScheduleTimer_Elapsed;
            scheduleTimer.Start();
            timer.Start();
            XDocument doc = new XDocument();

            if (!File.Exists("Schedule.xml"))
            {
                doc.Add(new XElement("Schedule"));
                //doc.Element("Schedule")?.Add(new XElement("Market"));
                //doc.Element("Schedule")?.Add(new XElement("Order"));
                //doc.Element("Order")?.Add(new XElement("Bid"));
                //doc.Element("Order")?.Add(new XElement("Qty"));
                //doc.Element("Order")?.Add(new XElement("Expiry"));
                doc.Save("Schedule.xml");
                //string marketName = "BTC-LTC";

                //var order = new Order
                //{
                //    OrderType = OrderType.Buy,
                //    Bid = (decimal)2.0,
                //    Qty = (decimal)2.0,
                //    Expiry = DateTime.Now,
                //    Sent = null
                //};
                //ScheduleHandler.AddOrder(marketName, order);
            }
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
