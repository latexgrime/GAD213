#region Imported Namespaces

using LeonardoEstigarribia.InventorySystem.inventoryItem;
using UnityEngine;

#endregion

namespace LeonardoEstigarribia.InventorySystem.itemGrid
{
    /// <summary>
    ///     Script in charge of displaying the inventory.
    /// </summary>
    public class ItemGrid : MonoBehaviour
    {
        public const float tileSizeWidth = 100;
        public const float tileSizeHeight = 100;

        private InventoryItemNormalShaped[,] inventoryItemSlot; // This represents a slot in the inventory.

        private RectTransform
            invRectTransform; // Rect transform of the InventoryGrid GameObject (the inventory itself).

        private Vector2 positionOnGrid;
        private Vector2Int tileGridPosition;

        /// <summary>
        ///     Number of tiles in the horizontal plane of the grid.
        /// </summary>
        [SerializeField] private int inventoryRowQuantity;

        /// <summary>
        ///     Number of tiles in the vertical plane of the grid.
        /// </summary>
        [SerializeField] private int inventoryColumnQuantity;

        [SerializeField] private GameObject inventoryItemPrefab;

        private void Start()
        {
            invRectTransform = GetComponent<RectTransform>();
            InitializeItemGrid(inventoryRowQuantity, inventoryColumnQuantity);
        }

        public InventoryItemNormalShaped GetItem(int x, int y)
        {
            return inventoryItemSlot[x, y];
        }

        // Checks if there is an item in the passed coordinates.
        public InventoryItemNormalShaped GetItemToPickUp(int x, int y)
        {
            InventoryItemNormalShaped toReturn = inventoryItemSlot[x, y];

            if (toReturn == null) return null;

            CleanGridReference(toReturn);

            return toReturn;
        }

        // Gets the coordinates of the row and tile the pointer sits in.
        public Vector2Int GetTileCoordinatesFromGrid(Vector2 mousePosition)
        {
            // Draws a vector from the pivot of the inventory rect to the position of the pointer on the screen.
            positionOnGrid.x = mousePosition.x - invRectTransform.position.x;
            positionOnGrid.y = invRectTransform.position.y - mousePosition.y;

            // Get the coordinates by dividing the sizeDelta of the tile size by the vector to get integer coordinates.
            tileGridPosition.x = (int)(positionOnGrid.x / tileSizeWidth);
            tileGridPosition.y = (int)(positionOnGrid.y / tileSizeHeight);

            return tileGridPosition;
        }
        
        // Clean the Item from the grid independently from their width and size.
        private void CleanGridReference(InventoryItemNormalShaped itemNormalShaped)
        {
            for (var ix = 0; ix < itemNormalShaped.invItemWidth; ix++)
            for (var iy = 0; iy < itemNormalShaped.invItemHeight; iy++)
                inventoryItemSlot[itemNormalShaped.onGridPositionX + ix, itemNormalShaped.onGridPositionY + iy] = null;
        }

        // Will set the ROWS (x numbers of tiles) and COLUMNS (y numbers of tiles) the grid will have by passing both integer parameters.
        private void InitializeItemGrid(int rowAmount, int columnAmount)
        {
            inventoryItemSlot = new InventoryItemNormalShaped[rowAmount, columnAmount];
            var size = new Vector2(rowAmount * tileSizeWidth, columnAmount * tileSizeHeight);
            invRectTransform.sizeDelta = size;
        }
        

        // Places the item being dragged to the tile the mouse interacts (left click) with.
        public bool PlaceItemOnGrid(InventoryItemNormalShaped inventoryItemNormalShaped, int posX, int posY, ref InventoryItemNormalShaped overlapItemNormalShaped)
        {
            // Check the boundaries of the item.
            if (BoundaryCheck(posX, posY, inventoryItemNormalShaped.invItemWidth, inventoryItemNormalShaped.invItemHeight) == false)
                return false;

            // Check if its overlapping another item.
            if (OverlapCheck(posX, posY, inventoryItemNormalShaped.invItemWidth, inventoryItemNormalShaped.invItemHeight, ref overlapItemNormalShaped) ==
                false)
            {
                overlapItemNormalShaped = null;
                return false;
            }

            if (overlapItemNormalShaped != null) CleanGridReference(overlapItemNormalShaped);

            HandleItemPlacing(inventoryItemNormalShaped, posX, posY);

            // If the item was managed to be placed.
            return true;
        }

