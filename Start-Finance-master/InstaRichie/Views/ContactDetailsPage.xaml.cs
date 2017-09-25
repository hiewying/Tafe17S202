using SQLite.Net;
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
using StartFinance.Models;
using Windows.UI.Popups;





// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContactDetailsPage : Page
    {

        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ContactDetailsPage()
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
            conn.CreateTable<ContactDetails>();
            var query = conn.Table<ContactDetails>();
            ContactListView.ItemsSource = query.ToList();
        }

        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (IDtextBox.Text.ToString() == "" || firstNTextBox.Text.ToString() == "" || LastNTextBox.Text.ToString() == "" || phoneTextBox.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Pleas fill in the required fields", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.Insert(new ContactDetails()
                    {
                        contactID = IDtextBox.Text,
                        firstName = firstNTextBox.Text,
                        lastName = LastNTextBox.Text,
                        phoneNumber = phoneTextBox.Text,
                    });
                    Results();
                }
            }

            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    //No Idea
                }
            }
        }

        private async void ClearFileds_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ClearDialog = new MessageDialog("Cleared", "information");
            await ClearDialog.ShowAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Deleting this Contact will delete all details of this contact", "Important");
            ShowConf.Commands.Add(new UICommand("Yes, Delete")
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
                try
                {
                    string ContactLabel = ((ContactDetails)ContactListView.SelectedItem).contactID;
                    var delQuery = conn.Query<ContactDetails>("DELETE FROM ContactDetails WHERE contactID='" + ContactLabel + "'");
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //No Idea
            }

        }

        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Do You Want To Update", "Important");
            ShowConf.Commands.Add(new UICommand("Yes Update")
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
                try
                {
                    string ContactLabels = ((ContactDetails)ContactListView.SelectedItem).contactID;
                    var updateQuery = conn.Query<ContactDetails>("UPDATE ContactDetails set contactID ='" + IDtextBox.Text + "', firstName ='" + firstNTextBox.Text + "', lastName ='" + LastNTextBox.Text + "',phoneNumber ='" + phoneTextBox.Text + "' where contactID =" + ContactLabels);
                    Results();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDailog = new MessageDialog("Please Select the Item to Update", "Oops....!");
                    await ClearDailog.ShowAsync();
                }
            }


        }

        private void ContactListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(ContactListView.SelectedItem != null)
            {
                IDtextBox.Text = ((ContactDetails)ContactListView.SelectedItem).contactID;
                firstNTextBox.Text = ((ContactDetails)ContactListView.SelectedItem).firstName;
                LastNTextBox.Text = ((ContactDetails)ContactListView.SelectedItem).lastName;
                phoneTextBox.Text = ((ContactDetails)ContactListView.SelectedItem).phoneNumber;
            }
        }
    }
}


