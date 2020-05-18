using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
	private void OnCollisionExit2D(Collision2D collision)
	{
		Debug.Log($"Real exit velocity: {GetComponent<Rigidbody2D>().velocity}");
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		Debug.Log($"Real enter velocity: {GetComponent<Rigidbody2D>().velocity}");
	}
}
