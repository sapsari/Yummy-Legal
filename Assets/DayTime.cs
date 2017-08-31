using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DayTime : MonoBehaviour {

    GameObject scorpion;

    GameObject popup;
    Image popupImage;
    Button popupButton;
    Text popupText;

    public int DayNo = 1;

    Chef chef;
    Recipe recipe;
    Tooltip tooltip;

    /*
    1 day -> 2 mins
    8hrs -> 2mins
    1hr -> .25min
    1hr -> 15 sec

    360/12->30
    */

	// Use this for initialization
	void Start () {

        this.tooltip = GameObject.Find("Tooltip").GetComponent<Tooltip>();

        this.scorpion = GameObject.Find("Scorpion");//.GetComponent<Sprite>();

        this.popup = GameObject.Find("Popup");
        this.popupImage = popup.GetComponent<Image>();
        this.popupButton = popup.GetComponent<Button>();
        this.popupText = popup.GetComponentInChildren<Text>();

        this.chef = GameObject.FindObjectOfType<Chef>();
        this.recipe = GameObject.FindObjectOfType<Recipe>();

        this.popupImage.enabled = true;//was hidden for editor

        NewDay();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.P))
        {
            EndDay();
            return;
        }

        var z = scorpion.transform.rotation.eulerAngles.z;
        if (z > 90 && z < 180)
        {
            EndDay();
            return;
        }

        //Debug.Log(z);

        const float multiplier = 1;// 0.01f;//**--

        scorpion.transform.Rotate(0, 0, Time.deltaTime * -2 * multiplier);
        
	}

    void OnMouseEnter()
    {
        tooltip.Show("Day " + DayNo.ToString());
    }
    void OnMouseExit()
    {
        tooltip.Hide();
    }

    public void NewDay()
    {
        recipe.ClearOrders();

        scorpion.transform.rotation = Quaternion.identity;
        scorpion.transform.Rotate(0, 0, 90);

        popup.SetActive(false);
    }

    public void EndDay()
    {
        if (popup.activeSelf)
            return;

        chef.Clear();
        recipe.Back(); recipe.Back(); recipe.Back(); recipe.Back(); recipe.Back();
        recipe.ClearOrders();

        DayNo++;

        var banText = PlaceBan();

        popupText.text = "Day " + DayNo + "\n";
        popupText.text += banText;

        popup.SetActive(true);
    }

    public bool IsPopupActive
    {
        get
        {
            return this.popup.activeSelf;
        }
    }

    string PlaceBan()
    {
        recipe.AddRecipeOfTheDay(DayNo);

        if (DayNo ==2)
        {
            var tomatoSauce = GameObject.Find("tomato sauce").GetComponent<Piece>();
            tomatoSauce.Ban();

            var dressing = GameObject.Find("dressing").GetComponent<Piece>();
            dressing.Ban();

            return "Canned products are banned";
        }
        else if (DayNo ==3)
        {
            var pasta = GameObject.Find("pasta").GetComponent<Piece>();
            pasta.Ban();

            return "Imported goods are no more";
        }
        else if (DayNo == 4)
        {
            var processor = GameObject.Find("Processor").GetComponent<Tool>();
            processor.Ban();

            return "Alien technology is banished";
        }
        else if (DayNo == 5)
        {
            var egg = GameObject.Find("egg").GetComponent<Piece>();
            egg.Ban();

            return "All chickens are taken into custody";
        }

        return "One more day in paradise\n\n\nThank you for playing the game\nGame is over, but you can continue cooking if you want to";
    }

    /* BAN LIST
    gun 1. yok
    gun 2. salça,sos->konserve,sağlık için
    gun 3. makarna
    gun 4. blender->yabancı aletler, telekulak
    gun 5. yumurta->chickenlar suçlu
    gun 6. gg
    */

}
