using HotelManager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace HotelManager.Views.Reservations.Model
{
    public class FullReservation : IValidatableObject
    {
        public int Id { get; set; }
        [Display(Name = "Room number:")]
        public int RoomId { get; set; }
        // public Room ReservedRoom { get; set; }
        // public int ClientID { get; set; }
        [Display(Name = "Type the emails of the clients:")]
        public string ClientString { get; set; }
        public List<Client> Client { get; set; }
        [Display(Name = "Employee:")]
        public string UserEmployeeEmail { get; set; }

        [Display(Name = "Accomodation Date:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime AccomodationDate { get; set; }

        [Display(Name = "Release Date:")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ReleaseDate { get; set; }
        [Display(Name = "Is breakfast included:")]
        public bool BreakfastIncluded { get; set; }
        [Display(Name = "Is all inclusive:")]
        public bool IsAllInclusive { get; set; }
        [Display(Name = "Final price:")]
        public double FinalPrice { get; set; }
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (ReleaseDate < AccomodationDate)
            {
                yield return new ValidationResult("Release date must be greater than Accomodation date");
            }
        }
    }
}
