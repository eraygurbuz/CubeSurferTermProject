using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    bool isGameEnded = false;

    public GameObject completeLevelUI;
    public TMPro.TextMeshProUGUI completeLevelGemTMP;
    public TMPro.TextMeshProUGUI completeLevelTMP;
    public GameObject failLevelUI;
    public GameObject mainScreenUI;
    public GameObject inGameUI;
    public GameObject sliderUI;
    public GameObject pauseMenuUI;
    public AnimationController animControl;

    public TMPro.TextMeshProUGUI gemCounterTMP;

    public GameObject player;

    public TMPro.TextMeshProUGUI[] levelTxt;
    public TMPro.TextMeshProUGUI progressBarTMP;

    int currentGemCounter = 0;
    int currentMultiplier = 1;
    //ADS
    bool doubleReward = false;
    bool skipRequest = false;
    bool doubleRewardRequest = false;

    //IAP
    bool noAds = false;

    int totalGemCounter = 0;

    bool isLevelFinished = false;
    bool isLevelCompleted = false;

    int currentLevel;

    private void Awake()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            currentLevel = PlayerPrefs.GetInt("Level");
        }
        else
        {
            PlayerPrefs.SetInt("Level", 1);
            currentLevel = 1;
        }
        //this works for initial state
        if (SceneManager.GetActiveScene().buildIndex != ((currentLevel - 1) % 5))
        {
            if (currentLevel % 5 != 0)
                SceneManager.LoadScene((currentLevel - 1) % 5);
            else
                SceneManager.LoadScene(0);
        }

        if (PlayerPrefs.HasKey("Gems"))
        {
            totalGemCounter = PlayerPrefs.GetInt("Gems");
        }
        else
        {
            PlayerPrefs.SetInt("Gems", 0);
            totalGemCounter = 0;
        }
        
        if(IAPManager.Instance.IsNoAdsPurchased())
        {
            noAds = true;
        }
        
        
        gemCounterTMP.SetText(totalGemCounter + "");
        progressBarTMP.SetText(currentLevel+"");
        if (currentLevel % 5 == 1)
        {
            levelTxt[0].SetText(currentLevel + "");
            levelTxt[1].SetText((currentLevel + 1) + "");
            levelTxt[2].SetText((currentLevel + 2) + "");
            levelTxt[3].SetText((currentLevel + 3) + "");
            levelTxt[4].SetText((currentLevel + 4) + "");
        }
        else if (currentLevel % 5 == 2)
        {
            levelTxt[0].SetText((currentLevel - 1) + "");
            levelTxt[1].SetText(currentLevel + "");
            levelTxt[2].SetText((currentLevel + 1) + "");
            levelTxt[3].SetText((currentLevel + 2) + "");
            levelTxt[4].SetText((currentLevel + 3) + "");
        }
        else if (currentLevel % 5 == 3)
        {
            levelTxt[0].SetText((currentLevel - 2) + "");
            levelTxt[1].SetText((currentLevel - 1) + "");
            levelTxt[2].SetText(currentLevel + "");
            levelTxt[3].SetText((currentLevel + 1) + "");
            levelTxt[4].SetText((currentLevel + 2) + "");
        }
        else if (currentLevel % 5 == 4)
        {
            levelTxt[0].SetText((currentLevel - 3) + "");
            levelTxt[1].SetText((currentLevel - 2) + "");
            levelTxt[2].SetText((currentLevel - 1) + "");
            levelTxt[3].SetText(currentLevel + "");
            levelTxt[4].SetText((currentLevel + 1) + "");
        }
        else
        {
            levelTxt[0].SetText((currentLevel - 4) + "");
            levelTxt[1].SetText((currentLevel - 3) + "");
            levelTxt[2].SetText((currentLevel - 2) + "");
            levelTxt[3].SetText((currentLevel - 1) + "");
            levelTxt[4].SetText(currentLevel + "");
        }

        if(!noAds)
            AdsManager.Instance.ShowBannerAd();
    }

    public void StopLevel()
    {
        if (isLevelFinished)
        {
            animControl.Dance();
            CompleteLevel(player.GetComponent<CubeHandler>().GetMultiplier());
        }
        else if (!isGameEnded && !isLevelFinished)
        {
            isGameEnded = true;
            DisablePlayerInput();
            failLevelUI.SetActive(true);
            inGameUI.SetActive(false);
            animControl.Death();
            Debug.Log("GAME OVER!");
        }
    }

    public void CompleteLevel(int multiplier)
    {
        if (!isLevelCompleted)
        {
            doubleReward = false;
            currentMultiplier = multiplier;
            isLevelCompleted = true;
            Debug.Log("Complete Level!");
            DisablePlayerInput();

            completeLevelTMP.SetText("GREAT!\n" + multiplier + "X");
            completeLevelGemTMP.SetText(currentGemCounter * multiplier + "");
            completeLevelUI.SetActive(true);
            inGameUI.SetActive(false);
        }
    }

    public void DoubleGem()
    {
        if (!doubleReward)
        {
            currentGemCounter *= 2;
            completeLevelGemTMP.SetText(currentGemCounter * currentMultiplier + "");
            doubleReward = true;
        }
    }

    public void StartLevel()
    {
        Debug.Log("Level Started!");
        sliderUI.SetActive(false);
        mainScreenUI.SetActive(false);
        inGameUI.SetActive(true);
        EnablePlayerInput();
        currentGemCounter = 0;
        currentMultiplier = 1;
        skipRequest = false;
        doubleRewardRequest = false;
        doubleReward = false;
    }

    public void SkipLevel()
    {
        if(!skipRequest)
        {
            skipRequest = true;
            AdsManager.Instance.ShowRewardedAd();
        }
    }

    public void DoubleRewardButton()
    {
        if(!doubleRewardRequest)
        {
            doubleRewardRequest = true;
            AdsManager.Instance.ShowRewardedAd();
        }
    }

    public void AdsResponse()
    {
        if (skipRequest)
        {
            GameObject.Find("SkipButton").GetComponent<Button>().interactable = false;
            LoadNextLevel();
        }
        else if(doubleRewardRequest)
        {
            GameObject.Find("DoubleGemButton").GetComponent<Button>().interactable = false;
            DoubleGem();
        }
    }

    public void RestartButton()
    {
        if(!noAds)
            AdsManager.Instance.ShowInterstitialAd();
        Invoke("Restart", 1f);
    }

    public void NextButton()
    {   
        totalGemCounter += (currentGemCounter * currentMultiplier);
        PlayerPrefs.SetInt("Gems", totalGemCounter);
        if (!noAds)
            AdsManager.Instance.ShowInterstitialAd();
        Invoke("LoadNextLevel", 1f);
    }

    public void SwipeToPlayButton()
    {
        StartLevel();
    }

    void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel()
    {
        PlayerPrefs.SetInt("Level", currentLevel+1);
        if (currentLevel % 5 != 0)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);//loads next scene on the build order
        else
            SceneManager.LoadScene(0);//loads next scene on the build order
    }

    public void TakeGem()
    {
        currentGemCounter++;
        gemCounterTMP.SetText(totalGemCounter + currentGemCounter + "");
    }

    public void LevelFinished()
    {
        isLevelFinished = true;
    }
    void DisablePlayerInput()
    {
        player.GetComponent<SwerveInputSystem>().DisableSwerveInput();
        player.GetComponent<SwerveMovement>().DisableSwerveMove();
    }

    void EnablePlayerInput()
    {
        player.GetComponent<SwerveInputSystem>().EnableSwerveInput();
        player.GetComponent<SwerveMovement>().EnableSwerveMove();
    }

    public void PauseButton()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeButton()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1;
    }

    //IAP Buttons
    public void NoAdsButton()
    {
        IAPManager.Instance.PurchaseProduct("NoAdsProduct");
    }
    public void RemoveAds()
    {
        noAds = true;
        AdsManager.Instance.HideBannerAd();
    }

    public void BuyGems(int amount)
    {
        if(amount == 10000)
        {
            IAPManager.Instance.PurchaseProduct("10KGems");
        }
    }


    public void AddGems(int amount)
    {
        totalGemCounter += amount;
        PlayerPrefs.SetInt("Gems", totalGemCounter);
    }
}
