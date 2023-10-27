using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Specific")]
public class Objective_Specific : Objective
{
    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return IsComplete ? 100f : 0f;
    }

    #endregion
}
