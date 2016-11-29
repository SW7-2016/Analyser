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
        List<string[]> oldTokensList = new List<string[]>();
        public int deletedDoublicates = 0;

        public new void Add(T item)
        {
            string[] newItemTokens = generateCompareString(item);

            if (oldTokensList == null && newItemTokens != null)
            {
                base.Add(item);
            }
            else if (newItemTokens != null)
            {
                bool isDup = false;

                foreach (var oldTokens in oldTokensList)
                {
                    bool isEqual = true;

                    foreach (string oldToken in oldTokens)
                    {
                        if (!newItemTokens.Contains(oldToken) && oldToken != "")
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    foreach (string newToken in newItemTokens)
                    {
                        if (!oldTokens.Contains(newToken) && newToken != "")
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual)
                    {
                        foreach (var newstr in newItemTokens)
                        {
                            //Debug.Write(newstr + " ");
                        }

                        //Debug.WriteLine("");

                        foreach (var oldstr in oldTokens)
                        {
                           // Debug.Write(oldstr + " ");
                        }
                        //Debug.WriteLine("");
                        //Debug.WriteLine("");

                        deletedDoublicates++;
                        isDup = true;
                        break;
                    }
                }

                if (!isDup)
                {
                    oldTokensList.Add(newItemTokens);
                    base.Add((T)(object)item);
                }
            }
        }

        private string[] generateCPUString(CPU newItem)
        {
            string newString = newItem.CpuSeries + " " + newItem.Model + " " + newItem.Manufacturer;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace("/", " ").Split(' ');

            return newStringArray;
        }


        private string[] generateGPUString(GPU newItem)
        {
            string newString = newItem.GraphicsProcessor + " " + newItem.Manufacturer + " " + newItem.Model;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace("/", " ").Split(' ');

            return newStringArray;
        }

        //Not implementing, does normalize HDD Type, and then add like a normal list.
        private string[] generateHardDriveString(HardDrive newItem)
        {
            if (newItem.Type == "SSD" || newItem.Type == "Solid state drive")
            {
                newItem.Type = "SSD";
                base.Add((T)(object)newItem);
            }
            else if (newItem.Type == "HDD" || newItem.Type == "Harddisk")
            {
                newItem.Type = "HDD";
                base.Add((T)(object)newItem);
            }
            else if (newItem.Type == "Hybrid harddisk" || newItem.Type == "Solid state / harddisk")
            {
                newItem.Type = "Hybrid";
                base.Add((T)(object)newItem);
            }
            else
            {
                return null;
            }

            return null;
        }

        //Not implementing, just adding item like a normal list.
        private string[] generateChassisString(Chassis newItem)
        {
            base.Add((T)(object)newItem);

            return null;
        }

        //Not implementingcompletely, just checks if the name is the same
        private string[] generateMotherboardString(Motherboard newItem)
        {

            string newString = newItem.Name;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace(".", " ").Replace("/", " ").Replace(",", " ").Split(' ');

            return newStringArray;
        }

        //Not implementing, just adding item like a normal list.
        private string[] generatePSUString(PSU newItem)
        {
            base.Add((T)(object)newItem);

            return null;
        }

        //Not implementing, just adding item like a normal list.
        private string[] generateRAMString(RAM newItem)
        {
            base.Add((T)(object)newItem);

            return null;
        }

        private string[] generateCompareString(T item)
        {
            string[] result;

            if (item.GetType() == typeof(CPU))
            {
                result = generateCPUString((CPU)(object)item);
            }
            else if (item.GetType() == typeof(GPU))
            {
                result = generateGPUString((GPU)(object)item);
            }
            else if (item.GetType() == typeof(Motherboard))
            {
                result = generateMotherboardString((Motherboard)(object)item);
            }
            else if (item.GetType() == typeof(Chassis))
            {
                result = generateChassisString((Chassis)(object)item);
            }
            else if (item.GetType() == typeof(HardDrive))
            {
                result = generateHardDriveString((HardDrive)(object)item);
            }
            else if (item.GetType() == typeof(PSU))
            {
                result = generatePSUString((PSU)(object)item);
            }
            else if (item.GetType() == typeof(RAM))
            {
                result = generateRAMString((RAM)(object)item);
            }
            else
            {
                result = null;
            }

            return result;
        }
    }
}