using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Tile Counts")]
public class Objective_TileCount : Objective
{
    #region Inspector Variables

    [SerializeField] private List<int> goalCounts;

    #endregion

    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return IsComplete ? 100f : 0f;
    }

    #endregion

    #region Public Functions

    public bool DoCountsMatch(LimitedQueue<List<int>> previousLayouts)
    {
        //Goal Counts -> get a 9 then a 1 then a 1
        //9 <- first
        //1 <- second
        //1 <- third

        //past count
        //1 <- most recent  <- sixth    [5]
        //1                 <- fifth    [4]
        //9                 <- fourth   [3]
        //8                 <- third    [2]
        //2                 <- second   [1]
        //6 <- least recent <- first    [0]

        //Go to the [prevLayouts.count - 1 - (goalcount - (1 + sublist))] entry in the queue
        //Add to sublist
        //6 - 1 - (3 - (1 + 0)) = 3
        //

        if (previousLayouts.Count < goalCounts.Count)
            return false;

        List<int> subList = new List<int>();

        while (subList.Count < goalCounts.Count)
        {
            subList.Add(previousLayouts.ElementAt(previousLayouts.Count - 1 - (goalCounts.Count - (1 + subList.Count))).Count);
        }

        return goalCounts.SequenceEqual(subList);
    }

    #endregion
}
