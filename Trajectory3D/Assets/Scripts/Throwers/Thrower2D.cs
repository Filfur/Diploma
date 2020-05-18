using UnityEngine;
using System.Collections;

public class Thrower2D : MonoBehaviour
{
    public GameObject ballPrefab;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            var ballObject = Instantiate(ballPrefab, transform.position, Quaternion.identity);
            Vector3 vel = GetForceFrom(transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition));
            ballObject.GetComponent<Rigidbody2D>().AddForce(vel, ForceMode2D.Impulse);
        }
    }

    private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
    {
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * 1;
    }
}
