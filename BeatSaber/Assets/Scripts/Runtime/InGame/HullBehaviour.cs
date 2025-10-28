using System.Collections;
using UnityEngine;

namespace BeatSaber.Runtime.InGame
{
    public class HullBehaviour : MonoBehaviour
    {
        public void DoMove(Vector3 velocity, Vector3 normal, float deceleration, float rotationSpeed)
        {
            StartCoroutine(C_Moving(velocity, normal, deceleration, rotationSpeed));
        }

        IEnumerator C_Moving(Vector3 velocity, Vector3 normal, float deceleration, float rotationSpeed)
        {
            Transform cam = Camera.main.transform;

            while (true)
            {
                float dt = Time.deltaTime;
                transform.position += velocity * dt;
                velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * dt);

                if (velocity.sqrMagnitude <= 0.01f)
                {
                    Destroy(gameObject);
                    yield break;
                }

                Vector3 toCam = (cam.position - transform.position).normalized;
                Quaternion targetRotation = Quaternion.FromToRotation(normal, toCam) * transform.rotation;
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * dt);
                yield return null;
            }
        }
    }
}