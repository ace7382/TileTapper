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
        currentIndex                = 0;
        additionalCorrectChance     = new List<float>();
        layouts                     = new List<List<int>>();

        Debug.Log("Event triggered");

        if (choice < ProfileManager.instance.OuterRim) //OuterRim
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
                    5f
                ,   8f
                ,   10f
                ,   12f
                ,   15f
                ,   18f
                ,   21f
                ,   50f
            };

            eventCounterOffset = 35f;
        }
        else if (choice < ProfileManager.instance.Cols) //Cols
        {
            layouts = new List<List<int>>()
            {
                    new List<int>() { 0, 3, 6 }
                ,   new List<int>() { 1, 4, 7 }
                ,   new List<int>() { 2, 5, 8 }
            };

            additionalCorrectChance = new List<float>()
            {
                    10f
                ,   10f
                ,   10f
            };

            eventCounterOffset = 20f;
        }
        else if (choice < ProfileManager.instance.Rows) //Rows
        {
            layouts = new List<List<int>>()
            {
                    new List<int>() { 0, 1, 2 }
                ,   new List<int>() { 3, 4, 5 }
                ,   new List<int>() { 6, 7, 8 }
            };

            additionalCorrectChance = new List<float>()
            {
                    10f
                ,   10f
                ,   10f
            };

            eventCounterOffset = 20f;
        }
        else if (choice < ProfileManager.instance.Unlucky) //Unlucky
        {
            int unlucky = Random.Range(1, 10);

            for (int i = 0; i < unlucky; i++)
            {
                int numOfButtons = Random.Range(2, 10);
                List<int> buttons = new List<int>();

                for (int j = 1; j <= numOfButtons; j++)
                {
                    buttons.Add(Random.Range(0, 9));
                }

                layouts.Add(buttons.Distinct<int>().ToList());
                additionalCorrectChance.Add(-999f);
            }

            eventCounterOffset = -20f;
        }
        else if (choice < ProfileManager.instance.Dice) //Dice
        {
            List<List<int>> temp = new List<List<int>>()
            {
                new List<int>() { 4 },
                new List<int>() { 2, 6 },
                new List<int>() { 2, 4, 6 },
                new List<int>() { 0, 2, 6, 8 },
                new List<int>() { 0, 2, 4, 6, 8 },
                new List<int>() { 0, 2, 3, 5, 6, 8 },
            };

            layouts = new List<List<int>>()
            {
                temp[Random.Range(0, temp.Count)]
            };

            eventCounterOffset = 30f;
        }
        else if (choice < ProfileManager.instance.Lucky7s) //777
        {
            for (int i = 0; i < 3; i++)
            {
                int numOfButtons = Random.Range(1, 10);
                List<int> buttons = new List<int>();

                for (int j = 1; j <= numOfButtons; j++)
                {
                    buttons.Add(Random.Range(0, 9));
                }

                if (!buttons.Contains(6))
                    buttons.Add(6);

                layouts.Add(buttons.Distinct<int>().ToList());
                additionalCorrectChance.Add(100f);
            }

            eventCounterOffset = 999f;
        }
        else if (choice < ProfileManager.instance.InARow) //0 - 8
        {
            for (int i = 0; i < 9; i++)
            {
                int numOfButtons = Random.Range(1, 10);
                List<int> buttons = new List<int>();

                for (int j = 1; j <= numOfButtons; j++)
                {
                    buttons.Add(Random.Range(0, 9));
                }

                if (!buttons.Contains(i))
                    buttons.Add(i);

                layouts.Add(buttons.Distinct<int>().ToList());
                additionalCorrectChance.Add(2f * i);
            }

            eventCounterOffset = 999f;
        }
        else if (choice < ProfileManager.instance.FiveXFive) //5x5
        {
            System.Random r         = new System.Random();

            for (int i = 0; i < 5; i++)
            {
                List<int> randLayout = Enumerable.Range(0, 8).OrderBy(x => r.Next()).Take(5).ToList();
                layouts.Add(randLayout);
            }

            eventCounterOffset = 999f;
        }
        else //Streak
        {
            int[] possibleStreaks   = new int[23] { 5, 5, 5, 5, 5, 5, 5,
                                                    9, 9, 9, 9, 9, 9,
                                                    10, 10, 10, 10,
                                                    15, 15, 15,
                                                    20, 20,
                                                    24 };
            int streak              = possibleStreaks[Random.Range(0, possibleStreaks.Length)];

            for (int i = 0; i < streak; i++)
            {
                int numOfButtons    = Random.Range(1, 10);
                List<int> buttons   = new List<int>();

                for (int j = 1; j <= numOfButtons; j++)
                {
                    buttons.Add(Random.Range(0, 9));
                }

                layouts.Add(buttons.Distinct<int>().ToList());
                additionalCorrectChance.Add(100f);
            }

            eventCounterOffset      = streak * 7f;
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
