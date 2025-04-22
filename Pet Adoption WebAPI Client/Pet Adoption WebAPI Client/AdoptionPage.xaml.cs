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
	public sealed partial class AdoptionPage : Page
	{
		private readonly IPetRepository petRepository;
		private readonly IAdoptionRepository adoptionRepository;
		public AdoptionPage()
		{
			this.InitializeComponent();
			petRepository = new PetRepository();
			adoptionRepository = new AdoptionRepository();


            ShowAdoptions();
			
		}
		private async void ShowAdoptions()
		{
			//Show Progress
			progRing.IsActive = true;
			progRing.Visibility = Visibility.Visible;

			try
			{
				List<Adoption> adoptions;
				adoptions = await adoptionRepository.GetAdoptions();
				adoptionList.ItemsSource = adoptions;

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

        private void  adoptionGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the detail page
            Frame.Navigate(typeof(AdoptionDetailPage), (Adoption)e.ClickedItem);
        }

        // Open the chat link in the default browser 
        private async void OpenChatInBrowser(object sender, RoutedEventArgs e)
        {
            var uri = new Uri("https://tawk.to/chat/67ee189dce5c9019179c16f0/1int109ff");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            Adoption newAdoption = new Adoption();

            // Navigate to the detail page
            Frame.Navigate(typeof(AdoptionDetailPage), newAdoption);
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
		{
			ShowAdoptions();
		}
	}
}
