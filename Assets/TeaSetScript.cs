using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using KModkit;
using Rnd = UnityEngine.Random;

public class TeaSetScript : MonoBehaviour
{
    static int _moduleIdCounter = 1;
    int _moduleID = 0;

    public KMBombModule Module;
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] Ingredients;
    public KMSelectable Lid, Retry, Speaker, Touch;
    public Image[] DialoguePointers, PeopleRends, Thumbs;
    public Image FullView, Pencil, StatusBottom, StageIndicator, StatusTop, Teapot;
    public Sprite[] FullViewSprites, IngredientSprites, LidSprites, PencilSprites, PeopleSprites, RetrySprites, StageSprites, StatusSprites, TeapotSprites, ThumbSprites;
    public Text BottomText, TopText;
    public GameObject[] ScreenParents;

    private KMAudio.KMAudioRef Sound;
    private Coroutine BlipCoroutine, LidTeeterCoroutine, PresentDialogueCoroutine, TeaPotBulgeCoroutine;
    private static readonly List<Recipe> Recipes = new List<Recipe>()
    {
        new Recipe("Citrus Classic", new List<int>(){ 0, 1, 2 }, new List<string>(){ "happy1 Oh, it's a little sour and a little\nsweet. Yum.", "happy0 I certainly have to tip my hat to\nthis tea's drinkability. I imagine it\nwould be delicious iced, as well.", "happy1 Yep, and since it's nice and light,\nI'm sure just about anyone would\nbe happy to have a cup.", "happy0 Mmm, yes. I myself am partial to\nits lovely fruity aftertaste." }),
        new Recipe("Oasis Berry", new List<int>(){ 0, 1, 1 }, new List<string>(){ "happy1 Deeeelicious! I just love how sweet\nthis tea is, Professor!", "happy0 Well, it's a bit too sweet for me,\nbut I understand why so many\nchildren enjoy it.", "happy1 I think I could drink a whole pot of\nthis stuff by myself.\nJust watch me!", "happy0 Ha ha ha! You've got quite the\nsweet tooth, Luke. Just try not to\ndrink it too fast." }),
        new Recipe("Sugar Smoke", new List<int>(){ 0, 2, 7 }, new List<string>(){ "happy0 Hmm. What an interesting blend of\nfruitiness and subtle smokiness.", "happy1 I've heard you can use the tea\nleaves from this blend to predict\nyour love life.", "happy0 I'm impressed, Luke. It heartens\nme that you take such interest\nin the blending of tea.", "happy1 But of course, Professor! I\ncouldn't rightly call myself your\napprentice if I didn't!" }),
        new Recipe("Root Remedy", new List<int>(){ 0, 4, 5 }, new List<string>(){ "happy0 This tea seems like just the thing\nfor driving away the chills when\nyou feel a cold coming on.", "sad1 Maybe so, Professor, but it's a\nlittle bitter for my tastes.", "happy0 Now then Luke, no good medicine\nis ever easy to drink! That's\nhow you know this is effective.", "happy1 Oh, I think you might be on to\nsomething there, Professor!\nI feel all warm inside now!" }),
        new Recipe("Cherry Boost", new List<int>(){ 0, 6, 6 }, new List<string>(){ "happy1 Ooh, spicy! My whole mouth is\ntingly now!", "happy0 Yes, there's quite a healthy dose\nof Peppercherry in there. No\ndoubt about it.", "happy1 I kind of like it. It makes me feel\nvery chirpy. Next time I'm feeling\ndown, I know what to reach for.", "happy0 Yes, a good cup of Cherry Boost\ncan be quite nice every once in\na while." }),
        new Recipe("Bitter Fruit", new List<int>(){ 1, 2, 5 }, new List<string>(){ "happy0 Oh, this is simply exquisite.\nThere's a depth of flavour here\nthat is lacking in most other teas.", "sad1 Really? Gosh, maybe my tongue's\njust broken or something. I don't\nsee what all the fuss is about...", "happy0 Ha ha! You may be a bit young for\nthis one. It's what some might call\na tea for the experienced palate.", "sad1 Well, I don't know about that.\nAs far as I'm concerned, it's too\nbitter to be tasty." }),
        new Recipe("Dream Spice", new List<int>(){ 1, 3, 6 }, new List<string>(){ "happy0 My, what a peculiar blend this is.\nSubtle and spicy all at once.", "happy1 And sweet at the end as well.", "happy0 I especially like how the initial\nheat helps to balance out the\nsweetness that comes later.", "happy1 I never imagined spicy and sweet\ncould work together so well." }),
        new Recipe("Clover Quencher", new List<int>(){ 1, 4, 7 }, new List<string>(){ "happy0 Impressive. One sip and I feel\nrejuvinated. It's smoky, yet it\nhas a wonderful sweetness to it.", "happy1 This is a whole new flavour for\nme, Professor. I've never tried anything quite like it.", "happy0 It's interesting, and I'm sure it's\ngood for when you've worked up a\nthirst, but it's not quite my thing.", "happy1 Really? Well, I for one think it's\nrather tasty." }),
        new Recipe("Radiance Blend", new List<int>(){ 2, 3, 4 }, new List<string>(){ "happy1 Mmm, now this smells nice.", "happy0 Indeed. There's something about\nthe aroma of this tea that's quite\nsoothing.", "sad1 You hit the nail on the head,\nProfessor. I think it's making me\nsleepy. Yaaaaaawn...", "happy0 I suppose that's further proof of\nits relaxing effect. It does seem -\nyaaaawn - rather potent, eh?" }),
        new Recipe("Cayenne Twilight", new List<int>(){ 3, 4, 6 }, new List<string>(){ "happy0 Ahhh, just take in that aroma.\nIt's incredibly relaxing!", "happy1 Sort of makes you forget all your\ntroubles for a minute, eh,\nProfessor?", "happy0 I must confess, this is rather nice.\nIt's like the tea drove all the\nweariness from me.", "happy1 Wow, all that with just a couple of\nsips? The good a cup of herb tea\ncan do never fails to amaze me!" }),
        new Recipe("Cinder Flower", new List<int>(){ 3, 5, 7 }, new List<string>(){ "happy1 Um... What's this flavour? It's a\nbit...odd.", "happy0 It certainly has a distinct taste,\nbut you know, Luke, the more I\ndrink it, the more I like it.", "sad1 If you're that fond of it, you're\nmore than welcome to drink my\ncup as well.", "happy0 Sorry to hear that Cinder Flower\nisn't your...erm...cup of tea,\nLuke." }),
        new Recipe("The Layton Elixir", new List<int>(){ 4, 6, 7 }, new List<string>(){ "happy1 Wow! This tea is out of this world!", "sad0 I agree, Luke. I'm amazed that\nit's possible to make something\nlike this with common ingredients.", "sad1 The world's just full of\nmysteries like that, isn't it,\nProfessor?", "sad0 Indeed, my boy. Well, consider\nme mystified..." }),
    };
    private List<Recipe> Solution = new List<Recipe>();
    private Recipe RecognisedRecipe;
    private float DefaultGameMusicVolume;
    private List<Image> IngredientHighlights = new List<Image>();
    private List<Person> People = new List<Person>() { new Person("layton", 0, new List<string>() { "happy", "sad" }), new Person("luke", 1, new List<string>() { "happy", "sad", "angry" }) };
    private List<List<List<int>>> Responses = new List<List<List<int>>>();
    private List<int> AddedIngredients = new List<int>();
    private List<int> Order = new List<int>(); //Keeps track of which ingredients are on the bottom screen in reading order
    private List<string> DialogueList = new List<string>();
    private int CurrentStatement, Stage;
    private static readonly string[][] NegativeResponses = new string[][] { new[] { "sad1 This tea smells a bit, well, funny.\nAre you sure we can drink this,\nProfessor?", "happy0 Why not take a sip and find out\nfor yourself?", "angry1 Eww! I think I'll give this one a\nmiss.", "sad0 Where did I go wrong? I was so sure\nI'd brewed something good, too." },
new[] { "sad1 Well...", "sad0 Well...", "sad1 Um, Professor? I think I'm going to\nthrow up.", "sad0 I'm not surprised. This is really\nnot the kind of tea that a\ngentleman should be making." },
new[] { "sad0 This is just terrible.", "sad1 Sorry, Professor. I don't think I\ncan choke down another sip.", "sad0 That's quite all right, Luke. No one\nshould ever feel obliged to drink\nsomething as rank as this tea.", "sad1 Everything looked good going in.\nI wonder where you went wrong." },
new[] { "angry1 Professor, this tea is...foaming!", "sad0 Oh, dear. I suppose that's a clear\nsign that this tea isn't fit for\nhuman consumption, then.", "sad1 I don't understand. We checked all\nthe ingredients. Why all this foam?", "sad0 I'm afraid I have no answer there,\nLuke. But perhaps we can use it\nto clean the floor." },
new[] { "sad0 Oh, this won't do at all.", "sad1 Ugh, this tea is making me sad.\nOne sip was enough to make me\nwant to cry.", "sad0 I don't think I've ever come out\nof tea time wearing anything but\na smile...until now.", "sad1 Believe me, Professor, you're not\nthe only one frowning." },
new[] { "angry1 P-Professor! There's smoke\ncoming out of this tea!", "sad0 We may have unwittingly invented\na drink that should never have\nbeen.", "sad1 I'm no coward, but I'm not brave\nenough to drink any of that stuff.", "sad0 Well then, our course of action is\nclear. Let's seal this recipe away\nand never make it again." },
new[] { "happy1 Professor...this tea is...peculiar.", "happy0 I admit, I can't seem to find a more\nappropriate word for this blend\nthan peculiar.", "sad1 What is that taste anyway?\nOld shoe?", "sad0 All right, I get the picture, Luke.\nLet's rip up the recipe and never\nmake it again." },
new[] { "sad0 Interesting. I've never seen tea\ncongeal before.", "sad1 It looks more like pudding than\ntea, Professor. We might need a\nspoon to drink it.", "sad0 The individual ingredients we used\nseem fine. Technically speaking,\nwe should be able to drink it.", "angry1 Er...None for me, thanks." },
new[] { "sad1 Professor, this doesn't look like\ntea at all. If I didn't know better,\nI'd swear we've brewed some mud.", "sad0 It is rather...viscous. How in the\nworld did this happen?", "sad1 We must have brewed a few\nincompatible ingredients together.", "happy0 You're quite right there, Luke.\nNext time, let's try a different\ncombination, shall we?" },
new[] { "sad0 Oh, dear...", "sad1 Professor, is this tea supposed to\nlook so...erm, brown and murky?", "sad0 I hate to say it, but I think I may\nbe wide of the mark with this\nblend. It's completely unpalatable.", "happy1 Let's write this one down so we\ndon't ever try to make it again." },
new[]{ "happy0 The real beauty of tea is the way\ndifferent flavours come together.\nTry using a variety of ingredients.", "sad1 Yeah. We put in lots of the same\ningredient, but all it made was a\nboring, one-note tea." } };
    private static readonly string[] PassiveResponses = new string[] { "0 This should do nicely.", "0 Here's our first ingredient.", "0 What to mix with this? Hmm...", "0 Hmm... What to make with this?", "0 That's an excellent first choice.", "1 What would go nicely with that?", "1 This is a good place to start.", "1 Now, which one should we add\nnext?", "1 This is a great first choice!", "1 I've got a tasty idea!", "0 Let's see how these ingredients\nblend together.", "0 Into the pot with this one.", "0 Be it puzzles or brewing tea,\ncreative thinking pays off.", "0 Ah, we've got a fascinating\ncombination on our hands now.", "0 An interesting selection of herbs!", "1 Do you suppose these ingredients\nwill go well together?", "1 That's two down. But which\ningredient do we finish with?", "1 Gosh, I didn't see that combination\ncoming.", "1 Hopefully those two mix together\nwell!", "1 What to add in next...?", "0 I do believe we've assembled a\nrather delectable blend.", "0 I'm anxious to see how this tea\ncomes out. Let's put the lid on\nand start brewing.", "0 What kind of tea will this make?\nI can hardly contain my excitement.", "0 I'm quite proud of this concoction.\nI hope the tea tastes as good as\nit looks.", "0 Well, that takes care of the\ningredients. Now all we need to\ndo is start brewing!", "1 This tea's going to be great!\nI'd bet my hat on that!", "1 Now all we need to do is slap the\nlid on our pot and start brewing!", "1 As long as this tea is drinkable,\nI'll be as happy as a clam.", "1 My fingers are crossed on this\none! Now let's put the lid on\nthe pot and start brewing!", "1 I sure hope this blend makes\na tasty pot of tea." };
    private static readonly string[] BottomTextOptions = new string[] { "Add ingredients to the teapot.", "Add two more ingredients to\nthe teapot.", "Add one more ingredient to\nthe teapot.", "Put the lid on the teapot to start\nbrewing." };
    private static readonly string[] IngredientNames = new string[] { "Oasis Leaf", "Brisk Berry", "Citronia Seed", "Dream Fluff", "Joy Root Clover", "Tonic Flower", "Peppercherry", "Cinder Horse" };
    private string CurrentLine;
    private bool CannotPress, Focused, InDialogue, Muted, Solved, Talking;

    private class Person
    {
        public string Name;
        public int Ix;
        public List<string> Poses;

        public Person(string name, int ix, List<string> poses)
        {
            Name = name;
            Ix = ix;
            Poses = poses;
        }
    }

    private class Recipe
    {
        public string Name;
        public List<int> Ingredients;
        public List<string> Dialogue;

        public Recipe(string name, List<int> ingredients, List<string> dialogue)
        {
            Name = name;
            Ingredients = ingredients;
            Dialogue = dialogue;
        }
    }

    private string GetBottomText()
    {
        if (AddedIngredients.Count() < 3)
            return BottomTextOptions[AddedIngredients.Count()];
        if (RecognisedRecipe != null)
            return RecognisedRecipe.Name;
        return BottomTextOptions[3];
    }

    private Recipe FindRecipe(List<int> ingredients)
    {
        var ingredList = Recipes.Select(x => x.Ingredients).ToList();
        ingredients.Sort();
        if (ingredients.Count() != 3)
            return null;
        for (int i = 0; i < ingredList.Count(); i++)
        {
            bool success = true;
            for (int j = 0; j < 3; j++)
                if (ingredients[j] != ingredList[i][j])
                    success = false;
            if (success)
                return Recipes[i];
        }
        return null;
    }

    void Awake()
    {
        _moduleID = _moduleIdCounter++;
        BottomText.text = GetBottomText();
        try
        {
            DefaultGameMusicVolume = GameMusicControl.GameMusicVolume;
        }
        catch (Exception) { }
        Bomb.OnBombExploded += delegate
        {
            try { GameMusicControl.GameMusicVolume = DefaultGameMusicVolume; } catch (Exception) { }
            try { Sound.StopSound(); } catch (Exception) { }
        };
        Bomb.OnBombSolved += delegate
        {
            try { GameMusicControl.GameMusicVolume = DefaultGameMusicVolume; } catch (Exception) { }
            try { Sound.StopSound(); } catch (Exception) { }
        };
        Module.GetComponent<KMSelectable>().OnFocus += delegate { Focused = true; if (!Muted) { Sound = Audio.PlaySoundAtTransformWithRef("music", transform); try { GameMusicControl.GameMusicVolume = 0; } catch (Exception) { } } };
        Module.GetComponent<KMSelectable>().OnDefocus += delegate { Focused = false; if (Sound != null) Sound.StopSound(); try { GameMusicControl.GameMusicVolume = DefaultGameMusicVolume; } catch (Exception) { } };
        Module.OnActivate += delegate { StartCoroutine(InitAnimationHinge()); };
        Order = Enumerable.Range(0, 8).ToList();
        Order.Shuffle();
        for (int i = 0; i < Ingredients.Length; i++)
        {
            int x = i;
            IngredientHighlights.Add(Ingredients[x].GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First());
            IngredientHighlights[x].transform.localScale = Vector3.zero;
            Ingredients[x].OnInteract += delegate { IngredientPress(x); return false; };
            Ingredients[x].OnHighlight += delegate { IngredientHighlights[x].transform.localScale = Vector3.one; if (!CannotPress) { FullView.color = Color.white; FullView.sprite = FullViewSprites[x]; BottomText.text = ""; } };
            Ingredients[x].OnHighlightEnded += delegate { IngredientHighlights[x].transform.localScale = Vector3.zero; if (!CannotPress) { FullView.color = Color.clear; BottomText.text = GetBottomText(); } };
            Ingredients[x].transform.localPosition = new Vector3(new[] { -101.5f, -72.5f, -43.5f, -14.5f, 14.5f, 43.5f, 72.5f, 101.5f }[Order.IndexOf(x)], -51.5f, 0);
        }

        var hlRetry = Retry.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First();
        hlRetry.transform.localScale = Vector3.zero;
        Retry.OnInteract += delegate { RetryPress(); return false; };
        Retry.OnInteractEnded += delegate { Retry.GetComponent<Image>().sprite = RetrySprites[0]; };
        Retry.OnHighlight += delegate { hlRetry.transform.localScale = Vector3.one; };
        Retry.OnHighlightEnded += delegate { hlRetry.transform.localScale = Vector3.zero; };

        var hlLid = Lid.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First();
        hlLid.transform.localScale = Vector3.zero;
        Lid.OnInteract += delegate { LidPress(); return false; };
        Lid.OnHighlight += delegate { hlLid.transform.localScale = Vector3.one; };
        Lid.OnHighlightEnded += delegate { hlLid.transform.localScale = Vector3.zero; };

        var hlSpeaker = Speaker.GetComponentsInChildren<SpriteRenderer>().Where(y => y.name == "Highlight").First();
        hlSpeaker.transform.localScale = Vector3.zero;
        Speaker.OnInteract += delegate { SpeakerPress(); return false; };
        Speaker.OnHighlight += delegate { hlSpeaker.transform.localScale = Vector3.one; };
        Speaker.OnHighlightEnded += delegate { hlSpeaker.transform.localScale = Vector3.zero; };
        var xSpeaker = Speaker.GetComponentsInChildren<SpriteRenderer>().Where(y => y.name == "X").First();
        xSpeaker.transform.localScale = Vector3.zero;

        var hlTouch = Touch.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First();
        hlTouch.transform.localScale = Vector3.zero;
        Touch.OnHighlight += delegate { hlTouch.transform.localScale = Vector3.one; };
        Touch.OnHighlightEnded += delegate { hlTouch.transform.localScale = Vector3.zero; };
        Touch.OnInteract += delegate { TouchPress(); return false; };
        for (int i = 0; i < Thumbs.Length; i++)
            Thumbs[i].color = Color.clear;
        FullView.color = Color.clear;
        StatusBottom.transform.localScale = Vector3.zero;
        StatusTop.transform.localScale = Vector3.zero;
        ScreenParents[0].transform.localEulerAngles = new Vector3(0, 90, -90);
        ScreenParents[1].transform.localEulerAngles = new Vector3(0, -90, -90);
        ScreenParents[0].transform.parent.localPosition = new Vector3(ScreenParents[0].transform.parent.localPosition.x, -0.001f, ScreenParents[0].transform.parent.localPosition.z);
        for (int i = 0; i < 2; i++)
            DialoguePointers[i].color = Color.clear;
        StartCoroutine(PencilAnim());
    }

    // Use this for initialization
    void Start()
    {
        Touch.transform.localScale = Vector3.zero;
        Calculate();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Calculate()
    {
        for (int i = 0; i < 8; i++)
        {
            Responses.Add(new List<List<int>>());
            for (int j = 0; j < 8 - i; j++)
            {
                Responses[i].Add(new List<int>());
                for (int k = 0; k < 8 - i - j; k++)
                {
                    if (j == 0 && k == 0)
                        Responses[i][j].Add(10);
                    else
                        Responses[i][j].Add(Rnd.Range(0, 10));
                }
            }
        }

        var nums = Bomb.GetSerialNumberNumbers().Select(x => x % 8).ToList();
        if (nums.Count() == 2)
            nums.Add(Order.First());
        if (nums.Count() == 4)
            nums = nums.Take(3).ToList();
        while (FindRecipe(nums.ToList()) != null || (nums[0] == nums[1] && nums[1] == nums[2]))
            nums[2] = (nums[2] + 1) % 8;
        Debug.LogFormat("[The Tea Set #{0}] The first three ingredients that you should enter are {1}.", _moduleID, IngredientNames[nums[0]] + ", " + IngredientNames[nums[1]] + " and " + IngredientNames[nums[2]]);
        nums.Sort();
        var nValues = new List<int>() { Responses[nums[0]][nums[1] - nums[0]][nums[2] - nums[1]] };
        Debug.LogFormat("[The Tea Set #{0}] This will cause Layton and Luke to give response number {1}.", _moduleID, nValues[0].ToString());
        for (int i = 0; i < 2; i++)
        {
            var sums = (i == 0 ? new List<int>() { Order.Take(4).Sum() + 4, Order.Take(3).Sum() + 3, Order.Take(2).Sum() + 2 } : new List<int>() { Order.Skip(4).Sum() + 4, Order.Skip(5).Sum() + 3, Order.Skip(6).Sum() + 2 }).Select(x => (x + nValues[i]) % 8).ToList();
            while (FindRecipe(sums.ToList()) != null || (sums[0] == sums[1] && sums[1] == sums[2]))
                sums[2] = (sums[2] + 1) % 8;
            Debug.LogFormat("[The Tea Set #{0}] The {1} three ingredients that you should enter are {2}.", _moduleID, i == 0 ? "second" : "third", IngredientNames[sums[0]] + ", " + IngredientNames[sums[1]] + " and " + IngredientNames[sums[2]]);
            sums.Sort();
            nValues.Add(Responses[sums[0]][sums[1] - sums[0]][sums[2] - sums[1]]);
            Debug.LogFormat("[The Tea Set #{0}] This will cause Layton and Luke to give response number {1}.", _moduleID, nValues[i + 1].ToString());
        }
        Debug.LogFormat("[The Tea Set #{0}] Now for the edgework conditions.", _moduleID);
        if (!Bomb.GetOnIndicators().Contains("TRN"))
        {
            if (Bomb.GetBatteryHolderCount() == 2)
            {
                nValues[0] += 2;
                Debug.LogFormat("[The Tea Set #{0}] The bomb has exactly 2 battery holders, so 2 has been added to N0.", _moduleID);
            }
            if (Bomb.GetIndicators().Count() == 2)
            {
                nValues[1] += 2;
                Debug.LogFormat("[The Tea Set #{0}] The bomb has exactly 2 indicators, so 2 has been added to N1.", _moduleID);
            }
            if (Bomb.GetPortPlateCount() == 2)
            {
                nValues[2] += 2;
                Debug.LogFormat("[The Tea Set #{0}] The bomb has exactly 2 port plates, so 2 has been added to N2.", _moduleID);
            }
            for (int i = 0; i < 3; i++)
            {
                if (Order[i] == 7)
                {
                    nValues[i] += 3 + i;
                    Debug.LogFormat("[The Tea Set #{0}] The {1} digit of I is 8, so {2} has been added to N{3}.", _moduleID, new[] { "first", "second", "third" }[i], (3 + i).ToString(), i.ToString());
                }
                else if (Order[i] > 3)
                {
                    nValues[i] += 3;
                    Debug.LogFormat("[The Tea Set #{0}] The {1} digit of I is greater than 4, so 3 has been added to N{2}.", _moduleID, new[] { "first", "second", "third" }[i], i.ToString());
                }
                if (Bomb.GetSerialNumberLetters().Any(x => "AEIOU".Contains(x)))
                {
                    nValues[i] += 1 + i;
                    Debug.LogFormat("[The Tea Set #{0}] The serial number contains a vowel, so {1} has been added to N{2}.", _moduleID, (1 + i).ToString(), i.ToString());
                }
            }
        }
        else
            Debug.LogFormat("[The Tea Set #{0}] A lit TRN indicator is on the bomb, so the conditions have been skipped.", _moduleID);
        nValues = nValues.Select(x => x % 12).ToList();
        while (nValues[0] == nValues[1] || nValues[0] == nValues[2] || nValues[1] == nValues[2])
        {
            if (nValues[1] == nValues[2])
                nValues[1] = (nValues[1] + 1) % 12;
            if (nValues[0] == nValues[1] || nValues[0] == nValues[2])
                nValues[0] = (nValues[0] + 1) % 12;
        }
        Solution = nValues.Select(x => Recipes[x]).ToList();
        Debug.LogFormat("[The Tea Set #{0}] The recipes needed to solve the module are {1}.", _moduleID, Solution[0].Name + ", " + Solution[1].Name + " and " + Solution[2].Name);
    }

    void IngredientPress(int pos)
    {
        if (AddedIngredients.Count() < 3 && !CannotPress)
        {
            Audio.PlaySoundAtTransform("select", Ingredients[pos].transform);
            Ingredients[pos].AddInteractionPunch(0.5f);
            if (TeaPotBulgeCoroutine != null)
                StopCoroutine(TeaPotBulgeCoroutine);
            TeaPotBulgeCoroutine = StartCoroutine(TeaPotBulge());
            AddedIngredients.Add(pos);
            Lid.GetComponent<Image>().sprite = LidSprites[0];
            Thumbs[AddedIngredients.Count() - 1].color = Color.white;
            Thumbs[AddedIngredients.Count() - 1].sprite = ThumbSprites[pos];
            TopText.alignment = TextAnchor.UpperLeft;
            if (PresentDialogueCoroutine != null)
                StopCoroutine(PresentDialogueCoroutine);
            if (BlipCoroutine != null)
                StopCoroutine(BlipCoroutine);
            PresentDialogueCoroutine = StartCoroutine(PresentDialogue(PassiveResponses[Rnd.Range(0, 10) + ((AddedIngredients.Count() - 1) * 10)], false));
            if (AddedIngredients.Count() == 3)
            {
                RecognisedRecipe = FindRecipe(AddedIngredients);
                if (RecognisedRecipe != null)
                    Audio.PlaySoundAtTransform("known", Lid.transform);
                else
                    Audio.PlaySoundAtTransform("unknown", Lid.transform);
                LidTeeterCoroutine = StartCoroutine(LidTeeter());
            }
        }
    }

    void RetryPress()
    {
        if (!CannotPress)
        {
            Retry.GetComponent<Image>().sprite = RetrySprites[1];
            Lid.GetComponent<Image>().sprite = LidSprites[0];
            if (LidTeeterCoroutine != null)
                StopCoroutine(LidTeeterCoroutine);
            AddedIngredients = new List<int>();
            for (int i = 0; i < Thumbs.Length; i++)
                Thumbs[i].color = Color.clear;
            Audio.PlaySoundAtTransform("retry", Retry.transform);
            Retry.AddInteractionPunch();
            if (PresentDialogueCoroutine != null)
                StopCoroutine(PresentDialogueCoroutine);
            if (BlipCoroutine != null)
                StopCoroutine(BlipCoroutine);
            BottomText.text = GetBottomText();
            TopText.alignment = TextAnchor.MiddleCenter;
            TopText.text = "Combine three ingredients to brew\na tasty pot of tea!";
            for (int i = 0; i < 2; i++)
                DialoguePointers[i].color = Color.clear;
        }
    }

    void LidPress()
    {
        if (!CannotPress && AddedIngredients.Count() == 3)
        {
            CannotPress = true;
            Audio.PlaySoundAtTransform("lid", Lid.transform);
            Lid.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First().color = Color.clear;
            Retry.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First().color = Color.clear;
            for (int i = 0; i < 8; i++)
                Ingredients[i].GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First().color = Color.clear;
            StartCoroutine(Brew());
            Lid.AddInteractionPunch(0.5f);
        }
    }

    void SpeakerPress()
    {
        Muted = !Muted;
        Audio.PlaySoundAtTransform("select", Speaker.transform);
        Speaker.AddInteractionPunch();
        if (Muted)
        {
            Speaker.GetComponentsInChildren<SpriteRenderer>().Where(y => y.name == "X").First().transform.localScale = Vector3.one;
            if (Sound != null)
                Sound.StopSound();
            try { GameMusicControl.GameMusicVolume = DefaultGameMusicVolume; } catch (Exception) { }
        }
        else
        {
            Speaker.GetComponentsInChildren<SpriteRenderer>().Where(y => y.name == "X").First().transform.localScale = Vector3.zero;
            Sound = Audio.PlaySoundAtTransformWithRef("music", transform);
            try { GameMusicControl.GameMusicVolume = 0; } catch (Exception) { }
        }
    }

    void TouchPress()
    {
        Touch.AddInteractionPunch(0.5f);
        if (Talking)
        {
            StopCoroutine(PresentDialogueCoroutine);
            var text = CurrentLine.Split(' ').ToList();
            text.RemoveAt(0);
            var line = text.Join(" ");
            TopText.text = line;
            StopCoroutine(BlipCoroutine);
            Talking = false;
        }
        else if (CurrentStatement < DialogueList.Count() - 1)
            AdvanceDialogue();
        else
            ReInit();
    }
    void AdvanceDialogue()
    {
        CurrentStatement++;
        PresentDialogueCoroutine = StartCoroutine(PresentDialogue(DialogueList[CurrentStatement]));
    }

    void ReInit()
    {
        AddedIngredients = new List<int>();
        BottomText.text = GetBottomText();
        Touch.transform.localScale = Vector3.zero;
        PeopleRends[0].sprite = PeopleSprites[0];
        PeopleRends[1].sprite = PeopleSprites[2];
        for (int i = 0; i < Thumbs.Length; i++)
            Thumbs[i].color = Color.clear;
        StatusBottom.transform.localScale = Vector3.zero;
        StatusTop.transform.localScale = Vector3.zero;
        for (int i = 0; i < 2; i++)
            DialoguePointers[i].color = Color.clear;
        Lid.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First().color = Color.white;
        Retry.GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First().color = Color.white;
        for (int i = 0; i < 8; i++)
            Ingredients[i].GetComponentsInChildren<Image>().Where(y => y.name == "Highlight").First().color = Color.white;
        Lid.transform.localPosition = new Vector3(45.5f, -16.5f, 0);
        Audio.PlaySoundAtTransform("lid", Lid.transform);
        TopText.alignment = TextAnchor.MiddleCenter;
        TopText.text = "Combine three ingredients to brew\na tasty pot of tea!";
        StatusBottom.transform.localPosition = new Vector3(0, 4, 0);
        CannotPress = false;
        InDialogue = false;
    }

    private IEnumerator PresentDialogue(string line, bool usePose = true, float interval = 0.0125f)
    {
        Talking = true;
        CurrentLine = line;
        TopText.text = "";
        Person person = People[int.Parse(line.Split(' ').ToList().First().Last().ToString())];
        DialoguePointers[person.Ix].color = Color.white;
        DialoguePointers[1 - person.Ix].color = Color.clear;
        int pose = usePose ? person.Poses.IndexOf(line.Split(' ').ToList().First().Substring(0, line.Split(' ').ToList().First().Length - 1)) + (person.Ix * 2) : person.Ix * 2;
        PeopleRends[person.Ix].sprite = PeopleSprites[pose];
        var text = line.Split(' ').ToList();
        text.RemoveAt(0);
        line = text.Join(" ");
        BlipCoroutine = StartCoroutine(DoBlips());
        foreach (char character in line.Substring(0, line.Length - 1))
        {
            TopText.text += character;
            float timer = 0;
            while (timer < interval)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
        TopText.text = line;
        StopCoroutine(BlipCoroutine);
        Talking = false;
    }

    private IEnumerator DoBlips(float interval = 0.08f)
    {
        while (true)
        {
            if (Focused)
                Audio.PlaySoundAtTransform("blip", TopText.transform);
            float timer = 0;
            while (timer < interval)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }

    private IEnumerator LidTeeter()
    {
        float timer = 0;
        while (timer < 0.5f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        Image rend = Lid.GetComponent<Image>();
        int i = 0;
        while (true)
        {
            rend.sprite = LidSprites[new[] { 1, 0, 2, 0 }[i]];
            i = (i + 1) % 4;
            timer = 0;
            while (timer < 0.11f)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }

    private IEnumerator TeaPotBulge()
    {
        BottomText.text = "";
        for (int i = 0; i < 4; i++)
        {
            Teapot.sprite = TeapotSprites[i];
            float timer = 0;
            while (timer < 0.125f)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
        Teapot.sprite = TeapotSprites[0];
    }

    private IEnumerator Brew()
    {
        Debug.LogFormat("[The Tea Set #{0}] You added {1}.", _moduleID, IngredientNames[AddedIngredients[0]] + ", " + IngredientNames[AddedIngredients[1]] + " and " + IngredientNames[AddedIngredients[2]]);
        Lid.transform.localPosition = new Vector3(0.5f, 14.5f, 0);
        if (PresentDialogueCoroutine != null)
            StopCoroutine(PresentDialogueCoroutine);
        if (BlipCoroutine != null)
            StopCoroutine(BlipCoroutine);
        if (RecognisedRecipe != null)
        {
            BottomText.text = "Now brewing a pot of\n" + RecognisedRecipe.Name + ".";
            Debug.LogFormat("[The Tea Set #{0}] These ingredients make a pot of {1}.", _moduleID, RecognisedRecipe.Name);
        }
        else
        {
            BottomText.text = "Wait just a moment...";
            Debug.LogFormat("[The Tea Set #{0}] These ingredients don't make a recipe.", _moduleID);
        }
        TopText.alignment = TextAnchor.MiddleCenter;
        for (int i = 0; i < 2; i++)
            DialoguePointers[i].color = Color.clear;
        TopText.text = "Oh, the suspense! What will this\ntea taste like?";
        if (LidTeeterCoroutine != null)
            StopCoroutine(LidTeeterCoroutine);
        Lid.GetComponent<Image>().sprite = LidSprites[0];
        float timer = 0;
        while (timer < 1f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        StatusBottom.transform.localScale = Vector3.one;
        for (int i = 0; i < 3; i++)
        {
            if (Focused)
                Audio.PlaySoundAtTransform("brewing", StatusBottom.transform);
            if (Focused && i == 1)
                Audio.PlaySoundAtTransform("pour", StatusBottom.transform);
            StatusBottom.sprite = StatusSprites[i];
            float timer2 = 0;
            while (timer2 < 0.7f)
            {
                yield return null;
                timer2 += Time.deltaTime;
            }
        }
        TopText.text = "";
        if (RecognisedRecipe == null)
        {
            BottomText.text = "Your tea was no good.\nWhat a pity.";
            if (Focused)
                Audio.PlaySoundAtTransform("eww", StatusBottom.transform);
            StatusBottom.sprite = StatusSprites[3];
            PeopleRends[0].sprite = PeopleSprites[1];
            PeopleRends[1].sprite = PeopleSprites[3];
        }
        else
        {
            if (!Solved)
            {
                if (RecognisedRecipe.Name != Solution[Stage].Name)
                {
                    Module.HandleStrike();
                    yield return "strike";
                    Debug.LogFormat("[The Tea Set #{0}] That recipe isn't correct. Strike!", _moduleID);
                }
                else
                {
                    Stage++;
                    StageIndicator.sprite = StageSprites[Stage];
                    if (Stage == 3)
                    {
                        Module.HandlePass();
                        yield return "solve";
                        Solved = true;
                        Debug.LogFormat("[The Tea Set #{0}] That recipe was correct. Module solved!", _moduleID);
                    }
                    else
                        Debug.LogFormat("[The Tea Set #{0}] That recipe is correct. Onto the {1} recipe!", _moduleID, new[] { "second", "third" }[Stage - 1]);
                }
            }
            BottomText.text = "Well done! One pot of tea coming\nright up!";
            if (Focused)
                Audio.PlaySoundAtTransform("yum", StatusBottom.transform);
            StatusBottom.sprite = StatusSprites[4];
            PeopleRends[0].sprite = PeopleSprites[0];
            PeopleRends[1].sprite = PeopleSprites[2];
        }
        timer = 0;
        while (timer < 1.5f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        timer = 0;
        float duration = 0.25f;
        bool startedUpper = false;
        BottomText.text = "";
        if (Focused)
            Audio.PlaySoundAtTransform("move status", StatusBottom.transform);
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            StatusBottom.transform.localPosition = new Vector3(0, Mathf.FloorToInt(Mathf.Lerp(4, 132, timer / duration)), 0);
            if (StatusBottom.transform.localPosition.y >= 60 && !startedUpper)
            {
                startedUpper = true;
                StartCoroutine(BrewUpper());
            }
        }
        StatusBottom.transform.localPosition = new Vector3(0, 132, 0);
    }

    private IEnumerator BrewUpper()
    {
        StatusTop.sprite = StatusBottom.sprite;
        StatusTop.transform.localPosition = new Vector3(0, -131, 0);
        StatusTop.transform.localScale = Vector3.one;
        float timer = 0;
        float duration = (0.25f / 138) * 156;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            StatusTop.transform.localPosition = new Vector3(0, Mathf.FloorToInt(Mathf.Lerp(-131, 25, timer / duration)), 0);
        }
        StatusTop.transform.localPosition = new Vector3(0, 25, 0);
        timer = 0;
        while (timer < 0.4f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        if (RecognisedRecipe != null)
            BottomText.text = RecognisedRecipe.Name;
        else
            BottomText.text = "Unfortunately, you failed to make\nanything worth drinking.";
        InDialogue = true;
        Touch.transform.localScale = Vector3.one;
        CurrentStatement = 0;
        AddedIngredients.Sort();
        TopText.alignment = TextAnchor.UpperLeft;
        if (RecognisedRecipe == null)
        {
            DialogueList = NegativeResponses[Responses[AddedIngredients[0]][AddedIngredients[1] - AddedIngredients[0]][AddedIngredients[2] - AddedIngredients[1]]].ToList();
            var temp = DialogueList.First().Split(' ').ToList();
            temp.RemoveAt(0);
            var person = DialogueList.First().Split(' ').ToList().First().Last();
            Debug.LogFormat("[The Tea Set #{0}] The first response is \"{1}\", which is response number {2}.", _moduleID, new[] { "Layton: ", "Luke: " }[Array.IndexOf(new[] { '0', '1' }, person)] + temp.Join(" ").Replace("\n", " "), Responses[AddedIngredients[0]][AddedIngredients[1] - AddedIngredients[0]][AddedIngredients[2] - AddedIngredients[1]]);
        }
        else
            DialogueList = RecognisedRecipe.Dialogue.ToList();
        PresentDialogueCoroutine = StartCoroutine(PresentDialogue(DialogueList[0]));
    }

    private IEnumerator InitAnimationHinge()
    {
        var hinge = ScreenParents[0].transform.parent;
        Audio.PlaySoundAtTransform("strain", StatusBottom.transform);
        float timer = 0;
        int alt = 0;
        while (timer < 0.5f)
        {
            yield return null;
            timer += Time.deltaTime;
            hinge.localPosition = new Vector3(alt == 0 ? 0.0005f : -0.0005f, hinge.localPosition.y, hinge.localPosition.z);
            alt = 1 - alt;
        }
        hinge.localPosition = new Vector3(0, hinge.localPosition.y, hinge.localPosition.z);
        timer = 0;
        while (timer < 0.5f)
        {
            yield return null;
            timer += Time.deltaTime;
        }
        Audio.PlaySoundAtTransform("open", StatusBottom.transform);
        StartCoroutine(OpenScreens());
    }

    private IEnumerator OpenScreens(float duration = 0.05f)
    {
        var hinge = ScreenParents[0].transform.parent;
        float timer = 0;
        while (timer < duration)
        {
            yield return null;
            timer += Time.deltaTime;
            ScreenParents[0].transform.localEulerAngles = Vector3.Lerp(new Vector3(0, 90, -90), new Vector3(0, 10, -90), timer / duration);
            ScreenParents[1].transform.localEulerAngles = Vector3.Lerp(new Vector3(0, -90, -90), new Vector3(0, -10, -90), timer / duration);
            hinge.transform.localPosition = new Vector3(hinge.transform.localPosition.x, Mathf.Lerp(-0.001f, -0.00053f, timer / duration), hinge.transform.localPosition.z);
        }
        ScreenParents[0].transform.localEulerAngles = new Vector3(0, 10, -90);
        ScreenParents[1].transform.localEulerAngles = new Vector3(0, -10, -90);
        hinge.transform.localPosition = new Vector3(hinge.transform.localPosition.x, -0.00053f, hinge.transform.localPosition.z);
    }

    private IEnumerator PencilAnim(float interval = 0.225f)
    {
        int ix = 0;
        while (true)
        {
            Pencil.sprite = PencilSprites[ix];
            ix = (ix + 1) % 3;
            float timer = 0;
            while (timer < interval)
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }
    }

#pragma warning disable 414
    private string TwitchHelpMessage = "Use '!{0} 123r*' to add the first, second and third ingredients in reading order, press the retry button, then press the lid. Use '!{0} advance' to skip the dialogue. Use '!{0} mute' to press the mute button.";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        if (command == "advance")
        {
            if (!InDialogue)
            {
                yield return "sendtochaterror There's no dialogue to advance!";
                yield break;
            }
            else
            {
                yield return null;
                while (InDialogue)
                {
                    Touch.OnInteract();
                    float timer = 0;
                    while (timer < 0.1f)
                    {
                        yield return null;
                        timer += Time.deltaTime;
                    }
                }
            }
        }
        else if (command == "mute")
        {
            yield return null;
            Speaker.OnInteract();
        }
        else
        {
            foreach (char character in command)
                if (!"12345678r*".Contains(character))
                {
                    yield return "sendtochaterror Invalid command.";
                    yield break;
                }
            if (CannotPress)
            {
                yield return "sendtochaterror I can't select those right now!";
                yield break;
            }
            yield return null;
            foreach (char character in command)
            {
                if ("12345678".Contains(character))
                    Ingredients[Order["12345678".IndexOf(character)]].OnInteract();
                else if (character == 'r')
                    Retry.OnInteract();
                else
                    Lid.OnInteract();
                float timer = 0;
                while (timer < 0.1f)
                {
                    yield return null;
                    timer += Time.deltaTime;
                }
            }
        }
    }

    IEnumerator TwitchHandleForcedSolve()
    {
        while (Stage < 3)
        {
            while (CannotPress && !InDialogue)
                yield return true;
            if (Stage == 3)
                break;
            while (InDialogue)
            {
                Touch.OnInteract();
                yield return true;
            }
            if (AddedIngredients.Count() > 0)
            {
                Retry.OnInteract();
                yield return true;
            }
            for (int i = 0; i < 3; i++)
                Ingredients[Solution[Stage].Ingredients[i]].OnInteract();
            Lid.OnInteract();
            yield return true;
        }
    }
}
