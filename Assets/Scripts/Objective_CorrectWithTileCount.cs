using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Correct + Tile Count")]
public class Objective_CorrectWithTileCount : Objective
{
    #region Inspector Variables

    [SerializeField] private bool   correct;
    [SerializeField] private int    numOfTiles;

    #endregion
    
    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return IsComplete ? 100f : 0f;
    }

    #endregion

    #region Public Functions

    public bool CheckForMatch(bool guessCorrectness, int numOfTilesOnBoard)
    {
        return correct == guessCorrectness && numOfTilesOnBoard == numOfTiles;
    }

    #endregion
}
