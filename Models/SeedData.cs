using ItiUmplemFrigiderul.Data;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider
        serviceProvider)
        {
            using (var context = new ApplicationDbContext(
            serviceProvider.GetRequiredService
            <DbContextOptions<ApplicationDbContext>>()))
            {
                // Verificam daca in baza de date exista cel putin un^rol
                // insemnand ca a fost rulat codul
                // De aceea facem return pentru a nu insera rolurile inca o data
                // Acesta metoda trebuie sa se execute o singura data
                
                if (context.Roles.Any())
                {
                    return; // baza de date contine deja roluri
                }


                // CREAREA ROLURILOR IN BD
                // daca nu contine roluri, acestea se vor crea
                context.Roles.AddRange(

                new IdentityRole
                { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },

                new IdentityRole
                { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Name = "Collaborator", NormalizedName = "Collaborator".ToUpper() },

                new IdentityRole
                { Id = "2c5e174e-3b0e-446f-86af-483d56fd7212", Name = "User", NormalizedName = "User".ToUpper() }

);
                
                // o noua instanta pe care o vom utiliza pentru crearea parolelor utilizatorilor
                // parolele sunt de tip hash
                var hasher = new PasswordHasher<ApplicationUser>();

                // CREAREA USERILOR IN BD
                // Se creeaza cate un user pentru fiecare rol
                context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                    // primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },

                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb8",
                    // primary key
                    UserName = "collaborator1@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "COLLABORATOR1@TEST.COM",
                    Email = "collaborator1@test.com",
                    NormalizedUserName = "COLLABORATOR1@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,"Collaborator1!")
                },

                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb6",
                    // primary key
                    UserName = "collaborator2@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "COLLABORATOR2@TEST.COM",
                    Email = "collaborator2@test.com",
                    NormalizedUserName = "COLLABORATOR2@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Collaborator2!")
                },

                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb7",
                    // primary key
                    UserName = "collaborator3@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "COLLABORATOR3@TEST.COM",
                    Email = "collaborator3@test.com",
                    NormalizedUserName = "COLLABORATOR3@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Collaborator3!")
                },

                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    // primary key
                    UserName = "user1@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER1@TEST.COM",
                    Email = "user1@test.com",
                    NormalizedUserName = "USER1@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,"User1!")
                },

                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb3",
                    // primary key
                    UserName = "user2@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER2@TEST.COM",
                    Email = "user2@test.com",
                    NormalizedUserName = "USER2@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "User2!")
                },

                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb4",
                    // primary key
                    UserName = "user3@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER3@TEST.COM",
                    Email = "user3@test.com",
                    NormalizedUserName = "USER3@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "User3!")
                }
                );

                // ASOCIEREA USER-ROLE
                context.UserRoles.AddRange(
                //admin
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                },

                //colaborator
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",               
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                },

                //user
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",                
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                },

                //user
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb3"
                },

                //user
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb4"
                },

                //coll
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb8"
                },

                //user
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                },

                //colaborator
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb6"
                },

                //colaborator
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb7"
                }

                );
                context.SaveChanges();
            }
        }
    }
}
