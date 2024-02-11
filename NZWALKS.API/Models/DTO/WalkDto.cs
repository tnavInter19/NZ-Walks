using System.Numerics;

namespace NZWALKS.API.Models.DTO
{
    public class WalkDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double LengthInKm { get; set; }
        public string WalkImageUrl { get; set; }
        public int DifficultyId { get; set; }
        public IEnumerable<int> RegionIds { get; set; }

        public DifficultyDto Difficulty { get; set; }
        public IEnumerable<RegionDto> Regions { get; set; }
    }
}
