using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MyCellar.Common.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string ImgUrl { get; set; }
        public virtual ICollection<RecipeProduct> RecipeProducts { get; set; }
        public virtual ICollection<UserProduct> UserProducts { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        [JsonIgnore]
        public int CategoryId { get; set; }
    }
}
