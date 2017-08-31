using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCursor : MonoBehaviour {

    public Texture2D clickTexture;

    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Reset()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void Click()
    {
        Cursor.SetCursor(clickTexture, hotSpot, cursorMode);
    }
}
