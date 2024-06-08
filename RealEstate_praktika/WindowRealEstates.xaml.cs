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
    /// Логика взаимодействия для WindowRealEstates.xaml
    /// </summary>
    public partial class WindowRealEstates : Window
    {
        public WindowRealEstates()
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
                    string query = "SELECT r.Id_RealEstate, r.Id_District, r.Adress_City,r.Adress_Street, r.Adress_House, r.Adress_Number, r.Coordinate_latitude, r.Coordinate_longitude , r.Id_Apartment, r.Id_House, r.Id_Land , d.Names as NameDistrict, " +
                     " CASE " +
                           "    WHEN r.Id_Apartment IS NOT NULL  THEN 'Квартира' " +
                           "    WHEN r.Id_House IS NOT NULL THEN 'Дом' " +
                           "    WHEN r.Id_Land IS NOT NULL THEN 'Земля' " +
                           "END AS Type " +
                    "From RealEstates r " +
                    "LEFT JOIN Districts d ON r.Id_District = d.Id_District";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    RealEstatesDataGrid.ItemsSource = dataTable.DefaultView;
                   
                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
                finally { connection.Close(); }

            }
        }
        private void ApartamentRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender == ApartamentRadio)
            {
                FloorLabel.Content = "Этаж";
                FloorLabel.Visibility = Visibility.Visible;
                FloorTextBox.Visibility = Visibility.Visible;
                RoomsLabel.Visibility = Visibility.Visible;
                RoomsTextBox.Visibility = Visibility.Visible;
            }
            else if (sender == HouseRadio)
            {
                FloorLabel.Content = "Этажность";
                FloorLabel.Visibility = Visibility.Visible;
                FloorTextBox.Visibility = Visibility.Visible;
                RoomsLabel.Visibility = Visibility.Visible;
                RoomsTextBox.Visibility = Visibility.Visible;

            }
            else if (sender == LandRadio)
            {
                FloorLabel.Visibility = Visibility.Hidden;
                FloorTextBox.Visibility = Visibility.Hidden;
                RoomsLabel.Visibility = Visibility.Hidden;
                RoomsTextBox.Visibility = Visibility.Hidden;
            }
        }


        public string typeRealEstate;
        public int currentRealEstateId;
        public int currentApartmentID;
        public int currentHouseId;
        public int currentLandId;
        private void RealEstatesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double area;
            int rooms;
            int floor;
            if (RealEstatesDataGrid.SelectedItem != null)
            {
                DataRowView row = (DataRowView)RealEstatesDataGrid.SelectedItem;
                currentRealEstateId = Convert.ToInt32(row["Id_RealEstate"].ToString());
                typeRealEstate = row["Type"].ToString();

                if (typeRealEstate == "Квартира")
                {
                    currentApartmentID = Convert.ToInt32(row["Id_Apartment"].ToString());
                    FloorLabel.Content = "Этаж";
                    FloorLabel.Visibility = Visibility.Visible;
                    FloorTextBox.Visibility = Visibility.Visible;
                    RoomsLabel.Visibility = Visibility.Visible;
                    RoomsTextBox.Visibility = Visibility.Visible;

                    using (var connection = new SqlConnection(DbConnection.connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = $"SELECT a.TotalArea, a.Rooms, a.Floors " +
                                          "FROM RealEstates r " +
                                          "JOIN Apartments a ON r.Id_Apartment = a.Id_Apartment " +
                                          "WHERE r.Id_RealEstate = @currentRealEstateId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@currentRealEstateId", currentRealEstateId);

                            SqlDataReader reader = command.ExecuteReader();
                            try
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        area = reader.GetDouble(0);
                                    else
                                        area = 0;

                                    if (!reader.IsDBNull(1))
                                        rooms = reader.GetInt32(1);
                                    else
                                        rooms = 0;

                                    if (!reader.IsDBNull(2))
                                        floor = reader.GetInt32(2);
                                    else
                                        floor = 0;

                                    TotalAreaTextBox.Text = area.ToString();
                                    RoomsTextBox.Text = rooms.ToString();
                                    FloorTextBox.Text = floor.ToString();
                                }
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                           
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        finally { connection.Close(); }

                    }

                }
                else if (typeRealEstate == "Дом")
                {
                    FloorLabel.Content = "Этажность";
                    currentHouseId = Convert.ToInt32(row["Id_House"].ToString());
                    FloorLabel.Visibility = Visibility.Visible;
                    FloorTextBox.Visibility = Visibility.Visible;
                    RoomsLabel.Visibility = Visibility.Visible;
                    RoomsTextBox.Visibility = Visibility.Visible;

                    using (var connection = new SqlConnection(DbConnection.connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = $"SELECT h.TotalArea, h.TotalRooms, h.TotalFloor " +
                                          "FROM RealEstates r " +
                                          "JOIN Houses h ON r.Id_House = h.Id_House " +
                                          "WHERE r.Id_RealEstate = @currentRealEstateId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@currentRealEstateId", currentRealEstateId);

                            SqlDataReader reader = command.ExecuteReader();
                            try
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        area = reader.GetDouble(0);
                                    else
                                        area = 0;

                                    if (!reader.IsDBNull(1))
                                        rooms = reader.GetInt32(1);
                                    else
                                        rooms = 0;

                                    if (!reader.IsDBNull(2))
                                        floor = reader.GetInt32(2);
                                    else
                                        floor = 0;

                                    TotalAreaTextBox.Text = area.ToString();
                                    RoomsTextBox.Text = rooms.ToString();
                                    FloorTextBox.Text = floor.ToString();
                                }
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                            
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        finally { connection.Close(); }


                    }

                }
                else if (typeRealEstate == "Земля")
                {
                    FloorLabel.Visibility = Visibility.Hidden;
                    FloorTextBox.Visibility = Visibility.Hidden;
                    RoomsLabel.Visibility = Visibility.Hidden;
                    RoomsTextBox.Visibility = Visibility.Hidden;
                    currentLandId = Convert.ToInt32(row["Id_Land"].ToString());

                    using (var connection = new SqlConnection(DbConnection.connectionString))
                    {
                        try
                        {
                            connection.Open();
                            string query = $"SELECT l.TotalArea " +
                                          "FROM RealEstates r " +
                                          "JOIN Lands l ON r.Id_Land = l.Id_Land " +
                                          "WHERE r.Id_RealEstate = @currentRealEstateId";
                            SqlCommand command = new SqlCommand(query, connection);
                            command.Parameters.AddWithValue("@currentRealEstateId", currentRealEstateId);

                            SqlDataReader reader = command.ExecuteReader();
                            try
                            {
                                if (reader.Read())
                                {
                                    if (!reader.IsDBNull(0))
                                        area = reader.GetDouble(0);
                                    else
                                        area = 0;

                                    TotalAreaTextBox.Text = area.ToString();

                                }
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                           
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                        finally { connection.Close(); }
                    }


                }
                CityTextBox.Text = row["Adress_City"].ToString();
                StreetTextBox.Text = row["Adress_Street"].ToString();
                HouseTextBox.Text = row["Adress_House"].ToString();
                NumberTextBox.Text = row["Adress_Number"].ToString();
                LongitudeTextBox.Text = row["Coordinate_longitude"].ToString();
                LatitudeTextBox.Text = row["Coordinate_latitude"].ToString();


            }
        }

        private void AddWindow_Click(object sender, RoutedEventArgs e)
        {
            if (AddWindow.Content.ToString() == "Добавить")
            {
                ClearTextBox();
                RealEstatesDataGrid.IsEnabled = false;
                Update.IsEnabled = false;
                Delete.IsEnabled = false;
                TypeRealEstate.Visibility = Visibility.Visible;
                ApartamentRadio.IsChecked = true;
                AddWindow.Content = "Сохранить";
                Back.Visibility = Visibility.Visible;
            }
            else if (AddWindow.Content.ToString() == "Сохранить")
            {
                RealEstatesDataGrid.IsEnabled = true;

                AddWindow.Content = "Добавить";
                Update.IsEnabled = true;
                Delete.IsEnabled = true;

                if (ApartamentRadio.IsChecked == true)
                {

                    string city = CityTextBox.Text.Trim();
                    string street = StreetTextBox.Text.Trim();
                    int house = Convert.ToInt32(HouseTextBox.Text.Trim());
                    int number = Convert.ToInt32(NumberTextBox.Text.Trim());
                    int longitude = Convert.ToInt32(LongitudeTextBox.Text.Trim());
                    int latitude = Convert.ToInt32(LatitudeTextBox.Text.Trim());
                    double area = Convert.ToDouble(TotalAreaTextBox.Text.Trim());
                    int floor = Convert.ToInt32(FloorTextBox.Text.Trim());
                    int rooms = Convert.ToInt32(RoomsTextBox.Text.Trim());


                    AddApartment(city, street, house, number, longitude, latitude, area, floor, rooms);
                    LoadDataFromDatabase();
                    ClearTextBox();
                    Back.Visibility = Visibility.Collapsed;
                }
                else if (HouseRadio.IsChecked == true)
                {


                    string city = CityTextBox.Text.Trim();
                    string street = StreetTextBox.Text.Trim();
                    int house = Convert.ToInt32(HouseTextBox.Text.Trim());
                    int number = Convert.ToInt32(NumberTextBox.Text.Trim());
                    int longitude = Convert.ToInt32(LongitudeTextBox.Text.Trim());
                    int latitude = Convert.ToInt32(LatitudeTextBox.Text.Trim());
                    double area = Convert.ToDouble(TotalAreaTextBox.Text.Trim());
                    int floor = Convert.ToInt32(FloorTextBox.Text.Trim());
                    int rooms = Convert.ToInt32(RoomsTextBox.Text.Trim());


                    AddHouse(city, street, house, number, longitude, latitude, area, floor, rooms);
                    LoadDataFromDatabase();
                    ClearTextBox();
                }
                else if (LandRadio.IsChecked == true)
                {


                    string city = CityTextBox.Text.Trim();
                    string street = StreetTextBox.Text.Trim();
                    int house = Convert.ToInt32(HouseTextBox.Text.Trim());
                    int number = Convert.ToInt32(NumberTextBox.Text.Trim());
                    int longitude = Convert.ToInt32(LongitudeTextBox.Text.Trim());
                    int latitude = Convert.ToInt32(LatitudeTextBox.Text.Trim());
                    double area = Convert.ToDouble(TotalAreaTextBox.Text.Trim());


                    AddLand(city, street, house, number, longitude, latitude, area);
                    LoadDataFromDatabase();
                    ClearTextBox();

                }
                TypeRealEstate.Visibility = Visibility.Collapsed;
                Back.Visibility = Visibility.Collapsed;

            }
        }

        public void AddApartment(string city, string street, int house, int number, int longitude, int latitude, double area, int floor, int rooms)
        {
            decimal rightarea = Convert.ToDecimal(area);

            using (var connection = new SqlConnection(DbConnection.connectionString))
            {
                try
                {

                    connection.Open();
                    string queryAdd = $"INSERT INTO Apartments (TotalArea, Rooms, Floors) VALUES (@TotalArea, @Rooms, @Floors); SELECT SCOPE_IDENTITY();";
                    SqlCommand commandAdd = new SqlCommand(queryAdd, connection);
                    commandAdd.Parameters.AddWithValue("@TotalArea", rightarea);
                    commandAdd.Parameters.AddWithValue("@Rooms", rooms);
                    commandAdd.Parameters.AddWithValue("@Floors", floor);
                    int newApartmentId = Convert.ToInt32(commandAdd.ExecuteScalar());

                    string queryAddA = $"INSERT INTO RealEstates (Adress_City, Adress_Street, Adress_House, Adress_Number, Coordinate_latitude, Coordinate_longitude, Id_Apartment) VALUES (@City, @Street, @House, @Number, @Latitude, @Longitude, @Id_Apartment)";
                    SqlCommand commandAdda = new SqlCommand(queryAddA, connection);
                    commandAdda.Parameters.AddWithValue("@City", city);
                    commandAdda.Parameters.AddWithValue("@Street", street);
                    commandAdda.Parameters.AddWithValue("@House", house);
                    commandAdda.Parameters.AddWithValue("@Number", number);
                    commandAdda.Parameters.AddWithValue("@Latitude", latitude);
                    commandAdda.Parameters.AddWithValue("@Longitude", longitude);
                    commandAdda.Parameters.AddWithValue("@Id_Apartment", newApartmentId);
                    commandAdda.ExecuteNonQuery();


                    MessageBox.Show("Объект успешно добавлен!", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                   

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}");
                }
                finally { connection.Close(); } 
            }


        }

        public void AddHouse(string city, string street, int house, int number, int longitude, int latitude, double area, int floor, int rooms)
        {
            decimal rightarea = Convert.ToDecimal(area);
          
                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                try
                {
                    connection.Open();

                    string queryAdd = $"INSERT INTO Houses (TotalArea, TotalRooms, TotalFloor) VALUES (@TotalArea, @Rooms, @Floors); SELECT SCOPE_IDENTITY();";
                    SqlCommand commandAdd = new SqlCommand(queryAdd, connection);
                    commandAdd.Parameters.AddWithValue("@TotalArea", rightarea);
                    commandAdd.Parameters.AddWithValue("@Rooms", rooms);
                    commandAdd.Parameters.AddWithValue("@Floors", floor);
                    int newHouseId = Convert.ToInt32(commandAdd.ExecuteScalar());


                    string queryAddA = $"INSERT INTO RealEstates (Adress_City, Adress_Street, Adress_House, Adress_Number, Coordinate_latitude,Coordinate_longitude, Id_House) VALUES ('{city}', '{street}',{house},{number},{latitude},{longitude},{newHouseId} )";
                    SqlCommand commandAdda = new SqlCommand(queryAddA, connection);
                    commandAdda.ExecuteNonQuery();

                    MessageBox.Show("Объект успешно добавлен!", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                  


                }
                catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally { connection.Close(); }
        }

        }

        public void AddLand(string city, string street, int house, int number, int longitude, int latitude, double area)
        {
            decimal rightarea = Convert.ToDecimal(area);

          
                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                try
                {

                    connection.Open();
                    string queryAdd = $"INSERT INTO Lands (TotalArea) VALUES (@TotalArea); SELECT SCOPE_IDENTITY();";
                    SqlCommand commandAdd = new SqlCommand(queryAdd, connection);
                    commandAdd.Parameters.AddWithValue("@TotalArea", rightarea);

                    int newLandId = Convert.ToInt32(commandAdd.ExecuteScalar());


                    string queryAddA = $"INSERT INTO RealEstates (Adress_City, Adress_Street, Adress_House, Adress_Number, Coordinate_latitude,Coordinate_longitude, Id_Land) VALUES ('{city}', '{street}',{house},{number},{latitude},{longitude},{newLandId} )";
                    SqlCommand commandAdda = new SqlCommand(queryAddA, connection);
                    commandAdda.ExecuteNonQuery();

                    MessageBox.Show("Объект успешно добавлен!", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information );
                


                }
                catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}");
            }
            finally { connection.Close(); }
        }

        }
        private void ClearTextBox()
        {
            CityTextBox.Clear();
            StreetTextBox.Clear();
            HouseTextBox.Clear();
            NumberTextBox.Clear();
            LongitudeTextBox.Clear();
            LatitudeTextBox.Clear();
            TotalAreaTextBox.Clear();
            RoomsTextBox.Clear();
            FloorTextBox.Clear();
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            if (RealEstatesDataGrid.SelectedItem != null)
            {

                if (typeRealEstate == "Квартира")
                {
                    string city = CityTextBox.Text.Trim();
                    string street = StreetTextBox.Text.Trim();
                    int house = Convert.ToInt32(HouseTextBox.Text.Trim());
                    int number = Convert.ToInt32(NumberTextBox.Text.Trim());
                    int longitude = Convert.ToInt32(LongitudeTextBox.Text.Trim());
                    int latitude = Convert.ToInt32(LatitudeTextBox.Text.Trim());
                    double area = Convert.ToDouble(TotalAreaTextBox.Text.Trim());
                    int floor = Convert.ToInt32(FloorTextBox.Text.Trim());
                    int rooms = Convert.ToInt32(RoomsTextBox.Text.Trim());

                    UpdateApartment(city, street, house, number, longitude, latitude, area, floor, rooms);

                }
                if (typeRealEstate == "Дом")
                {
                    string city = CityTextBox.Text.Trim();
                    string street = StreetTextBox.Text.Trim();
                    int house = Convert.ToInt32(HouseTextBox.Text.Trim());
                    int number = Convert.ToInt32(NumberTextBox.Text.Trim());
                    int longitude = Convert.ToInt32(LongitudeTextBox.Text.Trim());
                    int latitude = Convert.ToInt32(LatitudeTextBox.Text.Trim());
                    double area = Convert.ToDouble(TotalAreaTextBox.Text.Trim());
                    int floor = Convert.ToInt32(FloorTextBox.Text.Trim());
                    int rooms = Convert.ToInt32(RoomsTextBox.Text.Trim());

                    UpdateHouse(city, street, house, number, longitude, latitude, area, floor, rooms);

                }
                if (typeRealEstate == "Земля")
                {
                    string city = CityTextBox.Text.Trim();
                    string street = StreetTextBox.Text.Trim();
                    int house = Convert.ToInt32(HouseTextBox.Text.Trim());
                    int number = Convert.ToInt32(NumberTextBox.Text.Trim());
                    int longitude = Convert.ToInt32(LongitudeTextBox.Text.Trim());
                    int latitude = Convert.ToInt32(LatitudeTextBox.Text.Trim());
                    double area = Convert.ToDouble(TotalAreaTextBox.Text.Trim());


                    UpdateLand(city, street, house, number, longitude, latitude, area);

                }
            }
            else { MessageBox.Show("Выберите объект для обновления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }
        public void UpdateApartment(string city, string street, int house, int number, int longitude, int latitude, double area, int floor, int rooms)
        {
            decimal rightarea = Convert.ToDecimal(area);
            try
            {

                string updateQuery = "UPDATE Apartments SET TotalArea = @TotalArea, Rooms = @Rooms, Floors = @Floors WHERE Id_Apartment = @currentApartmentID";
                string updateQuery2 = "UPDATE RealEstates SET Adress_City = @Adress_Ciry, Adress_Street = @Adress_Street, Adress_House = @Adress_House, Adress_Number = @Adress_Number , Coordinate_latitude = @Coordinate_latitude , Coordinate_longitude = @Coordinate_longitude WHERE Id_Apartment = @currentApartmentID";

                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try
                    {

                        connection.Open();
                        SqlCommand command = new SqlCommand(updateQuery, connection);
                        command.Parameters.AddWithValue("@TotalArea", rightarea);
                        command.Parameters.AddWithValue("@Rooms", rooms);
                        command.Parameters.AddWithValue("@Floors", floor);
                        command.Parameters.AddWithValue("@currentApartmentID", currentApartmentID);

                        int rowsAffectedPersons = command.ExecuteNonQuery();

                        if (rowsAffectedPersons > 0)
                        {
                            command = new SqlCommand(updateQuery2, connection);
                            command.Parameters.AddWithValue("@Adress_Ciry", city);
                            command.Parameters.AddWithValue("@Adress_Street", street);
                            command.Parameters.AddWithValue("@Adress_House", house);
                            command.Parameters.AddWithValue("@Adress_Number", number);
                            command.Parameters.AddWithValue("@Coordinate_latitude", latitude);
                            command.Parameters.AddWithValue("@Coordinate_longitude", longitude);

                            command.Parameters.AddWithValue("@currentApartmentID", currentApartmentID);

                            int rowsAffectedAgents = command.ExecuteNonQuery();

                            if (rowsAffectedAgents > 0)
                            {
                                MessageBox.Show("Данные успешно обновлены.", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadDataFromDatabase();
                                ClearTextBox();

                            }
                            else
                            {
                                MessageBox.Show("Не удалось обновить данные в таблице Apartments.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                           
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные в таблице RealEstates.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void UpdateHouse(string city, string street, int house, int number, int longitude, int latitude, double area, int floor, int rooms)
        {
            decimal rightarea = Convert.ToDecimal(area);
            try
            {

                string updateQuery = "UPDATE Houses SET TotalArea = @TotalArea, TotalRooms = @Rooms, TotalFloor = @Floors WHERE Id_House = @currentHouseId";
                string updateQuery2 = "UPDATE RealEstates SET Adress_City = @Adress_Ciry, Adress_Street = @Adress_Street, Adress_House = @Adress_House, Adress_Number = @Adress_Number , Coordinate_latitude = @Coordinate_latitude , Coordinate_longitude = @Coordinate_longitude WHERE Id_House = @currentHouseId";

                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand(updateQuery, connection);
                        command.Parameters.AddWithValue("@TotalArea", rightarea);
                        command.Parameters.AddWithValue("@Rooms", rooms);
                        command.Parameters.AddWithValue("@Floors", floor);
                        command.Parameters.AddWithValue("@currentHouseId", currentHouseId);

                        int rowsAffectedPersons = command.ExecuteNonQuery();

                        if (rowsAffectedPersons > 0)
                        {
                            command = new SqlCommand(updateQuery2, connection);
                            command.Parameters.AddWithValue("@Adress_Ciry", city);
                            command.Parameters.AddWithValue("@Adress_Street", street);
                            command.Parameters.AddWithValue("@Adress_House", house);
                            command.Parameters.AddWithValue("@Adress_Number", number);
                            command.Parameters.AddWithValue("@Coordinate_latitude", latitude);
                            command.Parameters.AddWithValue("@Coordinate_longitude", longitude);

                            command.Parameters.AddWithValue("@currentHouseId", currentHouseId);

                            int rowsAffectedAgents = command.ExecuteNonQuery();

                            if (rowsAffectedAgents > 0)
                            {
                                MessageBox.Show("Данные успешно обновлены.", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadDataFromDatabase();
                                ClearTextBox();

                            }
                            else
                            {
                                MessageBox.Show("Не удалось обновить данные в таблице Houses.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                            }
                           
                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные в таблице RealEstates.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
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

        public void UpdateLand(string city, string street, int house, int number, int longitude, int latitude, double area)
        {
            decimal rightarea = Convert.ToDecimal(area);
            try
            {

                string updateQuery = "UPDATE Lands SET TotalArea = @TotalArea WHERE Id_Land = @currentLandId";
                string updateQuery2 = "UPDATE RealEstates SET Adress_City = @Adress_Ciry, Adress_Street = @Adress_Street, Adress_House = @Adress_House, Adress_Number = @Adress_Number , Coordinate_latitude = @Coordinate_latitude , Coordinate_longitude = @Coordinate_longitude WHERE Id_Land = @currentLandId";

                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try {
                        connection.Open();
                        SqlCommand command = new SqlCommand(updateQuery, connection);
                        command.Parameters.AddWithValue("@TotalArea", rightarea);
                        command.Parameters.AddWithValue("@currentLandId", currentLandId);

                        int rowsAffectedPersons = command.ExecuteNonQuery();

                        if (rowsAffectedPersons > 0)
                        {
                            command = new SqlCommand(updateQuery2, connection);
                            command.Parameters.AddWithValue("@Adress_Ciry", city);
                            command.Parameters.AddWithValue("@Adress_Street", street);
                            command.Parameters.AddWithValue("@Adress_House", house);
                            command.Parameters.AddWithValue("@Adress_Number", number);
                            command.Parameters.AddWithValue("@Coordinate_latitude", latitude);
                            command.Parameters.AddWithValue("@Coordinate_longitude", longitude);

                            command.Parameters.AddWithValue("@currentLandId", currentLandId);

                            int rowsAffectedAgents = command.ExecuteNonQuery();

                            if (rowsAffectedAgents > 0)
                            {
                                MessageBox.Show("Данные успешно обновлены.", "Выполнено", MessageBoxButton.OK, MessageBoxImage.Information);
                                LoadDataFromDatabase();
                                ClearTextBox();

                            }
                            else
                            {
                                MessageBox.Show("Не удалось обновить данные в таблице Lands.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
                            }


                        }
                        else
                        {
                            MessageBox.Show("Не удалось обновить данные в таблице RealEstates.", "Внимание", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (RealEstatesDataGrid.SelectedItem != null)
            {

                if (typeRealEstate == "Квартира")
                {
                    DeleteApartment();
                }
                if (typeRealEstate == "Дом")
                {
                    DeleteHouse();
                }
                if (typeRealEstate == "Земля")
                {
                    DeleteLand();
                }
            }
            else { MessageBox.Show("Выберите объект для удаления!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void DeleteApartment()
        {
            if (MessageBox.Show($"Удалить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                
                    using (var connection = new SqlConnection(DbConnection.connectionString))
                    {
                    try
                    {
                        connection.Open();

                        string query = $"DELETE FROM RealEstates WHERE Id_Apartment = {currentApartmentID}";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Удалено!");

                        LoadDataFromDatabase();
                        ClearTextBox();
                        

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally { connection.Close(); }
                }
               
            }
        }

        private void DeleteHouse()
        {
            if (MessageBox.Show($"Удалить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
              
                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try
                    {
                        connection.Open();

                        string query = $"DELETE FROM RealEstates WHERE Id_House = {currentHouseId}";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Удалено!");

                        LoadDataFromDatabase();
                        ClearTextBox();
                      
                }


                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally { connection.Close(); }
            }
            }
        }

        private void DeleteLand()
        {
            if (MessageBox.Show($"Удалить?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                using (var connection = new SqlConnection(DbConnection.connectionString))
                {
                    try
                    {

                        connection.Open();
                        string query = $"DELETE FROM RealEstates WHERE Id_Land = {currentLandId}";
                        SqlCommand command = new SqlCommand(query, connection);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Удалено!");

                        LoadDataFromDatabase();
                        ClearTextBox();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally { connection.Close(); }

                }
            }
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            RealEstatesDataGrid.IsEnabled = true;
            ApartamentRadio.IsChecked = true;
            AddWindow.Content = "Добавить";
            Update.IsEnabled = true;
            Delete.IsEnabled = true;
            Back.Visibility = Visibility.Collapsed;
            TypeRealEstate.Visibility = Visibility.Hidden;
        }
    }
}
