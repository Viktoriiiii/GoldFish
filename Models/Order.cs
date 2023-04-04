namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderProduct = new HashSet<OrderProduct>();
        }

        public int OrderID { get; set; }

        [Column(TypeName = "date")]
        public DateTime OrderDate { get; set; }

        public TimeSpan OrderTime { get; set; }

        [Column(TypeName = "date")]
        public DateTime OrderDeliveryDate { get; set; }

        public int? PointID { get; set; }

        public int? ClientID { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderCodeForGetOrder { get; set; }

        public int? StatusID { get; set; }

        public virtual Client Client { get; set; }

        public virtual Point Point { get; set; }

        public virtual Status Status { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }
    }
}
