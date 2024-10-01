using UnityEngine;
using UnityEngine.Advertisements;

public class BannerAdvertisment : MonoBehaviour, IUnityAdsInitializationListener
{
    [SerializeField] BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

    [SerializeField] string androidGameId = "YOUR_ANDROID_GAME_ID";
    [SerializeField] string iosGameId = "YOUR_IOS_GAME_ID";
    [SerializeField] string androidAdUnitId = "Banner_Android";
    [SerializeField] string iosAdUnitId = "Banner_iOS";

    private string gameId;
    private string adUnitId;
    private bool isBannerLoaded = false;

    void Start()
    {
#if UNITY_IOS
        gameId = iosGameId;
        adUnitId = iosAdUnitId;
#elif UNITY_ANDROID
        gameId = androidGameId;
        adUnitId = androidAdUnitId;
#endif

        if (string.IsNullOrEmpty(gameId))
        {
            return;
        }

        Advertisement.Initialize(gameId, testMode: false, this);

        Advertisement.Banner.SetPosition(bannerPosition);
    }

    public void OnInitializationComplete()
    {
        LoadBanner();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
    }

    private void LoadBanner()
    {
        if (!Advertisement.isInitialized)
        {
            return;
        }

        if (isBannerLoaded)
        {
            Advertisement.Banner.Hide(true);
            isBannerLoaded = false;
        }

        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerError
        };

        Advertisement.Banner.Load(adUnitId, options);
    }

    void OnBannerLoaded()
    {
        isBannerLoaded = true;
        ShowBanner();
    }

    void OnBannerError(string message)
    {
        isBannerLoaded = false;

        Advertisement.Banner.Hide(true);

        Invoke("LoadBanner", 5f);
    }

    private void ShowBanner()
    {
        BannerOptions options = new BannerOptions
        {
            clickCallback = OnBannerClicked,
            hideCallback = OnBannerHidden,
            showCallback = OnBannerShown
        };

        Advertisement.Banner.Show(adUnitId, options);
    }

    void OnBannerClicked()
    {
    }

    void OnBannerShown()
    {
    }

    void OnBannerHidden()
    {
        isBannerLoaded = false;
    }
}