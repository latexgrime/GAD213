using UnityEngine;

namespace LeonardoEstigarribia.InventorySystem.itemData.complexShaped
{
    [CreateAssetMenu(menuName = "Inventory Item/Normal shaped item")]
    public class ItemDataComplexShaped : ScriptableObject
    {
        /// <summary>
        ///     Width of the item (space that will occupy on a grid).
        /// </summary>
        public int width = 1;

        /// <summary>
        ///     Height of the item (space that will occupy on a grid).
        /// </summary>
        public int height = 1;

        /// <summary>
        ///     Icon sprite of the item.
        /// </summary>
        public Sprite itemIcon;
    }
}
