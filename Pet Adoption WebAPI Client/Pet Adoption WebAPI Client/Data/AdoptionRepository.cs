using Pet_Adoption_WebAPI_Client.Models;
using Pet_Adoption_WebAPI_Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Data
{
	public class AdoptionRepository : IAdoptionRepository
	{
		private readonly HttpClient client = new HttpClient();

		public AdoptionRepository()
		{
			client.BaseAddress = Jeeves.DBUri;
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<Adoption>> GetAdoptions()
		{
			HttpResponseMessage response = await client.GetAsync("api/adoption");
			if (response.IsSuccessStatusCode)
			{
				List<Adoption> adoptions = await response.Content.ReadAsAsync<List<Adoption>>();
				return adoptions;
			}
			else
			{
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
		}
		public async Task<Adoption> GetAdoption(int AdoptionID)
		{
			HttpResponseMessage response = await client.GetAsync($"api/adoption/{AdoptionID}");
			if (response.IsSuccessStatusCode)
			{
				Adoption adoption = await response.Content.ReadAsAsync<Adoption>();
				return adoption;
			}
			else
			{
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
		}


        public async Task AddAdoption(Adoption adoptionToAdd)
        {
            var response = await client.PostAsJsonAsync("/api/adoption", adoptionToAdd);
            if (!response.IsSuccessStatusCode)
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }

        public async Task UpdateAdoption(Adoption adoptionToUpdate)
        {
            var response = await client.PutAsJsonAsync($"/api/adoption/{adoptionToUpdate.ID}", adoptionToUpdate);
            if (!response.IsSuccessStatusCode)
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }

        public async Task DeleteAdoption(Adoption adoptionToDelete)
        {
            var response = await client.DeleteAsync($"/api/adoption/{adoptionToDelete.ID}");
            if (!response.IsSuccessStatusCode)
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }
    }
}
