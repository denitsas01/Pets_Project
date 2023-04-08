using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections;
using System.Configuration;
using System.ComponentModel.Design;

namespace Pets_Project
{
    public partial class MainWindow : Window
    {
        private int petID;
        private int petType;
        private int vaccID;
        private bool isSelected;

        SqlConnection myConnection;
        Login login = new Login();

        List<int> vaccIDs = new List<int>();
        public MainWindow(int petID, int petType)
        {
            InitializeComponent();

            //get the current date
            current_date.Text = DateTime.Now.ToString("D");
            current_date2.Text = DateTime.Now.ToString("D");
            current_date3.Text = DateTime.Now.ToString("D");

            this.petID = petID;
            this.petType = petType;

            load_receivedVaccs();
            load_vaccines();
            load_help_panel();
            personal_info_load();
        }

        //logout panel 
        private void logout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        //panel for upcoming vaccines content load
        private void load_vaccines()
        {
            myConnection = new SqlConnection(login.cs);
            using (myConnection)
            {
                myConnection.Open();

                SqlCommand command = new SqlCommand("SELECT pets.pet_name AS pet_name, pets_type.type_name AS type_name, vaccinations.vacc_name AS vacc_name, " +
                    "vaccinations.vacc_id AS vacc_id, received_vaccs.isReceived as isReceived "
                     + "FROM pets "
                     + "JOIN pets_type ON pets.type_id = pets_type.type_id "
                     + "JOIN vaccinations ON pets_type.type_id = vaccinations.type_id "
                     + "JOIN received_vaccs ON vaccinations.vacc_id = received_vaccs.vacc_id "
                     + "WHERE pets.pet_id = @ID AND pets.type_id = @type AND received_vaccs.isReceived = 0", myConnection);
                command.Parameters.Add(new SqlParameter("ID", petID));
                command.Parameters.Add(new SqlParameter("type", petType));

                SqlDataAdapter adapter = new SqlDataAdapter(command);

                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("isReceived", typeof(bool)));
                adapter.Fill(dataTable);

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);
                while (reader.Read())
                {
                    vaccIDs.Add((int)reader["vacc_id"]); // add each value of vacc_id to the list
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    row["isReceived"] = false; // за checkbox-a

                }
                dataGrid1.ItemsSource = dataTable.DefaultView;
            }
        }

        //button to mark a vaccine as received
        private void save_vaccs_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (int id in vaccIDs)
                {
                    foreach (DataRowView rowView in dataGrid1.ItemsSource)
                    {
                        DataRow row = rowView.Row;
                        //bool isSelected = (bool)row["isReceived"];
                        isSelected = (bool)row["isReceived"];
                        vaccID = (int)row["vacc_id"];
                        if (isSelected && id == vaccID)
                        {
                            using (SqlConnection connection = new SqlConnection(login.cs))
                            {
                                //query to insert to table received_vaccs
                                connection.Open();
                                SqlCommand command2 = new SqlCommand("UPDATE received_vaccs SET [isReceived] = 1 WHERE vacc_id=@vaccID", connection);
                                command2.Parameters.AddWithValue("@vaccID", vaccID);
                                command2.ExecuteNonQuery();
                                connection.Close();
                            }
                        }
                    }
                }
                load_vaccines();
                load_receivedVaccs();
                MessageBox.Show("Информацията е запазена!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //received vaccs panel content load
        private void load_receivedVaccs() 
        {
            myConnection = new SqlConnection(login.cs);

            using (myConnection)
            {
                myConnection.Open();
                SqlCommand command = new SqlCommand("SELECT pets_type.type_name, vaccinations.vacc_name, received_vaccs.isReceived, received_vaccs.date_received AS vacc_date " +
                    " FROM received_vaccs " +
                    " JOIN pets ON received_vaccs.pet_id = pets.pet_id " +
                    " JOIN pets_type ON pets.type_id = pets_type.type_id " +
                    " JOIN vaccinations ON received_vaccs.vacc_id = vaccinations.vacc_id " +
                    " WHERE pets.pet_id = @ID AND pets.type_id = @type AND received_vaccs.isReceived = 1 ", myConnection);
                command.Parameters.Add(new SqlParameter("ID", petID));
                command.Parameters.Add(new SqlParameter("type", petType));

                SqlDataAdapter adapter = new SqlDataAdapter(command);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);

                SqlDataReader reader = command.ExecuteReader(CommandBehavior.CloseConnection);

                dataGrid2.ItemsSource = dataTable.DefaultView;

                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Dispose();
                }

            }
        }
        //personal info panel content load
        private void personal_info_load()
        {
            myConnection = new SqlConnection(login.cs);

            using (myConnection)
            {
                string birthdate = "";
                DateTime dob = new DateTime();
                myConnection.Open();

                SqlCommand myCommand = new SqlCommand("SELECT birthdate, health, pet_name FROM pets WHERE pet_id=@petID", myConnection);
                myCommand.Parameters.Add(new SqlParameter("petID", petID));

                SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (myReader.Read() == true)
                {
                    greetings.Content = "Здравей, " + (string)myReader["pet_name"] + "!";
                    dob = (DateTime)myReader["birthdate"];
                    textBox.Text = dob.ToShortDateString();
                    textBox_Copy.Text = CalculateAge(dob).ToString();
                                        
                    string healthFromDb = (string)myReader["health"];
                    richTextBox.Document.Blocks.Clear();
                    richTextBox.Document.Blocks.Add(new Paragraph(new Run(healthFromDb)));
                    
                }
                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Dispose();
                }
            }
        }

        //method to calculate the age of the animal
        private string CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;

            int ageInMonths = (today.Year - dateOfBirth.Year) * 12 + today.Month - dateOfBirth.Month;
            int years = ageInMonths / 12;
            int months = ageInMonths % 12;
            
            if (years == 0 && months == 0)
            {
                return "0 години";
            }
            else if (years == 0 && months > 0)
            {
                return months + " месеца";
            }
            else if (years > 0 && months == 0)
            {
                return years + " години";
            }
            else
            {
                return years + " години и " + months + " месеца";
            }
        }

        //update personal info method
        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            myConnection = new SqlConnection(login.cs);

            SqlCommand myCommand = new SqlCommand("UPDATE pets SET [health] = N'" + StringFromRichTextBox(richTextBox) + "' WHERE pet_id=@petID", myConnection);
            myCommand.Parameters.Add(new SqlParameter("petID", petID));
            myCommand.Connection.Open();
            myCommand.ExecuteNonQuery();
            myConnection.Close();
            string text = StringFromRichTextBox(richTextBox);
            if (text!="")
            { 
                MessageBox.Show("Информацията е запазена!");
            }
            else //PROBLEM!!!
            {
                MessageBox.Show("Няма информация за запазване!");
            }

            if (myConnection.State == ConnectionState.Open)
            {
                myConnection.Dispose();
            }

        }

        //Rich Text Box method
        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }

        //help panel content
        private void load_help_panel()
        {
            myConnection = new SqlConnection(login.cs);

            using (myConnection)
            {
                myConnection.Open();

                SqlCommand commandHelp = new SqlCommand("SELECT pets_type.type_name AS type_name, vaccinations.vacc_name AS vacc_name, vaccinations.vacc_desc AS vacc_desc "
                     + "FROM vaccinations "
                     + "JOIN pets_type ON vaccinations.type_id = pets_type.type_id "
                     + "WHERE vaccinations.type_id = @pType", myConnection);
                commandHelp.Parameters.Add(new SqlParameter("pType", petType));

                SqlDataAdapter adapterHelp = new SqlDataAdapter(commandHelp);

                DataTable dataTableHelp = new DataTable();
                adapterHelp.Fill(dataTableHelp);

                dataGridHelp.ItemsSource = dataTableHelp.DefaultView;

                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Dispose();
                }
            }
        }
    }

}
