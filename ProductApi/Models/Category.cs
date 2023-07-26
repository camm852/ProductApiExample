using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ProductApi.Models
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? Description { get; set; }

        [JsonIgnore] //Ignorar para referencias ciclicas 
        public virtual ICollection<Product> Products { get; set; }
    }
}
