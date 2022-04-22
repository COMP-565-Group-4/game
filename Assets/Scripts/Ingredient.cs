using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class Ingredient : MonoBehaviour
{
    public virtual IEnumerable<Ingredient> ChildIngredients => Enumerable.Empty<Ingredient>();
}
