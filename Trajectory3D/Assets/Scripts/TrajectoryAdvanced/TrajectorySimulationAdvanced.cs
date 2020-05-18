using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class TrajectorySimulationAdvanced : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public int segmentsCount = 20;

    public float segmentLength = 1;

    public Rigidbody2D objectPrefab;

    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
        {
            var points = GetTrajectoryPoints(transform.position, GetForce() / GetObjectMass());
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.Select(v => new Vector3(v.x, v.y, transform.position.z)).ToArray());
        }
    }

    private Vector3 GetForce()
    {
        var fromPos = transform.position;
        var toPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (toPos - fromPos) * 1;
    }

    public float GetObjectMass()
    {
        return objectPrefab.mass;
    }

    List<Vector3> GetTrajectoryPoints(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] segments = new Vector3[segmentsCount];

        segments[0] = startPosition;

        Vector3 velocity = initialVelocity;

        var bounciness = objectPrefab.sharedMaterial.bounciness;
        var drag = objectPrefab.drag;
        var gravityScale = objectPrefab.gravityScale;

        for (int i = 1; i < segmentsCount; i++)
        {
            float segTime = (velocity.sqrMagnitude != 0) ? segmentLength / velocity.magnitude : 0;

            RaycastHit2D hit = Physics2D.CircleCast(segments[i - 1], 0.5f, velocity, segmentLength);
            if (hit.collider != null && (Vector2)segments[i - 1] != hit.centroid)
            {
                var hitSegTime = hit.distance / velocity.magnitude;
                segments[i] = segments[i - 1] + velocity * hitSegTime + Physics.gravity * gravityScale * hitSegTime * hitSegTime / 2;

                velocity += Physics.gravity * gravityScale * hitSegTime;

                var normalVelocity = Vector3.Project(velocity, hit.normal);
                var tangentVelocity = velocity - normalVelocity;

                velocity = tangentVelocity - normalVelocity * bounciness;
                velocity *= Mathf.Clamp01(1f - drag * hitSegTime);
            }
            else
            {
                segments[i] = segments[i - 1] + velocity * segTime + Physics.gravity * gravityScale * segTime * segTime / 2;
                velocity += Physics.gravity * gravityScale * segTime;
                velocity *= Mathf.Clamp01(1f - drag * segTime);
            }
        }

        return segments.ToList();
    }

    List<Vector3> GetTrajectoryPointsLinear(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] segments = new Vector3[segmentsCount];

        segments[0] = startPosition;

        Vector3 velocity = initialVelocity;

        for (int i = 1; i < segmentsCount; i++)
        {
            float segTime = (velocity.sqrMagnitude != 0) ? segmentLength / velocity.magnitude : 0;
            segments[i] = segments[i - 1] + velocity * segTime;
        }

        return segments.ToList();
    }

    List<Vector3> GetTrajectoryPointsWithGravity(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] segments = new Vector3[segmentsCount];

        segments[0] = startPosition;

        Vector3 velocity = initialVelocity;

        var gravityScale = objectPrefab.GetComponent<Rigidbody2D>().gravityScale;

        for (int i = 1; i < segmentsCount; i++)
        {
            float segTime = (velocity.sqrMagnitude != 0) ? segmentLength / velocity.magnitude : 0;
            segments[i] = segments[i - 1] + velocity * segTime + Physics.gravity * gravityScale * segTime * segTime / 2;
            velocity += Physics.gravity * gravityScale * segTime;
        }

        return segments.ToList();
    }

    List<Vector3> GetTrajectoryPointsReflect(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] segments = new Vector3[segmentsCount];

        segments[0] = startPosition;

        Vector3 velocity = initialVelocity;

        var gravityScale = objectPrefab.GetComponent<Rigidbody2D>().gravityScale;

        for (int i = 1; i < segmentsCount; i++)
        {
            float segTime = (velocity.sqrMagnitude != 0) ? segmentLength / velocity.magnitude : 0;

            RaycastHit2D  hit = Physics2D.CircleCast(segments[i - 1], 0.5f, velocity, segmentLength);
            if (hit.collider != null && (Vector2)segments[i - 1] != hit.centroid)
            {
                var hitSegTime = hit.distance / velocity.magnitude;
                segments[i] = segments[i - 1] + velocity * hitSegTime + Physics.gravity * gravityScale * hitSegTime * hitSegTime / 2;

                velocity += Physics.gravity * gravityScale * hitSegTime;
                velocity = Vector2.Reflect(velocity, hit.normal);
            }
            else
            {
                segments[i] = segments[i - 1] + velocity * segTime + Physics.gravity * gravityScale * segTime * segTime / 2;
                velocity += Physics.gravity * gravityScale * segTime;
            }
        }

        return segments.ToList();
    }

    List<Vector3> GetTrajectoryPointsBounce(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] segments = new Vector3[segmentsCount];

        segments[0] = startPosition;

        Vector3 velocity = initialVelocity;

        var bounciness = objectPrefab.sharedMaterial.bounciness;
        var gravityScale = objectPrefab.GetComponent<Rigidbody2D>().gravityScale;

        for (int i = 1; i < segmentsCount; i++)
        {
            float segTime = (velocity.sqrMagnitude != 0) ? segmentLength / velocity.magnitude : 0;

            RaycastHit2D hit = Physics2D.CircleCast(segments[i - 1], 0.5f, velocity, segmentLength);
            if (hit.collider != null && (Vector2)segments[i - 1] != hit.centroid)
            {
                var hitSegTime = hit.distance / velocity.magnitude;
                segments[i] = segments[i - 1] + velocity * hitSegTime + Physics.gravity * gravityScale * hitSegTime * hitSegTime / 2;

                velocity += Physics.gravity * gravityScale * hitSegTime;

                var normalVelocity = Vector3.Project(velocity, hit.normal);
                var tangentVelocity = velocity - normalVelocity;

                velocity = tangentVelocity - normalVelocity * bounciness;
            }
            else
            {
                segments[i] = segments[i - 1] + velocity * segTime + Physics.gravity * gravityScale * segTime * segTime / 2;
                velocity += Physics.gravity * gravityScale * segTime;
            }
        }

        return segments.ToList();
    }
}
