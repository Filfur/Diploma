using UnityEngine;
public class TrajectorySimulation3D : TrajectorySimulation
{
    public Rigidbody ball;
    public override float GetObjectMass()
    {
        return ball.mass;
    }

    public override Vector3 GetForce()
    {
        return new Vector3(45, 45);
    }
}