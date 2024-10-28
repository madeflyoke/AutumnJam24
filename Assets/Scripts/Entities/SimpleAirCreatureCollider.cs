using UnityEngine;

namespace Entities
{
    public class SimpleAirCreatureCollider : MonoBehaviour
    {
        public bool collideSomething;

        [HideInInspector]
        public SimpleAirCreatureController controller;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.GetComponent<SimpleAirCreatureCollider>() == null && other.gameObject.GetComponent<LandingArea>() == null)
            {
                collideSomething = true;
            }
        }
    }
}