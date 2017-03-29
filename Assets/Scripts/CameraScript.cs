using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pool
{
    public class CameraScript : MonoBehaviour
    {
        [HideInInspector] public bool IsReady { get; set; }

        //Position and rotation of possible viewpoints
        Vector3[] viewPositions = { new Vector3(15, 10, 0), new Vector3(-15, 10, 0), new Vector3(8, 8, -20), new Vector3(-8, 8, -20), new Vector3(-8, 8, 20), new Vector3(8, 8, 20), };
        Vector3[] viewRotation = { new Vector3(40, -90, 0), new Vector3(40, 90, 0), new Vector3(30, -30, 0), new Vector3(30, 30, 0), new Vector3(30, 150, 0), new Vector3(30, 210, 0) };

        [HideInInspector]
        public GameObject player; // Reference to player

        /// <summary>
        /// Starts corouting for moving camera to closest viewpoint
        /// </summary>
        public void BallToViewpoint()
        {
            IsReady = false;
            StartCoroutine(ToViewpoint(2.5f));
        }

        /// <summary>
        /// Start coroutine for moving camera to ball
        /// </summary>
        public void CamToBall()
        {
            if (IsReady)
            {
                IsReady = false;
                Vector3 newCamPos = new Vector3(player.transform.position.x, player.transform.position.y + 1.0f, player.transform.position.z - 3.0f);
                Quaternion newCamRot = Quaternion.Euler(10.0f, 0.0f, 0.0f);
                StartCoroutine(ToBall(newCamPos, newCamRot));
            }
        }

        /// <summary>
        /// Coroutine for moving camera to closest viewpoint
        /// </summary>
        /// <param name="transitionDuration"></param>
        /// <returns></returns>
        public IEnumerator ToViewpoint(float transitionDuration = 2.0f)
        {
            int targetPos = GetClosestViewPoint();
            float t = 0.0f;
            Vector3 startingPos = transform.position;
            Quaternion startingRot = transform.rotation;

            while (t < 1.0f)
            {
                // Lerp range is 0 - 1, so we need to have t in that range
                t += Time.deltaTime * (Time.timeScale / transitionDuration);

                transform.position = Vector3.Lerp(startingPos, viewPositions[targetPos], t);
                transform.rotation = Quaternion.Lerp(startingRot, Quaternion.Euler(viewRotation[targetPos]), t);
                yield return null;
            }
            IsReady = true;
        }

        /// <summary>
        /// Coroutine for moving camera to ball
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="targetRot"></param>
        /// <param name="transitionDuration"></param>
        /// <returns></returns>
        public IEnumerator ToBall(Vector3 targetPos, Quaternion targetRot, float transitionDuration = 2.0f)
        {
            float t = 0.0f;
            Vector3 startingPos = transform.position;
            Quaternion startingRot = transform.rotation;

            while (t < 1.0f)
            {
                // Lerp range is 0 - 1, so we need to have t in that range
                t += Time.deltaTime * (Time.timeScale / transitionDuration);

                transform.position = Vector3.Lerp(startingPos, targetPos, t);
                transform.rotation = Quaternion.Lerp(startingRot, targetRot, t);
                yield return null;
            }
            IsReady = true;
        }

        /// <summary>
        ///  Find viewpoint closest to camera
        /// </summary>
        /// <returns></returns>
        private int GetClosestViewPoint()
        {
            int closestViewPoint = -1;
            float lastDist = -1.0f;
            float newDist = 0.0f;
            for (int i = 0; i < viewPositions.Length; ++i)
            {
                newDist = Vector3.Distance(this.transform.position, viewPositions[i]);
                if (newDist < lastDist || lastDist < 0)
                {
                    lastDist = newDist;
                    closestViewPoint = i;
                }
            }
            return closestViewPoint;
        }
    }
}
