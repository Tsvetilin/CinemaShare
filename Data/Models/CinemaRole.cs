using Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;

namespace Data.Models
{
    public class CinemaRole:IdentityRole<Guid>
    {
        RoleType Role { get; set; }
    }
}
