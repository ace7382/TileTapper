using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            if (currentIndex >= additionalCorrectChance.Count)
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

        if (choice < 50f) //OuterRim
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

            additionalCorrectChance = new List<float>()
            {
                    10f
                ,   10f
                ,   10f
                ,   10f
                ,   10f
                ,   10f
                ,   10f
                ,   10f
            };

            eventCounterOffset = 100f;
        }
        else //5 Always Corrects
        {
            for (int i = 0; i < 5; i++)
            {
                int numOfButtons    = Random.Range(1, 10);
                List<int> buttons   = new List<int>();

                for (int j = 1; j <= numOfButtons; j++)
                {
                    buttons.Add(Random.Range(0, 9));
                }

                layouts.Add(buttons.Distinct<int>().ToList());
            }

            additionalCorrectChance = new List<float>()
            {
                    100f
                ,   100f
                ,   100f
                ,   100f
                ,   100f
            };
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
