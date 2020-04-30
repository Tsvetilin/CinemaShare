using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class ProjectionTicket
    {
        [Key("id")]
        public string Id { get; set; }

        public stirng Type { get; set; }

        public double Price { get; set; }

        [ForeignKey("projectionId")]
        public string ProjectionId { get; set; }
        public ushort Seat { get; set; }

        [ForeignKey("holderId")]
        public string HolderId { get; set; }

    }
}
