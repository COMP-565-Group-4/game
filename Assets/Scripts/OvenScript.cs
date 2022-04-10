using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OvenScript : MonoBehaviour
{
    private bool _busy;
    private bool _done;
    private List<GameObject> _input;
    private List<string> _recipe;
    private GameObject _output;

    // Start is called before the first frame update
    void Start()
    {
        // initialize empty, idle oven
        _busy = false;
        _input = new List<GameObject>();
        _recipe = new List<string>();
    }

    // Update is called once per frame
    void Update() { }

    void Interaction()
    {
        Cook();
    }

    void Insert()
    {
        if (_done) {
            print("ERROR: Oven contains a finished dish. Remove it first!");
        } else if (!_busy) {
            // add inserted object to _input
            print("Adding " + Inventory.HeldItem.name + " to container...");
            _input.Add(Inventory.HeldItem);
            Inventory.HeldItem = null;
        } else {
            // oven is currently busy, don't let player put anything in
            print("ERROR: Oven is busy!");
        }
    }

    void Extract()
    {
        if (_done) {
            // oven is done, retrieve the finished dish from _output
            Inventory.HeldItem = _output;
            _output = null;
            _done = false;
        } else if (!_busy) {
            // oven hasn't started yet, ingredients can be removed from _input
            print("Removing " + _input[_input.Count - 1].name + " from container...");
            Inventory.HeldItem = _input[_input.Count - 1];
            _input.RemoveAt(_input.Count - 1);
        } else {
            // oven is currently busy, don't let player take anything out
            print("ERROR: Oven is busy!");
        }
    }

    void Cook()
    {
        if (_input.Count == 0) {
            print("ERROR: No items!");
            return;
        }

        // convert _input to _recipe
        foreach (GameObject ingredient in _input) {
            _recipe.Add(ingredient.name);
        }
        _recipe.Sort();

        // look up current recipe in recipe book
        if (RecipeBook.Recipes.ContainsKey(_recipe)) {
            print("Recipe found!");
            // if recipe is present, load the associated dish from Resources/Meals prefab
            _output =
                Instantiate(Resources.Load<GameObject>("Meals/" + RecipeBook.Recipes[_recipe]));

            _busy = true;
            _done = true;
            // wait some amount of time to cook it, and then proceed...
        } else {
            print("ERROR: Recipe not found!");
            // OPTIONAL TODO: if we find a "dubious food" or "unidentified lump" type asset, we can
            // load it into _output instead
            _output = null;
        }

        // either way, we're done cooking.
        _busy = false;
        _input.Clear();
        _recipe.Clear();
    }
}
