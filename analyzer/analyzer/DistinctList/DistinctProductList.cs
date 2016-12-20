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


        private Dictionary<string, List<string[]>> oldTokensIndex = new Dictionary<string, List<string[]>>();
        public Dictionary<string, bool> stopWord = new Dictionary<string, bool>();
        public int deletedDoublicates = 0;

        public DistinctProductList(List<T> Items, Dictionary<string, bool> StopWord, Dictionary<string, List<string[]>> OldTokensIndex, Dictionary<int, List<T>> PrunGroups)
        {
            AddRange(Items);
            stopWord = StopWord;
            oldTokensIndex = OldTokensIndex;
            prunGroups = PrunGroups;
        }

        public DistinctProductList(){ }

        //Tests if addition is a duplicate and saves intermidiate data, for later deduplication test. Also creates Inverted index for fast linking.
        public new void Add(T item)
        {
            string[] newItemTokens = generateSpecificData(item);


            if (oldTokensIndex == null && newItemTokens != null)
            {//if list is empthy add addition to the list.
                base.Add(item);
                AddProductToInvertedIndex(concatinateStrArray(newItemTokens), item);
            }
            else if (newItemTokens != null)
            {
                bool isDup = false; //if this is changed to true, the item is already added.
                
                //inverted index for merging. Contains only products that have some of the same tokens.
                List<string[]> comparisonGroup = new List<string[]>();

                //Find products that might be a duplicate, based on inverted matrix for merging.
                foreach (var newToken in newItemTokens)
                {
                    if (oldTokensIndex.ContainsKey(newToken) && !stopWord.ContainsKey(newToken))
                    {
                        comparisonGroup.AddRange(oldTokensIndex[newToken]);
                    }
                }

                //Check if new item is duplicated in the comprisonGroup
                foreach (var oldTokens in comparisonGroup)
                {
                    bool isEqual = true; // if this is changed to false, all tokens are not equal
                    
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
                        //one of the products contains 100% same tokens, while stop words are removed.
                        deletedDoublicates++; //For debugging, and information
                        isDup = true;         //Marks new product as duplicate
                        break;
                    }
                }

                //If the new addition is not a duplicate, add it
                if (!isDup)
                { 
                    foreach (var newToken in newItemTokens)
                    {//adding the tokens to the inverted index for merging
                        if (oldTokensIndex.ContainsKey(newToken))
                        {
                            oldTokensIndex[newToken].Add(newItemTokens);
                        }
                        else
                        {
                            oldTokensIndex.Add(newToken, new List<string[]>() { newItemTokens });
                        }
                    }

                    //adding the product to the list.
                    base.Add(item);
                    //adding the product in the inverted index for linking
                    AddProductToInvertedIndex(concatinateStrArray(newItemTokens), item);
                }
            }
        }

        //Add a product, and all its tokens to the inverted matrix
        private void AddProductToInvertedIndex(string productString, T product)
        {
            //Token is chosen only to be numbers, for best result.
            MatchCollection numbers = Regex.Matches(productString, "\\d+\\.\\d+|\\d+");

            List<int> nonDupNumbers = new List<int>();

            //Remove the number if it is duplicate, or if the number contains a dot.
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

            //if there is no numbers, add it to index 0.
            if (nonDupNumbers.Count() == 0)
            {
                AddUDAToIndex(0, product);
            }

            //add each number token as term or identifier, and UDA as date or document to the inverted index.
            foreach (int number in nonDupNumbers)
            {

                if ((product.Category.ToLower() == "gpu" && isGpuNumberPrunable(number))
                    || (product.Category.ToLower() == "cpu" && isCpuNumberPrunable(number)))
                {
                    AddUDAToIndex(number, product);
                }
            }
        }


        //Adding UDAs to the inverted index 
        private void AddUDAToIndex(int number, T product)
        {
            if (prunGroups.ContainsKey(number))
            {//If the token exists 
                prunGroups[number].Add(product);

                foreach (int[] testRow in testPruning)
                {//This is done to later be able to debug if the inverted index recieves too much data for one group.
                    if (testRow[0] == number)
                    {
                        testRow[1]++;
                        break;
                    }
                }
            }
            else
            {//if the token does not exist
                prunGroups.Add(number, new List<T>() { product });
                testPruning.Add(new int[2] { number, 1 }); //This is only done for test and debug on the inverted index
            }

            product.prunNumbers.Add(number);
        }

        //adds a string array to one string
        private string concatinateStrArray(string[] strArray)
        {
            string temp = "";

            foreach (var item in strArray)
            {
                temp += item + " ";
            }

            return temp.Trim();
        }

        //generate the UDAs for the new GPU
        private string[] generateCPUString(CPU newItem)
        {
            string newString = newItem.CpuSeries + " " + newItem.Model;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace("/", " ").Split(' ');

            return newStringArray;
        }

        //Generate the UDAs for the new GPU.
        private string[] generateGPUString(GPU newItem)
        {
            string newString = newItem.GraphicsProcessor + " " + newItem.Manufacturer + " " + newItem.Model;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace("/", " ").Split(' ');

            return newStringArray;
        }

        //Not implemented, needs to be changed before chassis can be used in the analyzer. Slight extra product feature resolution added.
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

        //Not implemented, needs to be changed before chassis can be used in the analyzer
        private string[] generateChassisString(Chassis newItem)
        {
            base.Add((T)(object)newItem);

            return null;
        }

        //Not implemented, needs to be changed before motherboard can be used in the analyzer
        private string[] generateMotherboardString(Motherboard newItem)
        {

            string newString = newItem.Name;

            string[] newStringArray = newString.ToLower().Replace("-", " ").Replace(".", " ").Replace("/", " ").Replace(",", " ").Split(' ');

            return newStringArray;
        }

        //Not implemented, needs to be changed before PSU can be used in the analyzer
        private string[] generatePSUString(PSU newItem)
        {
            base.Add((T)(object)newItem);

            return null;
        }

        //Not implemented, needs to be changed before RAM can be used in the analyzer
        private string[] generateRAMString(RAM newItem)
        {
            base.Add((T)(object)newItem);

            return null;
        }

        //Generate UDAs Tokens for a product. If this is the first product, it also adds the stopword list. 
        private string[] generateSpecificData(T item)
        {
            string[] result;

            if (item.GetType() == typeof(CPU))
            {
                if (stopWord.Count == 0)
                {
                    stopWord = new Dictionary<string, bool>()
                    {
                        {"quad", true},
                        {"octa", true},
                        {"dual", true},
                        {"duo", true},
                        {"a", true},
                        {"second", true},
                        {"first", true},
                        {"third", true},
                        {"serie", true},
                        {"series", true},
                        {"radeon", true},
                        {"extreme", true},
                        {"edition", true},
                        {"generation", true},
                        {"pentium", true},
                        {"r", true},
                        {"r1", true},
                        {"r2", true},
                        {"r3", true},
                        {"r4", true},
                        {"r5", true},
                        {"r6", true},
                        {"r7", true},
                        {"r8", true},
                        {"r9", true},
                        {"core", true},
                        {"i", true},
                        {"ii", true},
                        {"iii", true},
                        {"iiii", true},
                        {"amd", true},
                        {"intel", true}
                    };
                }

                result = generateCPUString((CPU)(object)item);
            }
            else if (item.GetType() == typeof(GPU))
            {
                if (stopWord.Count == 0)
                {
                    stopWord = new Dictionary<string, bool>()
                    {
                        {"nvidia", true},
                        {"geforce", true},
                        {"radeon", true},
                        {"amd", true},
                        {"gtx", true}
                    };
                }

                result = generateGPUString((GPU)(object)item);
            }//from here on down, the products are not yet generated, but ready to be.
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

        //returns a new list, but new additions should not be made to this list(those would be wrongly deduplicated.).
        public new DistinctProductList<T> GetRange(int index, int count)
        {
             return new DistinctProductList<T>(base.GetRange(index, count), stopWord, oldTokensIndex, prunGroups);
        }
    }
}