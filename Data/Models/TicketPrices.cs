using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models
{
    public class TicketPrices
    {
        public TicketPrices()
        {
            this.Id = Guid.NewGuid().ToString();
        }
        public string Id { get; set; }
        public double StudentPrice { get; set; }
        public double AdultPrice { get; set; }
        public double ChildrenPrice { get; set; }
    }
}
