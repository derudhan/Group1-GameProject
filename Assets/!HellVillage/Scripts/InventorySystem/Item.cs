using UnityEngine;

namespace HellVillage.InventorySystem {
    [System.Serializable]
    public class Item {
        public enum ItemType {
            Item1,
            Item2
        }

        public ItemType itemType;
        public int amount;

        public Sprite GetSprite() {
            switch (itemType) {
                case ItemType.Item1:
                    return AssetsManager.Instance.item1;
                case ItemType.Item2:
                    return AssetsManager.Instance.item2;
                default:
                    Debug.LogWarning("Unknown item type: " + itemType);
                    return null;
            }
        }

        public Color GetColor() {
            switch (itemType) {
                case ItemType.Item1:
                    return Color.gray;
                case ItemType.Item2:
                    return Color.red;
                default:
                    Debug.LogWarning("Unknown item type for color: " + itemType);
                    return Color.white;
            }
        }

        public bool IsStackable() {
            switch (itemType) {
                case ItemType.Item1:
                case ItemType.Item2:
                    return true;
                default:
                    Debug.LogWarning("Unknown item type for stackability: " + itemType);
                    return false;
            }
        }
    }
}
