using Data.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class GenreType
    {
        public GenreType()
        {
            Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public Genre Genre { get; set; }
    }
}
