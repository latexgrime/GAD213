#region Imported Namespaces

using LeonardoEstigarribia.InventorySystem.inventoryItem;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using UnityEngine;

#endregion

namespace LeonardoEstigarribia.InventorySystem.inventoryHighlight
{
    /// <summary>
    ///     In charge of highlighting items in a grid container.
    /// </summary>
    public class InventoryHightlight : MonoBehaviour
    {
        [SerializeField] private RectTransform itemHighlighter;

        public void Show(bool show)
        {
            itemHighlighter.gameObject.SetActive(show);
        }

        public void SetHighlighterSize(InventoryItemNormalShaped targetItemNormalShaped)
        {
            var size = new Vector2();
            size.x = targetItemNormalShaped.invItemWidth * ItemGrid.tileSizeWidth;
            size.y = targetItemNormalShaped.invItemHeight * ItemGrid.tileSizeHeight;
            itemHighlighter.sizeDelta = size;
        }

        /// <summary>
        ///     Sets the highlighter position when the player is about to make a selection in the grid.
        /// </summary>
        /// <param name="targetGrid"></param>
        /// <param name="targetItemNormalShaped"></param>
        public void SetHighlighterPositionSelection(ItemGrid targetGrid, InventoryItemNormalShaped targetItemNormalShaped)
        {
            var pos =
                targetGrid.CalculateIconPositionOnGrid(targetItemNormalShaped, targetItemNormalShaped.onGridPositionX, targetItemNormalShaped.onGridPositionY);

            // Change the local position because now this is a child of the targetGrid.
            itemHighlighter.localPosition = pos;
        }

        /// <summary>
        ///     Sets the highlighter position when the player already selected an item in the grid.
        /// </summary>
        /// <param name="targetGrid"></param>
        /// <param name="targetItemNormalShaped"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        public void SetHighlighterPositionSelected(ItemGrid targetGrid, InventoryItemNormalShaped targetItemNormalShaped, int posX, int posY)
        {
            var pos = targetGrid.CalculateIconPositionOnGrid(targetItemNormalShaped, posX, posY);

            // Change the local position because now this is a child of the targetGrid.
            itemHighlighter.localPosition = pos;
        }

        public void SetParentGrid(ItemGrid targetGrid)
        {
            if (targetGrid == null) return;
            itemHighlighter.SetParent(targetGrid.GetComponent<RectTransform>());
        }
    }
}
