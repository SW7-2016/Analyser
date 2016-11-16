using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products
{
    public abstract class ComputerComponents : Product
    {
        protected ComputerComponents(int id, string category) : base(id, category)
        {
        }

    }
}
