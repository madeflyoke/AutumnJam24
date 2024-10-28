using Lean.Pool;
using UnityEngine;

namespace Tools
{
    public class ParticlePoolReturner : MonoBehaviour
    {
        private void OnParticleSystemStopped()
        {
            LeanPool.Despawn(transform);
        }
    }
}
