using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

namespace Navigation
{
    public class MLAgentController : Agent
    {
        [Header("Agent Settings")]
        public float moveSpeed = 3f;
        public Transform[] attractions;

        private Vector3 startPosition;
        private Animator animator;

        private void Start()
        {
            startPosition = transform.position;
            animator = GetComponent<Animator>();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.position);
            sensor.AddObservation(transform.forward);

            if (GPSNavigationManager.Instance != null)
            {
                sensor.AddObservation(GPSNavigationManager.Instance.botTransform.position);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float move = actions.ContinuousActions[0];
            float turn = actions.ContinuousActions[1];

            MoveAgent(move, turn);

            if (GPSNavigationManager.Instance != null &&
                Vector3.Distance(transform.position, GPSNavigationManager.Instance.botTransform.position) < 1.0f)
            {
                SetReward(1f);
                EndEpisode();
            }

            if (HasCollided()) { SetReward(-1f); EndEpisode(); }

            SetReward(-0.01f);
        }

        private void MoveAgent(float move, float turn)
        {
            transform.Translate(Vector3.forward * move * moveSpeed * Time.deltaTime);
            transform.Rotate(Vector3.up, turn * moveSpeed * Time.deltaTime);

            if (move > 0) 
            {
                AnimationManager.Instance.PlayWalkingAnimation();
            }
            else
            {
                AnimationManager.Instance.PlayIdleAnimation();
            }
        }

        private bool HasCollided()
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1f))
            {
                return !IsAttraction(hit.transform);
            }
            return false;
        }

        private bool IsAttraction(Transform t)
        {
            foreach (var attraction in attractions)
            {
                if (t == attraction) return true;
            }
            return false;
        }

        // public override void EndEpisode()
        // {
        //     transform.position = startPosition;
        //     transform.rotation = Quaternion.identity;
        // }

        public void ResetAgent()
        {
            EndEpisode();
        }
    }
}
