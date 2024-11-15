#region Imported Namespaces

using System;
using System.Collections.Generic;
using System.Numerics;
using LeonardoEstigarribia.InventorySystem.inventoryItem;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

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

        private InventoryItem[,] inventoryItemSlot; // This represents a slot in the inventory.

        private RectTransform
            invRectTransform; // Rect transform of the InventoryGrid GameObject (the inventory itself).

        private Vector2 positionOnGrid;
        private Vector2Int tileGridPosition;

        /// <summary>
        ///     Number of tiles in the horizontal plane of the grid.
        /// </summary>
        [SerializeField] public int inventoryRowQuantity;

        /// <summary>
        ///     Number of tiles in the vertica`l plane of the grid.
        /// </summary>
        [SerializeField] public int inventoryColumnQuantity;

        [SerializeField] private GameObject inventoryItemPrefab;

        private void Start()
        {
            invRectTransform = GetComponent<RectTransform>();
            InitializeItemGrid(inventoryRowQuantity, inventoryColumnQuantity);
        }

        public InventoryItem GetItemFromPosition(int x, int y)
        {
            return inventoryItemSlot[x, y];
        }

        // Checks if there is an item in the passed coordinates and cleans it.
        public InventoryItem PickUpAndCleanItem(int posX, int posY)
        {
            Vector2Int position = new Vector2Int(posX, posY);
            if (inventoryItems.TryGetValue(position, out InventoryItem item))
            {
                CleanGridReference(item);
                return item;
            }
            // No items found at this coordinates.
            return null;
        }
        
        // Clean the Item from the grid independently from their width and size.
        private void CleanGridReference(InventoryItem item)
        {
            // Get the coordinates occupied by the item.
            List<Vector2Int> occupiedPositions =
                GetItemCoordinates(item.onGridPositionX, item.onGridPositionY, item.itemShape);
            foreach (var position in occupiedPositions)
            {
                inventoryItems.Remove(position);
            }
        }
        
        // Checks if there is an item in the passed coordinates.
        public InventoryItem CheckCoordinateForItem(int posX, int posY)
        {
            Vector2Int position = new Vector2Int(posX, posY);
            if (inventoryItems.TryGetValue(position, out InventoryItem item))
            {
                return item;
            }
            // No items found at this coordinates.
            return null;
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

        // Will set the ROWS (x numbers of tiles) and COLUMNS (y numbers of tiles) the grid will have by passing both integer parameters.
        private void InitializeItemGrid(int rowAmount, int columnAmount)
        {
            inventoryItemSlot = new InventoryItem[rowAmount, columnAmount];
            var size = new Vector2(rowAmount * tileSizeWidth, columnAmount * tileSizeHeight);
            invRectTransform.sizeDelta = size;
        }
        

        // Places the item being dragged to the tile the mouse interacts (left click) with.
        public bool PlaceItemOnGrid(InventoryItem item, int posX, int posY)
        {
            // Check the boundaries of the item.
            if (!CanPlaceItem(item, posX, posY))
            {
                return false;
            }
            
            // Set the corresponding tiles as the ones occupied by the item also depending on shape.
            List<Vector2Int> occupiedPositions = GetItemCoordinates(posX, posY, item.itemShape);
            foreach (Vector2Int position in occupiedPositions)
            {
                inventoryItems[position] = item;
            }
            
            // Give item its position.
            item.onGridPositionX = posX;
            item.onGridPositionY = posY;
            return true; // Item placed successfully
       }

        // posX and posY should be the coordinates of the tile that was selected by the mouse in the grid.
        public void HandleItemPlacing(InventoryItem selectedItem, int _selectedMouseOnTileCoordinateX, int _selectedMouseOnTileCoordinateY)
        {
            // Gets the rect of the selectedItem.
            RectTransform itemRectTransform = selectedItem.GetComponent<RectTransform>();
            // Set the inventory grid as a parent of the item icon.
            itemRectTransform.SetParent(invRectTransform); 

            // Sets the tiles data (depending on width and size) to be occupied by the item.
            for (int x = 0; x < selectedItem.complexWidth; x++)
            for (int y = 0; y < selectedItem.complexHeight; y++)
                // Fix this.
                inventoryItemSlot[_selectedMouseOnTileCoordinateX + x, _selectedMouseOnTileCoordinateY + y] =
                    selectedItem; 

            // Sets the "pivot space" from which it calculates the space its being occupied. 
            selectedItem.onGridPositionX = _selectedMouseOnTileCoordinateX;
            selectedItem.onGridPositionY = _selectedMouseOnTileCoordinateY;

            var itemIconPosition = CalculateIconPositionOnGrid(selectedItem, _selectedMouseOnTileCoordinateX, _selectedMouseOnTileCoordinateY);

            itemRectTransform.localPosition = itemIconPosition;
        }

        public Vector2 CalculateIconPositionOnGrid(InventoryItem inventoryItem, int _selectedMouseOnTileCoordinateX, int _selectedMouseOnTileCoordinateY)
        {
            var itemIconPosition = new Vector2();
            // Set the icon on its corresponding position in the grid.
            // The division of the inventoryXQuantity by 2 is done to create an offset that will center the item in the middle due to the pivot.
            itemIconPosition.x = _selectedMouseOnTileCoordinateX * tileSizeWidth + tileSizeWidth * inventoryItem.complexWidth / 2;
            itemIconPosition.y = -(_selectedMouseOnTileCoordinateY * tileSizeHeight + tileSizeHeight * inventoryItem.complexHeight / 2); // Negative because of the items' pivot.
            return itemIconPosition;
        }

        // Cycle through the grid to check if there is an Item in the passed item slot position.
        private bool OverlapCheck(int posX, int posY, int itemDataWidth, int itemDataHeight,
            ref InventoryItem overlapItem)
        {
            for (int ix = 0; ix < itemDataWidth; ix++)
            for (int iy = 0; iy < itemDataHeight; iy++)
                if (inventoryItemSlot[posX + ix, posY + iy] != null)
                {
                    if (overlapItem == null)
                        overlapItem = inventoryItemSlot[posX + ix, posY + iy];

                    else if (overlapItem != inventoryItemSlot[posX + ix, posY + iy]) return false;
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
        public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
        {
            int height = inventoryColumnQuantity - itemToInsert.complexWidth + 1;
            int width = inventoryRowQuantity - itemToInsert.complexHeight + 1;

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                if (CheckSpaceAvailability(x, y, itemToInsert.complexWidth, itemToInsert.complexHeight))
                    return new Vector2Int(x, y);
            return null;
        }

        private bool CheckSpaceAvailability(int posX, int posY, int itemDataWidth, int itemDataHeight)
        {
            for (int x = 0; x < itemDataWidth; x++)
            for (int y = 0; y < itemDataHeight; y++)
                if (inventoryItemSlot[posX + x, posY + y] != null)
                    return false;
            return true;
        }

        #region Complex Shaped
        
        public bool[,] complexGrid;

        Dictionary<Vector2Int, InventoryItem> inventoryItems =
            new Dictionary<Vector2Int, InventoryItem>();
        
        private void Awake()
        {
            InitializeGrid(inventoryRowQuantity, inventoryColumnQuantity);
        }
        
        // Initializes the grid.
        private void InitializeGrid(int width, int height)
        {
            complexGrid = new bool[width, height];
        }

        public bool IsOccupied(int x, int y)
        {
            return complexGrid[x, y];
        }

        private bool CanPlaceItem(InventoryItem _item, int posX, int posY)
        {
            List<Vector2Int> occupiedPositions = GetItemCoordinates(posX, posY, _item.itemShape);
            foreach (var position in occupiedPositions)
            {
                if (position.x >= inventoryRowQuantity || position.y >= inventoryColumnQuantity ||
                    inventoryItems.ContainsKey(position))
                {
                    return false;
                }
            }
            return true;
        }
        
        // Get the coordinates that are occupied by an item.
        public List<Vector2Int> GetItemCoordinates(int startX, int startY, bool [,] shape)
        {
            List<Vector2Int> occupiedCoordinates = new List<Vector2Int>();
            
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    if (shape[y, x])
                    {
                        occupiedCoordinates.Add(new Vector2Int(startX + x, startY + y));
                    }
                }
            }

            return occupiedCoordinates;
        }
        
        public Vector2 CalculateHighlightPosition(int coordinateX, int coordinateY)
        {
            Vector2 highlightPosition = new Vector2();
            highlightPosition.x = (coordinateX) * tileSizeWidth + tileSizeWidth / 2;
            highlightPosition.y = -((coordinateY) * tileSizeHeight + tileSizeHeight / 2);
            return highlightPosition;
        }
        
        #endregion
        
    }
}