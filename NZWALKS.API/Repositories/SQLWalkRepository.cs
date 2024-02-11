using NZWALKS.API.Data;
using NZWALKS.API.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace NZWALKS.API.Repositories
{
    public class SQLWalkRepository : IWalkRepository
    {
        private readonly NZWalksDbContext dbContext;

        public SQLWalkRepository(NZWalksDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        public async Task<Walk> CreateAsync(Walk walk, IEnumerable<int> regionIds)
        {
            // Add the walk to the context
            await dbContext.Walks.AddAsync(walk);

            // Add associations between walk and regions to the RegionWalk table
            foreach (var regionId in regionIds)
            {
                walk.RegionWalks.Add(new RegionWalk { WalkId = walk.Id, RegionId = regionId });
            }

            await dbContext.SaveChangesAsync();
            return walk;
        }

        public async Task<Walk?> DeleteAsync(int id)
        {
            var existingWalk = await dbContext.Walks.FirstOrDefaultAsync(x => x.Id == id);

            if (existingWalk == null)
            {
                return null;
            }

            dbContext.Walks.Remove(existingWalk);
            await dbContext.SaveChangesAsync();
            return existingWalk;
        }

        public async Task<List<Walk>> GetAllAsync(string? filterOn = null, string? filterQuery = null,
          string? sortBy = null, bool isAscending = true, int pageNumber = 1, int pageSize = 1000)
        {
            var walks = dbContext.Walks.Include(w => w.Difficulty)
                    .Include(w => w.RegionWalks) 
                        .ThenInclude(rw => rw.Region) 
                    .AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(filterOn) == false && string.IsNullOrWhiteSpace(filterQuery) == false)
            {
                if (filterOn.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = walks.Where(x => x.Name.Contains(filterQuery));
                }
            }

            // Sorting 
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if (sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.Name) : walks.OrderByDescending(x => x.Name);
                }
                else if (sortBy.Equals("Length", StringComparison.OrdinalIgnoreCase))
                {
                    walks = isAscending ? walks.OrderBy(x => x.LengthInKm) : walks.OrderByDescending(x => x.LengthInKm);
                }
            }

            // Pagination
            var skipResults = (pageNumber - 1) * pageSize;

            return await walks.Skip(skipResults).Take(pageSize).ToListAsync();
        }

        public async Task<Walk?> GetByIdAsync(int id)
        {
            return await dbContext.Walks
                .Include(w => w.Difficulty)
                    .Include(w => w.RegionWalks)
                        .ThenInclude(rw => rw.Region)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Walk?> UpdateAsync(int id, Walk walk, IEnumerable<int> regionIds)
        {
            var existingWalk = await dbContext.Walks.Include(w => w.RegionWalks)
                                                   .FirstOrDefaultAsync(x => x.Id == id);

            if (existingWalk == null)
            {
                return null;
            }

            // Update basic walk properties
            existingWalk.Name = walk.Name;
            existingWalk.Description = walk.Description;
            existingWalk.LengthInKm = walk.LengthInKm;
            existingWalk.WalkImageUrl = walk.WalkImageUrl;
            existingWalk.DifficultyId = walk.DifficultyId;

            // Update or add RegionWalk associations based on provided region IDs
            foreach (var regionId in regionIds)
            {
                // Check if the region ID is already associated with the walk
                var existingRegionWalk = existingWalk.RegionWalks.FirstOrDefault(rw => rw.RegionId == regionId);
                if (existingRegionWalk == null)
                {
                    // If not, add a new RegionWalk association
                    existingWalk.RegionWalks.Add(new RegionWalk { RegionId = regionId });
                }
            }

            // Remove RegionWalk associations for region IDs not provided
            foreach (var regionWalk in existingWalk.RegionWalks.ToList())
            {
                if (!regionIds.Contains(regionWalk.RegionId))
                {
                    dbContext.Regionwalk.Remove(regionWalk);
                }
            }

            await dbContext.SaveChangesAsync();

            return existingWalk;
        }
    }
}