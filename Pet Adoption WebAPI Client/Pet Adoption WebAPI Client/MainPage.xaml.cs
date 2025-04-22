using Pet_Adoption_WebAPI_Client.Data;
using Pet_Adoption_WebAPI_Client.Models;
using Pet_Adoption_WebAPI_Client.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Contacts;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Pet_Adoption_WebAPI_Client
{

	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{

		public MainPage()
		{
			this.InitializeComponent();

		}

		private void navMainNav_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
		{
			//Switch case statement to control the navivation of the application
			string selectedMenuItem = args.InvokedItemContainer.Content.ToString();
			Type goTo;
			switch (selectedMenuItem)
			{
				case "Home":
					goTo = typeof(HomePage);
					frmMainArea.Navigate(goTo);
					break;
				case "Pets":
					goTo = typeof(PetPage);
					frmMainArea.Navigate(goTo);
					break;
				case "Adoptions":
					goTo = typeof(AdoptionPage);
					frmMainArea.Navigate(goTo);
					break;
                case "Adoption Centre":
                    goTo = typeof(MapPage);
                    frmMainArea.Navigate(goTo);
                    break;

                default:
					break;
			}

		}
	}
}