        // posX and posY should be the coordinates of the tile that was selected by the mouse in the grid.
        public void HandleItemPlacing(InventoryItemNormalShaped _selectedItemNormalShaped, int _selectedMouseOnTileCoordinateX, int _selectedMouseOnTileCoordinateY)
        {
            // Gets the rect of the selectedItem.
            RectTransform itemRectTransform = _selectedItemNormalShaped.GetComponent<RectTransform>();
            // Set the inventory grid as a parent of the item icon.
            itemRectTransform.SetParent(invRectTransform); 

            // Sets the tiles data (depending on width and size) to be occupied by the item.
            for (var x = 0; x < _selectedItemNormalShaped.invItemWidth; x++)
            for (var y = 0; y < _selectedItemNormalShaped.invItemHeight; y++)
                inventoryItemSlot[_selectedMouseOnTileCoordinateX + x, _selectedMouseOnTileCoordinateY + y] =
                    _selectedItemNormalShaped; 

            // Sets the "pivot space" from which it calculates the space its being occupied. 
            _selectedItemNormalShaped.onGridPositionX = _selectedMouseOnTileCoordinateX;
            _selectedItemNormalShaped.onGridPositionY = _selectedMouseOnTileCoordinateY;

            var itemIconPosition = CalculateIconPositionOnGrid(_selectedItemNormalShaped, _selectedMouseOnTileCoordinateX, _selectedMouseOnTileCoordinateY);

            itemRectTransform.localPosition = itemIconPosition;
        }

        public Vector2 CalculateIconPositionOnGrid(InventoryItemNormalShaped inventoryItemNormalShaped, int _selectedMouseOnTileCoordinateX, int _selectedMouseOnTileCoordinateY)
        {
            var itemIconPosition = new Vector2();
            // Set the icon on its corresponding position in the grid.
            // The division of the inventoryXQuantity by 2 is done to create an offset that will center the item in the middle due to the pivot.
            itemIconPosition.x = _selectedMouseOnTileCoordinateX * tileSizeWidth + tileSizeWidth * inventoryItemNormalShaped.invItemWidth / 2;
            itemIconPosition.y =
                -(_selectedMouseOnTileCoordinateY * tileSizeHeight +
                  tileSizeHeight * inventoryItemNormalShaped.invItemHeight / 2); // Negative because of the items' pivot.
            return itemIconPosition;
        }

        // Cycle through the grid to check if there is an Item in the passed item slot position.
        private bool OverlapCheck(int posX, int posY, int itemDataWidth, int itemDataHeight,
            ref InventoryItemNormalShaped overlapItemNormalShaped)
        {
            for (var ix = 0; ix < itemDataWidth; ix++)
            for (var iy = 0; iy < itemDataHeight; iy++)
                if (inventoryItemSlot[posX + ix, posY + iy] != null)
                {
                    if (overlapItemNormalShaped == null)
                        overlapItemNormalShaped = inventoryItemSlot[posX + ix, posY + iy];

                    else if (overlapItemNormalShaped != inventoryItemSlot[posX + ix, posY + iy]) return false;
                }

            return true;
        }

        // Checks if the item is inside of the grid.
        private bool PositionCheck(int posX, int posY)
        {
            // Means its outside of the boundary left side and/or up.
            if (posX < 0 || posY < 0) return false;
            // Means its outside of the boundary on the right and/or bottom side.
            if (posX >= inventoryRowQuantity || posY >= inventoryColumnQuantity) return false;

            return true;
        }

        // Pass the position and the size 
        public bool BoundaryCheck(int posX, int posY, int objWidth, int objHeight)
        {
            // Checks the top and left position of the item.
            if (PositionCheck(posX, posY) == false) return false;

            // Checks the bottom right position of the item.
            posX += objWidth - 1;
            posY += objHeight - 1;
            if (PositionCheck(posX, posY) == false) return false;

            return true;
        }

        // Find space for an object in the grid.
        // Note: The '?' next to the struct turns it "nullable".
        public Vector2Int? FindSpaceForObject(InventoryItemNormalShaped itemNormalShapedToInsert)
        {
            var height = inventoryColumnQuantity - itemNormalShapedToInsert.invItemHeight + 1;
            var width = inventoryRowQuantity - itemNormalShapedToInsert.invItemWidth + 1;

            for (var y = 0; y < height; y++)
            for (var x = 0; x < width; x++)
                if (CheckSpaceAvailability(x, y, itemNormalShapedToInsert.invItemWidth, itemNormalShapedToInsert.invItemHeight))
                    return new Vector2Int(x, y);
            return null;
        }

        private bool CheckSpaceAvailability(int posX, int posY, int itemDataWidth, int itemDataHeight)
        {
            for (var x = 0; x < itemDataWidth; x++)
            for (var y = 0; y < itemDataHeight; y++)
                if (inventoryItemSlot[posX + x, posY + y] != null)
                    return false;
            return true;
        }
    }
}