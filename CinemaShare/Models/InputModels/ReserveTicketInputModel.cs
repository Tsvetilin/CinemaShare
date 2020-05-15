using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.InputModels
{
    public class ReserveTicketInputModel: TicketInputDataModel
    {
        public Dictionary<int,TicketInputModel> TicketInputModels { get; set; }
    }
}
