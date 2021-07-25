using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Models
{
    public enum RoomType { TwinBeds, Apartment, DoubleBed, Penthouse, Duplex }
    public class Room
    {
        public int Id { get; set; }
        [Range(1,20, ErrorMessage = "The capacity must be in range 1 - 20.")]
        [Required]
        public int Capacity { get; set; }

        [Display(Name = "Room type")]
        [Required]
        public RoomType RoomType { get; set; }
        [Display(Name = "Is free")]
        public bool IsFree { get; set; }

        [Display(Name = "Adult bed price")]
        [Required]
        public double AdultBedPrice { get; set; }

        [Display(Name = "Child bed price")]
        [Required]
        public double ChildBedPrice { get; set; }

        [Display(Name = "Room number")]
        [Required]
        public int RoomNumber { get; set; }
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}