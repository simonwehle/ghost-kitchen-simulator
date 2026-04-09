using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UnityConsent;

public class Analytics : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UnityServices.InitializeAsync();

        EndUserConsent.SetConsentState(new ConsentState
        {
            AnalyticsIntent = ConsentStatus.Granted,
            AdsIntent = ConsentStatus.Denied
        });
    }
}
