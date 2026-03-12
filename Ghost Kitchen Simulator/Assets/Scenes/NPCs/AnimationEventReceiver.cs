using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    // Diese Funktionen fangen die Events der Starter Asset Animationen ab
    public void OnFootstep(AnimationEvent animationEvent)
    {
        // Hier könntest du später Sound-Logik einbauen
    }

    public void OnLand(AnimationEvent animationEvent)
    {
        // Wird beim Landen nach einem Sprung aufgerufen
    }
}