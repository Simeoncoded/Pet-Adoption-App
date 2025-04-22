using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PetAdoption_WebApi.Models
{
    [ModelMetadataType(typeof(AdoptionMetaData))]
    public class Adoption : Auditable, IValidatableObject
    {
        public int ID { get; set; }


        #region SUMMARY PROPERTIES
        public string Summary
        {
            get
            {
                var adopterName = $"{FirstName} {LastName}";
                var petName = Pet?.Name ?? "No Pet Selected";
                var status = Status.ToString();

                return $"{adopterName} is requesting to adopt {petName} (Status: {status})";
            }
        }
        #endregion

        public string FirstName { get; set; } = "";

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = "";
        public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;
        public DateTime RequestDate { get; set; } = DateTime.Today;
        public string? Comments { get; set; }
        public string? Phone { get; set; }

        public string? Email { get; set; }

        public DateTime DOB { get; set; }

        public Byte[]? RowVersion { get; set; }
        public int PetID { get; set; }

        public Pet? Pet { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            
            var today = DateTime.Today;
            var age = today.Year - DOB.Year;

            if (DOB.Date > today.AddYears(-age))
            {
                age--;
            }

            //you must be 18 years
            if (age < 18)
            {
                yield return new ValidationResult("You must be at least 18 years old to adopt a pet.", new[] { nameof(DOB) });
            }


            // Request date should not be in the future
            if (RequestDate.Date > DateTime.UtcNow.Date)
            {
                yield return new ValidationResult("Request Date cannot be in the future", new[] { nameof(RequestDate) });
            }


        }
    }

}
