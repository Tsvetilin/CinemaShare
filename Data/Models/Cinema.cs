using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class Cinema
    {
        [Key("id")]
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [ForeignKey("manager")]
        public string Manager { get; set; }
        
    }
}
