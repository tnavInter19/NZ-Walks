﻿using System.ComponentModel.DataAnnotations;

namespace NZWALKS.API.Models.DTO
{
    public class AddWalkRequestDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0, 50)]
        public double LengthInKm { get; set; }
        public string? WalkImageUrl { get; set; }
        [Required]
        public int DifficultyId { get; set; }
        [Required]
        public ICollection<int> RegionIds { get; set; }
    }
}
