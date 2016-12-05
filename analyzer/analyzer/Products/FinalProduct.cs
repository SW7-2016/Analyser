using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using analyzer.Products.Retailers;

namespace analyzer.Products
{
    class FinalProduct
    {
        public static int numberOfIds;

        protected FinalProduct(int oldId)
        {
            ProductId = ++numberOfIds;
            OldProductId = oldId;
        }

        public int ProductId { get; }
        public int OldProductId { get; }

    }
}
