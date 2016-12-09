using analyzer.DistinctProductList;
using analyzer.Products.ProductComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace analyzer.Products.DistinctProductList.types
{
    public class DistinctProductList<T> : DistinctList<T> where T : Product
    {
        public Dictionary<int, List<T>> prunGroups = new Dictionary<int, List<T>>();
        public List<int[]> testPruning = new List<int[]>();

        List<string[]> oldTokensList = new List<string[]>();
        public Dictionary<string, bool> stopWord = new Dictionary<string, bool>();
        public int deletedDoublicates = 0;

        public DistinctProductList(List<T> Items, Dictionary<string, bool> StopWord, List<string[]> OldTokensList, Dictionary<int, List<T>> PrunGroups)
        {
            AddRange(Items);
            stopWord = StopWord;
            oldTokensList = OldTokensList;
            prunGroups = PrunGroups;
        }

        public DistinctProductList(){ }

        //*************
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
                        if (!newItemTokens.Contains(oldToken) && oldToken != "" && !stopWord.ContainsKey(oldToken))
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    foreach (string newToken in newItemTokens)
                    {
                        if (!oldTokens.Contains(newToken) && newToken != "" && !stopWord.ContainsKey(newToken))
                        {
                            isEqual = false;
                            break;
                        }
                    }

                    if (isEqual)
                    {
                        /*foreach (var newstr in newItemTokens)
                        {
                            Debug.Write(newstr + " ");
                        }

                        Debug.WriteLine("");

                        foreach (var oldstr in oldTokens)
                        {
                            Debug.Write(oldstr + " ");
                        }

                        Debug.WriteLine("");
                        Debug.WriteLine("");*/

                        deletedDoublicates++;
                        isDup = true;
                        break;
                    }
                }

                if (!isDup)
                {
                    oldTokensList.Add(newItemTokens);
                    base.Add(item);

                    //*************
                    CreatePruningList(concatinateStrArray(newItemTokens), item);
                }
            }
        }

        //*************
        private void CreatePruningList(string productString, T product)
        {
            //*************
            MatchCollection numbers = Regex.Matches(productString, "\\d+\\.\\d+|\\d+");

            List<int> nonDupNumbers = new List<int>();

            //*************
            foreach (Match number in numbers)
            {
                if (!number.Value.Contains(".") && number.Value.Count() < 6)
                {
                    int value = int.Parse(number.Value);

                    if (!nonDupNumbers.Contains(value))
                    {
                        nonDupNumbers.Add(value);
                    }
                }
            }

            //*************
            if (nonDupNumbers.Count() == 0)
            {
                AddToPrun(0, product);
            }

            //*************
            foreach (int number in nonDupNumbers)
            {

                if ((product.Category.ToLower() == "gpu" && isGpuNumberPrunable(number))
                    || (product.Category.ToLower() == "cpu" && isCpuNumberPrunable(number)))
                {
                    AddToPrun(number, product);
                }
            }
        }


        //*************
        private void AddToPrun(int number, T product)
        {
            if (prunGroups.ContainsKey(number))
            {
                prunGroups[number].Add(product);

                foreach (int[] testRow in testPruning)
                {
                    if (testRow[0] == number)
                    {
                        testRow[1]++;
                        break;
                    }
                }
            }
            else
            {
                prunGroups.Add(number, new List<T>() { product });
                testPruning.Add(new int[2] { number, 1 });
            }

            product.prunNumbers.Add(number);
        }


    private string concatinateStrArray(string[] strArray)
        {
            string temp = "";

            foreach (var item in strArray)
            {
                temp += item + " ";
            }

            return temp.Trim();
        }

        //*************
        private string[] generateCPUString(CPU newItem)
        {
            string newString = newItem.CpuSeries + " " + newItem.Model;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace("/", " ").Split(' ');

            return newStringArray;
        }

        //*************
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

        //*************
        private string[] generateCompareString(T item)
        {
            string[] result;

            if (item.GetType() == typeof(CPU))
            {
                stopWord = new Dictionary<string, bool>()
                {
                    { "quad", true },
                    { "octa", true },
                    { "dual", true },
                    { "duo", true },
                    { "a", true },
                    { "second", true },
                    { "first", true },
                    { "third", true },
                    { "serie", true },
                    { "series", true },
                    { "radeon", true },
                    { "extreme", true },
                    { "edition", true },
                    { "generation", true },
                    { "pentium", true },
                    { "r", true },
                    { "r1", true },
                    { "r2", true },
                    { "r3", true },
                    { "r4", true },
                    { "r5", true },
                    { "r6", true },
                    { "r7", true },
                    { "r8", true },
                    { "r9", true },
                    { "core", true },
                    { "i", true },
                    { "ii", true },
                    { "iii", true },
                    { "iiii", true },
                    { "amd", true },
                    { "intel", true }
                };

                result = generateCPUString((CPU)(object)item);
            }
            else if (item.GetType() == typeof(GPU))
            {
                stopWord = new Dictionary<string, bool>()
                {
                    { "nvidia", true },
                    { "geforce", true },
                    { "radeon", true },
                    { "amd", true },
                    { "gtx", true }
                };

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

        //*************
        public new DistinctProductList<T> GetRange(int index, int count)
        {
             return new DistinctProductList<T>(base.GetRange(index, count), stopWord, oldTokensList, prunGroups);
        }

        public void NearDublicates()
        { //test of merging of products
            Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine(""); Debug.WriteLine("");
            Debug.WriteLine("");
            int firstLoop = 0;

            foreach (string[] str1 in oldTokensList)
            {
                int ndLoop = 0;

                foreach (string[] str2 in oldTokensList)
                {
                    int i = 0;
                    int j = 0;

                    foreach (string token1 in str1)
                    {
                        
                        if (token1.Any(char.IsDigit) && str2.Contains(token1) && token1.Count() > 2 && firstLoop > ndLoop)
                        {
                            i++;
                        }
                    }
                    foreach (string token1 in str1)
                    {
                        foreach (string token2 in str2)
                        {

                            if (token1 == token2 && !stopWord.ContainsKey(token1) && !stopWord.ContainsKey(token2))
                            {
                                j++;
                                break;
                            }
                        }
                    }

                    if (i >= 1 && j > 2)
                    {
                        foreach (var newstr in str1)
                        {
                            Debug.Write(newstr + " ");
                        }

                        Debug.WriteLine("");

                        foreach (var oldstr in str2)
                        {
                            Debug.Write(oldstr + " ");
                        }

                        Debug.WriteLine("");
                        Debug.WriteLine("");
                    }
                    ndLoop++;
                }
                firstLoop++;
            }
        }
    }
}