using UnityEngine;

public class ParticalAutoAdjust : MonoBehaviour
{
    public ParticleSystem[] particleSystems;

    void Start()
    {
        particleSystems = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        AutoAdjust();
    }

    private void AutoAdjust()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            for (int i = 0; i < particleSystems.Length; i++)
            {
                particleSystems[i].maxParticles = particleSystems[i].maxParticles / 5;
            }
        }
    }
}