using System.ComponentModel.DataAnnotations;

namespace PetAdoption_WebApi.Models
{
    public class PetMetaData
    {

        #region SUMMARY PROPERTIES

        [Display(Name = "Pet Summary")]
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

        [Display(Name = "Pet Type")]
        [Required(ErrorMessage = "You must specify the pet type.")]
        public PetType? Type { get; set; }

        [Display(Name = "Pet Name")]
        [Required(ErrorMessage = "Pet name cannot be left blank.")]
        [StringLength(50, ErrorMessage = "Pet name cannot be more than 50 characters.")]
        public string Name { get; set; } = "";

        [Display(Name = "Pet Species")]
        [Required(ErrorMessage = "Species cannot be left blank.")]
        [StringLength(50, ErrorMessage = "Species cannot be more than 50 characters.")]
        public string Species { get; set; } = "";

        [Display(Name = "Pet Breed")]
        [StringLength(50, ErrorMessage = "Breed cannot be more than 50 characters.")]
        public string? Breed { get; set; }

        [Display(Name = "Pet Age")]
        [Required(ErrorMessage = "Pet age cannot be left blank.")]
        [Range(0, 30, ErrorMessage = "Age must be between 0 and 30 years.")]
        public int Age { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be more than 500 characters.")]
        public string? Description { get; set; }

        [Display(Name = "Adoption Status")]
        public bool IsAdopted { get; set; } = false;

        [Timestamp]
        public Byte[]? RowVersion { get; set; }
    }

}
