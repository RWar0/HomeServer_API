using HomeServer.Infrastructure.Data;

namespace HomeServer.Application.Base
{
    public abstract class BaseService
    {
        protected readonly MyDbContext _context;

        protected BaseService(MyDbContext context)
        {
            _context = context;
        }
    }
}
