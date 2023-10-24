using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GamePlayPage : Page
{
    #region Private Variables

    private List<VisualElement>         guessButtons;
    private List<ButtonStateChanger>    guessButtonStateChangers;
    //private Label                       currentStreakLabel;
    //private Label                       bestStreakLabel;
    //private Label                       correctLabel;
    //private Label                       incorrectLabel;

    private int                         currentCorrectIndex;
    private IEnumerator                 processingGuess;
    private WaitForSeconds              processingPause;

    #endregion

    #region Inherited Functions

    public override IEnumerator AnimateIn()
    {
        GetUIReferences();
        ResetGuessButtons();

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
    }

    public override void ShowPage(object[] args)
    {
        
    }

    #endregion

    #region Private Functions

    private void GetUIReferences()
    {
        processingPause             = new WaitForSeconds(.25f);

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

        //currentStreakLabel          = uiDoc.rootVisualElement.Q<Label>("CurrentStreak");
        //bestStreakLabel             = uiDoc.rootVisualElement.Q<Label>("BestStreak");
        //correctLabel                = uiDoc.rootVisualElement.Q<Label>("CorrectGuesses");
        //incorrectLabel              = uiDoc.rootVisualElement.Q<Label>("IncorrectGuesses");

        ProfileManager.instance.LinkUI(
            uiDoc.rootVisualElement.Q<Label>("CurrentStreak")
            , uiDoc.rootVisualElement.Q<Label>("BestStreak")
            , uiDoc.rootVisualElement.Q<Label>("CorrectGuesses")
            , uiDoc.rootVisualElement.Q<Label>("IncorrectGuesses")
        );

        ProfileManager.instance.ResetStats();

        VisualElement p             = uiDoc.rootVisualElement.Q<VisualElement>("Page");

        for (int i = 0; i < guessButtons.Count; i++)
        {
            ButtonStateChanger bsc = new ButtonStateChanger(guessButtons[i].Q<VisualElement>("BG"));
            guessButtonStateChangers.Add(bsc);

            guessButtons[i].RegisterCallback<PointerDownEvent>(guessButtonStateChangers[i].OnPointerDown);
            guessButtons[i].RegisterCallback<ClickEvent>(GuessButtonClicked);

            p.RegisterCallback<PointerUpEvent>(guessButtonStateChangers[i].OnPointerUp);
            p.RegisterCallback<PointerLeaveEvent>(guessButtonStateChangers[i].OnPointerOff);
        }
    }

    private void ResetGuessButtons()
    {
        processingGuess             = null;

        List<VisualElement> temp    = new List<VisualElement>(guessButtons);

        int numOfButtons            = GetNumOfButtons(); //Random.Range(2, 10);

        while (temp.Count > numOfButtons)
            temp.RemoveAt(Random.Range(0, temp.Count));

        currentCorrectIndex         = guessButtons.IndexOf(temp[Random.Range(0, temp.Count)]);

        Debug.Log("Current Correct Index: " + currentCorrectIndex.ToString());

        for (int i = 0; i < guessButtons.Count; i++)
        {
            guessButtons[i].Q<VisualElement>("BG").SetColor(Color.white);

            bool showButton                     = temp.Contains(guessButtons[i]);
            float opacity                       = showButton ? 1f : 0f;
            guessButtonStateChangers[i].Ignore  = !showButton;

            guessButtons[i].SetOpacity(opacity);
        }
    }

    private int GetNumOfButtons()
    {
        int roll = Random.Range(1, 101);

        if (roll < 2)
            return 1;
        if (roll < 20)
            return 2;
        if (roll < 42)
            return 3;
        if (roll < 59)
            return 4;
        if (roll < 72)
            return 5;
        if (roll < 82)
            return 6;
        if (roll < 91)
            return 7;
        if (roll < 97)
            return 8;

        return 9;
    }

    private void GuessButtonClicked(ClickEvent evt)
    {
        if (processingGuess != null)
            return;

        VisualElement button    = evt.currentTarget as VisualElement;

        if (guessButtonStateChangers[guessButtons.IndexOf(button)].Ignore)
            return;

        processingGuess         = ButtonSubmitted(button, button.Q<VisualElement>("BG"));

        PageManager.instance.StartCoroutine(processingGuess);
    }

    private IEnumerator ButtonSubmitted(VisualElement guessedButton, VisualElement buttonBG)
    {
        Debug.Log(string.Format("{0} clicked. {1} is the correct button. {2}"
            , guessedButton.name
            , guessButtons[currentCorrectIndex].name
            , guessedButton == guessButtons[currentCorrectIndex]));

        bool correct = guessedButton == guessButtons[currentCorrectIndex];

        buttonBG.SetColor(correct ? Color.green : Color.red);

        ProfileManager.instance.Guessed(correct);

        yield return processingPause;

        ResetGuessButtons();
    }

    #endregion
}
