using Pet_Adoption_WebAPI_Client.Data;
using Pet_Adoption_WebAPI_Client.Models;
using Pet_Adoption_WebAPI_Client.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
    public sealed partial class AdoptionDetailPage : Page
    {
        Adoption view;
        IAdoptionRepository adoptionRepository;
        IPetRepository petRepository;
        bool InsertMode;
        public AdoptionDetailPage()
        {
            this.InitializeComponent();
            adoptionRepository = new AdoptionRepository();
            petRepository = new PetRepository();
            fillDropDown();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            view = (Adoption)e.Parameter;

            if (view == null)
            {
                Jeeves.ShowMessage("Error", "Invalid Adoption details.");
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

        private async void fillDropDown()
        {
            try
            {
                List<Pet> pets = await petRepository.GetAvailablePets();

                if (!InsertMode && view.Pet != null)
                {
                    // If the pet isn't already in the list, add it
                    if (!pets.Any(p => p.ID == view.Pet.ID))
                    {
                        pets.Insert(0, view.Pet); 
                    }
                }

                // Bind to the ComboBox
                PetCombo.ItemsSource = pets.OrderBy(d => d.PSummary);

                // Now you can assign the DataContext for the page
                this.DataContext = view;
            }
            catch (ApiException apiEx)
            {
                string errMsg = "Errors:" + Environment.NewLine;
                foreach (var error in apiEx.Errors)
                {
                    errMsg += Environment.NewLine + "-" + error;
                }
                Jeeves.ShowMessage("Problem filling Pet Selection:", errMsg);
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


        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (view.PetID == 0)
                {
                    Jeeves.ShowMessage("Error", "You must select the Pet you want to Adopt");
                }
                else
                {
                    if (InsertMode)
                    {
                        await adoptionRepository.AddAdoption(view);
                    }
                    else
                    {
                        await adoptionRepository.UpdateAdoption(view);
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
            string strMsg = "Are you certain that you want to delete this adoption record " + view.Summary + "?";
            ContentDialogResult result = await Jeeves.ConfirmDialog(strTitle, strMsg);
            if (result == ContentDialogResult.Secondary)
            {
                try
                {
                    await adoptionRepository.DeleteAdoption(view);
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
