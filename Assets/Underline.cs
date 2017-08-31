using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// [ExecuteInEditMode]
public class Underline : MonoBehaviour
{
    const int max = 20;

    //public int underlineStart = 0;
    //public int underlineEnd = 0;

    private Text text = null;
    private RectTransform textRectTransform = null;
    private TextGenerator textGenerator = null;

    //private GameObject lineGameObject = null;
    //private Image lineImage = null;
    //private RectTransform lineRectTransform = null;

    List<RectTransform> trs = new List<RectTransform>(max);
    List<int> begins = new List<int>();
    List<int> ends = new List<int>();


    void Start()
    {
        text = gameObject.GetComponent<Text>();
        textRectTransform = gameObject.GetComponent<RectTransform>();
        textGenerator = text.cachedTextGenerator;


        for (int i = 0; i < max; i++)
        {
            var lineGameObject = new GameObject("Underline");
            var lineImage = lineGameObject.AddComponent<Image>();
            lineImage.color = text.color;
            var lineRectTransform = lineGameObject.GetComponent<RectTransform>();
            lineRectTransform.SetParent(transform, false);
            lineRectTransform.anchorMin = textRectTransform.pivot;
            lineRectTransform.anchorMax = textRectTransform.pivot;

            trs.Add(lineRectTransform);
        }

       /* begins.Add(0);
        begins.Add(3);
        ends.Add(1);
        ends.Add(5);*/
    }

    void Update()
    {
        if (textGenerator.characterCount < 0)
            return;
        UICharInfo[] charactersInfo = textGenerator.GetCharactersArray();
        //if (!(underlineEnd > underlineStart && underlineEnd < charactersInfo.Length))
          //  return;
        UILineInfo[] linesInfo = textGenerator.GetLinesArray();
        if (linesInfo.Length < 1)
            return;
        float height = linesInfo[0].height;
        Canvas canvas = gameObject.GetComponentInParent<Canvas>();
        float factor = 1.0f / canvas.scaleFactor;

        Debug.Assert(begins.Count == ends.Count);
        Debug.Assert(trs.Count >= begins.Count);

        for (int i = 0; i < begins.Count; i++)
        {
            var lineRectTransform = trs[i];
            var underlineStart = begins[i];
            var underlineEnd = ends[i];

            //Debug.Assert(charactersInfo.Length > underlineStart);
            //Debug.Assert(charactersInfo.Length > underlineEnd);

            // caused by the first frame after the text is set??
            if (charactersInfo.Length <= underlineEnd)
                return;

            lineRectTransform.anchoredPosition = new Vector2(
                factor * (charactersInfo[underlineStart].cursorPos.x + charactersInfo[underlineEnd].cursorPos.x) / 2.0f,
                factor * (charactersInfo[underlineStart].cursorPos.y - height / 1.0f)
                );
            lineRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, factor * Mathf.Abs(charactersInfo[underlineStart].cursorPos.x - charactersInfo[underlineEnd].cursorPos.x));
            lineRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height / 10.0f);
        }
        for(var i = begins.Count;i<max;i++)
        {
            var lineRectTransform = trs[i];
            var underlineStart = 0;// begins[i];
            var underlineEnd = 0;// ends[i];

            lineRectTransform.anchoredPosition = new Vector2(
                factor * (charactersInfo[underlineStart].cursorPos.x + charactersInfo[underlineEnd].cursorPos.x) / 2.0f,
                factor * (charactersInfo[underlineStart].cursorPos.y - height / 1.0f)
                );
            lineRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, factor * Mathf.Abs(charactersInfo[underlineStart].cursorPos.x - charactersInfo[underlineEnd].cursorPos.x));
            lineRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height / 10.0f);
        }
    }

    public void Clear()
    {
        begins.Clear();
        ends.Clear();
    }

    public void Add(int begin, int len)
    {
        Debug.Log("Underline.Add " + begin + " " + len);
        begins.Add(begin);
        ends.Add(begin+len);
    }

    public bool IsUnderlined(int index)
    {
        for (int i = 0; i < begins.Count; i++)
        {
            if (begins[i] <= index && ends[i] >= index)
                return true;
        }
        return false;
    }
}