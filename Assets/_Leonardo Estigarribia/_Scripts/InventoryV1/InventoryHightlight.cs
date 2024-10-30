using UnityEngine;

/// <summary>
/// In charge of highlighting items in a grid container.
/// </summary>
public class InventoryHightlight : MonoBehaviour
{
    [SerializeField] private RectTransform itemHighlighter;

    public void Show(bool show)
    {
        itemHighlighter.gameObject.SetActive(show);
    }   

    public void SetHighlighterSize(InventoryItem targetItem)
    {
        Vector2 size = new Vector2();
        size.x = targetItem.invItemWidth * ItemGrid.tileSizeWidth;
        size.y = targetItem.invItemHeight * ItemGrid.tileSizeHeight;
        itemHighlighter.sizeDelta = size;
    }

    /// <summary>
    /// Sets the highlighter position when the player is about to make a selection in the grid.
    /// </summary>
    /// <param name="targetGrid"></param>
    /// <param name="targetItem"></param>
    public void SetHighlighterPositionSelection(ItemGrid targetGrid, InventoryItem targetItem)
    {
        Vector2 pos =
            targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPositionX, targetItem.onGridPositionY);

        // Change the local position because now this is a child of the targetGrid.
        itemHighlighter.localPosition = pos;
    }

    /// <summary>
    /// Sets the highlighter position when the player already selected an item in the grid.
    /// </summary>
    /// <param name="targetGrid"></param>
    /// <param name="targetItem"></param>
    /// <param name="posX"></param>
    /// <param name="posY"></param>
    public void SetHighlighterPositionSelected(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        // Change the local position because now this is a child of the targetGrid.
        itemHighlighter.localPosition = pos;
    }
    
    public void SetParentGrid(ItemGrid targetGrid)
    {
        if (targetGrid == null) return;
        itemHighlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }
}