using HomeServer.Domain.Enums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Security;

namespace HomeServer.API.Commands
{
    public static class UserCommand
    {
        public static async Task CreateAsync(MyDbContext dbContext, IPasswordHasher passwordHasher)
        {
            Console.WriteLine("Name (display in app as person's name): ");
            var name = Console.ReadLine();
            while (String.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Name cannot be empty! Enter correct name:");
                name = Console.ReadLine();
            }
            name = name.Replace("\0", "");


            Console.WriteLine("Username (uses as login username): ");
            var username = Console.ReadLine();
            while (String.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("Username cannot be empty! Enter correct name:");
                username = Console.ReadLine();
            }
            username = username.Replace("\0", "");


            Console.WriteLine("Email: ");
            var email = Console.ReadLine();
            while (String.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email cannot be empty! Enter correct name:");
                email = Console.ReadLine();
            }
            email = email.Replace("\0", "");


            Console.WriteLine("Password: ");
            var password = Console.ReadLine();
            while (String.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("Password cannot be empty! Enter correct name:");
                password = Console.ReadLine();
            }
            password = password.Replace("\0", "");


            Console.WriteLine("Role (User/Moderator/Admin): ");
            var role = Console.ReadLine();
            RolesEnum userRole;
            while (!Enum.TryParse<RolesEnum>(role, true, out userRole))
            {
                Console.WriteLine("Role is not valid! - Available: User/Moderator/Admin!");
                Console.WriteLine("Enter correct:");
                role = Console.ReadLine();
            }

            var passwordHash = passwordHasher.GetHashedPassword(password!);

            try
            {

                dbContext.Users.Add(new User
                {
                    Name = name,
                    Username = username,
                    Email = email,
                    Password = passwordHash,
                    Role = userRole
                });

                await dbContext.SaveChangesAsync();

                Console.WriteLine();
                Console.WriteLine($"User '{username}' created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error during creating user:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}