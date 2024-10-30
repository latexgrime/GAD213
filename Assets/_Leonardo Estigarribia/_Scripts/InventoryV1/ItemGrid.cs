using UnityEngine;

/// <summary>
/// Script in charge of displaying the inventory.
/// </summary>
public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 100;
    public const float tileSizeHeight = 100;
    
    private InventoryItem[,] inventoryItemSlot; // This represents a slot in the inventory.

    private RectTransform invRectTransform; // Rect transform of the InventoryGrid GameObject (the inventory itself).

    private Vector2 positionOnGrid = new Vector2();
    private Vector2Int tileGridPosition = new Vector2Int();

    /// <summary>
    /// Number of tiles in the horizontal plane of the grid.
    /// </summary>
    [SerializeField] private int inventoryRowQuantity;
    /// <summary>
    /// Number of tiles in the vertical plane of the grid.
    /// </summary>
    [SerializeField] private int inventoryColumnQuantity;

    [SerializeField] private GameObject inventoryItemPrefab;
    
    private void Start()
    {
        invRectTransform = GetComponent<RectTransform>();
        InitializeItemGrid(inventoryRowQuantity, inventoryColumnQuantity);
    }

    public InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }
    
    public InventoryItem GetItemToPickUp(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) return null; 
        
        CleanGridReference(toReturn);
        
        return toReturn;
    }

    // Clean the Item from the grid independently from their width and size.
    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.invItemWidth; ix++)
        {
            for (int iy = 0; iy < item.invItemHeight; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;

            }
        }
    }

    // Will set the ROWS (x numbers of tiles) and COLUMNS (y numbers of tiles) the grid will have by passing both integer parameters.
    private void InitializeItemGrid(int rowAmount, int columnAmount)
    {
        inventoryItemSlot = new InventoryItem[rowAmount, columnAmount];
        Vector2 size = new Vector2(rowAmount * tileSizeWidth, columnAmount * tileSizeHeight);
        invRectTransform.sizeDelta = size;
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

    // Places the item being dragged to the tile the mouse interacts (left click) with.
    public bool PlaceItemOnGrid(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        // Check the boundaries of the item.
        if (BoundaryCheck(posX, posY, inventoryItem.invItemWidth, inventoryItem.invItemHeight) == false)
        {
            return false;
        }

        // Check if its overlapping another item.
        if (OverlapCheck(posX, posY, inventoryItem.invItemWidth, inventoryItem.invItemHeight, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }
        
        HandleItemPlacing(inventoryItem, posX, posY);

        // If the item was managed to be placed.
        return true;
    }

    public void HandleItemPlacing(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform itemRectTransform = inventoryItem.GetComponent<RectTransform>();
        itemRectTransform.SetParent(invRectTransform); // Set the inventory grid as a parent of the item icon.

        // Sets the tiles data (depending on width and size) to be occupied by the item.
        for (int x = 0; x < inventoryItem.invItemWidth; x++)
        {
            for (int y = 0; y < inventoryItem.invItemHeight; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem; // Sets the space the item icon is occupying on the grid.
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        
        Vector2 itemIconPosition = CalculatePositionOnGrid(inventoryItem, posX, posY);

        itemRectTransform.localPosition = itemIconPosition;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        // Set the icon on its corresponding position in the grid.
        Vector2 itemIconPosition = new Vector2();
        // The division of the inventoryXQuantity by 2 is done to create an offset that will center the item in the middle due to the pivot.
        itemIconPosition.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.invItemWidth / 2; 
        itemIconPosition.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.invItemHeight / 2); // Negative because of the items' pivot.
        return itemIconPosition;
    }

    // Cycle through the grid to check if there is an Item in the passed item slot position.
    private bool OverlapCheck(int posX, int posY, int itemDataWidth, int itemDataHeight, ref InventoryItem overlapItem)
    {
        for (int ix = 0; ix < itemDataWidth; ix++)
        {
            for (int iy = 0; iy < itemDataHeight; iy++)
            {
                if (inventoryItemSlot[posX + ix, posY + iy] != null)
                {
                    if (overlapItem == null)
                        overlapItem = inventoryItemSlot[posX + ix, posY + iy];
                    
                    else if (overlapItem != inventoryItemSlot[posX + ix, posY + iy]) return false;
                }
            }
        }
        
        return true;
    }

    // Checks if the item is inside of the grid.
    bool PositionCheck(int posX, int posY)
    {
        // Means its outside of the boundary left side and/or up.
        if (posX < 0 || posY < 0)
        {
            return false;
        }
        // Means its outside of the boundary on the right and/or bottom side.
        if (posX >= inventoryRowQuantity || posY >= inventoryColumnQuantity)
        {
            return false;
        }

        return true;
    }

    // Pass the position and the size 
    public bool BoundaryCheck(int posX, int posY, int objWidth, int objHeight)
    {
        // Checks the top and left position of the item.
        if(PositionCheck(posX, posY) == false) return false;

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
        int height = inventoryColumnQuantity - itemToInsert.invItemHeight + 1;
        int width = inventoryRowQuantity - itemToInsert.invItemWidth + 1;
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckSpaceAvailability(x, y, itemToInsert.invItemWidth, itemToInsert.invItemHeight))
                {
                    return new Vector2Int (x, y);
                }
            }
        }
        return null;
    }
    
    private bool CheckSpaceAvailability(int posX, int posY, int itemDataWidth, int itemDataHeight)
    {
        for (int x = 0; x < itemDataWidth; x++)
        {
            for (int y = 0; y < itemDataHeight; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {
                    return false;
                }
            }
        }
        return true; 
    }
}