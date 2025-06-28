using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Core.Helper
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string? Comment { get; set; }

        [Range(1, 5)]
        public float? Rating { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public int LikesCount { get; set; }
    }
}
