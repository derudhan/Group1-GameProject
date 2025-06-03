using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HellVillage {
    public class UIInventory : MonoBehaviour {
        private Inventory inventory;
        [SerializeField] private Transform itemSlotContainer;
        [SerializeField] private Transform itemSlotTemplate;

        private void Awake() {
            itemSlotTemplate.gameObject.SetActive(false);
        }

        public void SetInventory(Inventory inventory) {
            if (inventory == null) {
                Debug.LogWarning("Attempted to set a null inventory.");
                return;
            }

            this.inventory = inventory;
            inventory.OnItemListChanged += Inventory_OnItemListChanged;
            RefreshInventoryItems();
        }

        private void Inventory_OnItemListChanged(object sender, EventArgs e) {
            RefreshInventoryItems();
        }

        private void RefreshInventoryItems() {
            foreach (Transform child in itemSlotContainer) {
                if (child != itemSlotTemplate) {
                    Destroy(child.gameObject);
                }
            }

            foreach (Item item in inventory.GetItemList()) {
                RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
                itemSlotRectTransform.gameObject.SetActive(true);

                Image image = itemSlotRectTransform.GetComponent<Image>();
                image.sprite = item.GetSprite();

                TextMeshProUGUI uiText = itemSlotRectTransform.GetComponentInChildren<TextMeshProUGUI>();
                uiText.text = item.amount > 1 ? item.amount.ToString() : string.Empty;
            }

            if (inventory.GetItemList().Count == 0) {
                gameObject.SetActive(false);
            }
        }
    }
}
