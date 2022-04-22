using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class PreparedIngredient : Ingredient
{
    [Tooltip("Ingredients required to create this ingredient")]
    public Ingredient[] Ingredients;

    public override IEnumerable<Ingredient> ChildIngredients
    {
        get {
            var all = Enumerable.Empty<Ingredient>();
            foreach (var ingredient in Ingredients) {
                all = all.Concat(ingredient.ChildIngredients).Append(ingredient);
            }

            return all;
        }
    }
}
