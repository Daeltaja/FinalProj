using UnityEngine;
using System.Collections;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{	
		if(Input.GetKey (KeyCode.D))
		{
			rigidbody.transform.position += Vector3.right * 6f * Time.deltaTime;
		}
		if(Input.GetKey (KeyCode.A))
		{
			rigidbody.transform.position += -Vector3.right * 6f * Time.deltaTime;
		}
		if(Input.GetKey (KeyCode.W))
		{
			rigidbody.transform.position += Vector3.forward * 6f * Time.deltaTime;
		}
		if(Input.GetKey (KeyCode.S))
		{
			rigidbody.transform.position += -Vector3.forward * 6f * Time.deltaTime;
		}
	}
}
