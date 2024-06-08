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

namespace RealEstate_praktika
{
    /// <summary>
    /// Логика взаимодействия для WindowClients.xaml
    /// </summary>
    public partial class WindowClients : Window
    {
        public WindowClients()
        {
            InitializeComponent();
            LoadDataFromDatabase();
        }

        private void LoadDataFromDatabase()
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
                    ClientsDataGrid.ItemsSource = dataTable.DefaultView;
                    
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }
            }
        }
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchTextBox.Text.Trim();

            if (string.IsNullOrEmpty(searchText))
            {
                LoadDataFromDatabase();
            }
            else
            {
                List<PersonClients> filteredPersons = new List<PersonClients>();

                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try
                    {
                        connection.Open();
                        string query = "SELECT Persons.Id, Persons.LastName, Persons.FirstName, Persons.MiddleName, Clients.Email, Clients.Phone " +
                                       "FROM Persons " +
                                       "INNER JOIN Clients ON Persons.Id = Clients.Id_Client";
                        SqlCommand command = new SqlCommand(query, connection);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            string lastName = reader.GetString(1);
                            string firstName = reader.GetString(2);
                            string middleName = reader.GetString(3);
                            string email = reader.GetString(4);
                            string phone = reader.GetString(5);


                            if (LevenshteinDistance(searchText, lastName) <= 3 ||
                                LevenshteinDistance(searchText, firstName) <= 3 ||
                                LevenshteinDistance(searchText, middleName) <= 3)
                            {
                                filteredPersons.Add(new PersonClients
                                {
                                    Id = reader.GetInt32(0),
                                    LastName = lastName,
                                    FirstName = firstName,
                                    MiddleName = middleName,
                                    Email = email,
                                    Phone = phone

                                });
                            }
                        }

                        reader.Close();
                       
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                    finally { connection.Close(); }

                }

                ClientsDataGrid.ItemsSource = filteredPersons;
            }
        }
        private int LevenshteinDistance(string s1, string s2)
        {
            int[,] d = new int[s1.Length + 1, s2.Length + 1];

            for (int i = 0; i <= s1.Length; i++)
                d[i, 0] = i;
            for (int j = 0; j <= s2.Length; j++)
                d[0, j] = j;

            for (int i = 1; i <= s1.Length; i++)
            {
                for (int j = 1; j <= s2.Length; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);
                }
            }

            return d[s1.Length, s2.Length];
        }

        public class PersonClients
        {
            public int Id { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
        }

        public int currentClientId;
        private void ClientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)ClientsDataGrid.SelectedItem;
                FirstNameTextBox.Text = row["FirstName"].ToString();
                LastNameTextBox.Text = row["LastName"].ToString();
                PatronymicTextBox.Text = row["MiddleName"].ToString();
                PhoneTextBox.Text = row["Phone"].ToString();
                EmailTextBox.Text = row["Email"].ToString();
                currentClientId = Convert.ToInt32(row["Id"].ToString());
            }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (ClientsDataGrid.SelectedItem != null)
            {
                try
                {
                    string UpdateFirstName = FirstNameTextBox.Text;
                    string UpdateLastName = LastNameTextBox.Text;
                    string UpdateMiddleName = PatronymicTextBox.Text;
                    string UpdatePhone = PhoneTextBox.Text;
                    string UpdateEmail = EmailTextBox.Text;

                    string updateQuery = "UPDATE Persons SET FirstName = @UpdateFirstName, LastName = @UpdateLastName, MiddleName = @UpdateMiddleName WHERE Id = @currentClientId";
                    string updateQuery2 = "UPDATE Clients SET Phone = @UpdatePhone, Email = @UpdateEmail WHERE Id_Client = @currentClientId";

                    using (var connection = new SqlConnection(DbConnection.connectionString))
                    {
                        try
                        {

                            connection.Open();
                            SqlCommand command = new SqlCommand(updateQuery, connection);
                            command.Parameters.AddWithValue("@UpdateFirstName", UpdateFirstName);
                            command.Parameters.AddWithValue("@UpdateLastName", UpdateLastName);
                            command.Parameters.AddWithValue("@UpdateMiddleName", UpdateMiddleName);
                            command.Parameters.AddWithValue("@currentClientId", currentClientId);

                            int rowsAffectedPersons = command.ExecuteNonQuery();

                            if (rowsAffectedPersons > 0)
                            {
                                command = new SqlCommand(updateQuery2, connection);
                                command.Parameters.AddWithValue("@UpdatePhone", UpdatePhone);
                                command.Parameters.AddWithValue("@UpdateEmail", UpdateEmail);
                                command.Parameters.AddWithValue("@currentClientId", currentClientId);

                                int rowsAffectedAgents = command.ExecuteNonQuery();

                                if (rowsAffectedAgents > 0)
                                {
                                    MessageBox.Show("Данные успешно обновлены.", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                                    LoadDataFromDatabase();
                                    ClearTextBox();

                                }
                                else
                                {
                                    MessageBox.Show("Не удалось обновить данные в таблице Agents.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                                }

                            }

                            else
                            {
                                MessageBox.Show("Не удалось обновить данные в таблице Persons.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        finally { connection.Close(); }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите объект для обновления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            DataRowView selectedRow = (DataRowView)ClientsDataGrid.SelectedItem;
            if (selectedRow == null)
            {
                MessageBox.Show("Выберите объект для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                int ClientId = (int)selectedRow["Id"];

                string querySupply = $"SELECT COUNT(*) FROM Supply WHERE Id_Client = {ClientId}";
                int supplyCount;
                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand commandSupply = new SqlCommand(querySupply, connection);
                        supplyCount = (int)commandSupply.ExecuteScalar();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        return;
                    }
                    finally
                    {
                        connection.Close();
                    }

                    if (supplyCount > 0)
                    {
                        MessageBox.Show($"Клиент связан с предложением и не может быть удален.",  "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Проверка на наличие агента в таблице Demand
                    string queryDemand = $"SELECT COUNT(*) FROM Demands WHERE Id_Client = {ClientId}";
                    int demandCount;
                    using (var connection2 = new SqlConnection(DbConnection.connectionString))
                    {
                        try
                        {
                            connection2.Open();
                            SqlCommand commandDemand = new SqlCommand(queryDemand, connection2);
                            demandCount = (int)commandDemand.ExecuteScalar();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return;
                        }
                        finally
                        {
                            connection2.Close();
                        }

                        if (demandCount > 0)
                        {
                            MessageBox.Show($"Клиент связан с потребностью и не может быть удален.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                            return;
                        }

                        // Если проверки пройдены, то можно удалять агента
                        if (MessageBox.Show($"Удалить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            using (var connection3 = new SqlConnection(DbConnection.connectionString))
                            {
                                try
                                {
                                    connection3.Open();
                                    string query = $"DELETE FROM Persons WHERE Id = {ClientId}";
                                    SqlCommand command = new SqlCommand(query, connection3);
                                    command.ExecuteNonQuery();

                                    MessageBox.Show("Удалено!");

                                    LoadDataFromDatabase();
                                    ClearTextBox();


                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally { connection3.Close(); }
                            }
                        }
                    }
                }
            }
        }
        private void ClearTextBox()
        {
            LastNameTextBox.Clear();
            FirstNameTextBox.Clear();
            PatronymicTextBox.Clear();
            PhoneTextBox.Clear();
            EmailTextBox.Clear();
        }
        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            AddPerson addPerson = new AddPerson();
            addPerson.ClientsTable = ClientsDataGrid;
            addPerson.ShowDialog();
        }
    }
}
