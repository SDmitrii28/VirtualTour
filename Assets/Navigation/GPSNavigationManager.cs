using UnityEngine;
using System.Collections.Generic;

namespace Navigation
{
    public class GPSNavigationManager : MonoBehaviour
    {
        public static GPSNavigationManager Instance;

        [Header("Navigation")]
        public Transform botTransform;
        public float speed = 1.5f;
        public float arrivalThreshold = 1.2f;

        private Queue<Vector3> destinationQueue = new Queue<Vector3>();
        private Vector3 currentTarget;
        private bool isNavigating;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Update()
        {
            if (isNavigating)
                MoveBot();
        }

        public void StartNavigation(List<Vector3> points)
        {
            destinationQueue.Clear();

            foreach (var point in points)
                destinationQueue.Enqueue(point);

            if (destinationQueue.Count > 0)
            {
                currentTarget = destinationQueue.Dequeue();
                isNavigating = true;
                AnimationManager.Instance.PlayWalkingAnimation();
            }
        }

        private void MoveBot()
        {
            Vector3 direction = (currentTarget - botTransform.position).normalized;
            botTransform.position += direction * speed * Time.deltaTime;
            botTransform.LookAt(currentTarget);

            if (Vector3.Distance(botTransform.position, currentTarget) < arrivalThreshold)
            {
                if (destinationQueue.Count > 0)
                {
                    currentTarget = destinationQueue.Dequeue();
                }
                else
                {
                    isNavigating = false;
                    AnimationManager.Instance.PlayIdleAnimation();
                    BotInteractionManager.Instance.OnAttractionReached();
                }
            }
        }

        public void StopNavigation()
        {
            isNavigating = false;
            destinationQueue.Clear();
        }

        public bool IsNavigating() => isNavigating;
    }
}
