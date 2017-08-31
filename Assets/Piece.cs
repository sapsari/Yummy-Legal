using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour {

    Tooltip tooltip;
    Chef chef;
    Recipe recipe;

    public Tool Tool;

    public List<string> Tags;// = new List<string>();

    static Texture[] textures;

    public string[] TagOverrides;

    public bool IsBanned { get; private set; }

    // Use this for initialization
    void Start() {
        Initialize();
    }

    public void Initialize() { 
        chef = GameObject.Find("Chef").GetComponent<Chef>();
        recipe = GameObject.Find("Recipe").GetComponent<Recipe>();
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();

        Debug.Assert(recipe != null);

        // null for instantiated
        if (this.transform.parent != null)
            Tool = this.transform.parent.GetComponent<Tool>(); 

        if (textures == null)
            textures = Resources.LoadAll<Texture>("");

        foreach (var t in textures)
        {
            //Debug.Log("texture: " + t.name);
        }

        //Debug.Log(this.name + " started");
    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnMouseDown()
    {
        Debug.Log(this.name + " onMouseDown");

        if (chef.pieces.Contains(this)) return;

        Tool.OnMouseDown();
        chef.Pickup = this;
    }

    void OnMouseEnter()
    {
        string str = "";

        if (this.IsBanned)
        {
            str = "BANNED ";
        }

        if (Tags.Count == 0)
            str += this.name;
        else
        {
            var tt = this.Tags.ToArray();
            System.Array.Reverse(tt);
            str += System.String.Join(" ", tt);
        }
        tooltip.Show(str);
    }
    void OnMouseExit()
    {
        tooltip.Hide();
    }

    public void UpdateImage()
    {

        var maxCount = 0;
        Texture tex = null;
        foreach (var t in textures)
        {
            var names = t.name.Split(' ');

            var count = Count(names, this.Tags);


            count = count * 2 - (names.Length - count);

            if (count > maxCount)
            {
                maxCount = count;
                tex = t;
            }
        }

        if (tex != null)
            this.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(tex.name);
    }

    static int Count(string[] a, List<string>b)
    {
        var count = 0;
        foreach(var item in a)
        {
            if (b.Contains(item))
                count++;
        }
        return count;
    }

    public void Combine(List<Piece> pieces)
    {
        /*var tomato = pieces.Find(p => p.Tags.Contains("chopped") && p.Tags.Contains("tomato"));
        var cucumber = pieces.Find(p => p.Tags.Contains("chopped") && p.Tags.Contains("cucumber"));

        if (tomato != null && cucumber != null)
        {
            chef.Dump(cucumber);
            var salad = tomato;

            salad.Tags.Clear();
            salad.Tags.Add("salad");

            salad.UpdateImage();

            Debug.Log("Combined " + salad.ToString());

            //chef.UpdatePieces();
        }*/

        Debug.Assert(recipe != null);
        Debug.Assert(recipe.combinations != null);
        foreach(var combination in recipe.combinations)
        {
            var materials = combination.Value.Split(';');

            var all = true;
            foreach(var material in materials)
            {
                var materialList = material.Split(' ');
                var piece = pieces.Find(p => Contains(p.Tags, materialList));
                if (piece==null)
                {
                    all = false;
                    break;
                }
            }

            if (all)
            {
                Piece firstPiece = null;

                foreach (var material in materials)
                {
                    var materialList = material.Split(' ');
                    var piece = pieces.Find(p => Contains(p.Tags, materialList));
                    if (firstPiece == null)
                    {
                        firstPiece = piece;

                        firstPiece.Tags.Clear();
                        //firstPiece.Tags.Add(combination.Key);
                        firstPiece.Tags.AddRange(combination.Key.Split(' '));

                        {
                            var combinationObj = GameObject.Find(combination.Key);
                            if (combinationObj != null)
                            {
                                var combinationPiece = combinationObj.GetComponent<Piece>();

                                firstPiece.TagOverrides = new string[combinationPiece.TagOverrides.Length];
                                for (var i = 0; i < combinationPiece.TagOverrides.Length; i++)
                                {
                                    firstPiece.TagOverrides[i] = combinationPiece.TagOverrides[i];
                                }
                            }
                            else
                            {
                                firstPiece.TagOverrides = new string[0];
                            }
                        }

                        firstPiece.UpdateImage();

                        Debug.Log("Combined " + combination.Key);
                        foreach (var to in firstPiece.TagOverrides)
                        {
                            Debug.Log("tag override: " + to);
                        }

                    }
                    else
                    {
                        chef.Dump(piece);
                    }
                }
            }
        }
    }

    static bool Contains(List<string> set, string[] subset)
    {
        foreach(var item in subset)
        {
            if (!set.Contains(item))
                return false;
        }
        return true;
    }

    public override string ToString()
    {
        //return base.ToString();
        return System.String.Join(" ", this.Tags.ToArray()) + " " + this.name;
    }

    public void Ban()
    {
        IsBanned = true;
        //this.gameObject.cre
        //GameObject.CreatePrimitive(PrimitiveType.)
        var child = new GameObject("ban");
        child.transform.parent = this.gameObject.transform;
        child.transform.localPosition = new Vector3();
        child.transform.localScale = new Vector3(0.2f, 0.2f);
        var sr = child.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("ban");
        sr.sortingOrder = 2;
        
    }
}
