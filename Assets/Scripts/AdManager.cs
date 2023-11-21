using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    #region Singleton

    public static AdManager instance;

    #endregion

    #region Unity Functions

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        //TODO - not sure if this should be UNITY_IPHONE or _IOS
        #if UNITY_IOS
            string appKey = "1c8351c9d";
        #else
            string appKey = "unexpected_platform";
        #endif

        IronSource.Agent.validateIntegration();
        IronSource.Agent.init(appKey);
    }

    private void OnEnable()
    {
        IronSourceEvents.onSdkInitializationCompletedEvent += () => { Debug.Log("IronSource SDK Intialized"); };
    }

    private void OnApplicationPause(bool pause)
    {
        IronSource.Agent.onApplicationPause(pause);
    }

    #endregion

    #region Public Functions

    public void LoadBanner()
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
    }

    #endregion
}
