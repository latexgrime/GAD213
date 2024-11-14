using UnityEngine;

namespace LeonardoEstigarribia.InventorySystem.itemData.complexShaped
{
    [CreateAssetMenu(menuName = "Inventory Item/Shaped Item")]
    public class ItemDataComplexShaped : ScriptableObject
    {
      
        public bool[,] shape;
        public shapeList selectedShape;
        
        /// <summary>
        ///     Icon sprite of the item.
        /// </summary>
        public Sprite itemIcon;

        public bool[,] SetShape()
        {
            switch (selectedShape)
            {
                case shapeList.LShaped:
                    shape = new bool[,]
                    {
                        { false, true }, // Don't forget this dumbass comma again.
                        { false, true },
                        { true, true }
                    };
                    return shape;
                
                // "L" Shaped:
                
                // - *
                // - *
                // * *
                
                
                case shapeList.PlusShaped:
                    shape = new bool[,]
                    {
                        { false, true, false },
                        { true, true, true },
                        { false, true, false }
                    };
                    return shape;
                
                // Plus Shaped "+":
                
                // - * -
                // * * *
                // - * -
            }

            return null;
        }
        
        
    }

    public enum shapeList
    {
        LShaped,
        PlusShaped
    }
}
