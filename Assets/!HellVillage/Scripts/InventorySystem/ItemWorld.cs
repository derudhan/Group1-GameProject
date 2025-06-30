using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace HellVillage.InventorySystem {
    public class ItemWorld : MonoBehaviour {
        public static ItemWorld SpawnItemWorld(Item item, Vector2 position) {
            ItemWorld itemWorld = Instantiate(AssetsManager.Instance.itemWorldPrefab, position, Quaternion.identity);
            itemWorld.SetItem(item);
            return itemWorld;
        }

        private Item item;
        private SpriteRenderer spriteRenderer;
        private Light2D light2D;
        private TextMeshPro amountText;

        private void Awake() {
            spriteRenderer = GetComponent<SpriteRenderer>();
            light2D = GetComponentInChildren<Light2D>();
            amountText = GetComponentInChildren<TextMeshPro>();
        }

        public void SetItem(Item item) {
            this.item = item;
            spriteRenderer.sprite = item.GetSprite();
            light2D.color = item.GetColor();
            amountText.text = item.amount > 1 ? item.amount.ToString() : string.Empty;
        }

        public Item GetItem() {
            return item;
        }

        public void DestroySelf() {
            Destroy(gameObject);
        }
    }
}
