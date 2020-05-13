using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class FilmProjection
    {
        public FilmProjection()
        {
            this.Id = Guid.NewGuid().ToString();
            this.ProjectionTickets = new HashSet<ProjectionTicket>();
        }

        [Key]
        public string Id { get; set; }

        public string FilmId { get; set; }

        public string CinemaId { get; set; }

        public DateTime Date { get; set; }

        public ProjectionType ProjectionType { get; set; }

        public int TotalTickets { get; set; }

        public virtual Cinema Cinema { get; set; }

        public virtual Film Film { get; set; }

        public virtual IEnumerable<ProjectionTicket> ProjectionTickets { get; set; }

        public virtual TicketPrices TicketPrices { get; set; }

    }
}
