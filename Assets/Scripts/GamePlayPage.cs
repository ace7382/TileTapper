using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using TimeSpan = System.TimeSpan;

public class GamePlayPage : Page
{
    #region Private Variables

    private List<VisualElement>         guessButtons;
    private List<ButtonStateChanger>    guessButtonStateChangers;

    private int                         currentCorrectIndex;
    private IEnumerator                 processingGuess;
    private WaitForSeconds              processingPause;

    private LimitedQueue<List<int>>     previousLayouts;

    private TimeSpan                    noTapTimer;
    private TimeSpan                    playTimer;
    private TimeSpan                    oneSecond                   = new TimeSpan(0, 0, 1);
    private WaitForSeconds              oneSecondWait               = new WaitForSeconds(1f);

    private float                       eventChance;
    private LayoutEvent                 currentEvent;

    private float                       autoCorrectChance;

    #endregion

    #region Inherited Functions

    public override IEnumerator AnimateIn()
    {
        previousLayouts     = new LimitedQueue<List<int>>(10);
        eventChance         = 0f;
        currentEvent        = new LayoutEvent();

        autoCorrectChance   = 0f;

        GetUIReferences();
        ResetGuessButtons();

        noTapTimer          = new TimeSpan(0, 3, 0);
        playTimer           = new TimeSpan(0, 0, 0);

        PageManager.instance.StartCoroutine(NoTapsFor3Minutes());
        PageManager.instance.StartCoroutine(PlayTimer());

        return null;
    }

    public override IEnumerator AnimateOut()
    {
        return null;
    }

    public override void HidePage()
    {
        for (int i = 0; i < guessButtons.Count; i++)
        {
            //guessButtons[i].UnregisterCallback<PointerDownEvent>(GuessButtonDown);
            //guessButtons[i].UnregisterCallback<PointerUpEvent>(GuessButtonUp);
            //guessButtons[i].UnregisterCallback<PointerLeaveEvent>(GuessButtonOff);
        }

        uiDoc.rootVisualElement.Q<Label>("Instructions").UnregisterCallback<ClickEvent>(ObjectiveManager.instance.InstructionsTapped);
    }

    public override void ShowPage(object[] args)
    {
        
    }

    #endregion

    #region Private Functions

