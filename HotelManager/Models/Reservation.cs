using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace HotelManager.Models
{
    public class Reservation : IValidatableObject
    {
        public int Id { get; set; }
        [Display(Name = "Room Id")]
        public int RoomId { get; set; }
        [Display(Name = "Room Id")]
        public Room ReservedRoom { get; set; }

        public List<ReservationClient> ReservationClient { get; set; }
        public int UserEmployeeId { get; set; }

        public UserEmployee UserEmployee { get; set; }
        public string UserEmployeeEmail { get; set; }

        [Display(Name = "Accomodation Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime AccomodationDate { get; set; }

        [Display(Name = "Release Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public DateTime ReleaseDate { get; set ; }
        [Display(Name = "Is breakfast included:")]
        public bool BreakfastIncluded { get; set; }
        [Display(Name = "Is all inclusive:")]

        public bool IsAllInclusive { get; set; }
        [Display(Name = "Price:")]

        public double FinalPrice { get; set; }
        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            if (ReleaseDate < AccomodationDate)
            {
                yield return new ValidationResult("Release date must be greater than accomodation date");
            }
        }
    }
}