using System;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CryptoBot.Dialogs;
using BusinessLayer.Models;
using CryptoBot.Code.Connection;
using Timer = System.Timers.Timer;
using BusinessLayer.BusinessLogic;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Data;

namespace CryptoBot.UserControls
{
    /// <summary>
    /// Interaction logic for BuyScreen.xaml
    /// </summary>
    public partial class BuyScreen : UserControl
    {
        private MarketSummary SelectedMarket = null;
        private AccountBalance CurrentBalance;
        private readonly Timer timer = new Timer();

        public BuyScreen()
        {
            InitializeComponent();
            uxPriceCmbo_OnSelectionChanged(null, null);
            timer.Interval = 10000;
            timer.Elapsed += TimerOnElapsed;
            timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (SelectedMarket != null)
            {
                if (Visibility == Visibility.Visible)
                {
                    Task.Run(() => Populate());
                }
            }
        }

        public void Populate()
        {
            Dispatcher.Invoke(() =>
            {
                Window parentWindow = Window.GetWindow(this);
                if (parentWindow != null)
                {
                    parentWindow.Title = "CryptoBot - Buy Screen";
                }
            });


            if (SelectedMarket != null)
            {
                var market = Connection.FullMarketList.FirstOrDefault(d => d.MarketName == SelectedMarket.MarketName);
                if (market != null)
                {
                    Dispatcher.Invoke(PopulateTotal);

                    SelectedMarket = market;
                    var baseCurrency = SelectedMarket.MarketName.Split('-')[0];

                    ApiResult<AccountBalance> result = null;

                    var task = new Task<Task>(async () =>
                    {
                        result = await Connection.bittrexClient.GetBalance(baseCurrency);
                    });
                    task.Start();
                    task.Wait();
                    task.Result.Wait();
                    if (result != null && result.Success)
                    {
                        CurrentBalance = result.Result;
                    }
                }
                Dispatcher.Invoke(() =>
                {
                    Window parentWindow = Window.GetWindow(this);
                    if (parentWindow != null && SelectedMarket != null)
                    {
                        parentWindow.Title += ": " + SelectedMarket.MarketName;
                    }

                    BuyGroupBoxText.Content = "Buy " + SelectedMarket.MarketName.Split('-')[1];
                    uxBuySp.Visibility = Visibility.Visible;
                    InitialPanel.Visibility = Visibility.Collapsed;
                    uxLastVl.Content = SelectedMarket.Last;
                    uxVolVl.Content = Math.Round(SelectedMarket.BaseVolume, 2);
                    uxBidVl.Content = SelectedMarket.Bid;
                    uxAskVl.Content = SelectedMarket.Ask;
                    uxHighVl.Content = SelectedMarket.High;
                    uxLowVl.Content = SelectedMarket.Low;
                    PopulateGrid();
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    uxBuySp.Visibility = Visibility.Collapsed;
                });
            }
        }

        private void uxSelectMarket_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = Window.GetWindow(this);
            var win = new MarketSelector
            {
                Owner = parentWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.ShowDialog();
            if (win.DialogResult == true)
            {
                SelectedMarket = win.SelectedMarket;
                Populate();
            }
        }

        private void TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            decimal result;
            var textBox = sender as TextBox;
            PopulateTotal();
            if (textBox != null) e.Handled = !decimal.TryParse(textBox.Text + e.Text, out result);
        }

