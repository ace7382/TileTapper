using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Objective", menuName = "New Objective - Tile Index")]
public class Objective_TileIndex : Objective
{
    #region Inspector Variables

    [SerializeField] private List<int>  goalOrder;
    [SerializeField] private bool       tileRepeater;

    #endregion

    #region Private Variables

    private int currentIndex = 0; //TODO: This shouldn't need to be serialized but it's not getting reset between plays if it's not???

    #endregion

    #region Inherited Functions

    public override float GetProgressAsPercentage()
    {
        return IsComplete ? 100f : 0f;
    }

    public override void Reset()
    {
        base.Reset();
        currentIndex = 0;
    }

    #endregion

    #region Public Functions

    public void CheckIndex(int guessedIndex)
    {
        //If it's the first index, set the goal order to be all the same int, currentindex++
        if (tileRepeater)
        {
            if (currentIndex == 0)
            {
                for (int i = 0; i < goalOrder.Count; i++)
                    goalOrder[i] = guessedIndex;
            }

            if (guessedIndex == goalOrder[currentIndex])
                currentIndex++;
            else
            {
                currentIndex = 1;

                for (int i = 0; i < goalOrder.Count; i++)
                    goalOrder[i] = guessedIndex;
            }    
        }
        else
        {
            if (guessedIndex == goalOrder[currentIndex])
                currentIndex++;
            else
                currentIndex = 0;
        }

        if (currentIndex >= goalOrder.Count)
            OnComplete();
    }

    #endregion
}
