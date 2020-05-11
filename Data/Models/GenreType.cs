using Data.Enums;
using System;

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
