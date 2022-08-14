using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mobile.Entities.Entities
{
    public class SetProduct
    {
        public int Id { get; set; }

        public string Barcode { get; set; }
        public int Piece { get; set; }

        public int ProductId { get; set; }
        public Product Products { get; set; }
    }
}
