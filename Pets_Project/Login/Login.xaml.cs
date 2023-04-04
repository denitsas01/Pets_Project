using System.Data;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;

namespace Pets_Project
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public string cs = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\UNI\3\3.2\USP\Pets_Project-master\Pets_Project\Database\PetsDB.mdf;Integrated Security=True";
        public SqlConnection myConnection = default(SqlConnection);
        public SqlCommand myCommand = default(SqlCommand);

        public Login()
        {
            InitializeComponent();
        }

        int petID;

        private void register_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow window = new RegistrationWindow();
            window.Show();
            this.Hide();
        }

        //
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
                    petName = myReader.GetString(3);
                    //TO DO - create a custom message box in order to style it
                    MessageBox.Show("Добре дошъл " + petName + " !");
                    if(myReader.GetInt32(1) == 1)
                    {
                        this.Hide();
                        MainWindow window = new MainWindow();
                        BitmapImage bitmapImage = new BitmapImage();

                        // Set the source of the BitmapImage to the path of the image file
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("/Images/Buttons/happy.png", UriKind.Relative);
                        bitmapImage.EndInit();

                        // Set the Source property of the Image control to the BitmapImage
                        window.petimg.Source = bitmapImage;
                        window.petName.Text = petName;
                        window.Show();
                    }
                    else
                    {
                        this.Hide();
                        MainWindow window = new MainWindow();
                        BitmapImage bitmapImage = new BitmapImage();

                        // Set the source of the BitmapImage to the path of the image file
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("/Images/Buttons/laughing.png", UriKind.Relative);
                        bitmapImage.EndInit();

                        // Set the Source property of the Image control to the BitmapImage
                        window.petimg.Source = bitmapImage;
                        window.petName.Text = petName;
                        window.Show();
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
    }
}
