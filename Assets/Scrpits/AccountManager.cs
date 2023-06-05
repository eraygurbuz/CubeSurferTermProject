using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.UI;
using HmsPlugin;
using System;

public class AccountManager : MonoBehaviour
{
    private readonly string TAG = "[HMS] AccountManager ";
    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGIN_ERROR = "Error or cancelled login";
    [SerializeField] private TMPro.TMP_Text DisplayName;


    public static Action<string> AccountKitLog;

    public static Action AccountKitIsActive;

    #region Singleton

    public static AccountManager Instance { get; private set; }
    private void Singleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    void Start()
    {
        HMSAccountKitManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountKitManager.Instance.OnSignInFailed = OnLoginFailure;
        AccountKitLog?.Invoke(NOT_LOGGED_IN);
        HMSAccountKitManager.Instance.SilentSignIn();
    }

    public void SilentSignIn()
    {
        Debug.Log(TAG + "SilentSignIn");

        HMSAccountKitManager.Instance.SilentSignIn();
    }

    public void OnLoginSuccess(AuthAccount authHuaweiId)
    {
        AccountKitLog?.Invoke(string.Format(LOGGED_IN, authHuaweiId.DisplayName));
        DisplayName.SetText(authHuaweiId.DisplayName.ToUpper(), false);
    }

    public void OnLoginFailure(HMSException error)
    {
        AccountKitLog?.Invoke(LOGIN_ERROR);
        DisplayName.SetText("LOGIN", true);
    }

}