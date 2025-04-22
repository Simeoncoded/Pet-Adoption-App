using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetAdoption_WebApi.Data;
using PetAdoption_WebApi.Models;

namespace PetAdoption_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptionController : ControllerBase
    {
        private readonly PetAdoptionContext _context;

        public AdoptionController(PetAdoptionContext context)
        {
            _context = context;
        }

        // GET: api/Adoption
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdoptionDTO>>> GetAdoptions()
        {
            var adoptionDTOs = await _context.Adoptions
                    .Include(p => p.Pet)
                  .Select(d => new AdoptionDTO
                  {
                      ID = d.ID,
                      FirstName = d.FirstName,
                      MiddleName = d.MiddleName,
                      LastName = d.LastName,
                      Status = d.Status,
                      RequestDate = d.RequestDate,
                      Comments = d.Comments,
                      Phone = d.Phone,
                      Email = d.Email,
                      DOB = d.DOB,
                      RowVersion = d.RowVersion,
                      PetID = d.PetID,
                      Pet = d.Pet != null ? new PetDTO
                      {
                          ID = d.Pet.ID,
                          Type = d.Pet.Type,
                          Name = d.Pet.Name,
                          Species = d.Pet.Species,
                          Breed = d.Pet.Breed,
                          Age = d.Pet.Age,
                          Description = d.Pet.Description,
                          IsAdopted = d.Pet.IsAdopted
                      } : null
                  })
                  .ToListAsync();

            if (adoptionDTOs.Count() > 0)
            {
                return adoptionDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Adoption records found in the database." });
            }
        }

        // GET: api/Adoption/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AdoptionDTO>> GetAdoption(int id)
        {
            var adoptionDTO = await _context.Adoptions
                  .Include(e => e.Pet)
                  .Select(p => new AdoptionDTO
                  {
                      ID = p.ID,
                      FirstName = p.FirstName,
                      MiddleName = p.MiddleName,
                      LastName = p.LastName,
                      Status = p.Status,
                      RequestDate = p.RequestDate,
                      Comments = p.Comments,
                      Phone = p.Phone,
                      Email = p.Email,
                      DOB = p.DOB,
                      RowVersion = p.RowVersion,
                      PetID = p.PetID,
                      Pet = p.Pet != null ? new PetDTO
                      {
                          ID = p.Pet.ID,
                          Type = p.Pet.Type,
                          Name = p.Pet.Name,
                          Species = p.Pet.Species,
                          Breed = p.Pet.Breed,
                          Age = p.Pet.Age,
                          Description = p.Pet.Description,
                          IsAdopted = p.Pet.IsAdopted
                      } : null
                  })
                  .FirstOrDefaultAsync(p => p.ID == id);

            if (adoptionDTO == null)
            {
                return NotFound(new { message = "Error: That Adoption record was not found in the database." });
            }

            return adoptionDTO;
        }



        // PUT: api/Adoption/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAdoption(int id, AdoptionDTO adoptionDTO)
        {
            if (id != adoptionDTO.ID)
            {
                return BadRequest(new { message = "Error: ID does not match Adoption Record" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Get the record you want to update
            var adoptionToUpdate = await _context.Adoptions.FindAsync(id);

            // Check that you got it
            if (adoptionToUpdate == null)
            {
                return NotFound(new { message = "Error: Adoption record not found." });
            }

            // Concurrency check: Ensure the RowVersion matches to prevent updates with stale data
            if (adoptionDTO.RowVersion != null)
            {
                if (!adoptionToUpdate.RowVersion.SequenceEqual(adoptionDTO.RowVersion))
                {
                    return Conflict(new { message = "Concurrency Error: Adoption record has been changed by another user. Try editing the record again." });
                }
            }

            adoptionToUpdate.ID = adoptionDTO.ID;
            adoptionToUpdate.FirstName = adoptionDTO.FirstName;
            adoptionToUpdate.MiddleName = adoptionDTO.MiddleName;
            adoptionToUpdate.LastName = adoptionDTO.LastName;
            adoptionToUpdate.Status = adoptionDTO.Status;
            adoptionToUpdate.RequestDate = adoptionDTO.RequestDate;
            adoptionToUpdate.Comments = adoptionDTO.Comments;
            adoptionToUpdate.Phone = adoptionDTO.Phone;
            adoptionToUpdate.Email = adoptionDTO.Email;
            adoptionToUpdate.DOB = adoptionDTO.DOB;
            adoptionToUpdate.RowVersion = adoptionDTO.RowVersion;
            adoptionToUpdate.PetID = adoptionDTO.PetID;

            // If the adoption is approved, check for existing approvals
            if (adoptionDTO.Status == AdoptionStatus.Approved)
            {
               
                var existingApprovedAdoption = _context.Adoptions
                    .FirstOrDefault(a => a.PetID == adoptionDTO.PetID && a.Status == AdoptionStatus.Approved);

                if (existingApprovedAdoption != null)
                {
                    // Prevent approving a second adoption for the same pet
                    return Conflict(new { message = "Error: This pet has already been adopted." });
                }

                // Approve the adoption and mark the pet as adopted
                var pet = await _context.Pets.FindAsync(adoptionDTO.PetID);
                if (pet != null)
                {
                    pet.IsAdopted = true;
                }

                var otherPendingAdoptions = _context.Adoptions
                    .Where(a => a.PetID == adoptionDTO.PetID && a.ID != adoptionDTO.ID && a.Status == AdoptionStatus.Pending)
                    .ToList();

                foreach (var other in otherPendingAdoptions)
                {
                    other.Status = AdoptionStatus.Denied;
                }
            }
            else if (adoptionDTO.Status == AdoptionStatus.Denied)
            {
                var pet = await _context.Pets.FindAsync(adoptionDTO.PetID);
                if (pet != null)
                {
                    var otherApproved = _context.Adoptions
                        .Any(a => a.PetID == adoptionDTO.PetID && a.ID != adoptionDTO.ID && a.Status == AdoptionStatus.Approved);

                    pet.IsAdopted = otherApproved;
                }
            }
            else
            {
                var pet = await _context.Pets.FindAsync(adoptionDTO.PetID);
                if (pet != null)
                {
                    pet.IsAdopted = false;
                }
            }

            // Set the original RowVersion value for concurrency control
            _context.Entry(adoptionToUpdate).Property("RowVersion").OriginalValue = adoptionDTO.RowVersion;

            try
            {
                // Save the changes to the database
                await _context.SaveChangesAsync();
                return NoContent();  // No content response for a successful update
            }
            catch (DbUpdateConcurrencyException)
            {
                // Handle concurrency exception
                if (!AdoptionExists(id))
                {
                    return Conflict(new { message = "Concurrency Error: Adoption record has been removed." });
                }
                else
                {
                    return Conflict(new { message = "Concurrency Error: Adoption record has been updated by another user. Back out and try editing the record again." });
                }
            }
            catch (DbUpdateException dex)
            {
                // Handle other database-related exceptions
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate Phone numbers." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }


        // POST: api/Adoption
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Adoption>> PostAdoption(AdoptionDTO adoptionDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Adoption adoption = new Adoption
            {
                ID = adoptionDTO.ID,
                FirstName = adoptionDTO.FirstName,
                MiddleName = adoptionDTO.MiddleName,
                LastName = adoptionDTO.LastName,
                Status = adoptionDTO.Status,
                RequestDate = adoptionDTO.RequestDate,
                Comments = adoptionDTO.Comments,
                Phone = adoptionDTO.Phone,
                Email = adoptionDTO.Email,
                DOB = adoptionDTO.DOB,
                RowVersion = adoptionDTO.RowVersion,
                PetID = adoptionDTO.PetID
            };

            
            _context.Adoptions.Add(adoption);

            var pet = await _context.Pets.FindAsync(adoptionDTO.PetID);
            if (pet != null)
            {
                pet.IsAdopted = adoptionDTO.Status == AdoptionStatus.Approved;
            }

            try
            {
                await _context.SaveChangesAsync();

                // Assign DB generated values back to the DTO
                adoptionDTO.ID = adoption.ID;
                adoptionDTO.RowVersion = adoption.RowVersion;

                return CreatedAtAction(nameof(GetAdoption), new { id = adoption.ID }, adoptionDTO);
            }
            catch (DbUpdateException dex)
            {
                if (dex.GetBaseException().Message.Contains("UNIQUE"))
                {
                    return BadRequest(new { message = "Unable to save: Duplicate Phone number." });
                }
                else
                {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                }
            }
        }

        // DELETE: api/Adoption/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdoption(int id)
        {
			var adoption = await _context.Adoptions.FindAsync(id);
			if (adoption == null)
			{
				return NotFound(new { message = "Delete Error: Adoption record has already been removed." });
			}
			try
			{
				_context.Adoptions.Remove(adoption);
				await _context.SaveChangesAsync();
				return NoContent();
			}
			catch (DbUpdateException)
			{
				return BadRequest(new { message = "Delete Error: Unable to delete Adoption Record." });
			}
		}

        private bool AdoptionExists(int id)
        {
            return _context.Adoptions.Any(e => e.ID == id);
        }
    }
}
