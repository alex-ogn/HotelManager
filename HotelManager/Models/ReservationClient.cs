using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManager.Models
{
    public class ReservationClient
    {
        //[Key]
        //public int ReservationClientId { get; set; }
        public int ReservationID { get; set; }
        public Reservation Reservation { get; set; }
        public int ClientID { get; set; }
        public Client Client { get; set; }
    }
}
