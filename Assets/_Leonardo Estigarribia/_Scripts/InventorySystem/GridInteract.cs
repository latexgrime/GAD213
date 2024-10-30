#region Imported Namespaces

using LeonardoEstigarribia.InventorySystem.inventoryController;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace LeonardoEstigarribia.InventorySystem.gridInteraction
{
    /// <summary>
    ///     Script in charge of making a grid able to interact (being able to receive player input).
    /// </summary>
    [RequireComponent(typeof(ItemGrid))]
    public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private InventoryController inventoryController; // Reference to the inventory controller.
        private ItemGrid itemGrid; // Reference to the inventory item grid.

        private void Awake()
        {
            inventoryController = FindObjectOfType<InventoryController>(); // OPTIMIZE THIS LATER ---
            itemGrid = GetComponent<ItemGrid>();
        }

        // Using interfaces to detect if the mouse is hovering over a grid and reference it or not.
        public void OnPointerEnter(PointerEventData eventData)
        {
            inventoryController.SelectedGrid = itemGrid;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            inventoryController.SelectedGrid = null;
        }
    }
}