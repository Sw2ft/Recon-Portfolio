using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Splines;

namespace Cinemachine
{
    public class RiverDolly : MonoBehaviour
    {
        public SplineContainer splineContainer; // Reference to the SplineContainer
        public Transform targetCamera;         // Reference to the camera or target object
        public GameObject objectToMove;        // The object that will move along the spline

        void Update()
        {
            if (splineContainer != null && targetCamera != null && objectToMove != null)
            {
                // Find the closest point on the spline to the camera
                float closestT = GetClosestPointOnSpline(targetCamera.position);

                // Move the object to the closest point on the spline
                objectToMove.transform.position = splineContainer.Spline.EvaluatePosition(closestT);
            }
        }

        float GetClosestPointOnSpline(Vector3 targetPosition)
        {
            Spline spline = splineContainer.Spline;
            float closestT = 0f;
            float closestDistance = float.MaxValue;

            // Iterate over points on the spline
            const int samples = 100; // Number of samples along the spline
            for (int i = 0; i <= samples; i++)
            {
                float t = i / (float)samples; // Normalized position along the spline (0 to 1)
                Vector3 splinePoint = spline.EvaluatePosition(t);
                float distance = Vector3.Distance(targetPosition, splinePoint);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestT = t;
                }
            }

            return closestT;
        }
    }
}

