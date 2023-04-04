namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            OrderProduct = new HashSet<OrderProduct>();
        }

        [Key]
        [StringLength(100)]
        public string ProductArticleNumber { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductDescription { get; set; }

        public int? CategoryID { get; set; }

        public byte[] ProductPhoto { get; set; }

        public int? ManufacturerID { get; set; }

        public decimal ProductCost { get; set; }

        public int ProductDiscountAmount { get; set; }

        public int ProductQuantityInStock { get; set; }

        public int? UnitID { get; set; }

        public int ProductMaxDiscountAmount { get; set; }

        public virtual Category Category { get; set; }

        public virtual Manufacturer Manufacturer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderProduct> OrderProduct { get; set; }

        public virtual Unit Unit { get; set; }
    }
}
