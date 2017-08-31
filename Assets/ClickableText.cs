using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickableText : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    Tool[] tools;

    private Text _text;
    Recipe recipe;
    MyCursor cursor;
    Underline underline;

    int cursorIndex = -1;

    void Start()
    {
        _text = this.GetComponent<Text>();
        recipe = this.GetComponent<Recipe>();
        cursor = GameObject.FindObjectOfType<MyCursor>();
        underline = GameObject.FindObjectOfType<Underline>();
        tools = GameObject.FindObjectsOfType<Tool>();
    }

    void OnDrawGizmos()
    {
        var text = this.GetComponent<Text>();
        var textGen = text.cachedTextGenerator;
        var prevMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        for (int i = 0; i < textGen.characterCount; ++i)
        {
            Vector2 locUpperLeft = new Vector2(textGen.verts[i * 4].position.x, textGen.verts[i * 4].position.y);
            Vector2 locBottomRight = new Vector2(textGen.verts[i * 4 + 2].position.x, textGen.verts[i * 4 + 2].position.y);

            Vector3 mid = (locUpperLeft + locBottomRight) / 2.0f;
            Vector3 size = locBottomRight - locUpperLeft;

            Gizmos.DrawWireCube(mid, size);
        }
        Gizmos.matrix = prevMatrix;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        eventData.Use();
        int index = GetIndexOfClick(eventData.pressEventCamera.ScreenPointToRay(eventData.position));
        if (index != -1) Debug.Log(GetWordAtIndex(index)[0] + " " + GetWordAtIndex(index)[1] + " " + GetWordAtIndex(index)[2]);
        if (index != -1) recipe.OnClick(GetWordAtIndex(index));
    }

    // Called when the pointer enters our GUI component.
    // Start tracking the mouse
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine("TrackPointer");
    }

    // Called when the pointer exits our GUI component.
    // Stop tracking the mouse
    public void OnPointerExit(PointerEventData eventData)
    {
        StopCoroutine("TrackPointer");
        mouseOver = null;
    }

    public string FirstLetterToUpper(string str)
    {
        if (str == null)
            return null;

        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);

        return str.ToUpper();
    }

    GameObject mouseOver;
    GameObject highlighted;
    GameObject mouseOverPre;

    // to watch values in the coroutine
    int i, j;
    string over;
    IEnumerator TrackPointer()
    {
        var ray = GetComponentInParent<GraphicRaycaster>();
        var input = FindObjectOfType<StandaloneInputModule>();

        if (ray != null && input != null)
        {
            while (Application.isPlaying)
            {
                Vector2 localPos; // Mouse position  
                //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, Input.mousePosition, ray.eventCamera, out localPos);

                // local pos is the mouse position.

                {
                    int index = GetIndexOfClick(Camera.main.ScreenPointToRay(Input.mousePosition));
                    //if (index != -1) Debug.Log(GetWordAtIndex(index).Trim());
                    cursorIndex = index;

                    if (index != -1)
                    {

                        var strList = GetWordAtIndex(index);
                        strList[0] = FirstLetterToUpper(strList[0]);
                        strList[1] = FirstLetterToUpper(strList[1]);
                        strList[2] = FirstLetterToUpper(strList[2]);
                        over = strList[0];

                        //over = FirstLetterToUpper(over);

                        //Debug.Log("over " + "\"" + over + "\"");
                        //mouseOver = GameObject.Find(over);

                        //mouseOver = null;
                        mouseOver = GetObjectByName(strList[1] + " " + over);
                        if (mouseOver == null)
                        {
                            mouseOver = GetObjectByName(over + " "+ strList[2]);
                        }
                        if (mouseOver == null)
                        {
                            mouseOver = GetObjectByName(over);
                        }
                    }
                    else
                    {
                        mouseOver = null;
                    }


                }

                yield return 0;
            }
        }
        else
            UnityEngine.Debug.LogWarning("Could not find GraphicRaycaster and/or StandaloneInputModule");
    }

    private /*static */GameObject GetObjectByName(string over)
    {
        GameObject mouseOver = null;

        for (i = 0; i < tools.Length; i++)
        {
            for (j = 0; tools[i].Functions != null && j < tools[i].Functions.Length; j++)
            {
                if (string.Compare(tools[i].Functions[j], over, true) == 0)
                    mouseOver = tools[i].gameObject;
            }
        }

        if (mouseOver == null)
        {
            mouseOver = GameObject.Find(over);
        }
        if (mouseOver == null)
        {
            over = over.ToLower();
            mouseOver = GameObject.Find(over);
        }

        return mouseOver;
    }

    int GetIndexOfClick(Ray ray)
    {
        Ray localRay = new Ray(
          transform.InverseTransformPoint(ray.origin),
          transform.InverseTransformDirection(ray.direction));

        Vector3 localClickPos =
          localRay.origin +
          localRay.direction / localRay.direction.z * (transform.localPosition.z - localRay.origin.z);

        Debug.DrawRay(transform.TransformPoint(localClickPos), Vector3.up / 10, Color.red, 2.0f);

        var textGen = _text.cachedTextGenerator;
        for (int i = 0; i < textGen.characterCount; ++i)
        {
            Vector2 locUpperLeft = new Vector2(textGen.verts[i * 4].position.x, textGen.verts[i * 4].position.y);
            Vector2 locBottomRight = new Vector2(textGen.verts[i * 4 + 2].position.x, textGen.verts[i * 4 + 2].position.y);

            if (localClickPos.x >= locUpperLeft.x &&
             localClickPos.x <= locBottomRight.x &&
             localClickPos.y <= locUpperLeft.y &&
             localClickPos.y >= locBottomRight.y
             )
            {
                return i;
            }
        }

        return -1;
    }

    string[] GetWordAtIndex(int index)
    {
        int beginIndex, endIndex;
        int temp1, temp2;
        var word = GetWordAtIndexAux(index, out beginIndex, out endIndex);
        var before = beginIndex > 0 ? GetWordAtIndexAux(beginIndex - 1, out temp1, out temp2) : null;
        var after = endIndex < _text.text.Length - 1 ? GetWordAtIndexAux(endIndex + 1, out temp1, out temp2) : null;

        return new string[] { word, before, after };
    }


    string GetWordAtIndexAux(int index, out int begIndex, out int lastIndex)
    {
        // Debug.Log("GetWordAtIndexAux " + index);

        /*int */
                begIndex = -1;
        int marker = index;

        if (marker < 0) marker = 0;
        if (marker > _text.text.Length - 1) marker = _text.text.Length - 1;

        while (begIndex == -1)
        {
            marker--;
            if (marker < 0)
            {
                begIndex = 0;
            }
            else if (!char.IsLetter(_text.text[marker]))
            {
                begIndex = marker;
            }
        }

        /*int */lastIndex = -1;
        marker = index;
        while (lastIndex == -1)
        {
            marker++;
            if (marker > _text.text.Length - 1)
            {
                lastIndex = _text.text.Length - 1;
            }
            else if (!char.IsLetter(_text.text[marker]))
            {
                lastIndex = marker;
            }
        }

        return _text.text.Substring(begIndex, lastIndex - begIndex).Trim();
    }


    void Update()
    {
        if (mouseOver != highlighted && highlighted != null)
        {
            var sprite = highlighted.GetComponent<SpriteRenderer>();
            var color = sprite.color;
            color.a = 1;
            sprite.color = color;

            StopCoroutine("Highlight");
            highlighted = null;

        }
        if (mouseOver != null && mouseOver != highlighted)
        {
            highlighted = mouseOver;
            StartCoroutine("Highlight");
        }
        
        /*if (mouseOver != mouseOverPre)
        {
            if (mouseOver == null)
               cursor.Reset();
            if (mouseOverPre == null)
                cursor.Click();
        }*/

        if (cursorIndex != -1 && underline.IsUnderlined(cursorIndex))
        {
            cursor.Click();
        }
        else
        {
            cursor.Reset();
        }


        mouseOverPre = mouseOver;

        //Debug.Log(mouseOver);
    }

    IEnumerator Highlight()
    {
        var sprite = highlighted.GetComponent<SpriteRenderer>();
        var color = sprite.color;

        while (true)
        {
            for (int i = 0; i < 10; i++)
            {
                color.a -= 0.05f;
                sprite.color = color;
                yield return new WaitForSeconds(0.05f);
            }
            for (int i = 0; i < 10; i++)
            {
                color.a += 0.05f;
                sprite.color = color;
                yield return new WaitForSeconds(0.05f);
            }
        }
    }



}
