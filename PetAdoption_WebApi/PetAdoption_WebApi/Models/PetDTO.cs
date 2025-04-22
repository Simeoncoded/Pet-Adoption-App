using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace PetAdoption_WebApi.Models
{
    [ModelMetadataType(typeof(PetMetaData))]
    public class PetDTO
    {
        public int ID { get; set; }
        public PetType? Type { get; set; }

        public string Name { get; set; } = "";
        public string Species { get; set; } = "";

        public string? Breed { get; set; }
        public int Age { get; set; }

        public string? Description { get; set; }

        public bool IsAdopted { get; set; } = false;
        public Byte[]? RowVersion { get; set; }
        public ICollection<AdoptionDTO>? Adoptions { get; set; } 
    }

}
