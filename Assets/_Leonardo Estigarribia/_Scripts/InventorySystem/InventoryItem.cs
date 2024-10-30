using LeonardoEstigarribia.InventorySystem.itemData;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;
using UnityEngine.UI;

namespace LeonardoEstigarribia.InventorySystem.inventoryItem
{
    /// <summary>
    ///     Script in charge of managing the items that can be stored in the inventory.
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        public ItemData itemData;

        // Changes values depending if its rotated or not.
        public int invItemHeight
        {
            get
            {
                if (isRotated == false) return itemData.height;

                return itemData.width;
            }
        }

        // Changes values depending if its rotated or not.
        public int invItemWidth
        {
            get
            {
                if (isRotated == false) return itemData.width;

                return itemData.height;
            }
        }

        public int onGridPositionX;
        public int onGridPositionY;

        public bool isRotated;

        // Sets the data of the item scriptable object to the empty prefab.
        public void Set(ItemData itemData)
        {
            this.itemData = itemData;

            GetComponent<Image>().sprite = itemData.itemIcon;

            var size = new Vector2();
            size.x = itemData.width * ItemGrid.tileSizeWidth;
            size.y = itemData.height * ItemGrid.tileSizeHeight;
            GetComponent<RectTransform>().sizeDelta = size;
        }

        public void RotateItem()
        {
            isRotated = !isRotated;

            var itemRect = GetComponent<RectTransform>();

            // This checks if the isRotated bool is true.
            // If TRUE, the angle value will be 90. 
            // If FALSE, the angle value will be 0.
            itemRect.rotation = Quaternion.Euler(0, 0, isRotated ? 90f : 0);
        }
    }
}