using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public Enemy enemy;

    public void OnDeathAnimationComplete()
    {
        enemy.OnDeathAnimationComplete();
    }
}