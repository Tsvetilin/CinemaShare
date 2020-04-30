using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ProjectionTicket
    {
        [Key]
        public string Id { get; set; }

        public string Type { get; set; }

        public double Price { get; set; }

        public ushort Seat { get; set; }
        
        public virtual FilmProjection Projection { get; set; }
        public virtual CinemaUser Holder { get; set; }

    }
}
