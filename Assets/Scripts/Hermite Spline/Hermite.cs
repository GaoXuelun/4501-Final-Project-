/*Attach Hermite.cs script to an object. In inspector, set way points in Way point Objects, 
set tangents of way points in Tangents. Amount of way points and tangents should be same.*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hermite : MonoBehaviour
{
    public List<GameObject> waypointObjects;    // List of waypoints
    public List<Vector3> tangents;  // List of tangent
    public float moveSpeed = 20f;    // Movement speed
    public int numSegments = 50;    // Number of segments

    private void Start()
    {
        // Check if the waypoint and tangent lists have the same length
        if (waypointObjects.Count != tangents.Count)
        {
            Debug.LogError("WaypointObjects and tangents lists must have the same length.");
            return;
        }
        
        StartCoroutine(MoveAlongHermiteCurve());
    }

    private IEnumerator MoveAlongHermiteCurve()
    {
        //Loop all waypoints, moving the object
        for (int i = 0; i < waypointObjects.Count - 1; i++)
        {
            Vector3 pointA = waypointObjects[i].transform.position;
            Vector3 pointB = waypointObjects[i + 1].transform.position;
            Vector3 tangentA = tangents[i];
            Vector3 tangentB = tangents[i + 1];
            // the length of the curve between two waypoints
            float journeyLength = CalculateCurveLength(pointA, pointB, tangentA, tangentB);
            float startTime = Time.time;

            while (true)
            {
                // progress along the curve
                float distanceCovered = (Time.time - startTime) * moveSpeed;
                float t = Mathf.Clamp01(distanceCovered / journeyLength);
                float easedT = SmoothStep(t);
                //  current point on the Hermite curve
                Vector3 currentPoint = CalculateHermitePoint(pointA, pointB, tangentA, tangentB, easedT);
                transform.position = currentPoint;
                //  forward side and up directions
                Vector3 forwardDirection = CalculateHermiteTangent(pointA, pointB, tangentA, tangentB, easedT);
                Vector3 sideDirection = Vector3.Cross(CalculateHermiteTangent(pointA, pointB, tangentA, tangentB, easedT), CalculateHermiteTangent2(pointA, pointB, tangentA, tangentB, easedT));
                Vector3 upDirection = Vector3.Cross(forwardDirection, sideDirection);
                
                if (tangentB.z < 0)
                    transform.rotation = Quaternion.LookRotation(-upDirection);
                if (tangentB.z > 0)
                    transform.rotation = Quaternion.LookRotation(upDirection);
                // Break when the object reaches the next waypoint
                if (t >= 1)
                    break;
                yield return null;
            }
        }
    }

    // length of Hermite curve between two way points
    private float CalculateCurveLength(Vector3 pointA, Vector3 pointB, Vector3 tangentA, Vector3 tangentB)
    {
        float length = 0f;
        Vector3 previousPoint = pointA;
        // total distances between curve points
        for (int i = 1; i <= numSegments; i++)
        {
            float t = (float)i / numSegments;
            Vector3 currentPoint = CalculateHermitePoint(pointA, pointB, tangentA, tangentB, t);
            length += Vector3.Distance(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        return length;
    }
    // position of two points, tangents

    private Vector3 CalculateHermitePoint(Vector3 start, Vector3 end, Vector3 tangentStart, Vector3 tangentEnd, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        float h00 = 2f * t3 - 3f * t2 + 1f;
        float h01 = -2f * t3 + 3f * t2;
        float h10 = t3 - 2f * t2 + t;
        float h11 = t3 - t2;

        return h00 * start + h01 * end + h10 * tangentStart + h11 * tangentEnd;
    }
    // first derivative of tangent
    private Vector3 CalculateHermiteTangent(Vector3 start, Vector3 end, Vector3 tangentStart, Vector3 tangentEnd, float t)
    {
        float t2 = t * t;

        float h00 = 6f * t2 - 6f * t;
        float h01 = -6f * t2 + 6f * t;
        float h10 = 3f * t2 - 4f * t + 1f;
        float h11 = 3f * t2 - 2f * t;

        return h00 * start + h01 * end + h10 * tangentStart + h11 * tangentEnd;
    }
    // second derivative
    private Vector3 CalculateHermiteTangent2(Vector3 start, Vector3 end, Vector3 tangentStart, Vector3 tangentEnd, float t)
    {
        float t2 = t * t;

        float h00 = 12f * t - 6f;
        float h01 = -12f * t + 6f;
        float h10 = 6f * t - 4f;
        float h11 = 6f * t - 2f;

        return h00 * start + h01 * end + h10 * tangentStart + h11 * tangentEnd;
    }
    //  ease the movement
    private float SmoothStep(float t)
    {
        if (t < 0.5f)
            return 4f * t * t * t;
        else
        {
            t = 2f * t - 2f;
            return 0.5f * t * t * t + 1f;
        }
    }
}

