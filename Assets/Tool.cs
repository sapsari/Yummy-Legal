using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : MonoBehaviour {

    public float Speed;
    public string[] Functions;

    Chef chef;
    public Piece piece;
    UnityEngine.UI.Text progressText;
    float progress;

    DayTime dayTime;
    Recipe recipe;
    Tooltip tooltip;

    public bool IsBanned { get; private set; }

    // Use this for initialization
    void Start () {
        chef = GameObject.Find("Chef").GetComponent<Chef>();
        progressText = GameObject.Find("ProgressBar").GetComponent<UnityEngine.UI.Text>();
        dayTime = GameObject.Find("Clock").GetComponent<DayTime>();
        recipe = GameObject.Find("Recipe").GetComponent<Recipe>();
        tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();
        progressText.enabled = false;
    }

    // Update is called once per frame
    void Update () {
	}

    public void OnMouseDown()
    {
        if (dayTime.IsPopupActive) return;

        Debug.Log(this.name +  " onMouseDown");

        chef.Destination = this.transform.GetChild(0).position;
        chef.ClickHandled = true;
        chef.Tool = this;
        progress = 0;
    }

    void OnMouseEnter()
    {
        var str = "";
        if (this.IsBanned)
            str = "BANNED ";
        str += this.name;
        tooltip.Show(str);
    }
    void OnMouseExit()
    {
        tooltip.Hide();
    }

    public void Process(Piece piece)
    {

        if (piece == null) return;
        if (this.IsBanned) return;

        progressText.enabled = true;
        progress += Speed * Time.deltaTime;
        progressText.text = ((int)progress).ToString();

        if (progress < 100) return;

        Debug.Log(this.name + " process " + piece.name);

        if (this.name == "Processor")
        {
            if (piece.Tags.Contains("sauce") || piece.Tags.Contains("scrambled"))
            {
                // no op
            }
            else if (piece.Tags.Contains("minced"))
            {
                piece.Tags.Remove("minced");
                piece.Tags.Add("sauce");
            }
            else if (piece.Tags.Contains("chopped"))
            {
                piece.Tags.Remove("chopped");
                piece.Tags.Add("sauce");
            }
            else
            {
                piece.Tags.Add("sauce");
            }
        }

        if (this.name == "Stove")
        {
            if (piece.Tags.Contains("cooked"))
            {
                // overcooked, burnt?
            }
            else if (piece.Tags.Contains("boiled"))
            {
                //
            }
            else if (piece.Tags.Contains("hot"))
            {
                //
            }
            else
            {
                piece.Tags.Add("cooked");
            }
        }

        if (this.name == "Cutting Board")
        {
            if (piece.Tags.Contains("sauce") || piece.Tags.Contains("scrambled"))
            {
                // no op
            }
            else if (piece.Tags.Contains("minced"))
            {
                piece.Tags.Remove("minced");
                piece.Tags.Add("sauce");
            }
            else if (piece.Tags.Contains("chopped"))
            {
                piece.Tags.Remove("chopped");
                piece.Tags.Add("minced");
            }
            else
            {
                piece.Tags.Add("chopped");
            }
        }

        foreach (var to in piece.TagOverrides)
        {
            var split = to.Split(';');
            Debug.Assert(split.Length == 2);
            if (piece.Tags.Contains(split[0]))
            {
                piece.Tags.Remove(split[0]);
                if (split[1].Length > 0)
                    piece.Tags.Add(split[1]);
            }
        }

        piece.UpdateImage();


        Debug.Log(">> " + piece.ToString());

        piece.Combine(chef.pieces);
        chef.pieces[0].Combine(chef.pieces);
        chef.pieces[0].Combine(chef.pieces);
        chef.pieces[0].Combine(chef.pieces);
        chef.pieces[0].Combine(chef.pieces);

        // dump liquid
        if (this.name == "Faucet")
        {
            if (chef.Piece.Tags.Contains("water")||
                chef.Piece.Tags.Contains("sauce"))
            {
                chef.Dump(chef.Piece);
            }
        }

        if (this.name == "Garbage")
        {
            chef.Dump(chef.Piece);
        }

        if (this.name == "Serve")
        {
            var serve = chef.Piece;
            chef.Dump(chef.Piece);
            //recipe.OnServed(serve.Tags[serve.Tags.Count - 1]);
            recipe.OnServed(serve.Tags);
        }

        chef.Tool = null;
        progressText.enabled = false;

    }

    public void Ban()
    {
        this.IsBanned = true;

        var child = new GameObject("ban");
        child.transform.parent = this.gameObject.transform;
        child.transform.localPosition = new Vector3();
        child.transform.localScale = Vector3.one * 0.8f;
        var sr = child.AddComponent<SpriteRenderer>();
        sr.sprite = Resources.Load<Sprite>("ban");
        sr.sortingOrder = 2;
    }


}
