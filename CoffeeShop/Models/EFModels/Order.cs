namespace CoffeeShop.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public int? MemberId { get; set; }

        public DateTime OrderDate { get; set; }

        public int Status { get; set; }

        [StringLength(50)]
        public string TableNumber { get; set; }

        public int TotalAmount { get; set; }

        public int? DiscountTotalAmount { get; set; }

        public DateTime Createdtime { get; set; }

        public DateTime Modifytime { get; set; }

        public virtual Member Member { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
