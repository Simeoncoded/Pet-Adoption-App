using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Models
{
	public class Pet
	{
		public int ID { get; set; }

		#region SUMMARY PROPERTIES

		public string Summary
		{
			get
			{
				var type = Type.ToString() ?? "Unknown Type";
				var species = string.IsNullOrEmpty(Species) ? "Unknown Species" : Species;
				var age = Age > 0 ? $"{Age} years old" : "Age not specified";
				var adopted = IsAdopted ? "Adopted" : "Available for Adoption";

				return $" {type}, {species}, {age}, {adopted}";
			}
		}

        public string PSummary
        {
            get
            {
                var type = Type.ToString() ?? "Unknown Type";
                var species = string.IsNullOrEmpty(Species) ? "Unknown Species" : Species;
                var age = Age > 0 ? $"{Age} years old" : "Age not specified";
                var adopted = IsAdopted ? "Adopted" : "Available for Adoption";
				var name = string.IsNullOrEmpty(Name) ? "No Name" : Name;

                return $"Name: {name}, Type: {type}, Specie: {species}, Age: {age}, Status: {adopted}";
            }
        }

        #endregion
        public PetType Type { get; set; }

		public string Name { get; set; }
		public string Species { get; set; }

		public string Breed { get; set; }
		public int Age { get; set; }

		public string Description { get; set; }

		public bool IsAdopted { get; set; }
		public byte[] RowVersion { get; set; }
        public ICollection<Adoption> Adoptions { get; set; }
    }
}
