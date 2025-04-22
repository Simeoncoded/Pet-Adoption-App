using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public class PetController : ControllerBase
    {
        private readonly PetAdoptionContext _context;

        public PetController(PetAdoptionContext context)
        {
            _context = context;
        }

        // GET: api/Pet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetPets()
        {
            var petDTOs = await _context.Pets
                  .Select(d => new PetDTO
                  {
                      ID = d.ID,
                      Type = d.Type,
                      Name = d.Name,
                      Species = d.Species,
                      Breed = d.Breed,
                      Age = d.Age,
                      Description = d.Description,
                      IsAdopted = d.IsAdopted,
                      RowVersion = d.RowVersion
                  })
                  .ToListAsync();

            if (petDTOs.Count() > 0)
            {
                return petDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No Pet records found in the database." });
            }
        }

        // GET: api/Pet/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<PetDTO>>> GetAvailablePets()
        {
            var petDTOs = await _context.Pets
                .Where(p => !p.IsAdopted &&
                //show only pets that are available and denied
                    (!p.Adoptions.Any() || p.Adoptions.All(a => a.Status == AdoptionStatus.Denied)))
                .Select(d => new PetDTO
                {
                    ID = d.ID,
                    Type = d.Type,
                    Name = d.Name,
                    Species = d.Species,
                    Breed = d.Breed,
                    Age = d.Age,
                    Description = d.Description,
                    IsAdopted = d.IsAdopted,
                    RowVersion = d.RowVersion
                })
                .ToListAsync();

            if (petDTOs.Count() > 0)
            {
                return petDTOs;
            }
            else
            {
                return NotFound(new { message = "Error: No available pets found in the database." });
            }
        }


        // GET: api/Pet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PetDTO>> GetPet(int id)
        {
            var petDTO = await _context.Pets
                 .Select(d => new PetDTO
                 {
                     ID = d.ID,
                     Type = d.Type,
                     Name = d.Name,
                     Species = d.Species,
                     Breed = d.Breed,
                     Age = d.Age,
                     Description = d.Description,
                     IsAdopted = d.IsAdopted,
                     RowVersion = d.RowVersion
                 })
                 .FirstOrDefaultAsync(d => d.ID == id);

            if (petDTO == null)
            {
                return NotFound(new { message = "Error: That Pet was not found in the database." });
            }

            return petDTO;
        }

        // PUT: api/Pet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPet(int id, PetDTO petDTO)
        {
			if (id != petDTO.ID)
			{
				return BadRequest(new { message = "Error: Incorrect ID for Pet." });
			}

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			//Get the record you want to update
			var petToUpdate = await _context.Pets.FindAsync(id);

			//Check that you got it
			if (petToUpdate == null)
			{
				return NotFound(new { message = "Error: Pet record not found." });
			}

			//Wow, we have a chance to check for concurrency even before bothering
			//the database!  Of course, it will get checked again in the database just in case
			//it changes after we pulled the record.  
			//Note using SequenceEqual becuase it is an array after all.
			if (petDTO.RowVersion != null)
			{
				if (!petToUpdate.RowVersion.SequenceEqual(petDTO.RowVersion))
				{
					return Conflict(new { message = "Concurrency Error: Pet has been changed by another user.  Back out and try editing the record again." });
				}
			}

			//Update the properties of the entity object from the DTO object

			petToUpdate.ID = petDTO.ID;
			petToUpdate.Type = petDTO.Type;
			petToUpdate.Name = petDTO.Name;
			petToUpdate.Species = petDTO.Species;
			petToUpdate.Breed = petDTO.Breed;
			petToUpdate.Age = petDTO.Age;
			petToUpdate.Description = petDTO.Description;
			petToUpdate.IsAdopted = petDTO.IsAdopted;
			petToUpdate.RowVersion = petDTO.RowVersion;




			//Put the original RowVersion value in the OriginalValues collection for the entity
			_context.Entry(petToUpdate).Property("RowVersion").OriginalValue = petDTO.RowVersion;

			try
			{
				await _context.SaveChangesAsync();
				return NoContent();
			}
			catch (DbUpdateConcurrencyException)
			{
				if (!PetExists(id))
				{
					return Conflict(new { message = "Concurrency Error: Pet has been Removed." });
				}
				else
				{
					return Conflict(new { message = "Concurrency Error: Pet has been updated by another user.  Back out and try editing the record again." });
				}
			}
            catch (DbUpdateException dex)
            {
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
            }
        }

        // POST: api/Pet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pet>> PostPet(PetDTO petDTO)
        {
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			Pet pet = new Pet
			{
				ID = petDTO.ID,
				Type = petDTO.Type,
				Name = petDTO.Name,
				Species = petDTO.Species,
				Breed = petDTO.Breed,
				Age = petDTO.Age,
				Description = petDTO.Description,
				IsAdopted = petDTO.IsAdopted,
				RowVersion = petDTO.RowVersion
			};

			try
			{
				_context.Pets.Add(pet);
				await _context.SaveChangesAsync();
				//Assign Database Generated values back into the DTO
				petDTO.ID = pet.ID;
				petDTO.RowVersion = pet.RowVersion;
				return CreatedAtAction(nameof(GetPet), new { id = pet.ID }, petDTO);
			}
            catch (DbUpdateException)
            {
               
              
                    return BadRequest(new { message = "Unable to save changes to the database. Try again, and if the problem persists see your system administrator." });
                
            }
        }

		// DELETE: api/Pet/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePet(int id)
		{
			var pet = await _context.Pets.FindAsync(id);
			if (pet == null)
			{
				return NotFound(new { message = "Delete Error: Pet has already been removed." });
			}

			try
			{
				_context.Pets.Remove(pet);
				await _context.SaveChangesAsync();
				return NoContent(); 
			}
			catch (DbUpdateException dex)
			{
				if (dex.GetBaseException().Message.Contains("FOREIGN KEY constraint failed"))
				{
					return BadRequest(new { message = "Delete Error: You cannot delete a Pet that has been adopted." });
				}
				else
				{
					return BadRequest(new { message = "Delete Error: Unable to delete Pet. Try again, and if the problem persists, contact the system administrator." });
				}
			}
		}


		private bool PetExists(int id)
        {
            return _context.Pets.Any(e => e.ID == id);
        }
    }
}
