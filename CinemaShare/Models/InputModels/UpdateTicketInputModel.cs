using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaShare.Models.InputModels
{
    public class UpdateTicketInputModel: TicketInputDataModel
    {
        public TicketInputModel Ticket { get; set; }
    }
}
