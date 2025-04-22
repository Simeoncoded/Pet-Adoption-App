using Pet_Adoption_WebAPI_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Data
{
	public interface IPetRepository
	{
		Task<List<Pet>> GetPets();

        Task<List<Pet>> GetAvailablePets();
		Task<Pet> GetPet(int ID);
        Task AddPet(Pet petToAdd);
        Task UpdatePet(Pet petToUpdate);
        Task DeletePet(Pet petToDelete);
    }
}
