namespace NZWALKS.API.Models.Domain
{
    public class RegionWalk
    {
        public int RegionId { get; set; }
        public Region Region { get; set; }

        public int WalkId { get; set; }
        public Walk Walk { get; set; }
    }
}
