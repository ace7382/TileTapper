using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Layout")]
public class Objective_Layout : Objective
{
    #region Inspector Variables

    [SerializeField] private List<int>  goalLayout;

    #endregion

    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return IsComplete ? 100f : 0f;
    }

    #endregion

    #region Public Functions

    public bool DoesLayoutMatch(List<int> currentLayout)
    {
        return
            currentLayout.Count == goalLayout.Count &&
            goalLayout.Intersect(currentLayout).ToList().Count == goalLayout.Count;
    }

    #endregion
}
