namespace CoffeeShop.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class RecommendMenu
    {
        public int Id { get; set; }

        public int MenuId { get; set; }

        public int DisplayOrder { get; set; }

        public bool Enabled { get; set; }

        public virtual Menu Menu { get; set; }
    }
}
