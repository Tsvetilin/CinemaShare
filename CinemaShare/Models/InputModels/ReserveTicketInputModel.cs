using System.Collections.Generic;

namespace CinemaShare.Models.InputModels
{
    public class ReserveTicketInputModel: TicketInputDataModel
    {
        public Dictionary<int,TicketInputModel> TicketInputModels { get; set; }
    }
}
