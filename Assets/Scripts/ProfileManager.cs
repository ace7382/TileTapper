using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Apple.GameKit;
using System.Threading.Tasks;
using System;
using System.IO;
using UnityEditor;

public class ProfileManager : MonoBehaviour
{
    #region Singleton

    public static ProfileManager    instance;

    #endregion

    #region Consts

    private const string            saveDirectory           = "/SaveData/";
    private const string            fileName                = "abc.sav";

    #endregion

    #region Inspector Variables

    [SerializeField] private int    oneThreshold;
    [SerializeField] private int    twoThreshold;
    [SerializeField] private int    threeThreshold;
    [SerializeField] private int    fourThreshold;
    [SerializeField] private int    fiveThreshold;
    [SerializeField] private int    sixThreshold;
    [SerializeField] private int    sevenThreshold;
    [SerializeField] private int    eightThreshold;

    #endregion

    #region Private Variables

    private int                     currentStreak;
    private int                     bestStreak;
    private int                     correctCount;
    private int                     incorrectCount;

    private Label                   currentStreakLabel;
    private Label                   bestStreakLabel;
    private Label                   correctLabel;
    private Label                   incorrectLabel;

    private GKLocalPlayer           gameKit_playerProfile;

    private SaveFile                saveData;

    #endregion

    #region Public Properties

    public int OneThresh    { get { return oneThreshold; } }
    public int TwoThresh    { get { return twoThreshold; } }
    public int ThreeThresh  { get { return threeThreshold; } }
    public int FourThresh   { get { return fourThreshold; } }
    public int FiveThresh   { get { return fiveThreshold; } }
    public int SixThresh    { get { return sixThreshold; } }
    public int SevenThresh  { get { return sevenThreshold; } }
    public int EightThresh  { get { return eightThreshold; } }

    public bool GKPlayer    { get { return gameKit_playerProfile != null; } }

    public int CorrectCount
    {
        set
        {
            correctCount            = value;
            correctLabel.text       = correctCount.ToString();
        }

        get
        {
            return correctCount;
        }
    }

    public int IncorrectCount
    {
        set
        {
            incorrectCount          = value;
            incorrectLabel.text     = incorrectCount.ToString();
        }

        get
        {
            return incorrectCount;
        }
    }

    public int CurrentStreak
    {
        set
        {
            currentStreak           = value;
            currentStreakLabel.text = currentStreak.ToString();

            if (currentStreak > bestStreak)
                bestStreak          = currentStreak;

            bestStreakLabel.text    = bestStreak.ToString();
        }

        get
        {
            return currentStreak;
        }
    }

    public int BestStreak
    {
        private set
        {
            bestStreak              = value;
            bestStreakLabel.text    = bestStreak.ToString();
        }

        get { return bestStreak; }
    }

    public int TotalGuesses
    {
        get { return CorrectCount + IncorrectCount; }
    }

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        if (!LoadGame())
            saveData = new SaveFile();
    }

    private async Task Start()
    {
        //This is set here (instead of in load) because the UI will be linked at this point
        CurrentStreak   = saveData.Current;
        BestStreak      = saveData.Best;
        CorrectCount    = saveData.Correct;
        IncorrectCount  = saveData.Incorrect;

        try
        {
            gameKit_playerProfile = await GKLocalPlayer.Authenticate();

            Debug.Log("$$$$$$$Done Authenticating");

            if (gameKit_playerProfile != null)
            {
                GKAccessPoint.Shared.Location = GKAccessPoint.GKAccessPointLocation.TopLeading;
                GKAccessPoint.Shared.IsActive = true;

                await CheckAchievementProgress();
            }
        }
        catch (Exception e)
        {
            Debug.Log("$$$$$$$$$RIP");
            Debug.Log(e.Message);
        }
    }

    #endregion

    #region Public Functions

    public void ResetStats()
    {
        CurrentStreak   = 0;
        CorrectCount    = 0;
        IncorrectCount  = 0;
    }

    public void LinkUI(Label currentStreak, Label bestStreak, Label correct, Label incorrect)
    {
        currentStreakLabel      = currentStreak;
        bestStreakLabel         = bestStreak;
        correctLabel            = correct;
        incorrectLabel          = incorrect;
    }

    public void Guessed(bool correct)
    {
        if (correct)
        {
            CorrectCount++;
            CurrentStreak++;
        }
        else
        {
            int temp            = currentStreak;
            IncorrectCount++;
            CurrentStreak       = 0;

            this.PostNotification(Notifications.STREAK_FAILED, temp);
        }

        SaveGame();
    }

    public void UpdateSaveGameOnObjectiveComplete(Objective obj)
    {
        saveData.Objectives.Add(obj.ID);

        SaveGame();
    }

    #endregion

    #region Private Functions

    private void SaveGame()
    {
        string dir              = Application.persistentDataPath + saveDirectory;

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        saveData.Current        = currentStreak;
        saveData.Best           = bestStreak;
        saveData.Correct        = correctCount;
        saveData.Incorrect      = incorrectCount;

        string json             = JsonUtility.ToJson(saveData, true);

        File.WriteAllText(dir + fileName, json);
    }

    private bool LoadGame()
    {
        string filePath         = Application.persistentDataPath + saveDirectory + fileName;

        if (File.Exists(filePath))
        {
            string json         = File.ReadAllText(filePath);

            saveData            = JsonUtility.FromJson<SaveFile>(json);

            ObjectiveManager.instance.UpdateObjectivesBasedOnSaveData(saveData);

            return true;
        }

        return false;
    }

    private async Task CheckAchievementProgress()
    {
        Debug.Log("$$$$$$$$$$$Here. Waiting 6s....");

        await Task.Delay(new TimeSpan(0, 0, 6));

        Debug.Log("$$$$$$$$$$Wait complete");

        //GKAchievement a = GKAchievement.Init("0");

        //Debug.Log(string.Format(
        //    "+--------------------------+\n" +
        //    "| Identifier: {0}\n" +
        //    "| IsComplete: {1}\n" +
        //    "+--------------------------+",
        //    a.Identifier, a.IsCompleted));

        //var gameCenter = GKGameCenterViewController.Init(GKGameCenterViewController.GKGameCenterViewControllerState.Default);
        //await gameCenter.Present();

        //Debug.Log("$$$$$$$$$$$$$game center view showing");

        //Apple.Core.Runtime.NSArray<GKAchievementDescription> achievements = await GKAchievementDescription.LoadAchievementDescriptions();

        var achievements = await GKAchievement.LoadAchievements();

        Debug.Log("$$$$$$$$$$$$$ach count:" + achievements.Count.ToString());

        //for (int i = 0; i < achievements.Count; i++)
        //{
        //    if (achievements[i].IsCompleted)
        //        ObjectiveManager.instance.UpdateObjectivesBasedOnGKAchievements(achievements[i]);
        //}
    }

    #endregion

#if UNITY_EDITOR
    #region Editor Functions

    [MenuItem("Dev Commands/Delete Save File")]
    public static void DeleteSaveFile()
    {
        if (Application.isPlaying)
        {
            Debug.LogWarning("Editor is in Playmode. This function cannot be used");
            return;
        }

        string filepath         = Application.persistentDataPath + saveDirectory + fileName;

        if (!File.Exists(filepath))
        {
            Debug.Log("Save File not found at " + filepath);
            return;
        }

        File.Delete(filepath);
    }

    #endregion
#endif

}