using System;
using System.Collections.Generic;
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
using CryptoBot.Properties;
using CryptoBot.Setup;

namespace CryptoBot.UserControls
{
    /// <summary>
    /// Interaction logic for Statistics.xaml
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();
            Populate();
        }

        public void Populate()
        {
            uxMinutesTxt.Text = Properties.Settings.Default.CheckScheduledTime.ToString();
        }


        private void UxSaveBtn_OnClick(object sender, RoutedEventArgs e)
        {
            long parsedMinutes;

            if (long.TryParse(uxMinutesTxt.Text, out parsedMinutes))
            {
                Properties.Settings.Default.CheckScheduledTime = parsedMinutes;
            }
            else
            {
                MessageBox.Show("Please enter a valid number of seconds");
                return;
            }
            Properties.Settings.Default.Save();
            MessageBox.Show("Saved Successfully");
        }

        private void UxResetApiBtn_OnClick(object sender, RoutedEventArgs e)
        {
            CryptoBot.Properties.Settings.Default.APISecureKey = null;
            CryptoBot.Properties.Settings.Default.APISecureSecret = null;
            CryptoBot.Properties.Settings.Default.InitialSetupCompleted = false;
            Properties.Settings.Default.Save();
            var parentWindow = Window.GetWindow(this);
            var win = new ApiSetup {
                Owner = parentWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.ShowDialog();
        }
    }
}
