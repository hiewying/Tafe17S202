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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace StartFinance.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AppointmentsPage : Page
    {
        SQLiteConnection conn;// adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");
        public AppointmentsPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //Initializing a database
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            //Creating table 
            conn.CreateTable<Appointments>();
            //DateStamp.Date = DateTime.Now; //gets current date and time
            AppDate.Date = DateTime.Now;

            Results();
        }

        public void Results()
        {
            // Creating table
            conn.CreateTable<Appointments>();
            var query = conn.Table<Appointments>();
            AppointmentList.ItemsSource = query.ToList();

        }

        private async void AddItem_Click(object sender, RoutedEventArgs e)
        {

            string Date = AppDate.Date.ToString("d");


            try
            {
                // checks if account name is null
                if (AppNametxt.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Appointment Name not entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (AppNametxt.Text.ToString() == "AppointmentName")
                {
                    MessageDialog variableerror = new MessageDialog("You cannot use this name", "Oops..!");
                }
                else
                {   // Inserts the data
                    conn.Insert(new Appointments()
                    {
                        AppointmentName = AppNametxt.Text,
                        DateOfApp = Date,
                        Time = TimePick.Text,
                        Meridiem = DayNightPick.SelectedValue.ToString(),

                    });
                    Results();
                    ClearFields();
                }

            }
            catch (Exception ex)
            {   // Exception to display when amount is invalid or not numbers
                if (ex is FormatException)
                {
                    MessageDialog dialog = new MessageDialog("You forgot to enter the Appointment or entered an invalid data", "Oops..!");
                    await dialog.ShowAsync();
                }   // Exception handling when SQLite contraints are violated
                else if (ex is SQLiteException)
                {
                    MessageDialog dialog = new MessageDialog("Appointment Name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }

        private async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

            MessageDialog ShowConf = new MessageDialog("Are you sure you want to delete all information about this appointment?", "Important");
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
                //checks if data is null else inserts
                try
                {
                    int SelectedInfo = ((Appointments)AppointmentList.SelectedItem).ID;
                    var querydel = conn.Query<Appointments>("DELETE FROM Appointments WHERE ID = '" + SelectedInfo + "'");
                    Results();
                    ClearFields();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Delete", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

        private async void UpdateItem_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog ShowConf = new MessageDialog("Are you sure you want to update all information about this appointment?", "Important");
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
                //checks if data is null else inserts
                try
                {
                    int SelectedInfo = ((Appointments)AppointmentList.SelectedItem).ID;
                    var queryUpdate = conn.Query<Appointments>("UPDATE Appointments SET AppointmentName = '" + AppNametxt.Text + "', DateOfApp ='" + AppDate.Date.ToString("d") + "', Time='" + TimePick.Text + "', Meridiem = '" + DayNightPick.SelectedValue.ToString() + "'WHERE ID =" + SelectedInfo);
                    Results();
                    ClearFields();
                }
                catch (NullReferenceException)
                {
                    MessageDialog ClearDialog = new MessageDialog("Please select the item to Update", "Oops..!");
                    await ClearDialog.ShowAsync();
                }
            }
            else
            {
                //
            }
        }

            private void ClearFields()
        {
            //Clear Fields
            AppNametxt.Text = "";
            AppDate.Date = DateTime.Now;
            TimePick.Text = "";
            DayNightPick.SelectedValue = -1;

        }

        private void AppointmentList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentList.SelectedItem != null)
            {
                AppNametxt.Text = ((Appointments)AppointmentList.SelectedItem).AppointmentName;
                string doa = ((Appointments)AppointmentList.SelectedItem).DateOfApp;
                AppDate.Date = DateTime.Parse(doa);
                TimePick.Text = ((Appointments)AppointmentList.SelectedItem).Time;
                DayNightPick.SelectedValue = ((Appointments)AppointmentList.SelectedItem).Meridiem;

            }
        }

    }
}
