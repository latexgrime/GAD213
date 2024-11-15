#region Imported Namespaces

using System.Collections.Generic;
using System.Numerics;
using LeonardoEstigarribia.InventorySystem.inventoryHighlight;
using LeonardoEstigarribia.InventorySystem.inventoryItem;
using LeonardoEstigarribia.InventorySystem.itemData.complexShaped;
using LeonardoEstigarribia.InventorySystem.itemData.normalShaped;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

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
        public ItemGrid selectedGrid;

        /*public ItemGrid SelectedGrid
        {
            get => selectedGrid;
            // This makes it so every time the selected grid is called, the inventoryHighlight SetParentGrid function happens
            // with the passed value being the selectedGrid.
            set
            {
                selectedGrid = value;
                inventoryHighlight.SetParentGrid(value);
            }
        }*/
        
        private InventoryItem selectedItem; // Currently selected Item (the one in the position of the mouse).
        private InventoryItem overlapItem; // The target Item to be overlapped.

        private RectTransform selectedItemRect;

        [SerializeField] private List<ItemData> existingItemsInProject; // DEBUG - To generate random items.
        [SerializeField] private List<ItemDataComplexShaped> existingComplexItemsInProject; // DEBUG - To generate random items.

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
                if (selectedItem != null) return;
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
            if (selectedItem != null)
            {
                selectedItemRect.position = Input.mousePosition;
                selectedItemRect.SetParent(canvasTransform);
            }
        }
        
        
        private InventoryItem itemToHighlight;
        // Handles the apparition of the highlighter over items with the mouse on top of them.
        private void HandleHighlight()
        {
            // Check for the grid first.
            if (selectedGrid == null) return;
            
            // Get the coordinates the mouse is hovering over.
            Vector2Int positionOnGrid = GetCurrentTileCoordinates();
            
            // If there is no item selected by the player (being dragged), look for an item below the mouse.
            if (selectedItem == null)
            {
                itemToHighlight = selectedGrid.CheckCoordinateForItem(positionOnGrid.x, positionOnGrid.y);
                // Makes sure its not instantiating highlights every frame.
                if (inventoryHighlight.ChangedCursorPosition(itemToHighlight, selectedGrid))
                {
                    // If there is an item under the cursor show the highlights.
                    if (itemToHighlight != null)
                    {
                        inventoryHighlight.Show(true);
                        inventoryHighlight.ShowHighlightsItemTiles(selectedGrid, itemToHighlight);
                    }
                    else
                    {
                        inventoryHighlight.Show(false);
                        inventoryHighlight.ClearActiveHighlights();
                    }
                }
            }
            if (selectedItem != null)
            {
                inventoryHighlight.Show(false);
                inventoryHighlight.ClearActiveHighlights();
            }
            
        }


        // Happens after clicking the mouse button.
        private void LeftMouseButtonPressAction()
        {
            Vector2Int mouseCoordinatesOnGrid = GetCurrentTileCoordinates();

            // If there is no object picked at the moment.
            if (selectedItem == null)
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

            if (selectedItem != null)
            {
                _mousePosition.x -= (selectedItem.complexWidth - 1) * ItemGrid.tileSizeWidth / 2;
                _mousePosition.y += (selectedItem.complexHeight - 1) * ItemGrid.tileSizeHeight / 2;
            }

            // Gets the tile coordinates clicked by the cursor.
            return selectedGrid.GetTileCoordinatesFromGrid(_mousePosition);
        }
        
        // Picks up the item from the position of the grid in the Vector2 parameter.
        private void PickUp(Vector2Int _mouseCoordinatesOnGrid)
        {
            // Checks if there is an item occupying that position and sets it as the selectedItem.
            selectedItem = selectedGrid.PickUpAndCleanItem(_mouseCoordinatesOnGrid.x, _mouseCoordinatesOnGrid.y);
            
            // If there IS an item in those coordinates, sets the rect of the selectedItem to the selectedItemRect rect.
            // Note: This will make use of the DragItem method. Making it so the selectedItemRect is on the position of the mouse. 
            if (selectedItem != null) selectedItemRect = selectedItem.GetComponent<RectTransform>();
        }
        
        // Places the item in the grid in the Vector2 parameter. 
        private void PlacePickedItem(Vector2Int _mouseCoordinatesOnGrid)
        {
            bool itemWasPlaced =
                selectedGrid.PlaceItemOnGrid(selectedItem, _mouseCoordinatesOnGrid.x, _mouseCoordinatesOnGrid.y) && selectedItem.CanFitInGrid(selectedGrid ,_mouseCoordinatesOnGrid.x, _mouseCoordinatesOnGrid.y);
            // If the item was successfully placed, nullify the selected item.
            if (itemWasPlaced)
            {
               
                selectedItem = null;
                
                // If there is a target Item that the player is trying to overlap. REMOVE OR UPDATE (THIS WAS PART OF THE OLD SYSTEM)
                if (overlapItem != null)
                {
                    // Set the overlap Item as the selected item and get its rect.
                    selectedItem = overlapItem;
                    overlapItem = null;
                }
            }
        }

        private void RotateItem()
        {
            if (selectedItem == null) return;

            selectedItem.RotateItemNormalShapedItem();
        }

        private void InsertRandomItem()
        {
            if (selectedGrid == null)
            {
                return;
            }

            CreateRandomItem();
            var itemToInsert = selectedItem;
            selectedItem = null;
            InsertItem(itemToInsert);
        }

        private void InsertItem(InventoryItem itemToInsert)
        {
            if (selectedGrid == null)
            {
                return;
            }

            var posOnGrid = selectedGrid.FindSpaceForObject(itemToInsert);

            if (posOnGrid == null) return;

            selectedGrid.HandleItemPlacing(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }


        // DEBUGGING - Creates random item and selects it.
        private void CreateRandomItem()
        {
            var inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            selectedItem = inventoryItem;

            selectedItemRect = inventoryItem.GetComponent<RectTransform>();
            selectedItemRect.SetParent(canvasTransform);

            var selectedItemID = Random.Range(0, existingComplexItemsInProject.Count);
            inventoryItem.SetComplexItem(existingComplexItemsInProject[selectedItemID]);
        }




        
        

    }
}