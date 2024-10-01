using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class WishListItem
    {
        public int ProductId { get; set; }
        public int WishListId { get; set; }

        public Product Product { get; set; }
        //public Wishlist Wishlist { get; set; }
    }
}
