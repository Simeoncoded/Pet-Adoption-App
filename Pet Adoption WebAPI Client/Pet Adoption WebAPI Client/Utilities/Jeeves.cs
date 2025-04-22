using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Pet_Adoption_WebAPI_Client.Utilities
{
	public static class Jeeves
	{
		//For Local API
		public static Uri DBUri = new Uri("https://petadoptionwebapi.azurewebsites.net/");

		//For API on the Internet

		internal static async void ShowMessage(string strTitle, string Msg)
		{
			ContentDialog diag = new ContentDialog()
			{
				Title = strTitle,
				Content = Msg,
				PrimaryButtonText = "Ok",
				DefaultButton = ContentDialogButton.Primary
			};
			_ = await diag.ShowAsync();
		}
		internal static async Task<ContentDialogResult> ConfirmDialog(string strTitle, string Msg)
		{
			ContentDialog diag = new ContentDialog()
			{
				Title = strTitle,
				Content = Msg,
				PrimaryButtonText = "No",
				SecondaryButtonText = "Yes",
				DefaultButton = ContentDialogButton.Primary
			};
			ContentDialogResult result = await diag.ShowAsync();
			return result;
		}

        public static ApiException CreateApiException(HttpResponseMessage response)
        {
            var httpErrorObject = response.Content.ReadAsStringAsync().Result;

            // Create an anonymous object to use as the template for deserialization:
            var anonymousErrorObject =
                new { message = "", errors = new Dictionary<string, string[]>() };

            // Deserialize:
            var deserializedErrorObject =
                JsonConvert.DeserializeAnonymousType(httpErrorObject, anonymousErrorObject);

            // Now wrap into an exception which best fullfills the needs of your application:
            var ex = new ApiException(response);

            //Check for a message
            if (deserializedErrorObject?.message != null)
            {
                ex.Data.Add(-1, deserializedErrorObject?.message);
            }
            // Or sometimes, there may be Model Errors:
            if (deserializedErrorObject.errors != null)
            {
                foreach (var err in deserializedErrorObject?.errors)
                {
                    //Note that we only want the first error message
                    //string for a "key" becuase it is the one we created
                    ex.Data.Add(err.Key, err.Value[0]);
                }
            }
            return ex;
        }


    }
}
