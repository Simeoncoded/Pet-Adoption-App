using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pet_Adoption_WebAPI_Client.Models
{
	public class Adoption
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

				return $"👤 {adopterName} is requesting to adopt 🐾 {petName}\n📌 Status: {status}";
			}
		}


        #endregion

        public string FirstName { get; set; }

		public string MiddleName { get; set; }

		public string LastName { get; set; }
		public AdoptionStatus Status { get; set; } = AdoptionStatus.Pending;
		public DateTime RequestDate { get; set; } = DateTime.UtcNow.Date;
		public string Comments { get; set; }
		public string Phone { get; set; }

		public string Email { get; set; }

		public DateTime DOB { get; set; } = DateTime.Today;

		public byte[] RowVersion { get; set; }
		public int PetID { get; set; }
		public Pet Pet { get; set; }
	}
}
