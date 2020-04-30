using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class FilmReview
    {
        [Key("id")]
        public string Id { get; set; }
        [ForeignKey("userId")]
        public string UserId { get; set; }
        [ForeignKey("filmId")]
        public string FilmId { get; set; }

        public string Content { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
