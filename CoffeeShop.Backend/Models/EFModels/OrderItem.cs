namespace CoffeeShop.Backend.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderItem
    {
        public int Id { get; set; }

        public int? OrderId { get; set; }

        public int? MenuId { get; set; }

        public int? Price { get; set; }

        [StringLength(100)]
        public string ItemCName { get; set; }

        [StringLength(100)]
        public string ItemEName { get; set; }

        public int TotalAmount { get; set; }

        public int Quantity { get; set; }

        [StringLength(100)]
        public string Notes { get; set; }

        public virtual Menu Menu { get; set; }

        public virtual Order Order { get; set; }
    }
}
