#region Imported Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LeonardoEstigarribia.InventorySystem.inventoryItem;
using LeonardoEstigarribia.InventorySystem.itemGrid;
using Unity.VisualScripting;
using UnityEditor.Search;
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
        [SerializeField] private List<RectTransform>activeHighlighters = new List<RectTransform>(); // List of all the current highlight game objects.
        [SerializeField] private int highlighterPoolSize = 10;

        private InventoryItem lastItemHighlighted;
        private List<Vector2Int> lastHighlightedCoordinates = new List<Vector2Int>();
        
        private void Awake()
        {
            // Instantiate a reusable pool of highlighters.
            for (int i = 0; i < highlighterPoolSize; i++)
            {
                RectTransform instantiatedHighlighter = Instantiate(itemHighlighter);
                instantiatedHighlighter.gameObject.SetActive(false);
                activeHighlighters.Add(instantiatedHighlighter);
            }
        }

        public void Show(bool show)
        {
            foreach (var highlighter in activeHighlighters)
            {
                highlighter.gameObject.SetActive(show);
            }
        }
        
        public bool ChangedCursorPosition(InventoryItem item, ItemGrid selectedGrid)
        {
            if (item == null) return lastHighlightedCoordinates.Count > 0;

            List<Vector2Int> newCoordinates =
                selectedGrid.GetItemCoordinates(item.onGridPositionX, item.onGridPositionY, item.itemShape);
            bool hasChanged = item != lastItemHighlighted || newCoordinates != lastHighlightedCoordinates;

            lastItemHighlighted = item;
            lastHighlightedCoordinates = newCoordinates;
            return hasChanged;
        }
        
        public void ShowHighlightsItemTiles(ItemGrid selectedGrid, InventoryItem item)
        {
            ClearActiveHighlights();
            
            List<Vector2Int> occupiedCoordinates =
                selectedGrid.GetItemCoordinates(item.onGridPositionX, item.onGridPositionY, item.itemShape);

            for (int i = 0; i < occupiedCoordinates.Count; i++)
            {
                if (i >= activeHighlighters.Count)
                {
                    break;
                }

                // Made this so its easier to read because I'm going crazy.
                Vector2Int currentCoordinate = occupiedCoordinates[i];
                RectTransform currentHighlighter = activeHighlighters[i];
                
                currentHighlighter.gameObject.SetActive(true);
                SetParentGrid(currentHighlighter, selectedGrid);
                UnityEngine.Vector2 targetPosition = selectedGrid.CalculateHighlightPosition(currentCoordinate.x, currentCoordinate.y);
                currentHighlighter.localPosition = targetPosition;
            }
    
        }
        
        public void ClearActiveHighlights()
        {
            // Set all highlights to inactive in the scene.
            foreach (var highlighter in activeHighlighters)
            {
                highlighter.gameObject.SetActive(false);
            }
        }
        
        public void SetParentGrid(RectTransform child,ItemGrid targetGrid)
        {
            if (targetGrid == null) return;
            child.SetParent(targetGrid.GetComponent<RectTransform>());
        }
    }
}
