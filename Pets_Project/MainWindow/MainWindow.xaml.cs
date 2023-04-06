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
            personal_info_load();

            //myCommand = new SqlCommand("SELECT [pet_name] FROM [dbo].[pets] WHERE username=@username AND password=@password", myConnection);
            //SqlParameter uName = new SqlParameter("@username", SqlDbType.VarChar);
            //SqlParameter uPassword = new SqlParameter("@password", SqlDbType.VarChar);
            //var login = Application.Current.Windows.OfType<Login>().FirstOrDefault();
            //uName.Value = login.username_tb.Text;
            //uPassword.Value = login.password_tb.Password;
            //myCommand.Parameters.Add(uName);
            //myCommand.Parameters.Add(uPassword);
            //myCommand.Connection.Open();
            //SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
            //if (myReader.Read() == true)
            //{
            //    greetings.Content = "Hi there, " + (string)myReader["pet_name"] + "!";
            //    Profile.Text = (string)myReader["pet_name"];
            //}
            //if (myConnection.State == ConnectionState.Open)
            //{
            //    myConnection.Dispose();
            //}

            
        }

        public String cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\petyt\source\repos\Pets_Project\Pets_Project\pets_db3.mdf;Integrated Security=True";

        public SqlConnection myConnection = default(SqlConnection);

        public SqlCommand myCommand = default(SqlCommand);

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


        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void personal_info_load()
        {
            myConnection = new SqlConnection(cs);

            using (myConnection)
            {
                string birthdate = "";
                DateTime dob = new DateTime();
                myConnection.Open();

                myCommand = new SqlCommand("SELECT birthdate FROM pets WHERE pet_id=@petID", myConnection);
                //SqlParameter uName = new SqlParameter("@username", SqlDbType.Text);
                //SqlParameter uPassword = new SqlParameter("@password", SqlDbType.Text);
                //var login = Application.Current.Windows.OfType<Login>().FirstOrDefault();
                //uName.Value = login.username_tb.Text;
                //uPassword.Value = login.password_tb.Password;
                //myCommand.Parameters.AddWithValue("@username", uName);
                //myCommand.Parameters.AddWithValue("@password", uPassword);

                myCommand.Parameters.Add(new SqlParameter("petID", petID));


                // myCommand.Parameters.Add(new SqlParameter("username",uName));
                //myCommand.Parameters.Add(new SqlParameter("password",uPassword));

                SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                if (myReader.Read() == true)
                {
                    dob = (DateTime)myReader["birthdate"];
                    textBox.Text = dob.ToShortDateString();
                    textBox_Copy.Text = CalculateAge(dob).ToString();
                }
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

        private static int CalculateAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }

        private void save_btn_Click(object sender, RoutedEventArgs e)
        {
            myConnection = new SqlConnection(cs);

            myCommand = new SqlCommand("UPDATE [dbo].[pets] SET [health] = N'" + StringFromRichTextBox(richTextBox) + "' WHERE pet_id=@petID", myConnection);
            myCommand.Parameters.Add(new SqlParameter("petID", petID));
            myCommand.Connection.Open();
            myCommand.ExecuteNonQuery();
            /*if (myReader.Read() == true)
            { 
                MessageBox.Show("Your registration is successful!");
            }*/
            if (myConnection.State == ConnectionState.Open)
            {
                myConnection.Dispose();
            }
            
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(
                // TextPointer to the start of content in the RichTextBox.
                rtb.Document.ContentStart,
                // TextPointer to the end of content in the RichTextBox.
                rtb.Document.ContentEnd
            );

            // The Text property on a TextRange object returns a string
            // representing the plain text content of the TextRange.
            return textRange.Text;
        }
    }
}
