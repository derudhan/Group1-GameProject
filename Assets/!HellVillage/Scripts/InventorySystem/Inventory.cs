using System;
using System.Collections.Generic;
using UnityEngine;

namespace HellVillage.InventorySystem {
    public class Inventory {
        public event EventHandler OnItemListChanged;

        private List<Item> itemList;

        public Inventory() {
            itemList = new List<Item>();
        }

        public void AddItem(Item item) {
            if (item == null) {
                Debug.LogWarning("Attempted to add a null item to the inventory.");
                return;
            }

            if (item.IsStackable()) {
                bool itemAlreadyExists = false;
                foreach (Item inventoryItem in itemList) {
                    if (inventoryItem.itemType == item.itemType) {
                        inventoryItem.amount += item.amount;
                        itemAlreadyExists = true;
                    }
                }
                if (!itemAlreadyExists) {
                    itemList.Add(item);
                }
            } else {
                itemList.Add(item);
            }
            OnItemListChanged?.Invoke(this, EventArgs.Empty);
        }

        public List<Item> GetItemList() {
            return itemList;
        }
    }
}
