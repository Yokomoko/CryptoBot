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
using Hardcodet.Wpf.TaskbarNotification;

namespace CryptoBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TaskbarIcon tb;
        //Timer ValueTimer = new Timer();
        public MainWindow()
        {
            InitializeComponent();
            tb = (TaskbarIcon)FindResource("NotificationIcon");
            tb.Visibility = Visibility.Visible;
            tb.TrayMouseDoubleClick += TbOnTrayMouseDoubleClick;

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
                    uxStatsBtn.IsChecked = true;
                }
            }
            else
            {
                Stats.Visibility = Visibility.Visible;
                uxStatsBtn.IsChecked = true;
            }
        }

        private void TbOnTrayMouseDoubleClick(object sender, RoutedEventArgs routedEventArgs)
        {
            this.WindowState = WindowState.Normal;
        }

        private void UxBuyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            Buy.Visibility = Visibility.Visible;
            Stats.Visibility = Visibility.Collapsed;
            SettingsUc.Visibility = Visibility.Collapsed;
            Sell.Visibility = Visibility.Collapsed;
            Donate.Visibility = Visibility.Collapsed;
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

        private void Window_StateChanged(object sender, EventArgs e)
        {
            switch (this.WindowState)
            {
                case WindowState.Maximized:
                break;
                case WindowState.Minimized:
                tb.ShowBalloonTip("Minimised to Taskbar", "The application will continue to run in the background and schedule your trades", BalloonIcon.Info);
                tb.Visibility = Visibility.Visible;

                ShowInTaskbar = false;
                break;
                case WindowState.Normal:
                tb.Visibility = Visibility.Collapsed;
                ShowInTaskbar = true;
                break;
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            tb.Visibility = Visibility.Collapsed;
        }
    }
}
