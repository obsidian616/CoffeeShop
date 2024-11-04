namespace CoffeeShop.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("RecommendPhotos")]
    public partial class RecommendPhoto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string FileName { get; set; }

        public int DisplayOrder { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedTime { get; set; }

        public int ModifyUser { get; set; }

        public DateTime ModifyTime { get; set; }
    }
}
