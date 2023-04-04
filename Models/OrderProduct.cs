namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderProduct")]
    public partial class OrderProduct
    {
        public int OrderProductID { get; set; }

        public int? OrderID { get; set; }

        [StringLength(100)]
        public string ProductArticleNumber { get; set; }

        public int OrderProductCount { get; set; }

        public virtual Order Order { get; set; }

        public virtual Product Product { get; set; }
    }
}
