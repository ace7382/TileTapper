using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ProfileManager : MonoBehaviour
{
    #region Singleton

    public static ProfileManager instance;

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

    #region Private Properties

    private int CorrectCount
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

    private int IncorrectCount
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

    private int CurrentStreak
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
            IncorrectCount++;
            CurrentStreak       = 0;
        }
    }

    #endregion
}