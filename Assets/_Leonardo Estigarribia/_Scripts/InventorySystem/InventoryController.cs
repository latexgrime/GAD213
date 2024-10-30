using System.Collections.Generic;
using LeonardoEstigarribia.InventorySystem.inventoryHighlight;
using LeonardoEstigarribia.InventorySystem.inventoryItem;
using LeonardoEstigarribia.InventorySystem.itemData;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LeonardoEstigarribia.InventorySystem.inventoryController
{
    /// <summary>
    ///     Script in charge of letting the player control the inventory.
    /// </summary>
    public class InventoryController : MonoBehaviour
    {
        private InventoryHightlight inventoryHightlight;

        // Future proof, in case of use of multiple grids.
        private ItemGrid selectedGrid;

        public ItemGrid SelectedGrid
        {
            get => selectedGrid;
            set
            {
                selectedGrid = value;
                inventoryHightlight.SetParentGrid(value);
            }
        }


        private InventoryItem selectedItem; // Currently selected Item (the one in the position of the mouse).
        private InventoryItem overlapItem; // The target Item to be overlapped.

        private RectTransform selectedItemRect;

        [SerializeField] private List<ItemData> items; // DEBUG - To generate random items.

        [SerializeField] private GameObject
            itemPrefab; // An prefab that have empty containers for information to be filled by scriptable objects. 

        [SerializeField] private Transform canvasTransform;

        private void Awake()
        {
            inventoryHightlight = GetComponent<InventoryHightlight>();
        }

        private void Update()
        {
            // If there is a selected Item, drag it.
            DragItemIcon();

            // DEBUG - Spawn a random selected Item
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (selectedItem != null) return;
                CreateRandomItem();
            }

            // DEBUG - Pretend to grab a random item (i.e. the player walks over an item to put it in their inventory.)
            if (Input.GetKeyDown(KeyCode.T)) InsertRandomItem();

            if (Input.GetKeyDown(KeyCode.R)) RotateItem();

            // If there is no grid selected.
            if (selectedGrid == null)
            {
                // Hide the highlighter over items.
                inventoryHightlight.Show(false);
                return;
            }

            HandleHighlight();

            if (Input.GetMouseButtonDown(0)) LeftMouseButtonPressAction();
        }

        private void RotateItem()
        {
            if (selectedItem == null) return;

            selectedItem.RotateItem();
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
            var itemToInsert = selectedItem;
            selectedItem = null;
            InsertItem(itemToInsert);
        }

        private void InsertItem(InventoryItem itemToInsert)
        {
            if (selectedGrid == null)
            {
                Debug.LogError(
                    "No current selected grid. Select the grid by hovering your mouse over the inventory you want to insert the item to.");
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

            var selectedItemID = Random.Range(0, items.Count);
            inventoryItem.Set(items[selectedItemID]);
        }


        private Vector2Int oldPosition;

        private InventoryItem itemToHighlight;

        // Handles the apparition of the highlighter over items with the mouse on top of them.
        private void HandleHighlight()
        {
            var positionOnGrid = GetCurrentTileCoordinates();
            if (oldPosition == null) return;
            oldPosition = positionOnGrid;
            // Check if there is a selected item or not.
            if (selectedItem == null)
            {
                itemToHighlight = selectedGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

                // Highlight the Item that might be picked up by the player.
                if (itemToHighlight != null)
                {
                    inventoryHightlight.Show(true);
                    inventoryHightlight.SetHighlighterSize(itemToHighlight);

                    inventoryHightlight.SetHighlighterPositionSelection(selectedGrid, itemToHighlight);
                }
                else
                {
                    inventoryHightlight.Show(false);
                }
            }
            else
            {
                // This line uses the BoundaryCheck from the ItemGrid class to check if the highlighter is in the allowed area.
                inventoryHightlight.Show(selectedGrid.BoundaryCheck(positionOnGrid.x, positionOnGrid.y,
                    selectedItem.invItemWidth, selectedItem.invItemHeight));

                inventoryHightlight.SetHighlighterSize(selectedItem);

                inventoryHightlight.SetHighlighterPositionSelected(selectedGrid, selectedItem, positionOnGrid.x,
                    positionOnGrid.y);
            }
        }

        // Happens after clicking the mouse button.
        private void LeftMouseButtonPressAction()
        {
            var tileOnGridPosition = GetCurrentTileCoordinates();

            // If there is no object picked at the moment.
            if (selectedItem == null)
                // Pick up the object in the tile position the mouse is over.
                PickUp(tileOnGridPosition);
            // If there is a picked object.
            else
                // Place the object in the tile position the mouse is over.
                PlacePickedItem(tileOnGridPosition);
        }

        private Vector2Int GetCurrentTileCoordinates()
        {
            // Offset the item position based on the item size.
            Vector2 _mousePosition = Input.mousePosition;

            if (selectedItem != null)
            {
                _mousePosition.x -= (selectedItem.invItemWidth - 1) * ItemGrid.tileSizeWidth / 2;
                _mousePosition.y += (selectedItem.invItemHeight - 1) * ItemGrid.tileSizeHeight / 2;
            }

            // Gets the tile coordinates clicked by the cursor.
            return selectedGrid.GetTileCoordinatesFromGrid(_mousePosition);
        }

        // Places the item in the grid in the Vector2 parameter. 
        private void PlacePickedItem(Vector2Int tileGridPosition)
        {
            var itemWasPlaced =
                selectedGrid.PlaceItemOnGrid(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);

            // If the item was successfully placed, nullify the selected item.
            if (itemWasPlaced)
            {
                selectedItem = null;

                // If there is a target Item that the player is trying to overlap.
                if (overlapItem != null)
                {
                    // Set the overlap Item as the selected item and get its rect.
                    selectedItem = overlapItem;
                    overlapItem = null;
                    selectedItemRect = selectedItem.GetComponent<RectTransform>();
                }
            }
        }

        // Picks up the item from the position of the grid in the Vector2 parameter.
        private void PickUp(Vector2Int tileGridPosition)
        {
            // Gets the item position from the selected grid. 
            selectedItem = selectedGrid.GetItemToPickUp(tileGridPosition.x, tileGridPosition.y);
            if (selectedItem != null) selectedItemRect = selectedItem.GetComponent<RectTransform>();
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
    }
}