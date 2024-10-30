#region Imported Namespaces

using System.Collections.Generic;
using LeonardoEstigarribia.InventorySystem.inventoryHighlight;
using LeonardoEstigarribia.InventorySystem.inventoryItem;
using LeonardoEstigarribia.InventorySystem.itemData.normalShaped;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

#endregion

namespace LeonardoEstigarribia.InventorySystem.inventoryController
{
    /// <summary>
    ///     Script in charge of letting the player control the inventory.
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        private InventoryHightlight inventoryHighlight;

        // A GRID is a container of items (i.e. the player inventory or a chest with items).
        // Currently selected grid container.
        private ItemGrid selectedGrid;

        public ItemGrid SelectedGrid
        {
            get => selectedGrid;
            // This makes it so every time the selected grid is called, the inventoryHighlight SetParentGrid function happens
            // with the passed value being the selectedGrid.
            set
            {
                selectedGrid = value;
                inventoryHighlight.SetParentGrid(value);
            }
        }
        
        private InventoryItemNormalShaped selectedItemNormalShaped; // Currently selected Item (the one in the position of the mouse).
        private InventoryItemNormalShaped overlapItemNormalShaped; // The target Item to be overlapped.

        private RectTransform selectedItemRect;

        [SerializeField] private List<ItemData> existingItemsInProject; // DEBUG - To generate random items.

        [SerializeField] private GameObject itemPrefab; // An prefab that have empty containers for information to be filled by scriptable objects. 

        [SerializeField] private Transform canvasTransform; // The transform of the pixelScaled canvas that displays the grids.

        private void Awake()
        {
            inventoryHighlight = GetComponent<InventoryHightlight>();
        }

        private void Update()
        {
            // If there is a selected Item, drag it.
            DragItemIcon();

            #region Debug Inputs

            // DEBUG - Spawn a random selected Item
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (selectedItemNormalShaped != null) return;
                CreateRandomItem();
            }

            // DEBUG - Pretend to grab a random item (i.e. the player walks over an item to put it in their inventory.)
            if (Input.GetKeyDown(KeyCode.T)) InsertRandomItem();

            #endregion

            if (Input.GetKeyDown(KeyCode.R)) RotateItem(); // In the future, make this work with the InputManager.

            // If there is no grid selected.
            if (selectedGrid == null)
            {
                // Hide the highlighter over items.
                inventoryHighlight.Show(false);
                return;
            }

            HandleHighlight();

