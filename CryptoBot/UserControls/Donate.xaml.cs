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
    public partial class Donate : UserControl
    {
        private List<CoinAddresses> AddressList = new List<CoinAddresses>();

        public Donate()
        {
            InitializeComponent();
            Populate();
        }

        public void Populate()
        {
            PopulateAddressList();
        }

        private void PopulateAddressList()
        {
            AddressList.Clear();
            AddressList.Add(new CoinAddresses { Abbreviation = "BTC", Address = "1PyhngKTzTArwYFPFDDucTVissYD5yXeQf", Label = "Bitcoin" });
            AddressList.Add(new CoinAddresses { Abbreviation = "ETH", Address = "0x8b50935335D263dD16ADcdcf129708595CaA45B8", Label = "Ethereum" });
            AddressList.Add(new CoinAddresses { Abbreviation = "XRP", Address = "rPVMhWBsfF9iMXYj3aAzJVkPDTFNSyWdKy", Label = "Ripple", Tag = "1597612855" });
            AddressList.Add(new CoinAddresses { Abbreviation = "BCC/BCH", Address = "1HAangNwpQWWMuJoQhG5brUyHzhxV9Xt9u", Label = "Bitcoin Cash" });
            AddressList.Add(new CoinAddresses { Abbreviation = "LTC", Address = "LgsT4MEFJK126RmmMGaUZ4UvUMoc8RaMur", Label = "Litecoin" });
            AddressList.Add(new CoinAddresses { Abbreviation = "DASH", Address = "XvNDpNBdAUbgGmZ7zmKXq92fccGCs8KP7Y", Label = "Dash" });
            AddressList.Add(new CoinAddresses { Abbreviation = "NEO", Address = "AdfupkQqVLunN9vHLXNDXebqH7Cc9M7jjb", Label = "Neo" });
            //AddressList.Add(new CoinAddresses { Abbreviation = "XMR", Address = "ID: aa5db566979a4358888ffd29694328a6e54c3b4861164e2183b81d2eacba38ca", Label = "Monero", Tag = "Base: 463tWEBn5XZJSxLU6uLQnQ2iY9xuNcDbjLSjkn3XAXHCbLrTTErJrBWYgHJQyrCwkNgYvyV3z8zctJLPCZy24jvb3NiTcTJ" });
            AddressList.Add(new CoinAddresses { Abbreviation = "ETC", Address = "0xe0aeca80a4578c451572fba08c8af79c8be5ee37", Label = "Ethereum Classic" });
            AddressList.Add(new CoinAddresses { Abbreviation = "OMG", Address = "0x3997be4083ae005efd12baecbb2fb1d47944f56d", Label = "OmiseGO" });
            AddressList.Add(new CoinAddresses { Abbreviation = "DCR", Address = "DsoWYL2HvyCk749znPjhW8Pcw1mTQb4Yh8F", Label = "Decred" });
            AddressList.Add(new CoinAddresses { Abbreviation = "VTC", Address = "ViPnrR2yk9atSZxtdFibvLv1FcEmjmHaou", Label = "Vertcoin" });
            AddressList.Add(new CoinAddresses { Abbreviation = "XVG", Address = "DHBiQT1iSfKhH2aDFbZzKaXAKbocWDtuxs", Label = "Verge" });
            AddressList.Add(new CoinAddresses { Abbreviation = "NXS", Address = "2RZGmqUjRmMzjUaxaGjTjpuS1kjj6QFvjrrkQHxxkDcvf6rkevW", Label = "Nexus Earth" });
            AddressList.Add(new CoinAddresses { Abbreviation = "GNT", Address = "0x18881be7017dea51019a674b734cd3d43ac0b2ac", Label = "Golem" });

            uxDonateGrid.ItemsSource = AddressList;
        }

        private void UxName_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var filtered = AddressList.Where(d => d.Label.ToLower().Contains(uxName.Text.ToLower()) || d.Abbreviation.ToLower().Contains(uxName.Text.ToLower()));
            uxDonateGrid.ItemsSource = filtered;
        }

        public class CoinAddresses
        {
            public enum OrderType
            {
                BTC,
                ETH,
                XRP,
                BCH,
                LTC,
                DASH,
                NEO,
                XMR,
                ETC,
                OMG,
                DCR,
                VTC,
                GNT,
                NXS,
                WTC,
                XVG,
                QTUM,
                ARK
            }

            public string Label { get; set; }
            public string Abbreviation { get; set; }
            public string Address { get; set; }
            public string Tag { get; set; }
        }


  
    }
}
