using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BusinessLayer.Models;
using CryptoBot.Code.Connection;

namespace CryptoBot.Dialogs
{
    /// <summary>
    /// Interaction logic for MarketSelector.xaml
    /// </summary>
    public partial class MarketSelector : Window
    {
        public Delegate MarketSelectedEvent;

        public EventHandler MarketSelected;

        private List<MarketSummary> MarketSummaries { get; set; }
        public MarketSummary SelectedMarket => ((MarketSummary)uxFilterListDg.SelectedItem);

        public MarketSelector()
        {
            InitializeComponent();
            MarketSummaries = Connection.FullMarketList.Where(d => !d.MarketName.Contains("USDT")).ToList();
            PopulateGrid();
            uxMarketNameTxt.Focus();
        }



        private void PopulateGrid()
        {
            var filteredSummaries = MarketSummaries.Where(d => d.MarketName.ToLower().Contains(uxMarketNameTxt.Text.ToLower()));
            uxFilterListDg.ItemsSource = filteredSummaries;
        }

        private void uxMarketNameTxt_TextChanged(object sender, TextChangedEventArgs e)
        {
            PopulateGrid();
        }

        private void RowDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (System.Windows.Interop.ComponentDispatcher.IsThreadModal)
            {
                DialogResult = true;
            }
            else
            {
                this.Close();
            }
        }

        private void uxFilterListDg_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
