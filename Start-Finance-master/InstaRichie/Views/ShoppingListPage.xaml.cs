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
    public sealed partial class ShoppingListPage : Page
    {
        SQLiteConnection conn; // adding an SQLite connection
        string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "Findata.sqlite");

        public ShoppingListPage()
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
            conn.CreateTable<ShoppingList>();
            var query = conn.Table<ShoppingList>();
            ShoppingInfoList.ItemsSource = query.ToList();
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (shoppingDate.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Shopping date not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (itemName.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Item name not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else if (priceQuoted.Text.ToString() == "")
                {
                    MessageDialog dialog = new MessageDialog("Price not Entered", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    // insert the data
                    conn.Insert(new ShoppingList()
                    {
                        ShoppingDate = shoppingDate.Date.ToString("d"),
                        NameOfItem = itemName.Text.ToString(),
                        PriceQuoted = priceQuoted.Text.ToString(),
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
                    MessageDialog dialog = new MessageDialog("This item name already exist, Try Different Name", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    /// no idea
                }
            }
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

                    int ID = ((ShoppingList)ShoppingInfoList.SelectedItem).ID;
                    var queryUpdate = conn.Query<ShoppingList>("UPDATE ShoppingList set ShoppingDate ='" + shoppingDate.Date.ToString("d") + "', NameOfItem = '" + itemName.Text + "', PriceQuoted = '" + priceQuoted.Text + "' where ID = " + ID);
                    ShoppingInfoList.ItemsSource = queryUpdate.ToList();
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
                int InfoSelection = ((ShoppingList)ShoppingInfoList.SelectedItem).ID;
                if (InfoSelection == 0)
                {
                    MessageDialog dialog = new MessageDialog("No item is selected", "Oops..!");
                    await dialog.ShowAsync();
                }
                else
                {
                    conn.CreateTable<ShoppingList>();
                    var query1 = conn.Table<ShoppingList>();
                    var query3 = conn.Query<ShoppingList>("DELETE FROM ShoppingList WHERE ID ='" + InfoSelection + "'");
                    ShoppingInfoList.ItemsSource = query1.ToList();
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

            if (ShoppingInfoList.SelectedItem != null)
            {
                string dateShopping = ((ShoppingList)ShoppingInfoList.SelectedItem).ShoppingDate;
                shoppingDate.Date = DateTime.Parse(dateShopping);
                itemName.Text = ((ShoppingList)ShoppingInfoList.SelectedItem).NameOfItem;
                priceQuoted.Text = ((ShoppingList)ShoppingInfoList.SelectedItem).PriceQuoted;
            }
        }

        private void ResetFields()
        {
            shoppingDate.Date = DateTime.Now;
            itemName.Text = string.Empty;
            priceQuoted.Text = string.Empty;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Results();
        }
    }
}
