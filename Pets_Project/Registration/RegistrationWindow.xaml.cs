using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Pets_Project
{
    /// <summary>
    /// Interaction logic for RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        public String cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\petyt\source\repos\Pets_Project\Pets_Project\pets_db3.mdf;Integrated Security=True";

        public SqlConnection myConnection = default(SqlConnection);

        public SqlCommand myCommand = default(SqlCommand);


        private void button_Click(object sender, RoutedEventArgs e)
        {
            int value = 0;
            if ((bool)radioButton_dog.IsChecked)
                value = 1;
            else if((bool)radioButton_cat.IsChecked)
                value = 2;


            if (password_reg_tb.Password != confirm_password_tb.Password) {
                MessageBox.Show("Паролите не съвпадат!");
                confirm_password_tb.Clear();
            }


            bool result = IsValidEmailAddress(email_tb.Text);
            if (email_tb.Text!="" && !result)
            {
                MessageBox.Show("Невалиден имейл адрес!");
                email_tb.Clear(); 
            }


            try
            {
                if (String.IsNullOrEmpty(email_tb.Text) || String.IsNullOrEmpty(username_reg_tb.Text) ||
                    String.IsNullOrEmpty(password_reg_tb.ToString()) || String.IsNullOrEmpty(confirm_password_tb.ToString()) ||
                    String.IsNullOrEmpty(pet_name_tb.Text) || String.IsNullOrEmpty(password_reg_tb.ToString()) || value == 0 ||
                    String.IsNullOrEmpty(calendar.SelectedDate.Value.ToString()))
                {
                    MessageBox.Show("Моля въведете данни във всички полета!");
                }
                else
                {
                    myConnection = new SqlConnection(cs);
                    /*myCommand.CommandType = System.Data.CommandType.Text;
                    myCommand.CommandText = "INSERT INTO [dbo].[pets] ([type_id], [username], [password], [birthdate], [pet_name], [email]) VALUES (" +
                                        value + ", '" + username_reg_tb.Text + "', '" + password_reg_tb.Password + "', '" +
                                        calendar.SelectedDate.Value.ToString("dd/MM/yyyy") + "', '" + pet_name_tb.Text + "', '" + email_tb.Text + "')";
                    myCommand.Connection = myConnection;

                    myConnection.Open();
                    myCommand.ExecuteNonQuery();
                    Login login = new Login();
                    login.Show();
                    this.Close();
                    myConnection.Close();*/
                    myCommand = new SqlCommand("INSERT INTO [dbo].[pets] ([type_id], [username], [password], [birthdate], [pet_name], [email]) VALUES (" +
                        value + ", '" + username_reg_tb.Text + "', '" + password_reg_tb.Password + "', '" +
                        calendar.SelectedDate.Value.ToString("dd/MM/yyyy") + "', N'" + pet_name_tb.Text + "', '" +
                        email_tb.Text + "')", myConnection);
                    myCommand.Connection.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    /*if (myReader.Read() == true)
                    { 
                        MessageBox.Show("Your registration is successful!");
                    }*/
                    if (myConnection.State == ConnectionState.Open)
                    {
                        myConnection.Dispose();
                    }
                    MessageBox.Show("Your registration is successful!");
                    Login login = new Login();
                    login.Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }



        private void email_tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        public static bool IsValidEmailAddress(string email)
        {
            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(email);
        }
    }
}

