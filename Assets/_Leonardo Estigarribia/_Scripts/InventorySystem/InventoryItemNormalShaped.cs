#region Imported Namespaces

using LeonardoEstigarribia.InventorySystem.itemData.normalShaped;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace LeonardoEstigarribia.InventorySystem.inventoryItem
{
    /// <summary>
    ///     Script in charge of managing the items that can be stored in the inventory.
    /// </summary>
    public class InventoryItemNormalShaped : MonoBehaviour
    {
        public ItemData itemData;

        // Changes values depending if its rotated or not.
        public int invItemHeight
        {
            get // This applies logic "behind the scenes" every time this variable is accessed.
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

        // The position the item is occupying.
        public int onGridPositionX; 
        public int onGridPositionY;

        public bool isRotated;

        // Sets the data of the item scriptable object to the empty prefab.
        public void SetNormalShapedItemData(ItemData itemData)
        {
            // Set the reference of the passed itemData to be the itemData referenced in this script (which should be in the itemPrefabGameObject).
            this.itemData = itemData;

            // Set the image.
            GetComponent<Image>().sprite = itemData.itemIcon;

            // Set the size of the image by using the rect sizeDelta.
            var size = new Vector2();
            size.x = itemData.width * ItemGrid.tileSizeWidth;
            size.y = itemData.height * ItemGrid.tileSizeHeight;
            GetComponent<RectTransform>().sizeDelta = size;
        }

        public void RotateItemNormalShapedItem()
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