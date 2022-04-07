using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeBook : MonoBehaviour
{
    public static Dictionary<List<string>, string> Recipes =
        new Dictionary<List<string>, string>() {
            // giant list of recipes goes here! copy the following syntax:
            // {new List<string> {"Item1", "Item2", "Item3"}, "Result"},
            // try to put them in alphabetical order, i'm probably gonna do a thing later that
            // relies on it
        };
}
