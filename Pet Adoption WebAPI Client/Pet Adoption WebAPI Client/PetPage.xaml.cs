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
	public sealed partial class PetPage : Page
	{
		private readonly IPetRepository petRepository;
		private readonly IAdoptionRepository adoptionRepository;
		public PetPage()
		{
			this.InitializeComponent();
			petRepository = new PetRepository();
			adoptionRepository = new AdoptionRepository();

			ShowPets();
		}

		private async void ShowPets()
		{
			//Show Progress
			progRing.IsActive = true;
			progRing.Visibility = Visibility.Visible;

			try
			{
				List<Pet> pets;
				pets = await petRepository.GetPets();
				petList.ItemsSource = pets.OrderBy(p => p.Name);

			}
            catch (ApiException apiEx)
            {
                string errMsg = "Errors:" + Environment.NewLine;
                foreach (var error in apiEx.Errors)
                {
                    errMsg += Environment.NewLine + "-" + error;
                }
                Jeeves.ShowMessage("Problem accessing Pets:", errMsg);
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
			finally
			{
				progRing.IsActive = false;
				progRing.Visibility = Visibility.Collapsed;
			}
		}
        private void petGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the detail page
            Frame.Navigate(typeof(PetDetailPage), (Pet)e.ClickedItem);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Pet newPet = new Pet();

            // Navigate to the detail page
            Frame.Navigate(typeof(PetDetailPage), newPet);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			ShowPets();
		}
	}
}
