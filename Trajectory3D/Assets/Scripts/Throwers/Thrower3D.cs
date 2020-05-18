using UnityEngine;
using System.Collections;

public class Thrower3D : MonoBehaviour
{
    public GameObject ballPrefab;

    public Vector3 force;


    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var ballObject = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            Vector3 vel = GetForce();
            ballObject.GetComponent<Rigidbody>().AddForce(vel, ForceMode.Impulse);
        }
    }

    public Vector3 GetForce()
    {
        return force;
    }
}
