using Pet_Adoption_WebAPI_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Data
{
	public interface IAdoptionRepository
	{
		Task<List<Adoption>> GetAdoptions();
		Task<Adoption> GetAdoption(int ID);

        Task AddAdoption(Adoption adoptionToAdd);
        Task UpdateAdoption(Adoption adoptionToUpdate);
        Task DeleteAdoption(Adoption adoptionToDelete);
    }
}
