using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace PetAdoption_WebApi.Models
{
    public class AdoptionMetaData
    {


        [Display(Name = "First Name")]
        [Required(ErrorMessage = "The first name cannot be left blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; } = "";

        [Display(Name = "Middle Name")]
        [StringLength(50, ErrorMessage = "Middle name cannot be more than 50 characters long.")]
        public string? MiddleName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "The last name cannot be left blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 100 characters long.")]
        public string LastName { get; set; } = "";
        public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;

        [DataType(DataType.Date)]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Display(Name = "Comments")]
        [StringLength(500, ErrorMessage = "Comments cannot be more than 500 characters long.")]
        public string? Comments { get; set; }

        [Display(Name = "Contact Phone")]
        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Phone Number cannot be left blank.")]
        [StringLength(10)]
        public string? Phone { get; set; }

        [Display(Name = "Contact Email")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Please follow the correct email format test@email.com")]
        [Required(ErrorMessage = "Email Address cannot be left blank.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "You must enter your Date of Birth.")]
        [Display(Name = "Date of Birth")]
		[DataType(DataType.Date)]
		public DateTime DOB { get; set; }

        [Timestamp]
        public Byte[]? RowVersion { get; set; }

        [Display(Name = "Pet")]
        [Required(ErrorMessage = "You must select the Pet you want to adopt.")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a valid Pet.")]
        public int PetID { get; set; }


       
    }

}
