using System.Collections.Generic;

namespace CinemaShare.Models.ViewModels
{
    public class TicketsIndexViewModel : PageViewModel
    {
        public List<TicketCardViewModel> Tickets { get; set; }
    }
}
