using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Streak Fail")]
public class Objective_StreakFail : Objective
{
    #region Inspector Variables

    [SerializeField] private int    streak;

    #endregion

    #region Public Properties

    public int                      Streak      { get { return streak; } }

    #endregion

    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return IsComplete ? 100f : 0f;
    }

    #endregion
}
