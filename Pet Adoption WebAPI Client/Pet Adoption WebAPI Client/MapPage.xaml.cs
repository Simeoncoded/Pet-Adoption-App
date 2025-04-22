using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;

namespace Pet_Adoption_WebAPI_Client
{
	public sealed partial class MapPage : Page
	{
		private Geopoint userLocation; // Store user location

		public MapPage()
		{
			this.InitializeComponent();
			this.Loaded += MapPage_Loaded;
		}

		private async void MapPage_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
		{
			await SetUserLocation();
			LoadLocationsWithoutKey();
		}

		private async System.Threading.Tasks.Task SetUserLocation()
		{
			try
			{
				var geolocator = new Geolocator { DesiredAccuracyInMeters = 10 };
				Geoposition pos = await geolocator.GetGeopositionAsync();

				userLocation = pos.Coordinate.Point;

			//your location pin
				MyMap.MapElements.Add(new MapIcon
				{
					Location = userLocation,
					Title = "Your location",
					NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
					ZIndex = 1
				});
			}
			catch (Exception ex)
			{
				await new MessageDialog($"Failed to get user location: {ex.Message}").ShowAsync();
			}
		}

		private void LoadLocationsWithoutKey()
		{
			var locations = new List<(string Name, double Lat, double Lon)>
			{
				("Robyn’s Rescue Mutts", 43.00918636439926, -79.25288244387845),
				("Niagara SPCA Cat Adoption Ctr", 43.016089081344596,  -79.24695028747657),
				("Welland & District Humane Society", 42.97126484272049,  -79.25948156774632),
				("Humane Society of Greater Niagara", 43.158326165760364, -79.26687804700167),
				("Happy Days Sanctuary", 42.95885476639752, -79.1093272622447),
				("Buddy's Second Chance Rescue", 43.039692716233176, -78.81222983048599),
				("Awesome Paws Rescue", 42.944872915767235, -78.78437774850913),
				("Port Colborne Animal Shelter", 42.9221174545292, -79.24706383433697),
				("Hamilton/Burlington SPCA", 43.18911830356691, -79.82221592961291),
				("Oakville & Milton Humane Society", 43.46737962455167, -79.66765787983122),
				("Fort Erie SPCA", 42.93321878394531, -78.91629171587738)
			};

			foreach (var loc in locations)
			{
				var geopoint = new Geopoint(new BasicGeoposition { Latitude = loc.Lat, Longitude = loc.Lon });

				var icon = new MapIcon
				{
					Location = geopoint,
					Title = loc.Name,
					NormalizedAnchorPoint = new Windows.Foundation.Point(0.5, 1),
					ZIndex = 0
				};

				MyMap.MapElements.Add(icon);
			}

			// Center on the first location
			MyMap.Center = new Geopoint(new BasicGeoposition { Latitude = locations[0].Lat, Longitude = locations[0].Lon });
			MyMap.ZoomLevel = 12;
		}

		private async void MyMap_MapElementClick(MapControl sender, MapElementClickEventArgs args)
		{
			if (args.MapElements[0] is MapIcon clickedIcon && userLocation != null)
			{
				BasicGeoposition placePos = clickedIcon.Location.Position;
				BasicGeoposition userPos = userLocation.Position;

				var placePoint = new Geopoint(placePos);
				var userPoint = new Geopoint(userPos);

				double distance = GetDistanceInKm(userPos, placePos);

				string msg = $"Distance from your location to \"{clickedIcon.Title}\" is about {distance:F2} km.";
				await new MessageDialog(msg).ShowAsync();
			}
		}

		private double GetDistanceInKm(BasicGeoposition pos1, BasicGeoposition pos2)
		{
			var R = 6371.0; // Earth's radius in KM
			var dLat = ToRadians(pos2.Latitude - pos1.Latitude);
			var dLon = ToRadians(pos2.Longitude - pos1.Longitude);

			var lat1 = ToRadians(pos1.Latitude);
			var lat2 = ToRadians(pos2.Latitude);

			var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
					Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
			var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
			return R * c;
		}

		private double ToRadians(double deg)
		{
			return deg * Math.PI / 180.0;
		}
	}
}