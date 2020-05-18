using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//[ExecuteInEditMode]
public class TrajectorySimulationAdvanced3D : MonoBehaviour
{
    public LineRenderer lineRenderer;

    public int segmentsCount = 20;

    public float segmentLength = 1;

    public GameObject objectPrefab;


    private void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
        {
            var points = GetTrajectoryPoints(transform.position, GetForce() / GetObjectMass());
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }

    //private Vector3 GetForce()
    //{
    //    var fromPos = transform.position;
    //    var toPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    return (toPos - fromPos) * 1;
    //}

    private Vector3 GetForce()
    {
        return gameObject.GetComponent<Thrower3D>().force;
    }

    public float GetObjectMass()
    {
        return objectPrefab.GetComponent<Rigidbody>().mass;
    }

    List<Vector3> GetTrajectoryPoints(Vector3 startPosition, Vector3 initialVelocity)
    {
        Vector3[] segments = new Vector3[segmentsCount];

        segments[0] = startPosition;

        Vector3 velocity = initialVelocity;
        var collider = objectPrefab.GetComponent<Collider>();
        var rigidbody = objectPrefab.GetComponent<Rigidbody>();

        var drag = rigidbody.drag;
        var gravityScale = rigidbody.useGravity ? 1 : 0;
        var bounciness = collider.material.bounciness;
        var bounceCombine = collider.material.bounceCombine;


        for (int i = 1; i < segmentsCount; i++)
        {
            bool hasHit = Physics.SphereCast(segments[i - 1], 0.5f, velocity, out RaycastHit hit, segmentLength);
            if (hasHit)
            {
                var hitSegTime = hit.distance / velocity.magnitude;
                segments[i] = segments[i - 1] + velocity * hitSegTime + Physics.gravity * gravityScale * hitSegTime * hitSegTime / 2;

                velocity += Physics.gravity * gravityScale * hitSegTime;

                var normalVelocity = Vector3.Project(velocity, hit.normal);
                var tangentVelocity = velocity - normalVelocity;

                var otherBounciness = hit.collider.material.bounciness;
                var otherBounceCombine = hit.collider.material.bounceCombine;

                var hitBounceCombine = (PhysicMaterialCombine)Mathf.Max((int)bounceCombine, (int)otherBounceCombine);

                var hitBounciness = hitBounceCombine == PhysicMaterialCombine.Average ? (bounciness + otherBounciness) / 2
                    : hitBounceCombine == PhysicMaterialCombine.Maximum ? Mathf.Max(bounciness, otherBounciness)
                    : hitBounceCombine == PhysicMaterialCombine.Minimum ? Mathf.Min(bounciness, otherBounciness)
                    : bounciness * otherBounciness;

                velocity = tangentVelocity - normalVelocity * hitBounciness;
                velocity *= Mathf.Clamp01(1f - drag * hitSegTime);
            }
            else
            {
                float segTime = (velocity.sqrMagnitude != 0) ? segmentLength / velocity.magnitude : 0;
                segments[i] = segments[i - 1] + velocity * segTime + Physics.gravity * gravityScale * segTime * segTime / 2;
                velocity += Physics.gravity * gravityScale * segTime;
                velocity *= Mathf.Clamp01(1f - drag * segTime);
            }
        }

        return segments.ToList();
    }
}
