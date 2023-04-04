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

namespace Pets_Project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int petID;

        public MainWindow(int petID)
        {
            InitializeComponent();

            //get the current date
            current_date.Text = DateTime.Now.ToString("D");
            current_date2.Text = DateTime.Now.ToString("D");
            current_date3.Text = DateTime.Now.ToString("D");

            this.petID = petID;

        }

        Login login = new Login();
        SqlConnection myConnection;
        SqlCommand myCommand;
        SqlDataAdapter adapt;

        private void logout_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void mark_as_completed(object sender, EventArgs e)
        {
            try
            {
                myConnection = new SqlConnection(login.cs);
                myCommand = new SqlCommand("SELECT vaccinations.vacc_name, received.isReceived FROM vaccinations JOIN received_vaccs ON vaccinations.vacc_id = received_vaccs.vacc_id WHERE received_vaccs.pet_id=@petID", myConnection);
                SqlParameter pet_id = new SqlParameter("@petID", SqlDbType.Int);
                pet_id.Value = petID;
                myCommand.Parameters.Add(pet_id);

                myCommand.Connection.Open();
                SqlDataReader myReader = myCommand.ExecuteReader(CommandBehavior.CloseConnection);

                FlowDocument flowDocument = new FlowDocument();
                List list = new List();
                list.MarkerStyle = TextMarkerStyle.Disc;
                Paragraph paragraph = new Paragraph();


                // Loop through the rows in the result set and add each item to the bullet list
                while (myReader.Read())
                {
                    // Create a new ListItem to hold the checkbox and text
                    ListItem listItem = new ListItem();

                    // Create a new StackPanel to hold the checkbox and text
                    StackPanel stackPanel = new StackPanel();

                    // Create a new CheckBox and set its name and content
                    CheckBox checkBox = new CheckBox();
                    checkBox.Name = "checkbox" + myReader["isReceived"].ToString();

                    // Create a new TextBlock and set its text
                    TextBlock textBlock = new TextBlock();
                    textBlock.Text = myReader["vacc_name"].ToString();

                    // Add the CheckBox and TextBlock to the StackPanel
                    stackPanel.Children.Add(textBlock);
                    stackPanel.Children.Add(checkBox);

                    paragraph.Inlines.Add(textBlock);
                    paragraph.Inlines.Add(checkBox);
                    // Set the StackPanel as the content of the ListItem
                    listItem.Blocks.Add(paragraph);

                    // Add the ListItem to the bullet list
                    list.ListItems.Add(listItem);
                }

                // Add the bullet list to the FlowDocument
                flowDocument.Blocks.Add(list);

                // Set the FlowDocument as the content of the RichTextBox
                richTextBox.Document = flowDocument;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Грешка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
