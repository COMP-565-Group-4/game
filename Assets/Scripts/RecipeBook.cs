using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RecipeBook : MonoBehaviour
{
    ListSameItems recList = new ListSameItems();
    public static Dictionary<List<string>, string> Recipes;

    void Start()
    {
        Recipes = new Dictionary<List<string>, string>(recList) {
            // giant list of recipes goes here! copy the following syntax:
            // {new List<string> {"Item1", "Item2", "Item3"}, "Result"},
            // try to put them in alphabetical order, i'm probably gonna do a thing later that
            // relies on it
            // { new List<string> { "Cylinder", "Sphere" }, "Capsule" } // dummy data
            { new List<string> { "Bread", "Egg" }, "Eggy Toast" },
            { new List<string> { "Bread", "Orange" }, "Orange French Toast" },
            { new List<string> { "Bread", "Egg", "Olive" }, "Olive French Toast" },
            { new List<string> { "Bread", "Cherry", "Egg", "Tomato" },
              "Cherry Tomato French Toast" },

        };
    }
}

// custom equality comparer to ensure that the book will recognize whatever recipes we compare it
// with
class ListSameItems : EqualityComparer<List<string>>
{
    public override bool Equals(List<string> l1, List<string> l2)
    {
        if (l1 == null && l2 == null)
            return true;
        else if (l1 == null || l2 == null)
            return false;

        l1.Sort();
        l2.Sort();
        bool isEqual = Enumerable.SequenceEqual(l1, l2);
        return isEqual;
    }

    public override int GetHashCode(List<string> li)
    {
        string hCode = "";
        li.ForEach(delegate(string item) { hCode = hCode + item; });
        return hCode.GetHashCode();
    }
}
