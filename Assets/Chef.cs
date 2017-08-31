using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chef : MonoBehaviour {

    public bool ClickHandled;
    public Vector3 Destination;
    public Tool Tool;
    public Piece Piece;
    public Piece Pickup;
    UnityEngine.UI.Text progressText;

    public List<Piece> pieces = new List<Piece>();

    // Use this for initialization
    void Start () {

        Destination = transform.position;

        progressText = GameObject.Find("ProgressBar").GetComponent<UnityEngine.UI.Text>();

    }

    // Update is called once per frame
    void Update () {


        if (Input.GetMouseButtonDown(0) && !ClickHandled)
        {
            /*
            Destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Destination.z = transform.position.z;

            this.Tool = null;

            Debug.Log("Chef move");
            */
        }

        const float movementSpeed = 10f;
        transform.position = Vector3.MoveTowards(transform.position, Destination, movementSpeed * Time.deltaTime);

        ClickHandled = false;
        progressText.enabled = false;

        if (transform.position == Destination)
        {
            if (Tool != null)
            {
                if (Pickup != null && Pickup.Tool == Tool && !Pickup.GetComponent<Piece>().IsBanned)
                {
                    /*if (this.Piece != null)
                    {
                        this.Piece.transform.parent = null;
                        DestroyImmediate(this.Piece.gameObject);
                        this.Piece = null;
                    }*/
                    var previous = this.Piece;

                    //this.Piece = Pickup;
                    this.Piece = Instantiate(Pickup);
                    this.Piece.Initialize();
                    if (this.Piece.Tags.Count == 0)
                        this.Piece.Tags.Add(this.Piece.name.Replace("(Clone)", "").ToLower());
                    Piece.Tool = Tool;
                    Pickup = null;

                    Piece.transform.parent = this.transform;
                    Piece.transform.localPosition = Vector3.zero;
                    Piece.transform.localScale = Vector3.one * 1f;
                    Piece.GetComponent<SpriteRenderer>().sortingOrder = 6;

                    /*if (previous != null)
                    {
                        Piece.Previous = previous;
                        previous.Next = Piece;
                    }*/

                    pieces.Insert(0, Piece);

                    pieces[0].Combine(pieces);
                    pieces[0].Combine(pieces);
                    pieces[0].Combine(pieces);
                    pieces[0].Combine(pieces);
                    pieces[0].Combine(pieces);

                    UpdatePieces();

                    Tool = null;

                    Debug.Log("Chef pickup " + Piece.name);
                }
                else
                {
                    Tool.Process(this.Piece);
                }

            }
        }

        var wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0.01f && Mathf.Abs(previousWheel) < 0.01f)
        {
            this.Tool = null;
            ChangePiece(true);
        }
        else if (wheel < -0.01f && Mathf.Abs(previousWheel) < 0.01f)
        {
            this.Tool = null;
            ChangePiece(false);
        }
        previousWheel = wheel;

        //var radial = GameObject.Find("ProgressRadialHollow").GetComponent<ProgressBar.ProgressRadialBehaviour>();
        //radial.Value = 50;
    }
    float previousWheel;

    public void OnToolClick()
    {
        Debug.Log("tool clicked");
    }

    public void UpdatePieces()
    {
        int i = 1;
        foreach (var p in pieces)
        {
            if (p != this.Piece)
            {
                p.transform.localPosition = new Vector3(i++ * .5f, 0);
            }
        }
        if (Piece != null)
            Piece.transform.localPosition = Vector3.zero;
    }

    public void Dump(Piece p)
    {
        if (p == null) return;
        //var p = this.Piece;

        if (p == Piece)
            Piece = null;
        this.pieces.Remove(p);
        p.transform.parent = null;
        DestroyImmediate(p.gameObject);

        if (Piece == null && pieces.Count > 0)
            Piece = pieces[0];

        UpdatePieces();
    }

    void ChangePiece(bool next)
    {
        if (pieces.Count < 2) return;

        int index = pieces.IndexOf(Piece);
        if (next)
            index++;
        else
            index--;
        index = (index + pieces.Count) % pieces.Count;

        Piece = pieces[index];

        UpdatePieces();

        Debug.Log("Switch " + next.ToString());
    }

    public void Move()
    {
        Destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Destination.z = transform.position.z;

        this.Tool = null;

        Debug.Log("Chef move 2");
    }

    public void Clear()
    {
        this.Tool = null;
        
        foreach(var p in pieces.ToArray())
        {
            Dump(p);
        }

    }
}


/*
2do for polish
highligt for tutorial (if player is idle, highlight order's pieces&tools)
served meal refund
served meal appreciation
*/