using System;
using System.Collections.Generic;

namespace ProductApi.Models
{
    public partial class Product
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public string? Mark { get; set; }
        public int? IdCategory { get; set; }
        public decimal? Price { get; set; }

        public virtual Category? oCategory { get; set; }
    }
}