            if (Input.GetMouseButtonDown(0)) LeftMouseButtonPressAction();
        }
        
        // Sets the current selected item to match the position of the mouse. 
        private void DragItemIcon()
        {
            if (selectedItemNormalShaped != null)
            {
                selectedItemRect.position = Input.mousePosition;
                selectedItemRect.SetParent(canvasTransform);
            }
        }
        
        // Handles the apparition of the highlighter over items with the mouse on top of them.
        private void HandleHighlight()
        {
            var positionOnGrid = GetCurrentTileCoordinates();
            if (oldPosition == null) return;
            oldPosition = positionOnGrid;
            // Check if there is a selected item or not.
            if (selectedItemNormalShaped == null)
            {
                itemNormalShapedToHighlight = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

                // Highlight the Item that might be picked up by the player.
                if (itemNormalShapedToHighlight != null)
                {
                    inventoryHighlight.Show(true);
                    inventoryHighlight.SetHighlighterSize(itemNormalShapedToHighlight);

                    inventoryHighlight.SetHighlighterPositionSelection(selectedGrid, itemNormalShapedToHighlight);
                }
                else
                {
                    inventoryHighlight.Show(false);
                }
            }
            else
            {
                // This line uses the BoundaryCheck from the ItemGrid class to check if the highlighter is in the allowed area.
                inventoryHighlight.Show(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y,
                    selectedItemNormalShaped.invItemWidth, selectedItemNormalShaped.invItemHeight));

                inventoryHighlight.SetHighlighterSize(selectedItemNormalShaped);

                inventoryHighlight.SetHighlighterPositionSelected(selectedGrid, selectedItemNormalShaped, positionOnGrid.x,
                    positionOnGrid.y);
            }
        }

        // Happens after clicking the mouse button.
        private void LeftMouseButtonPressAction()
        {
            Vector2Int mouseCoordinatesOnGrid = GetCurrentTileCoordinates();

            // If there is no object picked at the moment.
            if (selectedItemNormalShaped == null)
                // Pick up the object in the tile position the mouse is over.
                PickUp(mouseCoordinatesOnGrid);
           
            // If there is a picked object.
            else
                // Place the object in the tile position the mouse is over.
                PlacePickedItem(mouseCoordinatesOnGrid);
        }
        
        private Vector2Int GetCurrentTileCoordinates()
        {
            // Offset the item position based on the item size.
            Vector2 _mousePosition = Input.mousePosition;

            if (selectedItemNormalShaped != null)
            {
                _mousePosition.x -= (selectedItemNormalShaped.invItemWidth - 1) * ItemGrid.tileSizeWidth / 2;
                _mousePosition.y += (selectedItemNormalShaped.invItemHeight - 1) * ItemGrid.tileSizeHeight / 2;
            }

            // Gets the tile coordinates clicked by the cursor.
            return selectedGrid.GetTileCoordinatesFromGrid(_mousePosition);
        }
        
        // Picks up the item from the position of the grid in the Vector2 parameter.
        private void PickUp(Vector2Int _mouseCoordinatesOnGrid)
        {
            // Checks if there is an item occupying that position and sets it as the selectedItem.
            selectedItemNormalShaped = selectedGrid.GetItemToPickUp(_mouseCoordinatesOnGrid.x, _mouseCoordinatesOnGrid.y);
            
            // If there IS an item in those coordinates, sets the rect of the selectedItem to the selectedItemRect rect.
            // Note: This will make use of the DragItem method. Making it so the selectedItemRect is on the position of the mouse. 
            if (selectedItemNormalShaped != null) selectedItemRect = selectedItemNormalShaped.GetComponent<RectTransform>();
        }
        
        private void RotateItem()
        {
            if (selectedItemNormalShaped == null) return;

            selectedItemNormalShaped.RotateItemNormalShapedItem();
        }

        private void InsertRandomItem()
        {
            if (selectedGrid == null)
            {
                Debug.LogError(
                    "No current selected grid. Select the grid by hovering your mouse over the inventory you want to insert the item to.");
                return;
            }

            CreateRandomItem();
            var itemToInsert = selectedItemNormalShaped;
            selectedItemNormalShaped = null;
            InsertItem(itemToInsert);
        }

        private void InsertItem(InventoryItemNormalShaped itemNormalShapedToInsert)
        {
            if (selectedGrid == null)
            {
                Debug.LogError(
                    "No current selected grid. Select the grid by hovering your mouse over the inventory you want to insert the item to.");
                return;
            }

            var posOnGrid = selectedGrid.FindSpaceForObject(itemNormalShapedToInsert);

            if (posOnGrid == null) return;

            selectedGrid.HandleItemPlacing(itemNormalShapedToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }


        // DEBUGGING - Creates random item and selects it.
        private void CreateRandomItem()
        {
            var inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItemNormalShaped>();
            selectedItemNormalShaped = inventoryItem;

            selectedItemRect = inventoryItem.GetComponent<RectTransform>();
            selectedItemRect.SetParent(canvasTransform);

            var selectedItemID = Random.Range(0, existingItemsInProject.Count);
            inventoryItem.SetNormalShapedItemData(existingItemsInProject[selectedItemID]);
        }


        private Vector2Int oldPosition;

        private InventoryItemNormalShaped itemNormalShapedToHighlight;

        // Places the item in the grid in the Vector2 parameter. 
        private void PlacePickedItem(Vector2Int _mouseCoordinatesOnGrid)
        {
            bool itemWasPlaced =
                selectedGrid.PlaceItemOnGrid(selectedItemNormalShaped, _mouseCoordinatesOnGrid.x, _mouseCoordinatesOnGrid.y, ref overlapItemNormalShaped);

            // If the item was successfully placed, nullify the selected item.
            if (itemWasPlaced)
            {
                selectedItemNormalShaped = null;

                // If there is a target Item that the player is trying to overlap.
                if (overlapItemNormalShaped != null)
                {
                    // Set the overlap Item as the selected item and get its rect.
                    selectedItemNormalShaped = overlapItemNormalShaped;
                    overlapItemNormalShaped = null;
                    selectedItemRect = selectedItemNormalShaped.GetComponent<RectTransform>();
                }
            }
        }
        

        
        

    }
}