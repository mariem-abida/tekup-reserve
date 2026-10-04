using System;
using System.ComponentModel.DataAnnotations;

namespace TEKUP_RESERVE.Models
{
    public class Booking
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Salle")]
        public int RoomId { get; set; }

        // Navigation vers Room (pas Required)
        public Room? Room { get; set; }

        [Required]
        [Display(Name = "Motif")]
        public string? Purpose { get; set; }

        [Required]
        [Display(Name = "Début")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "Fin")]
        [DataType(DataType.DateTime)]
        public DateTime EndTime { get; set; }

        // Utilisateur (on ne le demande pas dans le formulaire, on le remplit nous-mêmes)
        [Display(Name = "Utilisateur")]
        public string UserId { get; set; }

    }
}
