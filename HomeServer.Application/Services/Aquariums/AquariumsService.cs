using HomeServer.Application.Base;
using HomeServer.Application.Common.Pagination;
using HomeServer.Application.Common.Results;
using HomeServer.Application.Dtos.Aquariums;
using HomeServer.Application.Dtos.Common;
using HomeServer.Application.Interfaces.Aquariums;
using HomeServer.Domain.Models;
using HomeServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HomeServer.Application.Services.Aquariums
{
    public class AquariumsService : BaseService, IAquariumsService
    {
        public AquariumsService(MyDbContext context) : base(context) { }

        public async Task<Result<MessageResponseDto>> Create(AquariumCreateDto createDto)
        {
            try
            {
                await _context.AddAsync(new Aquarium()
                {
                    Name = createDto.Name,
                    Volume = createDto.Volume,
                    CreationDate = createDto.CreationDate
                });

                await _context.SaveChangesAsync();

                return Result.Success(new MessageResponseDto()
                {
                    Message = $"Akwarium \"{createDto.Name}\" zostało utworzone!"
                });
            }
            catch
            {
                return Result.Failure("Nie dodano akwarium do bazy danych!", ProblemType.InternalServerError);
            }
        }

        public async Task<Result<AquariumGetResponseDto[]>> GetAll()
        {
            try
            {
                var items = await _context.Aquariums
                    .Include(item => item.Photos)
                    .Where(item => item.DeletedAt == null)
                    .Select(item => new AquariumGetResponseDto()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Volume = item.Volume,
                        CreationDate = item.CreationDate,
                        LastPhotoId = item.Photos
                                            .OrderByDescending(item => item.CreatedAt)
                                            .First().Id
                    })
                    .ToListAsync();

                return Result.Success(items.ToArray());
            }
            catch
            {
                return Result.Failure("Można pobrać akwariów z bazy danych!", ProblemType.InternalServerError);
            }
        }
    }
}
