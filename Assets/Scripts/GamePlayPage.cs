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

    private VisualElement               musicButton;
    private VisualElement               sfxButton;
    private VisualElement               gameCenterButton;
    private ButtonStateChanger          musicButtonStateChanger;
    private ButtonStateChanger          sfxButtonStateChanger;
    private ButtonStateChanger          gameCenterButtonStateChanger;

    private Label                       notificationLabel;

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

        AdManager.instance.LoadBanner();
        
        return null;
    }

    public override IEnumerator AnimateOut()
    {
        return null;
    }

    public override void HidePage()
    {
        uiDoc.rootVisualElement.Q<Label>("Instructions").UnregisterCallback<ClickEvent>(ObjectiveManager.instance.InstructionsTapped);
    }

    public override void ShowPage(object[] args)
    {
        
    }

    #endregion

    #region Public Functions

    public void ShowNotification(string notif)
    {
        PageManager.instance.ChangeAsyncUpdater(notificationLabel, notif);
    }

    public void HideNotification()
    {
        PageManager.instance.ChangeAsyncUpdater(notificationLabel, "");
    }

    #endregion

    #region Private Functions

    private void GetUIReferences()
    {
        processingPause                 = new WaitForSeconds(.175f);

        guessButtons                    = new List<VisualElement>();
        guessButtonStateChangers        = new List<ButtonStateChanger>();

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

        VisualElement p                 = uiDoc.rootVisualElement.Q<VisualElement>("Page"); 

        for (int i = 0; i < guessButtons.Count; i++)
        {
            ButtonStateChanger bsc      = new ButtonStateChanger(guessButtons[i].Q<VisualElement>("BG"));
            guessButtonStateChangers.Add(bsc);

            guessButtons[i].userData    = i;

            //guessButtons[i].RegisterCallback<PointerDownEvent>(guessButtonStateChangers[i].OnPointerDown);

            int temp                    = i;

            guessButtons[temp].RegisterCallback<PointerDownEvent>((evt) =>
            {
                if (processingGuess != null)
                    return;

                guessButtonStateChangers[temp].OnPointerDown(evt);
            });

            guessButtons[i].RegisterCallback<ClickEvent>(GuessButtonClicked);

            p.RegisterCallback<PointerUpEvent>(guessButtonStateChangers[i].OnPointerUp);
            p.RegisterCallback<PointerLeaveEvent>(guessButtonStateChangers[i].OnPointerOff);
        }

        Label instructions              = p.Q<Label>("Instructions");
        instructions.RegisterCallback<ClickEvent>(ObjectiveManager.instance.InstructionsTapped);

        //Notif
        notificationLabel               = p.Q<Label>("Notification");
        notificationLabel.text          = "";
        PageManager.instance.RegisterAsyncUpdater(notificationLabel);

        //Bottom Buttons
        sfxButton                       = p.Q<VisualElement>("SFXButton");
        musicButton                     = p.Q<VisualElement>("MusicButton");
        gameCenterButton                = p.Q<VisualElement>("GameCenterButton");

        sfxButtonStateChanger           = new ButtonStateChanger(sfxButton.Q<VisualElement>("BG"));
        musicButtonStateChanger         = new ButtonStateChanger(musicButton.Q<VisualElement>("BG"));
        gameCenterButtonStateChanger    = new ButtonStateChanger(gameCenterButton.Q<VisualElement>("BG"));

        p.RegisterCallback<PointerUpEvent>(sfxButtonStateChanger.OnPointerUp);
        p.RegisterCallback<PointerUpEvent>(musicButtonStateChanger.OnPointerUp);
        p.RegisterCallback<PointerUpEvent>(gameCenterButtonStateChanger.OnPointerUp);
        p.RegisterCallback<PointerLeaveEvent>(sfxButtonStateChanger.OnPointerOff);
        p.RegisterCallback<PointerLeaveEvent>(musicButtonStateChanger.OnPointerOff);
        p.RegisterCallback<PointerLeaveEvent>(gameCenterButtonStateChanger.OnPointerOff);

        sfxButton.RegisterCallback<PointerDownEvent>(sfxButtonStateChanger.OnPointerDown);
        musicButton.RegisterCallback<PointerDownEvent>(musicButtonStateChanger.OnPointerDown);
        gameCenterButton.RegisterCallback<PointerDownEvent>(gameCenterButtonStateChanger.OnPointerDown);

        sfxButton.RegisterCallback<ClickEvent>((evt) =>
        {
            sfxButton.Q<VisualElement>("Icon").SetImage(SoundManager.instance.ToggleSFX());
        });

        musicButton.RegisterCallback<ClickEvent>((evt) =>
        {
            musicButton.Q<VisualElement>("Icon").SetImage(SoundManager.instance.ToggleMusic());
        });

        gameCenterButton.RegisterCallback<ClickEvent>((evt) =>
        {
            ShowNotification("Signing into GameCenter...");
            ProfileManager.instance.AttemptGCSignIn();
        });
    }

    private void ResetGuessButtons()
    {
        processingGuess             = null;

        eventChance                 += Random.Range(2.5f, 5f); //Takes 20 - 40 turns to trigger an event

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

                eventChance = Mathf.Clamp(eventChance -currentEvent.EventCounterOffset, 0f, 100f);

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
        int bscIndex            = (int)button.userData;

        if (guessButtonStateChangers[bscIndex].Ignore)
            return;

        processingGuess         = ButtonSubmitted(button, button.Q<VisualElement>("BG"));

        PageManager.instance.StartCoroutine(processingGuess);

        SoundManager.instance.PlayClickSound();
    }

    private IEnumerator ButtonSubmitted(VisualElement guessedButton, VisualElement buttonBG)
    {
        noTapTimer      = new TimeSpan(0, 3, 0);

        bool correct    = guessedButton == guessButtons[currentCorrectIndex];

        if (currentEvent.AdditionalChance < 0)
        { 
            correct     = false;
        }
        else if (!correct)
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
        }

        autoCorrectChance += Random.Range(.2f, 18f);

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
