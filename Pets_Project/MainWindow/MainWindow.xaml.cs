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

namespace Pets_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int petID;
        private int petType;
        private int vaccID;

        Login login = new Login();
        SqlConnection myConnection;

        List<int> vaccIDs = new List<int>(); // create a list to store the values of vacc_id


        public MainWindow(int petID, int petType)
        {
            InitializeComponent();

            //get the current date
            current_date.Text = DateTime.Now.ToString("D");
            current_date2.Text = DateTime.Now.ToString("D");
            current_date3.Text = DateTime.Now.ToString("D");

            this.petID = petID;
            this.petType = petType;

            load_vaccines();
            load_help_panel();
        }
        private void logout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void load_vaccines ()
        {
            myConnection = new SqlConnection(login.cs);

            using (myConnection)
            {
                myConnection.Open();

                SqlCommand command = new SqlCommand("SELECT pets.pet_name AS pet_name, pets_type.type_name AS type_name, vaccinations.vacc_name AS vacc_name, vaccinations.vacc_id AS vacc_id "
                     + "FROM pets "
                     + "JOIN pets_type ON pets.type_id = pets_type.type_id "
                     + "JOIN vaccinations ON pets_type.type_id = vaccinations.type_id " 
                     + "WHERE pets.pet_id = @ID AND pets.type_id = @type", myConnection);
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
                    row["isReceived"] = false;

                }
                dataGrid1.ItemsSource = dataTable.DefaultView;

                if (myConnection.State == ConnectionState.Open)
                {
                    myConnection.Dispose();
                }
            }
        }

        private void save_vaccs_button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (int id in vaccIDs)
                {
                    foreach (DataRowView rowView in dataGrid1.ItemsSource)
                    {
                        DataRow row = rowView.Row;
                        bool isSelected = (bool)row["isReceived"];
                        vaccID = (int)row["vacc_id"];
                        if (isSelected && id == vaccID)
                        {
                            using (SqlConnection connection = new SqlConnection(login.cs))
                            {
                                connection.Open();
                                SqlCommand command2 = new SqlCommand("INSERT INTO received_vaccs VALUES (@vacc_id, @pet_id, @isReceived)", connection);
                                command2.Parameters.AddWithValue("@vacc_id", vaccID);
                                command2.Parameters.AddWithValue("@pet_id", petID);
                                command2.Parameters.AddWithValue("@isReceived", 1);
                                command2.ExecuteNonQuery();
                            }                            
                        }
                    } 
                }
                MessageBox.Show("Insert successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void load_help_panel() {
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
