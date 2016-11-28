using analyzer.Products.ProductComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzer.Products.DistinctProductList.types
{
    public class DistinctProductList<T> : List<T>
    {
        public new void Add(T item)
        {
            if(item.GetType() == typeof(CPU))
            {
                parseCPU((CPU)(object)item);
            }
        }

        private void parseCPU(CPU newItem)
        {
            bool isDuplicate = false;
            bool isDup = false;

            foreach (var oldItem in this)
            {
                CPU oldItemP = (CPU)(object)oldItem;
                string oldString = oldItemP.CpuSeries + " " + oldItemP.Model + " " + oldItemP.Manufacturer;
                string newString = newItem.CpuSeries + " " + newItem.Model + " " + newItem.Manufacturer;
                oldString = oldString.ToLower();
                newString = newString.ToLower();

                string[] oldStringA = oldString.Replace("-", " ").Split(' ');
                string[] newStringA = newString.Replace("-", " ").Split(' ');

                bool isEqual = true;

                foreach (string oldS in oldStringA)
                {
                    if (!newString.Contains(oldS))
                    {
                        isEqual = false;
                    }
                }
                foreach (string newS in newStringA)
                {
                    if (!oldString.Contains(newS))
                    {
                        isEqual = false;
                    }
                }

                if (oldString != newString)
                {
                    isDuplicate = true;
                } 

                if (isEqual)
                {
                    isDup = true;
                    break;
                }
            }

            if (!isDuplicate && !isDup)
            {
                base.Add((T)(object)newItem);
            }
            else if (!isDup)
            {
                base.Add((T)(object)newItem);
            }
            else if (!isDuplicate)
            {
                base.Add((T)(object)newItem);
            }

            if (isDup && isDuplicate)
            {

            }
        }
    }
}
