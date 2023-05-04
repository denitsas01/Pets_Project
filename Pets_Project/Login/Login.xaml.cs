
using System.Data;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace Pets_Project
{
    public partial class Login : Window
    {
        public String cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\UNI\3\3.2\USP\Pets_Project-master\Pets_Project\Database\PetsDB.mdf;Integrated Security=True";
        public SqlConnection myConnection;
        public SqlCommand myCommand;

        public int petID { get; private set; }
        public int petType { get; private set; }
        

        public Login()
        {
            InitializeComponent();

        }

        private void register_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow window = new RegistrationWindow();
            window.Show();
            this.Hide();
        }

        private void sendReminders(int petID1, int petType1)
        {
            DateTime birthdate = DateTime.Now;
            bool isReceived = true;
            List<int> notReceivedVaccs = new List<int>();
            myConnection = new SqlConnection(cs);
            using (myConnection)
            {
                myConnection.Open();

                SqlCommand command1 = new SqlCommand("SELECT pets.birthdate as birthdate, received_vaccs.isReceived as isReceived "
                     + "FROM pets "
                     + "JOIN pets_type ON pets.type_id = pets_type.type_id "
                     + "JOIN vaccinations ON pets_type.type_id = vaccinations.type_id "
                     + "JOIN received_vaccs ON vaccinations.vacc_id = received_vaccs.vacc_id "
                     + "WHERE pets.pet_id = @ID AND pets.type_id = @type AND received_vaccs.isReceived = 0", myConnection);
                command1.Parameters.Add(new SqlParameter("ID", petID1));
                command1.Parameters.Add(new SqlParameter("type", petType1));

                SqlDataReader reader1 = command1.ExecuteReader(CommandBehavior.CloseConnection);
                if (reader1.Read())
                {
                    birthdate = (DateTime)reader1["birthdate"];
                }
                myConnection.Close();

                myConnection.Open();

                SqlCommand command = new SqlCommand("SELECT DISTINCT vaccinations.vacc_id AS vacc_id, received_vaccs.isReceived as isReceived "
                     + "FROM pets "
                     + "JOIN pets_type ON pets.type_id = pets_type.type_id "
                     + "JOIN vaccinations ON pets_type.type_id = vaccinations.type_id "
                     + "JOIN received_vaccs ON vaccinations.vacc_id = received_vaccs.vacc_id "
                     + "WHERE pets.pet_id = @ID AND pets.type_id = @type AND received_vaccs.isReceived = 0", myConnection);
                command.Parameters.Add(new SqlParameter("ID", petID1));
                command.Parameters.Add(new SqlParameter("type", petType1));

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    notReceivedVaccs.Add((int)reader["vacc_id"]);
                    isReceived = false;
                }
                myConnection.Close();
            }

            Dictionary<int, Tuple<int, int>> ageRanges = new Dictionary<int, Tuple<int, int>> { 
                { 1, Tuple.Create(45, 50) },
                { 2, Tuple.Create(56, 63) },
                { 3, Tuple.Create(85, 93) },
                { 4, Tuple.Create(450, 458) },
                { 5, Tuple.Create(56, 63) },
                { 6, Tuple.Create(85, 93) },
                { 8, Tuple.Create(450, 458) },
            };

            foreach (int vaccID in notReceivedVaccs)
            {
                if (ageRanges.ContainsKey(vaccID))
                {
                    Tuple<int, int> ageRange = ageRanges[vaccID];
                    int difference = (int)(DateTime.Now - birthdate).TotalDays;

                    if (difference >= ageRange.Item1 && difference <= ageRange.Item2 && isReceived == false)
                    {
                        switch (vaccID)
                        {
                            case 1:
                                MessageBox.Show("Време за първа ваксина!");
                                break;
                            case 2:
                                MessageBox.Show("Време за втора ваксина!");
                                break;
                            case 3:
                                MessageBox.Show("Време за трета ваксина!");
                                break;
                            case 4:
                                MessageBox.Show("Време за годишна ваксина!");
                                break;
                            case 5:
                                MessageBox.Show("Време за първа ваксина!");
                                break;
                            case 6:
                                MessageBox.Show("Време за втора ваксина!");
                                break;
                            case 7:
                                MessageBox.Show("Време за годишна ваксина!");
                                break;
                        }
                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myConnection = new SqlConnection(cs);
                myCommand = new SqlCommand("SELECT pet_id, type_id, birthdate, pet_name, health FROM pets WHERE username=@username AND password=@password", myConnection);
                SqlParameter uName = new SqlParameter("@username", SqlDbType.VarChar);
                SqlParameter uPassword = new SqlParameter("@password", SqlDbType.VarChar);
                uName.Value = username_tb.Text;
                uPassword.Value = password_tb.Password;
                myCommand.Parameters.Add(uName);
                myCommand.Parameters.Add(uPassword);
                myCommand.Connection.Open();

                SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);                
                String petName = "";
                if (myReader.Read() == true)
                {
                    petID = (int)myReader["pet_id"];
                    petName = myReader.GetString(3);
                    MessageBox.Show("Добре дошъл, " + petName + " !");
                    if(myReader.GetInt32(1) == 1)
                    {
                        petType = 1;
                        this.Hide();
                        MainWindow window = new MainWindow(petID, petType);
                        BitmapImage bitmapImage = new BitmapImage();

                        // Set the source of the BitmapImage to the path of the image file
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("/Images/Buttons/happy.png", UriKind.Relative);
                        bitmapImage.EndInit();

                        window.petimg.Source = bitmapImage;
                        window.petName.Text = petName;
                        window.Show();
                        sendReminders(petID, petType);
                    }
                    else if(myReader.GetInt32(1) == 2)
                    {
                        petType = 2;
                        this.Hide();
                        MainWindow window = new MainWindow(petID, petType);
                        BitmapImage bitmapImage = new BitmapImage();

                        // Set the source of the BitmapImage to the path of the image file
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("/Images/Buttons/laughing.png", UriKind.Relative);
                        bitmapImage.EndInit();

                        window.petimg.Source = bitmapImage;
                        window.petName.Text = petName;
                        window.Show();
                        sendReminders(petID, petType);
                    }
                    else
                    {
                        MessageBox.Show("Възникна грешка при вписване. Опитайте отново!", "Грешка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else{
                    MessageBox.Show("Неуспешен опит за вписване!", "Login Denied", MessageBoxButton.OK, MessageBoxImage.Error);
                    username_tb.Clear();
                    password_tb.Clear();
                    username_tb.Focus();
                }
                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Dispose();
                }
            }
            catch (Exception ex){

                MessageBox.Show(ex.Message, "Грешка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TurnOff_Button(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}

