﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace exercise.main
{
    public class Basket
    {
        Inventory inventory = new Inventory();
        public int MAX_BASKET_SIZE { get; set; } = 3;
        public List<Item> yourBasket = new List<Item>();
        public Dictionary<Item, int> itemCount = new Dictionary<Item, int>();
        public int items_in_basket = 0;
        
        public bool addItem(string itemType, string variant)
        {

            Item item = inventory.findItemByName(variant);
            
            if (items_in_basket >= MAX_BASKET_SIZE)
            {
                Console.WriteLine("Basket is full...");
                return false;
            }
            if (item != null) { 
                yourBasket.Add(item);
                items_in_basket++;
                return true;
            }
            else
            {
                Console.WriteLine("No such item...");
                return false;
            }
        }

        public void removeItem(string itemName)
        {
            Item item = inventory.findItemByName(itemName);
            if (!yourBasket.Contains(item))
            {
                Console.WriteLine("Your basket does not contain this item...");
                return;
            }
            else
            {
                yourBasket.Remove(item);
            }
        }

        public void changeCapacity(int newCapacity, Person person)
        {
            if ((newCapacity <= 0) || (person.role == Role.CUSTOMER))
            {
                Console.WriteLine("Cannot change capacity to 0 or lower or you do not have the permission to do this action...");
            }
            else
            {
                MAX_BASKET_SIZE = newCapacity;
            }
        }

        public double checkTotal()
        {
            return Math.Round(yourBasket.Sum(item => item.price), 2);
        }

        public double checkPriceForType(string type)
        {
            return inventory.findItemByName(type).price;
        }


        //extension 2
        public string Reciept()
        {
            string reciept = $"\n\n\n----     Reciept     ----\n";
            reciept += $"    {DateTime.Now.ToString()}    \n\n";
            reciept += $"-------------------------\n\n";
            double totalPrice = checkTotal();
            List<string> allreadyCountedItems = [];
            foreach (Item item in yourBasket)
            {
                double totalPriceForSpecifiedItem = 0;
                double itemCount = 0;

                if (!allreadyCountedItems.Contains(item.id))
                {

                    foreach (Item specificItem in yourBasket)
                    {
                        if (specificItem.id == item.id)
                        {
                            itemCount++;
                        }
                    }


                    totalPriceForSpecifiedItem = itemCount * item.price;
                    reciept += $"{item.name}, {item.variant}  {itemCount}x - {totalPriceForSpecifiedItem}\n";
                    allreadyCountedItems.Add(item.id);
                }
            }
            reciept += $"-------------------------\ntotal:            {totalPrice}";

            //Console.WriteLine(reciept);
            return reciept;
        }


        public double Discount()
        {
            foreach (Item item in yourBasket)
            {
                if (itemCount.ContainsKey(item))
                {
                    int count = itemCount[item] + 1;
                    itemCount[item] = count;
                }
                else
                {
                    itemCount.Add(item, 1);
                }
            }

            double coffeeDeal = 1.25;
            double sixBagels = 2.49;
            double twelveBagels = 3.99;

            List<Item> coffee = inventory._inventory.Where(item => item.id.Contains("BGL")).ToList();
            List<Item> bagel = inventory._inventory.Where(item => item.id.Contains("COF")).ToList();

            //my assumption here is that the coffee + bagel discounts only counts if those are the only things a customer is buying
            if(yourBasket.Count == 2 && (coffee.Any(x => x.id.Contains(itemCount.First().Key.id) || x.id.Contains(itemCount.Last().Key.id) 
            && (bagel.Any(x => x.id.Contains(itemCount.First().Key.id) || x.id.Contains(itemCount.Last().Key.id))))))
            {
                return coffeeDeal;
            }

            foreach(var item in itemCount)
            {
                if(item.Key.name == "Bagel" && item.Value == 6)
                {
                    return sixBagels;
                }
                else if (item.Key.name == "Bagel" && item.Value == 12)
                {
                    return twelveBagels;
                }
             
            }
            return 0;

        }

        public string recieptWithDiscount()
        {

            double coffeeDeal = 1.25;
            double sixBagels = 2.49;
            double twelveBagels = 3.99;

            double moneys = Discount();
            double moneySaved = 0;
            if (moneys == 0)
            {
                return Reciept();
            }

            string reciept = $"\n\n\n----     Reciept     ----\n\n";
            reciept += $"    {DateTime.Now.ToString()}    \n\n";
            reciept += $"-------------------------\n\n";
            double totalPrice = 0;

            if (moneys == sixBagels || moneys == twelveBagels)
            {
                
                
                foreach(var item in itemCount)
                {
                    double usualTotal = (Math.Round((item.Key.price * item.Value), 2));
                    if (item.Value == 6 || item.Value == 12)
                    {
                        reciept += $"{item.Key.name}, {item.Key.variant}  {item.Value}x - {moneys}\n";
                       
                        double moneySavedOnTheseItems = Math.Round((usualTotal - moneys), 2);
                        reciept += $"        discount: ({moneySavedOnTheseItems})\n\n";
                        moneySaved += moneySavedOnTheseItems;
                        totalPrice += moneys;

                    }
                    else
                    {
                        reciept += $"{item.Key.name}, {item.Key.variant}  {item.Value}x - {usualTotal}\n\n";
                        totalPrice += usualTotal;
                    }
                }
                
                
            }
            else if (moneys == coffeeDeal){
                reciept += $"coffeeNbagelDeal!    {moneys} \n{itemCount.First().Key.name}, {itemCount.First().Key.variant}\n{itemCount.Last().Key.name}, {itemCount.Last().Key.variant}\n\n";
                double usualTotal = itemCount.First().Key.price + itemCount.Last().Key.price;
                double moneySavedOnTheseItems = usualTotal - moneys;
                moneySaved += moneySavedOnTheseItems;
                totalPrice += moneys;
            }
            reciept += $"-------------------------\ntotal:               {Math.Round(totalPrice, 2)}\n";
            reciept += $"money saved:         {Math.Round(moneySaved,2)}\n";


            //Console.WriteLine(reciept);
            return reciept;
        }


        // LEGACY CODE

        //old method - extension 1 and 3 made into one method. Not pretty :)
        public string oldRecieptWithDiscount()
        {
            string reciept = $"----     Reciept     ----\n\n";
            reciept += $"    {DateTime.Now.ToString()}    \n";
            reciept += $"-------------------------\n";
            double totalPrice = 0;
            List<string> allreadyCountedItems = [];
            foreach (Item item in yourBasket)
            {
                double totalPriceForSpecifiedItem = 0;
                double discountedPrice = 0;
                double itemCount = 0;
                double discount = 0;

                if (!allreadyCountedItems.Contains(item.id))
                {

                    foreach (Item specificItem in yourBasket)
                    {
                        if (specificItem.id == item.id)
                        {
                            itemCount++;
                        }
                    }
                    if (itemCount == 6)
                    {
                        totalPriceForSpecifiedItem = itemCount * item.price;
                        discountedPrice = 2.49;
                        discount = totalPriceForSpecifiedItem - discount;
                        totalPrice += discountedPrice;
                        reciept += $"{item.name}: {item.variant}  {itemCount}x - {discountedPrice}\n";
                        reciept += $"          discount ({discount})\n";
                        allreadyCountedItems.Add(item.id);
                    }
                    else if (itemCount == 12)
                    {
                        totalPriceForSpecifiedItem = itemCount * item.price;
                        discountedPrice = 3.99;
                        discount = totalPriceForSpecifiedItem - discount;
                        totalPrice += discountedPrice;
                        reciept += $"{item.name}: {item.variant}  {itemCount}x - {discountedPrice}\n";
                        reciept += $"          discount ({discount})\n";
                        allreadyCountedItems.Add(item.id);
                    }
                    else
                    {
                        totalPriceForSpecifiedItem = itemCount * item.price;
                        reciept += $"{item.name}: {item.variant}  {itemCount}x - {totalPriceForSpecifiedItem}\n";
                        totalPrice += totalPriceForSpecifiedItem;
                        allreadyCountedItems.Add(item.id);
                    }

                }
            }


            reciept += $"-------------------------\ntotal:            {totalPrice}";

            Console.WriteLine(reciept);
            return reciept;
        }
    }
}



