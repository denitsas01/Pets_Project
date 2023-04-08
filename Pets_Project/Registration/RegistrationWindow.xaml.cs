using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
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
    public partial class RegistrationWindow : Window
    {
        Login login = new Login();
        SqlConnection myConnection;

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        public SqlCommand myCommand = default(SqlCommand);


        private void button_Click(object sender, RoutedEventArgs e)
        {
            myConnection = new SqlConnection(login.cs);
            int typeID = 0;
            int petID = 0;
            int vaccID = 0;
            List<int> vaccIDs = new List<int>();

            if ((bool)radioButton_dog.IsChecked)
                typeID = 1;
            else if((bool)radioButton_cat.IsChecked)
                typeID = 2;

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
                    String.IsNullOrEmpty(pet_name_tb.Text) || String.IsNullOrEmpty(password_reg_tb.ToString()) || typeID == 0 ||
                    String.IsNullOrEmpty(calendar.SelectedDate.Value.ToString()))
                {
                    MessageBox.Show("Моля въведете данни във всички полета!");
                }
                else
                {
                    myCommand = new SqlCommand("INSERT INTO [dbo].[pets] ([type_id], [username], [password], [birthdate], [pet_name], [health], [email]) " +
                        "VALUES ( @petID, @username, @pass, @birthdate, @name, @health, @email )" , myConnection);
                    myCommand.Parameters.AddWithValue("@petID", typeID);
                    myCommand.Parameters.AddWithValue("@username", username_reg_tb.Text);
                    myCommand.Parameters.AddWithValue("@pass", password_reg_tb.Password);
                    myCommand.Parameters.AddWithValue("@birthdate", calendar.SelectedDate.Value);
                    myCommand.Parameters.AddWithValue("@health", "няма");
                    myCommand.Parameters.AddWithValue("@email", email_tb.Text);
                    myCommand.Parameters.AddWithValue("@name", pet_name_tb.Text);
                    myCommand.Connection.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);
                    myCommand.Connection.Close();
                    
                    //ако животното е куче, избираме всички ваксини за куче, същото е и за котките
                    SqlCommand selectVacc = new SqlCommand("SELECT vacc_id FROM vaccinations WHERE type_id = @typeID", myConnection);
                    selectVacc.Parameters.Add(new SqlParameter("typeID", typeID));
                    selectVacc.Connection.Open();
                    SqlDataReader reader = selectVacc.ExecuteReader(CommandBehavior.CloseConnection);
                    while (reader.Read()) {
                        vaccIDs.Add((int)reader["vacc_id"]);
                    }
                    selectVacc.Connection.Close();

                    //избираме последното регистрирано животно и го записваме в променливата
                    SqlCommand selectPet = new SqlCommand("SELECT TOP 1 * FROM pets ORDER BY pet_id DESC", myConnection);
                    selectPet.Connection.Open();
                    SqlDataReader readerPet = selectPet.ExecuteReader(CommandBehavior.CloseConnection);
                    while (readerPet.Read())
                    {
                        petID = (int)readerPet["pet_id"];
                    }
                    selectPet.Connection.Close();

                    //запазваме всяка от ваксините от списъка в таблица "направени ваксини",
                    //като сетваме булевата променлива на false - тоест като ненаправена ваксина,
                    //за да може да се зареждат в "Предстоящи ваксини"
                    for (int i = 0; i < vaccIDs.Count; i++)
                    {
                        SqlCommand addToRV = new SqlCommand("INSERT INTO received_vaccs(vacc_id, pet_id, date_received, isReceived)" +
                            "VALUES(@vaccID, @petID, @dateReceived, 0 )", myConnection);
                        addToRV.Parameters.AddWithValue("@petID", petID);
                        addToRV.Parameters.AddWithValue("@vaccID", vaccIDs[i]);
                        addToRV.Parameters.AddWithValue("@dateReceived", DateTime.Now.ToShortTimeString());
                        addToRV.Connection.Open();
                        addToRV.ExecuteNonQuery();
                        addToRV.Connection.Close();
                    } 

                    MessageBox.Show("Регистрацията Ви е успешна!");
                    Login login = new Login();
                    login.Show();
                    this.Close();

                    if (myConnection.State == ConnectionState.Open)
                    {
                        myConnection.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public bool IsValidEmailAddress(string email)
        {
            /*try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }*/

            Regex regex = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
            return regex.IsMatch(email);
        }
    }
}

