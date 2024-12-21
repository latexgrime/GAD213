#region Imported Namespaces

using LeonardoEstigarribia.InventorySystem.itemData.complexShaped;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;
using Image = UnityEngine.UI.Image;

#endregion

namespace LeonardoEstigarribia.InventorySystem.inventoryItem
{
    /// <summary>
    ///     Script in charge of managing the items that can be stored in the inventory.
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        public ItemDataComplexShaped itemDataComplexShaped;
        
        public bool isRotated;
        public int complexHeight;
        public int complexWidth;
        public int onGridPositionX;
        public int onGridPositionY;

        // The shape of the individual item attached to this script.
        public bool[,] itemShape;


        public void SetComplexItem(ItemDataComplexShaped _itemDataComplexShaped)
        {
            itemDataComplexShaped = _itemDataComplexShaped;

            // Set the shape and set it to this specific object.
            itemDataComplexShaped.SetShape();
            itemShape = itemDataComplexShaped.shape;

            complexHeight = _itemDataComplexShaped.SetShape().GetLength(0);
            complexWidth = _itemDataComplexShaped.SetShape().GetLength(1);

            // Re-size the icon to match the amount of rows and columns the item takes.
            GetComponent<Image>().sprite = itemDataComplexShaped.itemIcon;
            var size = new Vector2(complexWidth * ItemGrid.tileSizeWidth, complexHeight * ItemGrid.tileSizeHeight);
            GetComponent<RectTransform>().sizeDelta = size;
        }

        // Updated to work with complex data shapes.
        public bool CanFitInGrid(ItemGrid selectedGrid, int mouseX, int mouseY)
        {
            for (var x = 0; x < complexWidth; x++)
            for (var y = 0; y < complexHeight; y++)
            {
                // Check that the bounds of the shape array are not being surpassed (prevent an out of Index error).
                if (x >= itemDataComplexShaped.shape.GetLength(0) || y >= itemDataComplexShaped.shape.GetLength(1))
                    // Skip this part if it is out of bounds (This is a workaround for items that return a false value for empty spaces).
                    continue;

                if (itemDataComplexShaped.shape[x, y])
                    if (mouseX + x >= selectedGrid.inventoryRowQuantity ||
                        mouseY + y >= selectedGrid.inventoryColumnQuantity ||
                        selectedGrid.IsOccupied(mouseX + x, mouseY + y))
                        return false;
            }

            // Otherwise everything is good.
            return true;
        }

        // Handle the rotation of the space the complex item is going to occupy.
        public void RotateComplexItem()
        {
            isRotated = !isRotated;

            RotateShapeArray();
            RotateItemIcon();
        }

        private int rotationState;

        private void RotateItemIcon()
        {
            // The item can only rotate 90 degrees (basically 4 rotations).
            rotationState = (rotationState + 1) % 4;

            var itemRect = GetComponent<RectTransform>();
            itemRect.rotation = Quaternion.Euler(0, 0, rotationState * 90f);
        }

        private void RotateShapeArray()
        {
            // This would be X.
            var rows = itemShape.GetLength(0);

            // This would be Y,
            var columns = itemShape.GetLength(1);
            var rotatedShape = new bool[columns, rows];

            // Rotate the array.
            for (var x = 0; x < columns; x++)
            for (var y = 0; y < rows; y++)
                // X and Y are saved in the rotatedShape array but inverted.
                // -1 because the array indexes start at 0.
                rotatedShape[x, y] = itemShape[rows - y - 1, x];

            // Update the shape 2D array to be the rotated one.
            itemShape = rotatedShape;
            // Transpose the dimensions.
            (complexWidth, complexHeight) = (complexHeight, complexWidth);
        }
    }
}