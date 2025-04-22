using Pet_Adoption_WebAPI_Client.Models;
using Pet_Adoption_WebAPI_Client.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Data
{
	public class PetRepository : IPetRepository
	{
		private readonly HttpClient client = new HttpClient();

		public PetRepository()
		{
			client.BaseAddress = Jeeves.DBUri;
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<List<Pet>> GetPets()
		{
			HttpResponseMessage response = await client.GetAsync("api/pet");
			if (response.IsSuccessStatusCode)
			{
				List<Pet> pets = await response.Content.ReadAsAsync<List<Pet>>();
				return pets;
			}
			else
			{
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
		}

        public async Task<List<Pet>> GetAvailablePets()
        {
            HttpResponseMessage response = await client.GetAsync("api/pet/available");
            if (response.IsSuccessStatusCode)
            {
                List<Pet> pets = await response.Content.ReadAsAsync<List<Pet>>();
                return pets;
            }
            else
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }

        public async Task<Pet> GetPet(int PetID)
		{
			HttpResponseMessage response = await client.GetAsync($"api/pet/{PetID}");
			if (response.IsSuccessStatusCode)
			{
				Pet pet = await response.Content.ReadAsAsync<Pet>();
				return pet;
			}
			else
			{
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }

        public async Task AddPet(Pet petToAdd)
        {
            var response = await client.PostAsJsonAsync("/api/pet", petToAdd);
            if (!response.IsSuccessStatusCode)
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }

        public async Task UpdatePet(Pet petToUpdate)
        {
            var response = await client.PutAsJsonAsync($"/api/pet/{petToUpdate.ID}", petToUpdate);
            if (!response.IsSuccessStatusCode)
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }

        public async Task DeletePet(Pet petToDelete)
        {
            var response = await client.DeleteAsync($"/api/pet/{petToDelete.ID}");
            if (!response.IsSuccessStatusCode)
            {
                var ex = Jeeves.CreateApiException(response);
                throw ex;
            }
        }
    }
}