        private void UxRefreshBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Populate());
        }

        private void PopulateTotal()
        {
            if (!string.IsNullOrEmpty(uxBidTxt.Text) && !string.IsNullOrEmpty(uxUnitsTxt.Text))
            {
                var text = (decimal.Parse(uxBidTxt.Text) * decimal.Parse(uxUnitsTxt.Text)).ToString();
                uxTotalTxt.Text = text.Length >= 10 ? text.Substring(0, 10) : text;
            }
            else
            {
                uxTotalTxt.Text = "0.00000000";
            }
        }

        private void uxPriceCmbo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (uxBuySp.Visibility == Visibility.Visible)
            {
                switch (uxPriceCmbo.SelectedIndex)
                {
                    case 0:
                        uxBidTxt.Text = SelectedMarket?.Last.ToString() ?? "0.00000000";
                        break;
                    case 1:
                        uxBidTxt.Text = SelectedMarket?.Bid.ToString() ?? "0.00000000";
                        break;
                    case 2:
                        uxBidTxt.Text = SelectedMarket?.Ask.ToString() ?? "0.00000000";
                        break;
                }
                Populate();
            }
        }

        private void UxMaxSwitch_OnChecked(object sender, RoutedEventArgs e)
        {
            if (uxMaxSwitch.IsChecked == true)
            {
                uxUnitsTxt.Text = decimal.Parse(uxBidTxt.Text) == 0 ? "0.0000000" : (CurrentBalance.Available / decimal.Parse(uxBidTxt.Text)).ToString().Substring(0, 10);
                uxUnitsTxt.IsEnabled = false;
            }
            else
            {
                uxUnitsTxt.IsEnabled = true;
            }
            Task.Run(() => Populate());
        }

        private void uxBuyBtn_Click(object sender, RoutedEventArgs e)
        {
            //Validate
            var baseCurrency = SelectedMarket.MarketName.Split('-')[0];
            var currencyToBuy = SelectedMarket.MarketName.Split('-')[1];
            var minTradeSize = Connection.GetMinimumBuyForCurrency(currencyToBuy);

            if (decimal.Parse(uxTotalTxt.Text) < minTradeSize)
            {
                MessageBox.Show($"Trade is less than the minimum trade size of {minTradeSize}. Please increase your bid or quantity and try again.");
                return;
            }

            var order = new Order
            {
                MarketName = SelectedMarket.MarketName,
                Bid = decimal.Parse(uxBidTxt.Text),
                Qty = decimal.Parse(uxUnitsTxt.Text),
                OrderType = OrderType.Buy,
                Expiry = DateTime.Now.AddDays(7)
            };

            if (uxTimeInForceCmbo.SelectedIndex == 1)
            {
                ScheduleHandler.AddOrder(SelectedMarket.MarketName, order);
                MessageBox.Show($"Order has been scheduled and will complete when you have {uxTotalTxt.Text} {baseCurrency} available");
                PopulateGrid();
                return;
            }
            else
            {
                order.ActualBid = order.Bid;
                try
                {
                    var task = new Task<Task>(async () =>
                    {
                        await Connection.SendBuyOrder(SelectedMarket.MarketName, order);
                    });
                    task.Start();
                    task.Wait();
                    task.Result.Wait();
                    MessageBox.Show("Order Sent Successfully");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to send order:\n{ex.InnerException.Message}");
                    return;
                }
            }
        }

        private void uxBidTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            PopulateTotal();
        }

        private void uxUnitsTxt_LostFocus(object sender, RoutedEventArgs e)
        {
            PopulateTotal();
        }

        private void PopulateGrid()
        {
            try
            {
                var orders = ScheduleHandler.MasterSchedule.Orders.Where(d => d.MarketName == SelectedMarket.MarketName && d.OrderType == OrderType.Buy).OrderByDescending(d => d.Sent ?? DateTime.Today.AddYears(-50)).ThenBy(f => f.CreatedTime);

                Binding b = new Binding();
                b.Mode = BindingMode.OneWay;
                b.Source = orders;


                if (!orders.Any())
                {
                    uxNoBuysLbl.Visibility = Visibility.Visible;
                    uxSchedules.Visibility = Visibility.Collapsed;
                }
                else
                {
                    uxSchedules.SetBinding(ItemsControl.ItemsSourceProperty, b);
                    uxSchedules.Visibility = Visibility.Visible;
                    uxNoBuysLbl.Visibility = Visibility.Collapsed;
                }

            }
            catch { }

        }
    }
}
