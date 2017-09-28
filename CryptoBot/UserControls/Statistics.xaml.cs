using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BusinessLayer.BusinessLogic;
using BusinessLayer.Models;
using CryptoBot.Code.Connection;

namespace CryptoBot.UserControls
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : UserControl
    {
        readonly Timer _refreshTimer = new Timer();
        public Statistics()
        {
            InitializeComponent();
            SetLoadingVisibility(true);
            Populate();
            _refreshTimer.AutoReset = true;
            _refreshTimer.Interval = 30000;
            _refreshTimer.Elapsed += RefreshTimerOnElapsed;
            _refreshTimer.Start();
        }

        private void RefreshTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var ShowLoading = new Task(() => SetLoadingVisibility(true));
            ShowLoading.RunSynchronously();
            ShowLoading.Wait();

            var PopulateLists = new Task(Populate);
            PopulateLists.RunSynchronously();
            PopulateLists.Wait();

            //Task.Run(() => Populate());

            var HideLoading = new Task(() => SetLoadingVisibility(false));
            HideLoading.RunSynchronously();
            HideLoading.Wait();
        }

        private void Populate()
        {
            if (Properties.Settings.Default.InitialSetupCompleted)
            {
                try
                {
                    if (Visibility == Visibility.Visible)
                    {
                        Dispatcher.Invoke(getOrders);
                        Dispatcher.Invoke(getBalances);
                        Dispatcher.Invoke(PopulateNotableFields);
                        Dispatcher.Invoke(PopulateSchedules);
                    }
                }
                catch (Exception e)
                {
                    //  MessageBox.Show($"Error: {e.Message}");
                }
            }
        }

        private void SetLoadingVisibility(bool visible)
        {
            Dispatcher.Invoke(() =>
            {
                uxBalanceLoad.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                uxOrdersLoad.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                uxInterestCurrenciesLoad.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
                uxSchedulesLoad.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
            });

        }

        private void getBalances()
        {
            if (Connection.AccountBalances.Count(d => d.Balance > 0) > 0)
            {
                uxAccountBalancesDg.ItemsSource = Connection.AccountBalances.Where(d => d.Balance > 0);
                uxBalanceLoad.Visibility = Visibility.Collapsed;
            }
            else
            {
                uxBalanceLoad.Visibility = Visibility.Visible;
            }

        }

        private void getOrders()
        {
            if (Connection.OpenOrders.Count(d => d.Quantity > 0) > 0)
            {
                uxOrdersDg.ItemsSource = Connection.OpenOrders.Where(d => d.Quantity > 0);
                uxOrdersLoad.Visibility = Visibility.Collapsed;
            }
            else
            {
                uxOrdersLoad.Visibility = Visibility.Visible;
            }

        }

        private void PopulateNotableFields()
        {
            uxInterestCurrenciesLoad.Visibility = Visibility.Visible;
            if (Connection.MarketSummaries.Count > 0)
            {
                uxNotableDg.ItemsSource = Connection.MarketSummaries;
                if (((ObservableCollection<MarketSummary>)uxNotableDg.ItemsSource).Any())
                {
                    uxInterestCurrenciesLoad.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                uxInterestCurrenciesLoad.Visibility = Visibility.Visible;
            }
        }

        private void PopulateSchedules()
        {
            UxScheduled.ItemsSource = ScheduleHandler.MasterSchedule?.Orders;
            uxSchedulesLoad.Visibility = Visibility.Collapsed;
        }

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //   uxAccountBalancesDg.Visibility = uxAccountBalancesDg.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
