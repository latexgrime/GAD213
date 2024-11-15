#region Imported Namespaces

using LeonardoEstigarribia.InventorySystem.itemData.complexShaped;
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
    public class InventoryItem : MonoBehaviour
    {
        public ItemData itemData;

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

        #region Complex-Shaped Items

        public ItemDataComplexShaped itemDataComplexShaped;
        public int complexHeight;
        public int complexWidth;
        public int onGridPositionX;
        public int onGridPositionY;

        public bool[,] itemShape =>
            itemDataComplexShaped.shape; // 2D Array of bools that holds the shape of the object.


        public void SetComplexItem(ItemDataComplexShaped _itemDataComplexShaped)
        {
            itemDataComplexShaped = _itemDataComplexShaped;

            complexHeight = _itemDataComplexShaped.SetShape().GetLength(0);
            complexWidth = _itemDataComplexShaped.SetShape().GetLength(1);

            GetComponent<Image>().sprite = itemDataComplexShaped.itemIcon;
            var size = new Vector2(complexWidth * ItemGrid.tileSizeWidth, complexHeight * ItemGrid.tileSizeHeight);
            GetComponent<RectTransform>().sizeDelta = size;
        }

        // Updated to work with complex data shapes.
        public bool CanFitInGrid(ItemGrid selectedGrid, int mouseX, int mouseY)
        {
            for (int x = 0; x < complexWidth; x++)
            {
                for (int y = 0; y < complexHeight; y++)
                {
                    // Check that the bounds of the shape array are not being surpassed (prevent an out of Index error).
                    if (x >= itemDataComplexShaped.shape.GetLength(0) || y >= itemDataComplexShaped.shape.GetLength(1))
                    {
                        // Skip this part if it is out of bounds. (This is a workaround for L shaped items that return a false value for empty spaces)
                        continue;
                    }

                    if (itemDataComplexShaped.shape[x, y])
                    {
                        if (mouseX + x >= selectedGrid.inventoryRowQuantity ||
                            mouseY + y >= selectedGrid.inventoryColumnQuantity ||
                            selectedGrid.IsOccupied(mouseX + x, mouseY + y))
                        {
                            return false;
                        }
                    }
                }
            }

            // Otherwise everything is good.
            return true;
        }

        #endregion
    }
}