namespace CoffeeShop.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Menu
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Menu()
        {
            OrderItems = new HashSet<OrderItem>();
            RecommendMenus = new HashSet<RecommendMenu>();
        }

        public int Id { get; set; }

        public int? CategoryID { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemCName { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemEName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public int Price { get; set; }

        public bool? Enabled { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }

        public DateTime Createdtime { get; set; }

        public DateTime Modifytime { get; set; }

        public virtual MenuCategory MenuCategory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RecommendMenu> RecommendMenus { get; set; }
    }
}
