namespace CoffeeShop.Backend.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GuestBook
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [StringLength(254)]
        public string Email { get; set; }

        [StringLength(10)]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        public string Message { get; set; }

        public DateTime Createdtime { get; set; }
    }
}
