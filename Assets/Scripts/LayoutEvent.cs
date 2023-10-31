using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutEvent
{
    #region Inspector Variables

    [SerializeField] private List<List<int>>    layouts;
    [SerializeField] private List<float>        additionalCorrectChance;
    [SerializeField] private float              eventCounterOffset;

    #endregion

    #region Private Variables

    private int currentIndex;

    #endregion

    #region Public Variables

    public float EventCounterOffset { get { return eventCounterOffset; } }

    public float AdditionalChance
    {
        get
        {
            if (additionalCorrectChance.Count == 0)
            {
                return 0f;
            }
            else
            {
                return additionalCorrectChance[currentIndex];
            }
        }
    }

    public bool IsEventOver
    {
        get
        {
            return currentIndex >= layouts.Count;
        }
    }

    #endregion

    #region Constructor

    public LayoutEvent()
    {
        layouts                     = new List<List<int>>();
        additionalCorrectChance     = new List<float>();
        currentIndex                = 0;
    }

    #endregion

    #region Public Functions

    public LayoutEvent Init(float choice)
    {
        currentIndex = 0;

        Debug.Log("Event triggered");

        if (choice < 999f) //OuterRim
        {
            layouts = new List<List<int>>()
            {
                    new List<int>() { 0, 1, 2 }
                ,   new List<int>() { 1, 2, 5 }
                ,   new List<int>() { 2, 5, 8 }
                ,   new List<int>() { 5, 8, 7 }
                ,   new List<int>() { 8, 7, 6 }
                ,   new List<int>() { 7, 6, 3 }
                ,   new List<int>() { 6, 3, 0 }
                ,   new List<int>() { 3, 0, 1 }
            };

            additionalCorrectChance = new List<float>();

            eventCounterOffset = 100f;
        }

        return this;
    }

    public List<int> GetNextLayout()
    {
        List<int> ret = layouts[currentIndex];

        currentIndex++;

        return ret;
    }

    #endregion
}
