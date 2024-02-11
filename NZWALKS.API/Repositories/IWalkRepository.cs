using NZWALKS.API.Models.Domain;
using System.Numerics;

namespace NZWALKS.API.Repositories
{
    public interface IWalkRepository
    {
        Task<Walk> CreateAsync(Walk walk, IEnumerable<int> regionIds);
        Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
            string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000);
        Task<Walk?> GetByIdAsync(int id);
        Task<Walk?> UpdateAsync(int id, Walk walk, IEnumerable<int> regionIds);
        Task<Walk?> DeleteAsync(int id);
    }
}
