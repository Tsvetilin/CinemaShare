using Data.Enums;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class CinemaRole:IdentityRole<Guid>
    {
        RoleType Role { get; set; }
    }
}
