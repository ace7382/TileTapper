using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileManager : MonoBehaviour
{
    #region Singleton

    public static ProfileManager instance;

    #endregion

    #region Inspector Variables

    [SerializeField] private int oneThreshold;
    [SerializeField] private int twoThreshold;
    [SerializeField] private int threeThreshold;
    [SerializeField] private int fourThreshold;
    [SerializeField] private int fiveThreshold;
    [SerializeField] private int sixThreshold;
    [SerializeField] private int sevenThreshold;
    [SerializeField] private int eightThreshold;

    #endregion

    #region Private Variables

    private int currentStreak;
    private int bestStreak;
    private int correctCount;
    private int incorrectCount;

    private Label currentStreakLabel;
    private Label bestStreakLabel;
    private Label correctLabel;
    private Label incorrectLabel;

    #endregion

    #region Public Properties

    public int OneThresh    { get { return oneThreshold; } }
    public int TwoThresh    { get { return twoThreshold; } }
    public int ThreeThresh  { get { return threeThreshold; } }
    public int FourThresh   { get { return fourThreshold; } }
    public int FiveThresh   { get { return fiveThreshold; } }
    public int SixThresh    { get { return sixThreshold; } }
    public int SevenThresh  { get { return sevenThreshold; } }
    public int EightThresh  { get { return eightThreshold; } }

    public int CorrectCount
    {
        set
        {
            correctCount            = value;
            correctLabel.text       = correctCount.ToString();
        }

        get
        {
            return correctCount;
        }
    }

    public int IncorrectCount
    {
        set
        {
            incorrectCount          = value;
            incorrectLabel.text     = incorrectCount.ToString();
        }

        get
        {
            return incorrectCount;
        }
    }

    public int CurrentStreak
    {
        set
        {
            currentStreak           = value;
            currentStreakLabel.text = currentStreak.ToString();

            if (currentStreak > bestStreak)
                bestStreak          = currentStreak;

            bestStreakLabel.text    = bestStreak.ToString();
        }

        get
        {
            return currentStreak;
        }
    }

    public int BestStreak
    {
        get { return bestStreak; }
    }

    public int TotalGuesses
    {
        get { return CorrectCount + IncorrectCount; }
    }

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    #endregion

    #region Public Functions

    public void ResetStats()
    {
        CurrentStreak   = 0;
        CorrectCount    = 0;
        IncorrectCount  = 0;
    }

    public void LinkUI(Label currentStreak, Label bestStreak, Label correct, Label incorrect)
    {
        currentStreakLabel      = currentStreak;
        bestStreakLabel         = bestStreak;
        correctLabel            = correct;
        incorrectLabel          = incorrect;
    }

    public void Guessed(bool correct)
    {
        if (correct)
        {
            CorrectCount++;
            CurrentStreak++;
        }
        else
        {
            int temp            = currentStreak;
            IncorrectCount++;
            CurrentStreak       = 0;

            this.PostNotification(Notifications.STREAK_FAILED, temp);
        }
    }

    #endregion
}