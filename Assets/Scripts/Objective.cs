using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Achievement = Apple.GameKit.GKAchievement;

public abstract class Objective : ScriptableObject
{
    #region Inspector Variables

    [SerializeField] protected string           id;
    //[SerializeField] protected string           description;
    //[SerializeField] protected Texture2D        icon;

    #endregion

    #region Private Variables

    [SerializeField] private bool isComplete = false; //TODO: Remove serialization; just for debug

    #endregion

    #region Public Properties

    public bool IsComplete { 
        get { return isComplete; } 
        protected set
        {
            if (value)
                OnComplete();

            isComplete = value;
        }
    }

    public string           ID                  { get { return id; } }
    //public string           Description         { get { return description; } }
    //public Texture2D        Icon                { get { return icon; } }

    #endregion

    #region Abstract Functions

    public abstract float GetProgressAsPercentage();

    #endregion

    #region Public Functions

    public void SetID(string newId)
    {
        if (Application.isPlaying)
        {
            Debug.Log("This is an editor script. Do not run in play mode");
            return;
        }

        id = newId;
    }

    public virtual void Reset()
    {
        isComplete      = false;
    }

    public void OnComplete()
    {
        ObjectiveManager.instance.MarkAsComplete(this);
        ProfileManager.instance.UpdateSaveGameOnObjectiveComplete(this);

        Debug.Log("Objective Complete " + name);

        if (ProfileManager.instance.GKPlayer)
        {
            Task.Run(() => UnlockGameCenterAchievement());
        }
    }

    public async void UnlockGameCenterAchievement()
    {
        string achievementID                = ID;
        int progressPercent                 = 100;
        bool showCompletionToast            = true;

        Debug.Log("pre load");

        var allAchievements                 = await Achievement.LoadAchievements();

        Debug.Log("Achievements loaded");
        Debug.Log(allAchievements.Count);

        Achievement gkAchievement           = allAchievements.FirstOrDefault(x => x.Identifier == achievementID);

        if (gkAchievement == null)
        {
            Debug.Log("Here");

            gkAchievement                   = Achievement.Init(achievementID);
        }

        if (!gkAchievement.IsCompleted)
        {
            Debug.Log("Now Here");

            gkAchievement.PercentComplete   = progressPercent;
            gkAchievement
                .ShowCompletionBanner       = showCompletionToast;

            await Achievement.Report(gkAchievement);

            Debug.Log("Done");
        }
    }

    #endregion
}
