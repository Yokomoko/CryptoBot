using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BusinessLayer.Models;
using CryptoBot.Code.Connection;
using CryptoBot.Properties;

namespace CryptoBot.UserControls
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Statistics : UserControl
    {
        readonly Timer refreshTimer = new Timer();
        public Statistics()
        {
            InitializeComponent();
            SetLoadingVisibility(true);
            Populate();
            refreshTimer.AutoReset = true;
            refreshTimer.Interval = 30000;
            refreshTimer.Elapsed += RefreshTimerOnElapsed;
            refreshTimer.Start();
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
                    }
                }
                catch (Exception e)
                {
                    //  MessageBox.Show($"Error: {e.Message}");
                }
            }
        }

        private void SetLoadingVisibility(bool Visible)
        {
            Dispatcher.Invoke(() =>
            {
                uxBalanceLoad.Visibility = Visible ? Visibility.Visible : Visibility.Collapsed;
                uxOrdersLoad.Visibility = Visible ? Visibility.Visible : Visibility.Collapsed;
                uxInterestCurrenciesLoad.Visibility = Visible ? Visibility.Visible : Visibility.Collapsed;
            });

        }

        private void getBalances()
        {
            if (Connection.AccountBalances.Count(d => d.Balance > 0) > 0)
            {
                uxAccountBalancesDg.ItemsSource = Code.Connection.Connection.AccountBalances.Where(d => d.Balance > 0);
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

        private void UIElement_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e){
         //   uxAccountBalancesDg.Visibility = uxAccountBalancesDg.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
