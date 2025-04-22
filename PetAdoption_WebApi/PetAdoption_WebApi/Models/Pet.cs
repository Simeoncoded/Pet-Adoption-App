using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PetAdoption_WebApi.Models
{
    [ModelMetadataType(typeof(PetMetaData))]
    public class Pet : Auditable
    {
        public int ID { get; set; }

        #region SUMMARY PROPERTIES

        public string Summary
        {
            get
            {
                var type = Type?.ToString() ?? "Unknown Type";
                var name = string.IsNullOrEmpty(Name) ? "No Name" : Name;
                var species = string.IsNullOrEmpty(Species) ? "Unknown Species" : Species;
                var age = Age > 0 ? $"{Age} years old" : "Age not specified";
                var adopted = IsAdopted ? "Adopted" : "Available for Adoption";

                return $"{name} - {type}, {species}, {age}, {adopted}";
            }
        }

        #endregion
        public PetType? Type { get; set; }

        public string Name { get; set; } = "";
        public string Species { get; set; } = "";

        public string? Breed { get; set; }
        public int Age { get; set; }

        public string? Description { get; set; }

        public bool IsAdopted { get; set; } = false;
        public Byte[]? RowVersion { get; set; }
        public ICollection<Adoption> Adoptions { get; set; } = new HashSet<Adoption>();
    }

}