    private void GetUIReferences()
    {
        processingPause             = new WaitForSeconds(.175f);

        guessButtons                = new List<VisualElement>();
        guessButtonStateChangers    = new List<ButtonStateChanger>();

        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button1_1"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button1_2"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button1_3"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button2_1"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button2_2"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button2_3"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button3_1"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button3_2"));
        guessButtons.Add(uiDoc.rootVisualElement.Q<VisualElement>("Button3_3"));

        ProfileManager.instance.LinkUI(
            uiDoc.rootVisualElement.Q<Label>("CurrentStreak")
            , uiDoc.rootVisualElement.Q<Label>("BestStreak")
            , uiDoc.rootVisualElement.Q<Label>("CorrectGuesses")
            , uiDoc.rootVisualElement.Q<Label>("IncorrectGuesses")
        );

        ProfileManager.instance.ResetStats();

        VisualElement p             = uiDoc.rootVisualElement.Q<VisualElement>("Page"); //TODO: move this to the top and any uidoc.root refs can probably be p refs

        for (int i = 0; i < guessButtons.Count; i++)
        {
            ButtonStateChanger bsc = new ButtonStateChanger(guessButtons[i].Q<VisualElement>("BG"));
            guessButtonStateChangers.Add(bsc);

            guessButtons[i].userData = i;

            guessButtons[i].RegisterCallback<PointerDownEvent>(guessButtonStateChangers[i].OnPointerDown);
            guessButtons[i].RegisterCallback<ClickEvent>(GuessButtonClicked);

            p.RegisterCallback<PointerUpEvent>(guessButtonStateChangers[i].OnPointerUp);
            p.RegisterCallback<PointerLeaveEvent>(guessButtonStateChangers[i].OnPointerOff);
        }

        Label instructions = p.Q<Label>("Instructions");
        instructions.RegisterCallback<ClickEvent>(ObjectiveManager.instance.InstructionsTapped);
    }

    private void ResetGuessButtons()
    {
        processingGuess             = null;

        eventChance                 += Random.Range(1f, 4f); //Takes 25 - 100 turns to trigger an event

        Debug.Log("EventChance: " + eventChance.ToString());

        if (currentEvent.IsEventOver && eventChance >= 100f)
        {
            currentEvent.Init(Random.Range(1, 101));
        }

        List<VisualElement> temp;

        if (currentEvent.IsEventOver)
        {
            temp                    = new List<VisualElement>(guessButtons);

            int numOfButtons        = GetNumOfButtons();

            while (temp.Count > numOfButtons)
                temp.RemoveAt(Random.Range(0, temp.Count));
        }
        else
        {
            List<int> eventButtons  = currentEvent.GetNextLayout();
            temp                    = new List<VisualElement>();

            for (int i = 0; i < eventButtons.Count; i++)
                temp.Add(guessButtons[eventButtons[i]]);

            if (currentEvent.IsEventOver)
            {
                Debug.Log("Event Over, decrementing EventChance");

                eventChance -= currentEvent.EventCounterOffset;

                Debug.Log("Post Event: " + eventChance.ToString());
            }
        }

        currentCorrectIndex         = guessButtons.IndexOf(temp[Random.Range(0, temp.Count)]);

        Debug.Log("Current Correct Index: " + currentCorrectIndex.ToString());

        List<int> currentLayout     = new List<int>();

        for (int i = 0; i < guessButtons.Count; i++)
        {
            guessButtons[i].Q<VisualElement>("BG").SetColor(Color.white);

            bool showButton                     = temp.Contains(guessButtons[i]);
            float opacity                       = showButton ? 1f : 0f;
            guessButtonStateChangers[i].Ignore  = !showButton;

            if (showButton)
                currentLayout.Add(i);

            guessButtons[i].SetOpacity(opacity);
        }

        previousLayouts.Enqueue(currentLayout);
    }

    private int GetNumOfButtons()
    {
        int roll = Random.Range(1, 101);

        //if (roll < 2)
        //    return 1;
        //if (roll < 20)
        //    return 2;
        //if (roll < 42)
        //    return 3;
        //if (roll < 59)
        //    return 4;
        //if (roll < 72)
        //    return 5;
        //if (roll < 82)
        //    return 6;
        //if (roll < 91)
        //    return 7;
        //if (roll < 97)
        //    return 8;
        
        if (roll < ProfileManager.instance.OneThresh)
            return 1;
        if (roll < ProfileManager.instance.TwoThresh)
            return 2;
        if (roll < ProfileManager.instance.ThreeThresh)
            return 3;
        if (roll < ProfileManager.instance.FourThresh)
            return 4;
        if (roll < ProfileManager.instance.FiveThresh)
            return 5;
        if (roll < ProfileManager.instance.SixThresh)
            return 6;
        if (roll < ProfileManager.instance.SevenThresh)
            return 7;
        if (roll < ProfileManager.instance.EightThresh)
            return 8;

        return 9;
    }

    private void GuessButtonClicked(ClickEvent evt)
    {
        if (processingGuess != null)
            return;

        VisualElement button    = evt.currentTarget as VisualElement;

        if (guessButtonStateChangers[(int)button.userData].Ignore)
            return;

        processingGuess         = ButtonSubmitted(button, button.Q<VisualElement>("BG"));

        PageManager.instance.StartCoroutine(processingGuess);
    }

    private IEnumerator ButtonSubmitted(VisualElement guessedButton, VisualElement buttonBG)
    {
        //Debug.Log(string.Format("{0} clicked. {1} is the correct button. {2}"
        //    , guessedButton.name
        //    , guessButtons[currentCorrectIndex].name
        //    , guessedButton == guessButtons[currentCorrectIndex]));

        noTapTimer      = new TimeSpan(0, 3, 0);

        bool correct    = guessedButton == guessButtons[currentCorrectIndex];

        //Debug.Log("Incorrect Tile Chosen. Auto Correct Chance: " + autoCorrectChance.ToString());
        //Debug.Log("AC + Event AC == " + (autoCorrectChance + currentEvent.AdditionalChance).ToString());

        if (!correct)
        {
            float rand = Random.Range(0f, 100f);

            if (rand <= currentEvent.AdditionalChance + autoCorrectChance)
            {
                Debug.Log(string.Format(
                    "Triggered auto correct. {0} + {1} >= {2}: "
                    , autoCorrectChance.ToString()
                    , currentEvent.AdditionalChance.ToString()
                    , rand.ToString()
                ));

                correct             = true;
                autoCorrectChance   = Mathf.Clamp(autoCorrectChance - rand, 0f, 100f);
            }

            //if (autoCorrectChance >= 100f)
            //{
            //    correct             = true;
            //    autoCorrectChance   -= 100f;
            //}
            //else if (autoCorrectChance + currentEvent.AdditionalChance >= 100f)
            //{
            //    correct             = true;
            //}
        }

        autoCorrectChance += Random.Range(.5f, 25f);

        buttonBG.SetColor(correct ? Color.green : Color.red);

        ProfileManager.instance.Guessed(correct);

        object[] data   = new object[3]
        {
            correct
            , (int)guessedButton.userData
            , previousLayouts
        };

        this.PostNotification(Notifications.GUESSED, data);

        yield return processingPause;

        ResetGuessButtons();
    }

    private IEnumerator NoTapsFor3Minutes()
    {
        while (noTapTimer.TotalSeconds > 0f)
        {
            noTapTimer = noTapTimer.Subtract(oneSecond);

            yield return oneSecondWait;
        }

        ObjectiveManager.instance.AwardNoTapAchievement();
    }

    private IEnumerator PlayTimer()
    {
        while (playTimer.TotalSeconds < 300f) //5 minutes
        {
            playTimer = playTimer.Add(oneSecond);

            yield return oneSecondWait;
        }

        ObjectiveManager.instance.UnlockPlayTimeAchievements(5);

        while (playTimer.TotalSeconds < 600f)
        {
            playTimer = playTimer.Add(oneSecond);

            yield return oneSecondWait;
        }

        ObjectiveManager.instance.UnlockPlayTimeAchievements(10);

        while (playTimer.TotalSeconds < 1200f)
        {
            playTimer = playTimer.Add(oneSecond);

            yield return oneSecondWait;
        }

        ObjectiveManager.instance.UnlockPlayTimeAchievements(20);
    }

    #endregion
}
