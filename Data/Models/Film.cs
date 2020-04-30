using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Film
    {
        [Key("id")]
        public string Id { get; set; }

        public byte Rating { get; set; }
        [ForeignKey("addedBy")]
        public string AddedBy { get; set; }
    }
}
