using UnityEngine;
using UnityEngine.AI;

namespace Jellies.Behaviors
{
    // TODO: Replace MonoBehavior with State class once it is implemented
    /// <summary>
    /// 
    /// </summary>
    public class Wandering : MonoBehaviour
    {

        // Private Non-serialized fields
        private bool _canWander = true;
        private float _nearDistance = 0.1f;
        private float _wanderDistance = 0.5f;
        private float _wanderRange = 2.0f;
        private NavMeshAgent _agent;
        // TODO: Uncomment when ItemBase class is implemented
        // private ItemBase _berryItemBase;
        private NavMeshPath _currentPath;

        // Public fields
        public System.Action<bool> OnChange;

        /// <summary>
        /// 
        /// </summary>
        private void Awake()
        {
            // TODO
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            // TODO
        }


        /// <summary>
        /// 
        /// </summary>
        private void Update()
        {
            // TODO
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public void ToggleWander(bool state)
        {
            // TODO
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nearDistance"></param>
        /// <returns></returns>
        private bool IsNearDestination(float nearDistance)
        {
            // TODO
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool ChooseDestination()
        {
            // TODO
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            // TODO
            result = Vector3.zero;
            return true;
        }


    }

}