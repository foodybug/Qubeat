using System;
using UnityEngine;
using System.Collections;
//using GoogleMobileAds;
//using GoogleMobileAds.Api;

public class AdvertisementManager : MonoBehaviour
{
    #region - singleton -
    static AdvertisementManager m_Instance; public static AdvertisementManager Instance { get { return m_Instance; } }
    #endregion

    //BannerView banner;
    //InterstitialAd interstitial;

    void Awake()
    {
        #region - singleton -
        m_Instance = this;
        #endregion

        DontDestroyOnLoad(gameObject);

#if !UNITY_EDITOR && UNITY_ANDROID && UNITY_IOS
        //banner
        banner = new BannerView("ca-app-pub-8674279891927404/5839737170", AdSize.Banner, AdPosition.Top);
        banner.OnAdLoaded += OnBannerLoaded;
        banner.OnAdFailedToLoad += OnBannerLoadingFailed;

        AdRequest request = new AdRequest.Builder().Build();
 
        banner.LoadAd(request);
        banner.Hide();
        
        //interstitial
        interstitial = new InterstitialAd("ca-app-pub-8674279891927404/8603136777");
        interstitial.OnAdLoaded += OnInterstitialLoaded;
        interstitial.OnAdFailedToLoad += OnInterstitialLoadingFailed;

        AdRequest requestInterstitial = new AdRequest.Builder().Build();
        interstitial.LoadAd(requestInterstitial);
#endif
    }

    public void Show_Banner()
    {
#if !UNITY_EDITOR && UNITY_ANDROID && UNITY_IOS
		banner.Show();
#endif
    }

    public void Close_Banner()
    {
#if !UNITY_EDITOR && UNITY_ANDROID && UNITY_IOS
		banner.Hide();
#endif
    }

    void OnDestroy()
    {
#if !UNITY_EDITOR && UNITY_ANDROID && UNITY_IOS
        banner.Destroy();
#endif
    }

    public void Show_Interstitial()
    {
#if !UNITY_EDITOR && UNITY_ANDROID && UNITY_IOS
        Debug.Log("interstitial.IsLoaded() == " + interstitial.IsLoaded());
        if (interstitial.IsLoaded())
        {
            interstitial.Show();
        }
#endif
    }

    public bool Check_InterstitialIsLoaded()
    {
#if !UNITY_EDITOR && UNITY_ANDROID && UNITY_IOS
        return interstitial.IsLoaded();
#endif
        return true;
    }

    #region callback
    //public void OnBannerLoaded(object sender, EventArgs args)
    //{
    //    print("OnBannerLoaded");
    //}

    //public void OnBannerLoadingFailed(object sender, AdFailedToLoadEventArgs args)
    //{
    //    print("OnBannerLoadingFailed : " + args.Message);
    //}

    //public void OnInterstitialLoaded(object sender, EventArgs args)
    //{
    //    print("OnInterstitialLoaded");
    //}

    //public void OnInterstitialLoadingFailed(object sender, AdFailedToLoadEventArgs args)
    //{
    //    print("OnInterstitialLoadingFailed : " + args.Message);
    //}
    #endregion
}