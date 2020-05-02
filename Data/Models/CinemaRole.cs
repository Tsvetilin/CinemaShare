using Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;

namespace Data.Models
{
    public class CinemaRole:IdentityRole
    {
#pragma warning disable IDE0051 // Remove unused private members
        public RoleType Role { get; set; }
#pragma warning restore IDE0051 // Remove unused private members
    }
}
