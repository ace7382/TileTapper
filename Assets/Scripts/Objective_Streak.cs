using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Streak")]
public class Objective_Streak : Objective
{
    #region Inspector Variables

    [SerializeField] private int    goal;

    #endregion

    #region Public Properties

    public int                      Goal {   get { return goal; } }

    #endregion

    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return ProfileManager.instance.BestStreak == 0 ? 0f : Mathf.Clamp(ProfileManager.instance.BestStreak / goal * 100f, 0f, 100f);
    }

    #endregion
}
