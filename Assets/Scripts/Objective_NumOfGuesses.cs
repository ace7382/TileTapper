using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Num of Guesses")]
public class Objective_NumOfGuesses : Objective
{
    #region Enums

    public enum GuessObjectiveType
    {
        CORRECT
        , INCORRECT
        , TOTAL
    }

    #endregion

    #region Inspector Variables

    [SerializeField] private GuessObjectiveType type;
    [SerializeField] private int                neededAmount;

    #endregion

    #region Public Properties

    public GuessObjectiveType                   Type                { get { return type; } }
    public int                                  Goal                { get { return neededAmount; } }

    #endregion

    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        if (type == GuessObjectiveType.CORRECT)
            return ProfileManager.instance.CorrectCount == 0 ? 0f : Mathf.Clamp((ProfileManager.instance.CorrectCount / neededAmount) * 100f, 0f, 100f);

        if (type == GuessObjectiveType.INCORRECT)
            return ProfileManager.instance.IncorrectCount == 0 ? 0f : Mathf.Clamp((ProfileManager.instance.IncorrectCount / neededAmount) * 100f, 0f, 100f);

        return ProfileManager.instance.TotalGuesses == 0 ? 0f : Mathf.Clamp((ProfileManager.instance.TotalGuesses / neededAmount) * 100f, 0f, 100f);
    }

    #endregion
}
