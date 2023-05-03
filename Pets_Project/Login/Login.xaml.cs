
﻿using System.Data;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using System.Windows.Documents;
using System.Collections;
using System.Collections.Generic;

namespace Pets_Project
{
    public partial class Login : Window
    {
        public String cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\USERS\PETYT\SOURCE\REPOS\PETS_PROJECT\PETS_PROJECT\PETS_DB3.MDF;Integrated Security=True";
        public SqlConnection myConnection = default(SqlConnection);
        public SqlCommand myCommand = default(SqlCommand);

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
                command1.Parameters.Add(new SqlParameter("ID", petID));
                command1.Parameters.Add(new SqlParameter("type", petType));

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
                    isReceived = (bool)reader["isReceived"];
                }

                myConnection.Close();

            }

            for (int vaccID = 0; vaccID < notReceivedVaccs.Count; vaccID++)
            {
                int difference = (int)(DateTime.Now - birthdate).TotalDays;

                if (petType1 == 1 && notReceivedVaccs.IndexOf(vaccID) == 1 && difference >= 45 && difference <= 50 && isReceived == false)
                {
                    MessageBox.Show("Време за първа ваксина!");
                }
                else if (petType1 == 1 && notReceivedVaccs.IndexOf(vaccID) == 2 && difference >= 56 && difference <= 63 && isReceived == false)
                {
                    MessageBox.Show("Време за втора ваксина!");
                }
                else if (petType1 == 1 && notReceivedVaccs.IndexOf(vaccID) == 3 && difference >= 85 && difference <= 93 && isReceived == false)
                {
                    MessageBox.Show("Време за трета ваксина!");
                }
                else if (petType1 == 1 && notReceivedVaccs.IndexOf(vaccID) == 4 && difference >= 450 && difference >= 458 && isReceived == false)
                {
                    MessageBox.Show("Време за годишна ваксина!");
                }
                else if (petType1 == 2 && notReceivedVaccs.IndexOf(vaccID) == 5 && difference >= 56 && difference <= 63 && isReceived == false)
                {
                    MessageBox.Show("Време за първа ваксина!");
                }
                else if (petType1 == 2 && notReceivedVaccs.IndexOf(vaccID) == 6 && difference >= 85 && difference <= 93 && isReceived == false)
                {
                    MessageBox.Show("Време за втора ваксина!");
                }
                else if (petType1 == 2 && notReceivedVaccs.IndexOf(vaccID) == 8 && difference >= 450 && difference >= 458 && isReceived == false)
                {
                    MessageBox.Show("Време за годишна ваксина!");
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                myConnection = new SqlConnection(cs);
                myCommand = new SqlCommand(@"SELECT pet_id, type_id, pet_name FROM pets WHERE username=@username AND password=@password", myConnection);
                SqlParameter uName = new SqlParameter("@username", SqlDbType.VarChar);
                SqlParameter uPassword = new SqlParameter("@password", SqlDbType.VarChar);
                uName.Value = username_tb.Text;
                uPassword.Value = password_tb.Password;
                myCommand.Parameters.Add(uName);
                myCommand.Parameters.Add(uPassword);
                myCommand.Connection.Open();

                SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                String petName = "";
                while(myReader.Read())
                {
                    petName = myReader.GetString(2);
                    //TO DO - create a custom message box in order to style it
                    MessageBox.Show("Добре дошъл, " + petName + "!");
                    if(myReader.GetInt32(1) == 1)
                    {
                        petType = 1;
                        this.Hide();
                        petID = (int)myReader["pet_id"];
                        MainWindow window = new MainWindow(petID, petType);
                        BitmapImage bitmapImage = new BitmapImage();

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
                        petID = (int)myReader["pet_id"];
                        MainWindow window = new MainWindow(petID, petType);
                        BitmapImage bitmapImage = new BitmapImage();

                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("/Images/Buttons/laughing.png", UriKind.Relative);
                        bitmapImage.EndInit();

                        window.petimg.Source = bitmapImage;
                        window.petName.Text = petName;
                        window.Show();
                        sendReminders(petID, petType);
                    }
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
