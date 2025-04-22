using Microsoft.EntityFrameworkCore;
using PetAdoption_WebApi.Models;
using System.Diagnostics;
using System.Numerics;

namespace PetAdoption_WebApi.Data
{
    public class PetAdoptionInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider,
            bool DeleteDatabase = false, bool UseMigrations = false, bool SeedSampleData = true)
        {
            using (var context = new PetAdoptionContext(
                serviceProvider.GetRequiredService<DbContextOptions<PetAdoptionContext>>()))
            {
                //Refresh the database as per the parameter options
                #region Prepare the Database
                try
                {
                    //Note: .CanConnect() will return false if the database is not there!
                    if (DeleteDatabase || !context.Database.CanConnect())
                    {
                        context.Database.EnsureDeleted(); //Delete the existing version 
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Create the Database and apply all migrations
                        }
                        else
                        {
                            context.Database.EnsureCreated(); //Create and update the database as per the Model
                        }
                        //Now create any additional database objects such as Triggers or Views
                        //--------------------------------------------------------------------
                        //Pet Table Triggers for Concurrency
                        string sqlCmd = @"
                            CREATE TRIGGER SetPetTimestampOnUpdate
                            AFTER UPDATE ON Pets
                            BEGIN
                                UPDATE Pets
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END;
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                        sqlCmd = @"
                            CREATE TRIGGER SetPetTimestampOnInsert
                            AFTER INSERT ON Pets
                            BEGIN
                                UPDATE Pets
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                        //Adoption Table Triggers for Concurrency
                        sqlCmd = @"
                            CREATE TRIGGER SetAdoptionTimestampOnUpdate
                            AFTER UPDATE ON Adoptions
                            BEGIN
                                UPDATE Adoptions
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END;
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                        sqlCmd = @"
                            CREATE TRIGGER SetAdoptionTimestampOnInsert
                            AFTER INSERT ON Adoptions
                            BEGIN
                                UPDATE Adoptions
                                SET RowVersion = randomblob(8)
                                WHERE rowid = NEW.rowid;
                            END
                        ";
                        context.Database.ExecuteSqlRaw(sqlCmd);

                    }
                    else //The database is already created
                    {
                        if (UseMigrations)
                        {
                            context.Database.Migrate(); //Apply all migrations
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                //Seed data needed for production and during development
                #region Seed Required Data
                try
                {

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.GetBaseException().Message);
                }
                #endregion

                #region Seed Sample Data
                if (!SeedSampleData) return;

                Random random = new Random();

                try
                {
                    // Seed Pets if none exist
                    if (!context.Pets.Any())
                    {
                        context.Pets.AddRange(
                            new Pet { Name = "Bella", Type = PetType.Dog, Breed = "Labrador", Species = "Domestic", Age = 3 },
                            new Pet { Name = "Charlie", Type = PetType.Cat, Breed = "Siamese", Species = "In House", Age = 2 },
                            new Pet { Name = "Luna", Type = PetType.Dog, Breed = "Golden Retriever", Species = "Akita", Age = 1 },
                            new Pet { Name = "Max", Type = PetType.Rabbit, Breed = "Holland Lop", Species = "Barbet", Age = 4 },
                            new Pet { Name = "Oliver", Type = PetType.Bird, Breed = "Parrot", Species = "In House", Age = 5 },
                            new Pet { Name = "Daisy", Type = PetType.Cat, Breed = "Persian", Species = "Domestic", Age = 3 }
                        );

                        context.SaveChanges();
                    }

                   
                    int[] petIDs = context.Pets.Select(p => p.ID).ToArray();
                    if (petIDs.Length == 0) return; // Ensure pets exist before seeding adoptions

                    // Seed Adoptions if none exist
                    if (!context.Adoptions.Any())
                    {
                        context.Adoptions.AddRange(
                        new Adoption
                        {
                            PetID = petIDs[0],
                            FirstName = "John",
                            LastName = "Doe",
                            Email = "jdoe@outlook.com",
                            Phone = "2324242424",
                            DOB = DateTime.UtcNow.AddYears(-20),
                            RequestDate = DateTime.UtcNow.AddDays(-random.Next(10, 200))
                        },
                    new Adoption
                        {
                            PetID = petIDs[1],
                            FirstName = "Jane",
                            LastName = "Smith",
                            Email = "jsim@outlook.com",
                            Phone = "2324242429",
                            DOB = DateTime.UtcNow.AddYears(-20),
                            RequestDate = DateTime.UtcNow.AddDays(-random.Next(10, 200))
                    },
                    new Adoption
                     {
                            PetID = petIDs[2],
                            FirstName = "Alice",
                            LastName = "Johnson",
                             Email = "aj@outlook.com",
                          Phone = "2324242409",
                             DOB = DateTime.UtcNow.AddYears(-20),
                              RequestDate = DateTime.UtcNow.AddDays(-random.Next(10, 200))
}

                        );

                        context.SaveChanges();
                    }

                    if (context.Pets.Count() < 10)
                    {
                        string[] petNames = { "Rocky", "Milo", "Coco", "Ruby", "Chloe", "Bailey", "Teddy", "Lily" };
                        string[] petBreeds = { "Poodle", "Husky", "Maine Coon", "Cockatoo", "Shih Tzu", "Goolan", "Basset", "Ragdoll" };
                        string[] petSpecies = { "Domestic", "In House", "Out Play", "Akita", "Barbet", "Basenji", "Azawakh", "Maine" };
                        PetType[] petTypes = (PetType[])Enum.GetValues(typeof(PetType));

                        var additionalPets = new List<Pet>();

                        foreach (string name in petNames)
                        {
                            var pet = new Pet
                            {
                                Name = name,
                                Species = petSpecies[random.Next(petSpecies.Length)],
                                Type = petTypes[random.Next(petTypes.Length)],
                                Breed = petBreeds[random.Next(petBreeds.Length)],
                                Age = random.Next(1, 12)
                            };

                            additionalPets.Add(pet);
                        }

                        context.Pets.AddRange(additionalPets);
                        context.SaveChanges();
                    }

                    // Remove unadopted pets older than 10 years
                    var oldUnadoptedPets = context.Pets
                        .Where(p => p.Age > 10 && !context.Adoptions.Any(a => a.PetID == p.ID))
                        .ToList();

                    if (oldUnadoptedPets.Any())
                    {
                        context.Pets.RemoveRange(oldUnadoptedPets);
                        context.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Seeding failed: {ex.GetBaseException().Message}");
                }
                #endregion


            }

        }
    }
}
