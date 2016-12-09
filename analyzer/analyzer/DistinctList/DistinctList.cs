using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.DistinctProductList
{
    public abstract class DistinctList<T> : List<T> 
    {
        //Stops some pruning groups from forming for CPU, because titles might contain specifications that we dont need.
        internal bool isCpuNumberPrunable(int number)
        {
            if (number > 20 && number < 15000
                && number != 775 && number != 1151
                && number != 1155 && number != 1156
                && number != 1366 && number != 2011)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Stops some pruning groups from forming for GPU, review title might contain non wanted pruning groups.
        internal bool isGpuNumberPrunable(int number)
        {
            if (number > 8 && number < 15000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
