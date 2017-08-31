using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour {

    Text text;
    Underline underline;

    Text header, footer;

    List<string> orders = new List<string>();
    Stack<string> stack = new Stack<string>();

    List<string> dishes = new List<string>()
    {
        "salad",
        "spaghetti",
        "tomato soup",
        "fried egg",
    };

    Dictionary<string, string> recipes = new Dictionary<string, string>()
    {
        {"salad", "chop tomato\nchop cucumber\nadd dressing and serve " },
        //{"dressing", "mix oil vinegar " }, // added with ban

        {"spaghetti", "boil pasta\nadd tomato sauce\ncook and serve " },
        //{ "tomato sauce", "blend tomato " }, // added with ban
        //{ "tomato sauce", "chop and mince and whack tomato " },

        //{ "pasta", "combine flour and water\nthen process " }, // added with ban
        //{ "pasta", "combine flour and water\nthen chop and mince and whack " },

        {"tomato soup", "chop and cook onion\nadd flour, tomato sauce and water\ncook and serve "},

        { "fried egg", "cook egg " },

    };

    public Dictionary<string, string> combinations = new Dictionary<string, string>()
    {
        {"salad", "chopped tomato;chopped cucumber;dressing" },
        {"dressing", "oil;vinegar" },

        {"spaghetti", "boiled pasta;cooked sauce tomato" },

        {"pasta", "dough sauce" },
        {"dough", "water;flour" },

        //{"tomato soup", "cooked chopped onion;flour;tomato sauce;water" },
        {"tomato soup", "cooked chopped onion;tomato sauce;dough" },

    };


    // Use this for initialization
    void Start () {
		
        this.text = this.GetComponent<Text>();
        this.underline = this.GetComponent<Underline>();

        this.header = GameObject.Find("Header").GetComponent<Text>();
        this.footer = GameObject.Find("Footer").GetComponent<Text>();

        /*
        orders.Add("salad");
        orders.Add("spaghetti");
        orders.Add("tomato soup");
        orders.Add("fried egg");
        */
        //orders.Add("sauce");

        UpdateStack();

        StartCoroutine(AddOrder());
    }
	
	// Update is called once per frame
	void Update () {
		
        /*if (state == -1)
        {
            UpdateStack();
        }*/


	}

    IEnumerator AddOrder()
    {
        yield return new WaitForSeconds(15);

        orders.Add("fried egg");
        UpdateStack();
        yield return new WaitForSeconds(15);

        orders.Add("salad");
        UpdateStack();
        yield return new WaitForSeconds(15);

        while (true)
        {
            orders.Add(dishes[Random.Range(0, dishes.Count)]);
            UpdateStack();
            yield return new WaitForSeconds(10 + Random.value * 10);
        }

    }

    public void ClearOrders()
    {
        this.orders.Clear();
        UpdateStack();
    }

    public void OnClick(string[] str)
    {
        var cur = str[0];
        var pre = str[1];
        var post = str[2];
       // if (state == -1)
        {

            if (recipes.ContainsKey(pre+" "+cur))
            {
                stack.Push(pre + " " + cur);
                UpdateStack();
            }
            else if (recipes.ContainsKey(cur+" "+post))
            {
                stack.Push(cur + " " + post);
                UpdateStack();
            }
            else if (recipes.ContainsKey(cur))
            {
                //var index = orders.FindIndex(o => o == str);
                //Debug.Assert(index >= 0);

                //state = index;

                stack.Push(cur);
                UpdateStack();
            }
        }
    }

    public void SetText(string t)
    {
        if (t != text.text)
        {
            underline.Clear();
            text.text = t;


            var reg = new System.Text.RegularExpressions.Regex("[\\w]+ [\\w]+");

            //foreach (System.Text.RegularExpressions.Match match in reg.Matches(t))
            {
                foreach (var recipe in recipes.Keys)
                {
                    if (recipe.Contains(" "))
                    {
                        reg = new System.Text.RegularExpressions.Regex(recipe);
                        if (reg.IsMatch(t))
                        {
                            var match = reg.Match(t);
                            //underline.Add(match.Index, match.Length);
                            var firstWord = recipe.Split(' ')[0];
                            underline.Add(match.Index, firstWord.Length);
                            //underline.Add(match.Index + firstWord.Length, 1);//this causes bugs
                            underline.Add(match.Index + firstWord.Length + 1, match.Length - firstWord.Length - 1);
                        }
                    }
                }
            }


            reg = new System.Text.RegularExpressions.Regex("[\\w]+");

            foreach (System.Text.RegularExpressions.Match match in reg.Matches(t))
            {
                foreach(var recipe in recipes.Keys)
                {
                    if (match.Value == recipe)
                    {
                        underline.Add(match.Index, match.Length);
                    }
                }
            }
        }
    }

    public void Back()
    {
        if (stack.Count > 0)
        {
            stack.Pop();
            UpdateStack();
        }
    }

    void UpdateStack()
    {
        if (stack.Count == 0)
        {
            header.text = "orders";

            if (orders.Count == 0)
            {
                SetText("there are no orders yet");
            }
            else
            {
                var s = string.Join(" \n", orders.ToArray());
                SetText(s + " ");
            }

            footer.gameObject.SetActive(false);
        }
        else
        {
            var str = stack.Peek();
            header.text = str;
            SetText(recipes[str]);
            footer.gameObject.SetActive(true);
        }
    }

    //public void OnServed(string order)
    public void OnServed(List<string> order)
    {
        for(var i=0;i<orders.Count;i++)
        {
            if ((order.Count == 1 && order[0] == orders[i]) ||
                ((order.Count == 2 && ((order[0] + " " + order[1])) == orders[i])) ||
                ((order.Count == 2 && ((order[1] + " " + order[0])) == orders[i]))
                )
            {
                orders.RemoveAt(i);
                break;
            }
        }

        stack.Clear();
        UpdateStack();
    }

    public void AddRecipeOfTheDay(int dayNo)
    {
        if (dayNo == 2)
        {
            recipes.Add("tomato sauce", "blend tomato ");
            recipes.Add("dressing", "mix oil vinegar ");
        }
        if (dayNo == 3)
        {
            recipes.Add("pasta", "combine flour and water\nthen process ");
        }
        if (dayNo == 4)
        {
            recipes["tomato sauce"] = "chop and mince and whack tomato ";
            recipes["pasta"] = "combine flour and water\nthen chop and mince and whack ";
        }
    }
}
