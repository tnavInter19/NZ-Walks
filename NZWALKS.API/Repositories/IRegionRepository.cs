using NZWALKS.API.Models.Domain;
using System.Numerics;
using System.Runtime.InteropServices;

namespace NZWALKS.API.Repositories
{
    public interface IRegionRepository
    {
        Task<List<Region>> GetAllAsync();

        Task<Region?> GetByIdAsync(int id);

        Task<Region> CreateAsync(Region region);

        Task<Region?> UpdateAsync(int id, Region region);

        Task<Region?> DeleteAsync(int id);
    }
}
