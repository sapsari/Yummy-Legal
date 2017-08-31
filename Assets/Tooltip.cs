using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    Text text;
    Image image;

	// Use this for initialization
	void Start () {
        this.text = this.GetComponentInChildren<Text>();
        this.image = this.GetComponent<Image>();
        text.enabled = false;
        image.enabled = false;
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Show(string str)
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = transform.position.z;
        transform.position = pos;
        //transform.po
        text.enabled = true;
        image.enabled = true;
        text.text = str;

        ((RectTransform)transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, text.preferredWidth+10);
    }

    public void Hide()
    {
        text.enabled = false;
        image.enabled = false;
    }
}
