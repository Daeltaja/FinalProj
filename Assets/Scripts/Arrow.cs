using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	Animator _anim;

	void Awake()
	{
		_anim.GetComponent<Animator>();
	}
	
	void OnCollisionEnter(Collision other)
	{
		rigidbody.isKinematic = true;
		//_anim.SetTrigger("Stuck");
		GetComponent<BoxCollider>().isTrigger = true;
		gameObject.layer = LayerMask.NameToLayer("Default");
	}

	void OnTriggerEnter (Collider other)
	{
		if(other.tag == "Ranger")
		{
			Destroy(gameObject);
		}
	}
}
