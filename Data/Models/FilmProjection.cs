using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class FilmProjection
    {
        [Key("id")]
        public string Id { get; set; }
        [ForeignKey("cinemaId")]
        public string CinemaId { get; set; }
        [ForeignKey("filmId")]
        public string FilmId { get; set; }
        public DateTime Date { get; set; }
        public ushort TotalTickets { get; set; }

    }
}
