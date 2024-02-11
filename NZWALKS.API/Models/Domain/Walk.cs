using NZWALKS.API.Models.DTO;
using System.Numerics;

namespace NZWALKS.API.Models.Domain
{
    public class Walk
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }

        public int DifficultyId { get; set; }

        //navigation properties
        public ICollection<RegionWalk> RegionWalks { get; set; } = new List<RegionWalk>();
        public Difficulty Difficulty { get; set; }
    }
}
