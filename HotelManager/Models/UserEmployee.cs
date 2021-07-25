using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManager.Models
{
    public class UserEmployee : IdentityUser
    {
        [Display(Name = "Fisrt name")]
        [Required]
        [Column]
        public string FisrtName { get; set; }
        [Display(Name = "Father  name")]
        [Required]
        [Column]
        public string FatherName { get; set; }
        [Display(Name = "Last name")]
        [Required]
        [Column]
        public string LastName { get; set; }
        [Required]
        [Column]
        public string EGN { get; set; }
        [Display(Name = "Starting date")]
        [Column]
        public DateTime StartingDate { get; set; }
        [Display(Name = "Is active")]
        [Column]
        public bool ActiveUser { get; set; } = false;
        [Display(Name = "release date")]
        [Column]
        public DateTime ReleaseDate { get; set; }


        [Column]
        public virtual ICollection<Reservation> Reservations { get; set; }

    }
}
