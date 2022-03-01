using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorkoutAPI.Models
{
    public class WorkoutModel
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }

        //public int ActivityID { get; set; }
        [Required]
        public string Userid { get; set; }

        //[ForeignKey("ActivityID")]
        //public ActivityModel? Activity { get; set; }


    }
}
