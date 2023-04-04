namespace GoldFish.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Hystory = new HashSet<Hystory>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserID { get; set; }

        [Required]
        [StringLength(100)]
        public string UserLastName { get; set; }

        [Required]
        [StringLength(50)]
        public string UserFirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string UserPatronymic { get; set; }

        [Required]
        public string UserLogin { get; set; }

        [Required]
        public string UserPassword { get; set; }

        public int? RoleID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Required]
        [StringLength(202)]
        public string UserFullName { get; set; }

        public virtual Client Client { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Hystory> Hystory { get; set; }

        public virtual Role Role { get; set; }
    }
}
