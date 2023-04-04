namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Client")]
    public partial class Client
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client()
        {
            Order = new HashSet<Order>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Required]
        [StringLength(8)]
        public string ClientCode { get; set; }

        [Required]
        [StringLength(4)]
        public string ClientSeriaPassport { get; set; }

        [Required]
        [StringLength(6)]
        public string ClientNumberPassport { get; set; }

        [Column(TypeName = "date")]
        public DateTime ClientBirthday { get; set; }

        [Required]
        [StringLength(100)]
        public string ClientAdress { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Order { get; set; }
    }
}
