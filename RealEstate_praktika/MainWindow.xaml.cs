using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RealEstate_praktika
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Icon = (BitmapImage)Application.Current.FindResource("AppIcon");
        }

        private void Btn_Agents_Click(object sender, RoutedEventArgs e)
        {
            WindowAgents windowAgents = new WindowAgents();
            windowAgents.Show();
        }

        private void Btn_Clients_Click(object sender, RoutedEventArgs e)
        {
            WindowClients windowClients = new WindowClients();  
            windowClients.Show();
        }

        private void Btn_RealEstate_Click(object sender, RoutedEventArgs e)
        {
            WindowRealEstates windowRealEstates = new WindowRealEstates();
            windowRealEstates.Show();
        }

        private void Btn_Demand_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_supply_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Deals_Click(object sender, RoutedEventArgs e)
        {
            WindowDeals windowDeals = new WindowDeals();
            windowDeals.Show();
        }
    }
}
