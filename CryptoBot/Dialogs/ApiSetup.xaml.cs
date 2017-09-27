using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Bittrex.Api.Client._2.BusinessLogic;
using BusinessLayer;
using BusinessLayer.Models;
using CryptoBot.Properties;

namespace CryptoBot.Setup
{
    /// <summary>
    /// Interaction logic for ApiSetup.xaml
    /// </summary>
    public partial class ApiSetup : Window
    {
        public ApiSetup()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var secureKey = StringCipher.Encrypt(uxKeyTxt.Text.Trim());
            var secureSecret = StringCipher.Encrypt(uxSecretTxt.Text.Trim());

            var insecureKey = StringCipher.Decrypt(secureKey);
            var insecureSecret = StringCipher.Decrypt(secureSecret);

            BittrexClient bc = new BittrexClient(Settings.Default.APIBaseAddress, StringCipher.Decrypt(secureKey), StringCipher.Decrypt(secureSecret));

            ApiResult<OpenOrder[]> summaries = null;

            var task = new Task<Task>(async () =>
            {
                summaries = await bc.GetOpenOrders();
            });
            task.Start();
            task.Wait();
            task.Result.Wait();

            if (summaries != null && summaries.Success)
            {
                Settings.Default.APISecureKey = secureKey;
                Settings.Default.APISecureSecret = secureSecret;
                Settings.Default.InitialSetupCompleted = true;
                Settings.Default.Save();
                DialogResult = true;
            }
            else
            {
                MessageBox.Show(this, "Unable to connect to Bittrex API. Please check you have a valid internet connection, and that the key/secret are correct.\n\nError Code: " + summaries.Message);
                return;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (DialogResult != true)
            {
                Application.Current.Shutdown();
            }
        }
    }
}
