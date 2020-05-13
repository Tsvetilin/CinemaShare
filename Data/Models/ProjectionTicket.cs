using Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ProjectionTicket
    {
        public ProjectionTicket()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        public string HolderId { get; set; }

        public string ProjectionId { get; set; }

        public TicketType Type { get; set; }

        public double Price { get; set; }

        public int Seat { get; set; }

        public DateTime ReservedOn { get; set; }

        public virtual FilmProjection Projection { get; set; }

        public virtual CinemaUser Holder { get; set; }

    }
}
