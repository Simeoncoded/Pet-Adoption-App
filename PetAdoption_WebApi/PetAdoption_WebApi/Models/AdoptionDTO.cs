using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PetAdoption_WebApi.Models
{
    [ModelMetadataType(typeof(AdoptionMetaData))]
    public class AdoptionDTO : IValidatableObject
    {
        public int ID { get; set; }



        public string FirstName { get; set; } = "";

        public string? MiddleName { get; set; }

        public string LastName { get; set; } = "";
        public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;
        public DateTime RequestDate { get; set; } = DateTime.Today;
        public string? Comments { get; set; }
        public string? Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        public DateTime DOB { get; set; }

        [Timestamp]
        public Byte[]? RowVersion { get; set; }
        public int PetID { get; set; }

        public PetDTO? Pet { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Ensure user is at least 18 years old
            var today = DateTime.Today;
            var minDOB = today.AddYears(-18);

            if (DOB > minDOB)
            { 
                yield return new ValidationResult("You must be at least 18 years old to adopt a pet.", new[] { nameof(DOB) });
            }


            // Request date should not be in the future
            if (RequestDate > DateTime.Today)
            {
                yield return new ValidationResult("Request Date cannot be in the future", new[] { nameof(RequestDate) });
            }
        }
    }

}
