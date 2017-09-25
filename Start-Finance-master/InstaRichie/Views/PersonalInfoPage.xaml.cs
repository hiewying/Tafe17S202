using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using StartFinance.Models;
using SQLite.Net;
using System.Data;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PersonalInfoPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public PersonalInfoPage()
        {
            this.InitializeComponent();

            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            /// Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);

            // Creating table   
            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<PersonalInfo>();
            var query = conn.Table<PersonalInfo>();
            InformationList.ItemsSource = query.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {


                // checks if values are null
                if (firstName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("First Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (lastName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Last Name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (dob.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("DOB not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (comboBox_gender.SelectedIndex == -1)
                {
                    MessageDialog dialog = new MessageDialog("Gender not Selected", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (email.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Email not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (phone.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Phone not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {   // Inserts the data
                    conn.Insert(new PersonalInfo()
                    {
                        FirstName = firstName.Text.ToString(),
                        LastName = lastName.Text.ToString(),
                        DOB = dob.Date.ToString("d"),
                        Gender = comboBox_gender.SelectedValue.ToString(),
                        EmailAddress = email.Text.ToString(),
                        Phone = phone.Text.ToString(),
                    });
                    Results();
                    ResetFields();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the details or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("This Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
        }

        // Clears the fields
        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Do you want to update", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Update")
            {
                Id = 0
            });
            ShowConf.Commands.Add(new UICommand("Cancel")
            {
                Id = 1
            });
            ShowConf.DefaultCommandIndex = 0;
            ShowConf.CancelCommandIndex = 1;


            var result = await ShowConf.ShowAsync();
            if ((int)result.Id == 0)
            {
                // checks if data is null else inserts
                try
                {

                    int ID = ((PersonalInfo)InformationList.SelectedItem).ID;
                    var queryUpdate = conn.Query<PersonalInfo>("UPDATE PersonalInfo set FirstName ='" + firstName.Text + "', LastName = '" + lastName.Text + "', DOB = '" + dob.Date.ToString("d") + "', Gender = '" + comboBox_gender.SelectionBoxItem + "', EmailAddress = '" + email.Text + "', Phone = '" + phone.Text + "' where ID = " + ID);
                    InformationList.ItemsSource = queryUpdate.ToList();
                    Results();
                    ResetFields();


                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("No item is selected", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }

        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int InfoSelection = ((PersonalInfo)InformationList.SelectedItem).ID;
                if (InfoSelection == 0)
                {
                    MessageDialog dialog = new MessageDialog("No item is selected", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<PersonalInfo>();
                    var query1 = conn.Table<PersonalInfo>();
                    var query3 = conn.Query<PersonalInfo>("DELETE FROM PersonalInfo WHERE ID ='" + InfoSelection + "'");
                    InformationList.ItemsSource = query1.ToList();
                    ResetFields();
                }
            }
            catch (NullReferenceException)
            {
                MessageDialog dialog = new MessageDialog("No item is selected", "Oops..!");
                await dialog.ShowAsync();
            }

        }

        private void InformationList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (InformationList.SelectedItem != null)
            {
                firstName.Text = ((PersonalInfo)InformationList.SelectedItem).FirstName;
                lastName.Text = ((PersonalInfo)InformationList.SelectedItem).LastName;
                string dateOB = ((PersonalInfo)InformationList.SelectedItem).DOB;
                dob.Date = DateTime.Parse(dateOB);
                comboBox_gender.SelectedValue = ((PersonalInfo)InformationList.SelectedItem).Gender;
                email.Text = ((PersonalInfo)InformationList.SelectedItem).EmailAddress;
                phone.Text = ((PersonalInfo)InformationList.SelectedItem).Phone;
            }
        }

        private void ResetFields()
        {
            firstName.Text = string.Empty;
            lastName.Text = string.Empty;
            dob.Date = DateTime.Now;
            comboBox_gender.SelectedValue = -1;
            email.Text = string.Empty;
            phone.Text = string.Empty;

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
