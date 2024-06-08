using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

namespace RealEstate_praktika
{
    /// <summary>
    /// Логика взаимодействия для AddPerson.xaml
    /// </summary>
    public partial class AddPerson : Window
    {
        public DataGrid AgentsTable { get; set; }

        public DataGrid ClientsTable { get; set; }
        public AddPerson()
        {
            InitializeComponent();
            ClientRadio.IsChecked = true;
            RealtorRadio.Checked += RealtorRadio_Checked;
            ClientRadio.Checked += RealtorRadio_Checked;
        }

        private void RealtorRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == RealtorRadio)
            {
                DealPercentStackPanel.Visibility = Visibility.Visible;
                PhoneAndEmailStackPanel.Visibility = Visibility.Collapsed;


            }
            else if (sender == ClientRadio)
            {
                DealPercentStackPanel.Visibility = Visibility.Collapsed;
                PhoneAndEmailStackPanel.Visibility = Visibility.Visible;

            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (RealtorRadio.IsChecked == true)
            {
                try
                {
                    string firstname = FirstNameTextBox.Text;
                    string lastname = LastNameTextBox.Text;
                    string middlename = MiddleNameTextBox.Text;
                    string dealshare = DealPercentTextBox.Text;

                
                        using (var connection = new SqlConnection(DbConnection.connectionString))
                        {

                        try
                        {
                            connection.Open();
                            string queryAdd = $"INSERT INTO Persons (LastName, FirstName, MiddleName) VALUES ('{lastname}', '{firstname}','{middlename}' )";
                            SqlCommand commandAdd = new SqlCommand(queryAdd, connection);
                            int rowsAffected = commandAdd.ExecuteNonQuery();


                            int newPersonId=0;
                            if (rowsAffected > 0)
                            {
                                string queryGetLastInsertedId = "SELECT SCOPE_IDENTITY()";
                                SqlCommand commandGetLastInsertedId = new SqlCommand(queryGetLastInsertedId, connection);
                                newPersonId = Convert.ToInt32(commandGetLastInsertedId.ExecuteScalar());
                            }
                            else
                            {
                                MessageBox.Show("Не удалось вставить запись в таблицу Persons.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }


                            string queryAddA = $"INSERT INTO Agents (Id_Agent, DealShare) VALUES ({newPersonId}, '{dealshare}')";
                            SqlCommand commandAdda = new SqlCommand(queryAddA, connection);
                            commandAdda.ExecuteNonQuery();

                            MessageBox.Show("Сотрудник успешно добавлен!", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDataFromDatabaseAgents();
                            this.Close();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}");
                        }
                        finally { connection.Close(); }
                    }

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }


            }
            else if (ClientRadio.IsChecked == true)
            {
                try
                {
                    string firstname = FirstNameTextBox.Text;
                    string lastname = LastNameTextBox.Text;
                    string middlename = MiddleNameTextBox.Text;
                    string phones = PhoneBox.Text;
                    string emails = EmailBox.Text;

                 
                        using (var connection = new SqlConnection(DbConnection.connectionString))
                        {
                        try
                        {

                            connection.Open();
                            string queryAdd = $"INSERT INTO Persons (LastName, FirstName, MiddleName) VALUES ('{lastname}', '{firstname}','{middlename}' )";
                            SqlCommand commandAdd = new SqlCommand(queryAdd, connection);
                            int rowsAffected = commandAdd.ExecuteNonQuery();


                            int newPersonId = 0;
                            if (rowsAffected > 0)
                            {
                                string queryGetLastInsertedId = "SELECT SCOPE_IDENTITY()";
                                SqlCommand commandGetLastInsertedId = new SqlCommand(queryGetLastInsertedId, connection);
                                newPersonId = Convert.ToInt32(commandGetLastInsertedId.ExecuteScalar());
                            }
                            else
                            {
                                MessageBox.Show("Не удалось вставить запись в таблицу Persons.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }


                            string queryAddA = $"INSERT INTO Clients (Id_Client, Phone, Email) VALUES ({newPersonId}, '{phones}', '{emails}')";
                            SqlCommand commandAdda = new SqlCommand(queryAddA, connection);
                            commandAdda.ExecuteNonQuery();

                            MessageBox.Show("Клиент успешно добавлен!", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDataFromDatabaseClients();

                            this.Close();
                            

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Произошла ошибка: {ex.Message}");
                        }
                        finally { connection.Close(); }
                    }
                
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }

            }
        }
        private void LoadDataFromDatabaseClients()
        {
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Persons INNER JOIN Clients ON Persons.Id = Clients.Id_Client";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    ClientsTable.ItemsSource = dataTable.DefaultView;
                }
                catch(Exception ex) { MessageBox.Show(ex.Message ); }
                finally { connection.Close(); }
            }
        }
        private void LoadDataFromDatabaseAgents()
        {
            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Persons INNER JOIN Agents ON Persons.Id = Agents.Id_Agent";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    AgentsTable.ItemsSource = dataTable.DefaultView;
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }
            }
        }
    }
}
