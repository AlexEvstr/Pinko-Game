using System.Runtime.InteropServices;
using UnityEngine;

public class HapticFeedbackManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void TriggerLightHaptic();

    [DllImport("__Internal")]
    private static extern void TriggerMediumHaptic();

    [DllImport("__Internal")]
    private static extern void TriggerHeavyHaptic();

    [DllImport("__Internal")]
    private static extern void TriggerErrorHaptic();

    public void LightHaptic()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            TriggerLightHaptic();
        }
    }

    public void MediumHaptic()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            TriggerMediumHaptic();
        }
    }

    public void HeavyHaptic()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            TriggerHeavyHaptic();
        }
    }

    public void ErrorHaptic()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            TriggerErrorHaptic();
        }
    }
}