using HmsPlugin;

using HuaweiMobileServices.IAP;
using HuaweiMobileServices.Utils;

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour
{
    public static Action<string> IAPLog;

    List<InAppPurchaseData> consumablePurchaseRecord = new List<InAppPurchaseData>();
    List<InAppPurchaseData> activeNonConsumables = new List<InAppPurchaseData>();

    [SerializeField]
    private Button NoAdsButton;

    #region Singleton

    public static IAPManager Instance { get; private set; }
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

    #region Monobehaviour

    private void OnEnable()
    {
        HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;
        HMSIAPManager.Instance.OnInitializeIAPSuccess += OnInitializeIAPSuccess;
        HMSIAPManager.Instance.OnInitializeIAPFailure += OnInitializeIAPFailure;
        HMSIAPManager.Instance.OnBuyProductFailure += OnBuyProductFailure;
    }

    private void OnDisable()
    {
        HMSIAPManager.Instance.OnBuyProductSuccess -= OnBuyProductSuccess;
        HMSIAPManager.Instance.OnInitializeIAPSuccess -= OnInitializeIAPSuccess;
        HMSIAPManager.Instance.OnInitializeIAPFailure -= OnInitializeIAPFailure;
        HMSIAPManager.Instance.OnBuyProductFailure -= OnBuyProductFailure;
    }

    void Awake()
    {
        Singleton();
        Screen.orientation = ScreenOrientation.Landscape;
    }

    void Start()
    {
        // Uncomment below if InitializeOnStart is not enabled in Huawei > Kit Settings > IAP tab.
        //HMSIAPManager.Instance.InitializeIAP();
    }

    #endregion

    public void InitializeIAP()
    {
        Debug.Log($"InitializeIAP");

        HMSIAPManager.Instance.InitializeIAP();
    }



    public void PurchaseProduct(string productID)
    {
        Debug.Log($"PurchaseProduct");

        HMSIAPManager.Instance.PurchaseProduct(productID);
    }


    private void RestoreProducts()
    {

        HMSIAPManager.Instance.RestorePurchaseRecords((restoredProducts) =>
        {
            foreach (var item in restoredProducts.InAppPurchaseDataList)
            {
                if ((IAPProductType)item.Kind == IAPProductType.Consumable)
                {
                    Debug.Log($"Consumable: ProductId {item.ProductId} , SubValid {item.SubValid} , PurchaseToken {item.PurchaseToken} , OrderID  {item.OrderID}");
                    consumablePurchaseRecord.Add(item);
                }
            }
        });

        HMSIAPManager.Instance.RestoreOwnedPurchases((restoredProducts) =>
        {
            foreach (var item in restoredProducts.InAppPurchaseDataList)
            {
                if ((IAPProductType)item.Kind == IAPProductType.NonConsumable)
                {
                    Debug.Log($"NonConsumable: ProductId {item.ProductId} , DaysLasted {item.DaysLasted} , SubValid {item.SubValid} , PurchaseToken {item.PurchaseToken} ,OrderID {item.OrderID}");
                    activeNonConsumables.Add(item);
                }
            }
        });

    }


    #region Callbacks

    private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        Debug.Log($"OnBuyProductSuccess");

        if (obj.InAppPurchaseData.ProductId == "removeads")
        {
            IAPLog?.Invoke("Ads Removed!");
        }
        else if (obj.InAppPurchaseData.ProductId == "coins100")
        {
            IAPLog?.Invoke("coins100 Purchased!");
        }
        else if (obj.InAppPurchaseData.ProductId == "premium")
        {
            IAPLog?.Invoke("premium subscribed!");
        }
    }

    private void OnInitializeIAPFailure(HMSException obj)
    {
        IAPLog?.Invoke("IAP is not ready.");
    }

    private void OnInitializeIAPSuccess()
    {
        IAPLog?.Invoke("IAP is ready.");

        RestoreProducts();
    }

    private void OnBuyProductFailure(int code)
    {
        IAPLog?.Invoke("Purchase Fail.");
    }

    #endregion
}
