using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelManager.Models
{
    public class Client
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Fisrt name")]
        public string FisrtName { get; set; }
        [Display(Name = "Last name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Phone number")]
        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }
        [Required]
        [RegularExpression(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Invalid email")]
        public string Email { get; set; }
        [Display(Name = "Is Adult")]

        public bool IsAdult { get; set; }
        public List<ReservationClient> ReservationClient { get; set; }
    }
}