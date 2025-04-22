using Pet_Adoption_WebAPI_Client.Data;
using Pet_Adoption_WebAPI_Client.Models;
using Pet_Adoption_WebAPI_Client.Utilities;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Pet_Adoption_WebAPI_Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PetDetailPage : Page
    {
        Pet view;
        IPetRepository petRepository;
        bool InsertMode;
        public PetDetailPage()
        {
            this.InitializeComponent();
            petRepository = new PetRepository();

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            view = (Pet)e.Parameter;

            if (view == null)
            {
                Jeeves.ShowMessage("Error", "Invalid pet details.");
                return;
            }


            if (view.ID == 0) //Inserting
            {
                //Disable the delete button if adding
                btnDelete.IsEnabled = false;
                InsertMode = true;
            }
            else
            {
                InsertMode = false;
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(view.Name))
                {
                    Jeeves.ShowMessage("Error", "You must provide the Pet Name.");
                }
                else if (view.Age < 0)
                {
                    Jeeves.ShowMessage("Error", "Age must be a valid number.");
                }
                else
                {
                    if (InsertMode)
                    {
                        await petRepository.AddPet(view);
                    }
                    else
                    {
                        await petRepository.UpdatePet(view);
                    }
                    Frame.GoBack();
                }
            }
            catch (AggregateException aex)
            {
                string errMsg = "Errors:" + Environment.NewLine;
                foreach (var exception in aex.InnerExceptions)
                {
                    errMsg += Environment.NewLine + exception.Message;
                }
                Jeeves.ShowMessage("One or more exceptions has occurred:", errMsg);
            }
            catch (ApiException apiEx)
            {
                string errMsg = "Errors:" + Environment.NewLine;
                foreach (var error in apiEx.Errors)
                {
                    errMsg += Environment.NewLine + "-" + error;
                }
                Jeeves.ShowMessage("Problem Saving the Record:", errMsg);
            }
            catch (Exception ex)
            {
                if (ex.GetBaseException().Message.Contains("connection with the server"))
                {
                    Jeeves.ShowMessage("Error", "No connection with the server.");
                }
                else
                {
                    Jeeves.ShowMessage("Error", "Could not complete operation.");
                }
            }
        }
        private async void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            string strTitle = "Confirm Delete";
            string strMsg = "Are you certain that you want to delete " + view.Name + "?";
            ContentDialogResult result = await Jeeves.ConfirmDialog(strTitle, strMsg);
            if (result == ContentDialogResult.Secondary)
            {
                try
                {
                    await petRepository.DeletePet(view);
                    Frame.GoBack();
                }
                catch (AggregateException aex)
                {
                    string errMsg = "Errors:" + Environment.NewLine;
                    foreach (var exception in aex.InnerExceptions)
                    {
                        errMsg += Environment.NewLine + exception.Message;
                    }
                    Jeeves.ShowMessage("One or more exceptions has occurred:", errMsg);
                }
                catch (ApiException apiEx)
                {
                    string errMsg = "Errors:" + Environment.NewLine;
                    foreach (var error in apiEx.Errors)
                    {
                        errMsg += Environment.NewLine + "-" + error;
                    }
                    Jeeves.ShowMessage("Problem Deleting the Record:", errMsg);
                }
                catch (Exception ex)
                {
                    if (ex.GetBaseException().Message.Contains("connection with the server"))
                    {
                        Jeeves.ShowMessage("Error", "No connection with the server.");
                    }
                    else
                    {
                        Jeeves.ShowMessage("Error", "Could not complete operation");
                    }
                }
            }
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }

}

