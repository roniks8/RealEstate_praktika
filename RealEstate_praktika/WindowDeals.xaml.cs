using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
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
using System.Windows.Shapes;
using System.IO;

namespace RealEstate_praktika
{
    /// <summary>
    /// Логика взаимодействия для WindowDeals.xaml
    /// </summary>
    public partial class WindowDeals : Window
    {
        private Dictionary<string, int> supplyItemsDict = new Dictionary<string, int>();
        private Dictionary<string, int> demandItemsDict = new Dictionary<string, int>();
        public WindowDeals()
        {
            InitializeComponent();
            LoadDataFromDatabaseSupply();
            LoadDataFromDatabaseDemand();
            LoadSupplyCb(); 
            LoadDemandCb();
        }
        private void LoadDataFromDatabaseSupply()
        {
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT s.Id_Supply, s.Price, s.Id_Client, s.Id_Agent, s.Id_RealEstate, c.Id_Client, " +
                        "CONCAT(p.LastName,' ', p.FirstName, ' ', p.MiddleName) as FullClient, " +
                        "CONCAT(pa.LastName,' ', pa.FirstName, ' ', pa.MiddleName) as FullAgent, " +
                        "CONCAT(r.Adress_City,' ', r.Adress_street, ' ', r.Adress_House, ' ', r.Adress_Number) as FullRealtEstate, " +
                        " CASE " +
                           "    WHEN r.Id_Apartment IS NOT NULL  THEN 'Квартира' " +
                           "    WHEN r.Id_House IS NOT NULL THEN 'Дом' " +
                           "    WHEN r.Id_Land IS NOT NULL THEN 'Земля' " +
                           "END AS Type " +
                        "From Supply s " +
                        "LEFT JOIN Clients c ON s.Id_Client = c.Id_Client " +
                        "LEFT JOIN Persons p ON p.Id = c.Id_Client " +
                        "LEFT JOIN Agents a ON s.Id_Agent = a.Id_Agent " +
                        "LEFT JOIN Persons pa ON pa.Id = a.Id_Agent " + 
                        "LEFT JOIN RealEstates r ON r.Id_RealEstate = s.Id_RealEstate ";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    SupplyDataGrid.ItemsSource = dataTable.DefaultView;

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }

            }

        }

        private void LoadDataFromDatabaseDemand()
        {
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT d.Id_Demand, d.MinPrice, d.MaxPrice, d.Id_Client, d.Id_Agent, d.Type_RealEstate as Type, d.Adress, " +
                        "CONCAT(p.LastName,' ', p.FirstName, ' ', p.MiddleName) as FullClient, " +
                        "CONCAT(pa.LastName,' ', pa.FirstName, ' ', pa.MiddleName) as FullAgent " +
                        "From Demands d " +
                        "LEFT JOIN Clients c ON d.Id_Client = c.Id_Client " +
                        "LEFT JOIN Persons p ON p.Id = c.Id_Client " +
                        "LEFT JOIN Agents a ON d.Id_Agent = a.Id_Agent " +
                        "LEFT JOIN Persons pa ON pa.Id = a.Id_Agent ";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    DemandDataGrid.ItemsSource = dataTable.DefaultView;

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }

            }

        }
        public int selectedSupplyId;
        public int selectedDemandId;

        private void LoadSupplyCb()
        {
            SupplyCb.Items.Clear();

            string query = "SELECT s.Id_Supply, s.Price, s.Id_Client, s.Id_Agent, s.Id_RealEstate, c.Id_Client, " +
                        "CONCAT(p.LastName,' ', p.FirstName, ' ', p.MiddleName) as FullClient, " +
                        "CONCAT(pa.LastName,' ', pa.FirstName, ' ', pa.MiddleName) as FullAgent, " +
                        "CONCAT(r.Adress_City,' ', r.Adress_street, ' ', r.Adress_House, ' ', r.Adress_Number) as FullRealtEstate, " +
                        " CASE " +
                           "    WHEN r.Id_Apartment IS NOT NULL  THEN 'Квартира' " +
                           "    WHEN r.Id_House IS NOT NULL THEN 'Дом' " +
                           "    WHEN r.Id_Land IS NOT NULL THEN 'Земля' " +
                           "END AS Type " +
                        "From Supply s " +
                        "LEFT JOIN Clients c ON s.Id_Client = c.Id_Client " +
                        "LEFT JOIN Persons p ON p.Id = c.Id_Client " +
                        "LEFT JOIN Agents a ON s.Id_Agent = a.Id_Agent " +
                        "LEFT JOIN Persons pa ON pa.Id = a.Id_Agent " +
                        "LEFT JOIN RealEstates r ON r.Id_RealEstate = s.Id_RealEstate " +
                   "WHERE s.Id_Supply NOT IN (SELECT Id_Supply FROM Deals)";
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string fullsupply = $"{reader["Type"]} / {reader["FullRealtEstate"]} / {reader["Price"]}";
                        SupplyCb.Items.Add(fullsupply);
                        supplyItemsDict[fullsupply] = (int)reader["Id_Supply"];
                    }

                    SupplyCb.SelectedIndex = 0;
                    reader.Close();

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }
            }



        }
        public string typeClientProdavec;
        private void LoadDemandCb()
        {
            DemandCb.Items.Clear();

            string query = "SELECT d.Id_Demand, d.MinPrice, d.MaxPrice, d.Id_Client, d.Id_Agent, d.Type_RealEstate as Type, d.Adress, " +
                        "CONCAT(p.LastName,' ', p.FirstName, ' ', p.MiddleName) as FullClient, " +
                        "CONCAT(pa.LastName,' ', pa.FirstName, ' ', pa.MiddleName) as FullAgent " +
                        "From Demands d " +
                        "LEFT JOIN Clients c ON d.Id_Client = c.Id_Client " +
                        "LEFT JOIN Persons p ON p.Id = c.Id_Client " +
                        "LEFT JOIN Agents a ON d.Id_Agent = a.Id_Agent " +
                        "LEFT JOIN Persons pa ON pa.Id = a.Id_Agent " +
                         "WHERE d.Id_Demand NOT IN (SELECT Id_Demand FROM Deals)"; ;

            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {

                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string fulldemand = $"{reader["Type"]} / {reader["Adress"]} / {reader["FullClient"]} / {reader["MinPrice"]} / {reader["MaxPrice"]}";
                        DemandCb.Items.Add(fulldemand);
                        demandItemsDict[fulldemand] = (int)reader["Id_Demand"];
                    }

                    reader.Close();

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }
            }

            DemandCb.SelectedIndex = 0;

        }




        private void SupplyCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = SupplyCb.SelectedIndex;
            if (selectedIndex >= 0)
            {
                string selectedSupplyText = SupplyCb.Items[selectedIndex].ToString();
                if (supplyItemsDict.TryGetValue(selectedSupplyText, out selectedSupplyId))
                {
                    LoadFilteredDemandCb(selectedSupplyText.Split(' ')[0]);
                }
                if (selectedSupplyText.Contains("Дом"))
                {
                    typeClientProdavec = "дом";
                }
                else if (selectedSupplyText.Contains("Квартира"))
                {
                    typeClientProdavec = "квартира";
                }
                else if (selectedSupplyText.Contains("Земля"))
                {
                    typeClientProdavec = "земля";
                }
            }
        }
        
        private void LoadFilteredDemandCb(string selectedSupplyType)
        {
           
            DemandCb.Items.Clear();

            string query = "SELECT d.Id_Demand, d.MinPrice, d.MaxPrice, d.Id_Client, d.Id_Agent, d.Type_RealEstate as Type, d.Adress, " +
                        "CONCAT(p.LastName,' ', p.FirstName, ' ', p.MiddleName) as FullClient, " +
                        "CONCAT(pa.LastName,' ', pa.FirstName, ' ', pa.MiddleName) as FullAgent " +
                        "From Demands d " +
                        "LEFT JOIN Clients c ON d.Id_Client = c.Id_Client " +
                        "LEFT JOIN Persons p ON p.Id = c.Id_Client " +
                        "LEFT JOIN Agents a ON d.Id_Agent = a.Id_Agent " +
                        "LEFT JOIN Persons pa ON pa.Id = a.Id_Agent " +
                        $"WHERE d.Type_RealEstate = '{selectedSupplyType}' AND "+
                         " d.Id_Demand NOT IN (SELECT Id_Demand FROM Deals)"; 

            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string fulldemand = $"{reader["Type"]} / {reader["Adress"]} / {reader["MinPrice"]} / {reader["MaxPrice"]}";
                        DemandCb.Items.Add(fulldemand);
                        demandItemsDict[fulldemand] = (int)reader["Id_Demand"];
                    }

                    DemandCb.SelectedIndex = 0;

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }


        }

        private void DemandCb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = DemandCb.SelectedIndex;
            if (selectedIndex >= 0)
            {
                string selectedDemandText = DemandCb.Items[selectedIndex].ToString();
                if (demandItemsDict.TryGetValue(selectedDemandText, out selectedDemandId))
                {
                    // Используйте selectedDemandId
                }
            }
        }

        private void Btn_Deal_Click(object sender, RoutedEventArgs e)
        {
          
            AddDeal(selectedSupplyId, selectedDemandId);
        }

        private void SupplyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void DemandDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        public void AddDeal(int supplyId, int dealId)
        {
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {

                    connection.Open();
                    string queryAdd = $"INSERT INTO Deals (Id_Supply, Id_Demand) VALUES (@supplyId, @dealId);";
                    SqlCommand commandAdd = new SqlCommand(queryAdd, connection);
                    commandAdd.Parameters.AddWithValue("@supplyId", supplyId);
                    commandAdd.Parameters.AddWithValue("@dealId", dealId);
                    int rowsAffectedDealss = commandAdd.ExecuteNonQuery();

                    if (rowsAffectedDealss > 0)
                    {

                        MessageBox.Show("Сделка успешно добавлена!" , "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadSupplyCb();  

                    }
                    else
                    {
                        MessageBox.Show("Не удалось добавить сделку.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
                finally { connection.Close(); }
            }
        }

        public int kClientyProdavcy;
        private void Btn_Rachet_Click(object sender, RoutedEventArgs e)
        {
            //    if (typeClientProdavec== "квартира")
            //    {
            //        ClientProdavec.Text=typeClientProdavec;
            //    }
            //    else if (typeClientProdavec == "дом")
            //    {
            //        ClientProdavec.Text = typeClientProdavec;
            //    }
            //    else if (typeClientProdavec == "земля")
            //    {
            //        ClientProdavec.Text = typeClientProdavec;
            //    }

        }
    }
}
