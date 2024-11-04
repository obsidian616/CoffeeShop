namespace CoffeeShop.Backend.Models.EFModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class GroupFunction
    {
        public int Id { get; set; }

        public int GroupId { get; set; }

        public int FunctionId { get; set; }

        public int ModifyUser { get; set; }

        public DateTime ModifyTime { get; set; }

        public bool Enabled { get; set; }

        public virtual Function Function { get; set; }

        public virtual Group Group { get; set; }
    }
}
