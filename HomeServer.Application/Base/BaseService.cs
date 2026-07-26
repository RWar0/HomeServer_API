using HomeServer.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace HomeServer.Application.Base
{
    public abstract class BaseService
    {
        protected readonly MyDbContext _context;
        protected readonly ILogger _logger;

        protected BaseService(MyDbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
        }
    }
}
