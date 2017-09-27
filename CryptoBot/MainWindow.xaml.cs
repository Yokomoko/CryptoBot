using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security;
using System.Timers;
using System.Windows.Controls;
using BusinessLayer;
using BusinessLayer.Models;
using CryptoBot.Code.Connection;
using CryptoBot.Dialogs;
using CryptoBot.Properties;
using CryptoBot.Setup;

namespace CryptoBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Timer ValueTimer = new Timer();
        public MainWindow()
        {
            InitializeComponent();
            //#if DEBUG
            //            Settings.Default.APISecureKey = "";
            //            Settings.Default.APISecureSecret = "";
            //            Settings.Default.InitialSetupCompleted = false;
            //            Settings.Default.Save();
            //#endif
            //   ValueTimer.Elapsed += (sender, args) =>
            //   {
            //       //Nothing
            //   };

            if (string.IsNullOrEmpty(Settings.Default.APISecureKey))
            {
                var window = new ApiSetup();
                window.ShowDialog();
                if (window.DialogResult == true)
                {
                    Stats.Visibility = Visibility.Visible;
                }
            }
            else
            {
                Stats.Visibility = Visibility.Visible;
            }
        }

        private void UxBuyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Buy.Visibility = Visibility.Visible;
            Stats.Visibility = Visibility.Collapsed;
            SettingsUc.Visibility = Visibility.Collapsed;
            Sell.Visibility = Visibility.Collapsed;
            Buy.Populate();
        }

        private void uxStatsBtn_Click(object sender, RoutedEventArgs e)
        {
            Stats.Visibility = Visibility.Visible;
            Buy.Visibility = Visibility.Collapsed;
            SettingsUc.Visibility = Visibility.Collapsed;
            Sell.Visibility = Visibility.Collapsed;
            Donate.Visibility = Visibility.Collapsed;
            Title = "CryptoBot - Statistics";
        }

        private void uxSellBtn_Click(object sender, RoutedEventArgs e)
        {
            Stats.Visibility = Visibility.Collapsed;
            Buy.Visibility = Visibility.Collapsed;
            SettingsUc.Visibility = Visibility.Collapsed;
            Donate.Visibility = Visibility.Collapsed;
            Sell.Visibility = Visibility.Visible;
            Sell.Populate();
        }

        private void UxSettingsBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Stats.Visibility = Visibility.Collapsed;
            Buy.Visibility = Visibility.Collapsed;
            SettingsUc.Visibility = Visibility.Visible;
            Donate.Visibility = Visibility.Collapsed;
            Sell.Visibility = Visibility.Collapsed;
            SettingsUc.Populate();
            Title = "CryptoBot - Settings";
        }

        private void UxDonateBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Stats.Visibility = Visibility.Collapsed;
            Buy.Visibility = Visibility.Collapsed;
            SettingsUc.Visibility = Visibility.Collapsed;
            Sell.Visibility = Visibility.Collapsed;
            Donate.Visibility = Visibility.Visible;

            Title = "CryptoBot - Donation";
        }
    }
}
