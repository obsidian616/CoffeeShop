namespace CoffeeShop.Backend.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserGroup
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GroupId { get; set; }

        public int ModifyUser { get; set; }

        public DateTime ModifyTime { get; set; }

        public virtual Group Group { get; set; }

        public virtual User User { get; set; }
    }
}
