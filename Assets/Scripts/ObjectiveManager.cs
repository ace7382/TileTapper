using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using Achievement = Apple.GameKit.GKAchievement;

public class ObjectiveManager : MonoBehaviour
{
    #region Singleton

    public static ObjectiveManager              instance;           

    #endregion

    #region Inspector Variables

    [SerializeField] private List<Objective>    objectives;

    #endregion

    #region Private Variables

    private int                                 totalObjectiveCount;
    private List<Objective>                     completeObjectives;

    #endregion

    #region Public Properties

    public int                                  CompletedObjectivesCount    { get { return completeObjectives.Count; } }
    public int                                  TotalObjectives             { get { return totalObjectiveCount; } }

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        totalObjectiveCount     = objectives.Count;

        completeObjectives      = new List<Objective>();

        //for (int i = 0; i < objectives.Count; i++)
        //    if (objectives[i].IsComplete)
        //        MarkAsComplete(objectives[i]);
    }

    private void OnEnable()
    {
        this.AddObserver(ProcessGuess, Notifications.GUESSED);
        this.AddObserver(ProcessStreakFail, Notifications.STREAK_FAILED);
    }

    private void OnDisable()
    {
        this.RemoveObserver(ProcessGuess, Notifications.GUESSED);
        this.RemoveObserver(ProcessStreakFail, Notifications.STREAK_FAILED);
    }

    #endregion

    #region Public Functions

    public static void ResetObjectives()
    {
        List<Objective> allobjectives = Resources.LoadAll<Objective>("Objectives").ToList();

        for (int i = 0; i < allobjectives.Count; i++)
        {
            allobjectives[i].Reset();
        }
    }

    public void UpdateObjectivesBasedOnSaveData(SaveFile saveData)
    {
        for (int i = 0; i < saveData.Objectives.Count; i++)
        {
            if (!String.IsNullOrEmpty(saveData.Objectives[i]))
                MarkAsComplete(saveData.Objectives[i]);
        }
    }

    public void UpdateObjectivesBasedOnGKAchievements(Apple.GameKit.GKAchievement achievement)
    {
        Objective obj = objectives.Find(x => x.ID == achievement.Identifier);

        if (obj != null)
        {
            ProfileManager.instance.UpdateSaveGameOnObjectiveComplete(obj);
            MarkAsComplete(obj);
        }
    }

    public void MarkAsComplete(Objective o)
    {
        objectives.Remove(o);
        completeObjectives.Add(o);
    }

    public void MarkAsComplete(string objectiveID)
    {
        Objective o = objectives.Find(x => x.ID == objectiveID);

        if (o != null)
            MarkAsComplete(o);
    }

    public void InstructionsTapped(UnityEngine.UIElements.ClickEvent evt)
    {
        Objective obj = objectives.Find(x => x.name == "Specific - Tap Instructions");

        if (obj != null)
            obj.OnComplete();
    }

    public void AwardNoTapAchievement()
    {
        Objective obj = objectives.Find(x => x.name == "Specific - No Taps for 3 Minutes");

        if (obj != null)
            obj.OnComplete();
    }

    public void UnlockPlayTimeAchievements(int minutes)
    {
        string objectiveName =
            minutes == 5 ?  "Specific - 5 Minutes" :
            minutes == 10 ? "Specific - 10 Minutes" :
                            "Specific - 20 Minutes";

        Objective obj = objectives.Find(x => x.name == objectiveName);

        if (obj != null)
            obj.OnComplete();
    }

    public async Task CheckAllGameCenterAchievments()
    {
        var allAchievements = await Achievement.LoadAchievements();

        for (int i = 0; i < completeObjectives.Count; i++)
        {
            Achievement ach = allAchievements.FirstOrDefault(x => x.Identifier == completeObjectives[i].ID);

            if (ach == null)
                ach = Achievement.Init(completeObjectives[i].ID);

            if (!ach.IsCompleted)
            {
                ach.PercentComplete         = 100f;
                ach.ShowCompletionBanner    = true;

                await Achievement.Report(ach);
            }
        }
    }

    #endregion

    #region Private Functions

    private void ProcessStreakFail(object sender, object info)
    {
        //info      -   int                     -   the streak that was lost

        int streakLost = (int)info;

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i] is Objective_StreakFail)
            {
                Objective_StreakFail obj = (Objective_StreakFail)objectives[i];

                if (obj.Streak == streakLost)
                    obj.OnComplete();
            }
        }
    }

    private void ProcessGuess(object sender, object info)
    {
        //info      -   object[]
        //info[0]   -   bool                    -   correct indicator
        //info[1]   -   int                     -   button index (0 - 8)
        //info[2]   -   LimitedQueue<List<int>> -   the previous board layouts (the last entered is the one that was guessed on)

        object[] data   = (object[])info;
        bool correct    = (bool)data[0];
        int index       = (int)data[1];

        //Debug.Log(string.Format(
        //    "Button {4} selected. {0} guess. CORRECT: {1}  INCORRECT: {2}  TOTAL: {3}"
        //    , correct ? "Correct" : "Incorrect"
        //    , ProfileManager.instance.CorrectCount
        //    , ProfileManager.instance.IncorrectCount
        //    , ProfileManager.instance.TotalGuesses
        //    , index));

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i] is Objective_NumOfGuesses)
            {
                Objective_NumOfGuesses obj = (Objective_NumOfGuesses)objectives[i];

                if (correct)
                {
                    if (obj.Type == Objective_NumOfGuesses.GuessObjectiveType.CORRECT && ProfileManager.instance.CorrectCount >= obj.Goal)
                        obj.OnComplete();
                }
                else
                {
                    if (obj.Type == Objective_NumOfGuesses.GuessObjectiveType.INCORRECT && ProfileManager.instance.IncorrectCount >= obj.Goal)
                        obj.OnComplete();
                }

                if (obj.Type == Objective_NumOfGuesses.GuessObjectiveType.TOTAL && ProfileManager.instance.TotalGuesses >= obj.Goal)
                    obj.OnComplete();
            }
            else if (objectives[i] is Objective_Streak)
            {
                Objective_Streak obj = (Objective_Streak)objectives[i];

                if (ProfileManager.instance.CurrentStreak >= obj.Goal)
                    obj.OnComplete();
            }
            else if (objectives[i] is Objective_Layout)
            {
                Objective_Layout obj                    = (Objective_Layout)objectives[i];
                LimitedQueue<List<int>> previousLayouts = (LimitedQueue<List<int>>)data[2];
                List<int> mostRecentLayout              = previousLayouts.Last();

                if (obj.DoesLayoutMatch(mostRecentLayout))
                    obj.OnComplete();
            }
            else if (objectives[i] is Objective_TileCount)
            {
                Objective_TileCount obj                 = (Objective_TileCount)objectives[i];
                LimitedQueue<List<int>> previousLayouts = (LimitedQueue<List<int>>)data[2];

                if (obj.DoCountsMatch(previousLayouts))
                    obj.OnComplete();
            }
            else if (objectives[i] is Objective_TileIndex)
            {
                Objective_TileIndex obj                 = (Objective_TileIndex)objectives[i];

                obj.CheckIndex(index);
            }
            else if (objectives[i] is Objective_CorrectWithTileCount)
            {
                Objective_CorrectWithTileCount obj      = (Objective_CorrectWithTileCount)objectives[i];
                LimitedQueue<List<int>> previousLayouts = (LimitedQueue<List<int>>)data[2];
                List<int> mostRecentLayout              = previousLayouts.Last();

                if (obj.CheckForMatch(correct, mostRecentLayout.Count))
                    obj.OnComplete();
            }
        }
    }

    #endregion


#if UNITY_EDITOR
    #region Dev Help Functions

    [MenuItem("Dev Commands/Link Objectives")]
    public static void LinkObjectivesToObjectiveManager()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Editor is in Playmode. This function cannot be used");
            return;
        }

        GameObject.FindObjectOfType<ObjectiveManager>().objectives = Resources.LoadAll<Objective>("Objectives").ToList();
    }

    //[MenuItem("Dev Commands/Number Objectives")]
    //public static void NumberAllObjectives()
    //{
    //    List<Objective> objs = Resources.LoadAll<Objective>("Objectives").ToList();

    //    for (int i = 0; i < objs.Count; i++)
    //    {
    //        objs[i].SetID(i.ToString());
    //    }
    //}

    [MenuItem("Dev Commands/Reset Objectives")]
    public static void ResetAllObjectives()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Editor is in Playmode. This function cannot be used");
            return;
        }

        ResetObjectives();
    }

    #endregion
#endif

}
    