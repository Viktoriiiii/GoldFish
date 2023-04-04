namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Hystory")]
    public partial class Hystory
    {
        public int HystoryID { get; set; }

        public int? UserID { get; set; }

        public DateTime HystoryLastSignIn { get; set; }

        public bool HystoryInput { get; set; }

        public virtual User User { get; set; }
    }
}
