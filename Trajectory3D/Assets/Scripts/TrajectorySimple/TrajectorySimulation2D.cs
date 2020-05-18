using UnityEngine;

public class TrajectorySimulation2D : TrajectorySimulation
{
    public Rigidbody2D ball;
    public override float GetObjectMass()
    {
        return ball.mass;
    }
}
