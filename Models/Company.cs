namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Company")]
    public partial class Company
    {
        [Key]
        [StringLength(25)]
        public string CompanyName { get; set; }

        public byte[] CompanyLogo { get; set; }
    }
}
