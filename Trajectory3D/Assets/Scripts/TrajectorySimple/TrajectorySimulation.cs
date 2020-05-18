using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public abstract class TrajectorySimulation : MonoBehaviour
{

    public LineRenderer lineRenderer;
    // TrajectoryPoint and Ball will be instantiated

    public float power = 1;
    public int numOfTrajectoryPoints = 30;

    void Update()
    {
        // when mouse button is pressed, cannon is rotated as per mouse movement and projectile trajectory path is displayed.
        if (Input.GetMouseButton(0) || Input.GetMouseButtonUp(0) || Input.GetMouseButtonDown(0))
        {
            Vector3 vel = GetForce();
            float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
            var points = GetTrajectoryPoints(transform.position, vel / GetObjectMass());
            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.Select(v => new Vector3(v.x, v.y, transform.position.z)).ToArray());
        }
    }

    public abstract float GetObjectMass();

    //---------------------------------------    
    // Following method returns force by calculating distance between given two points
    //---------------------------------------    
    public virtual Vector3 GetForce()
    {
        var fromPos = transform.position;
        var toPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (toPos - fromPos) * power;
    }
    //---------------------------------------    
    // Following method displays projectile trajectory path. It takes two arguments, start position of object(ball) and initial velocity of object(ball).
    //---------------------------------------    
    List<Vector3> GetTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = pVelocity.magnitude;
        float angleXY = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float angleZY = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.z));
        float fTime = 0;

        fTime += 0.1f;

        var points = new List<Vector3>();
        points.Add(transform.position);
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angleXY * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angleXY * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);
            float dz = velocity * fTime * Mathf.Cos(angleZY * Mathf.Deg2Rad);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, pStartPosition.z + dz);
            points.Add(pos);
            //trajectoryPoints[i].renderer.enabled = true;
            //trajectoryPoints[i].transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
        }
        return points;
    }
}
