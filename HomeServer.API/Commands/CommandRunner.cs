using HomeServer.Infrastructure.Data;
using HomeServer.Infrastructure.Security;

namespace HomeServer.API.Commands
{
    public class CommandRunner
    {
        public static async Task<int> RunAsync(string[] args, IServiceProvider services)
        {
            if (args.Length == 0)
                return 0;

            using var scope = services.CreateScope();

            switch (args[0])
            {
                case "user":
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<MyDbContext>();
                        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                        await UserCommand.CreateAsync(dbContext, passwordHasher);
                        return 0;
                    }

                default:
                    Console.WriteLine($"Unknown command: {args[0]}");
                    return 1;
            }
        }
    }
}
