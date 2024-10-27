using Lean.Pool;
using UnityEngine;

public class ParticlePoolReturner : MonoBehaviour
{
    private void OnParticleSystemStopped()
    {
        LeanPool.Despawn(transform);
    }
}
