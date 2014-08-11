using UnityEngine;
using System.Collections;

public class CameraCode : MonoBehaviour {

	Transform p1Transform, p2Transform, myTransform, childTransform;
	Vector3 distanceVec, followVec;
	public float height, distance, smoothTime;
	public float shakeAmount, shakeDuration;
	public iTween.EaseType easeType;

	void Awake ()
	{
		p1Transform = GameObject.FindWithTag("Warrior").transform;
		p2Transform = GameObject.FindWithTag("Ranger").transform;
		myTransform = this.transform;
		childTransform = this.transform.GetChild(0).transform;
	}

	void Update () 
	{
		followVec = new Vector3(distanceVector().x, height, distanceVector().z-distance);
		myTransform.position = Vector3.Lerp (myTransform.position, followVec, smoothTime * Time.deltaTime);
		//childTransform.position = myTransform.position;
	}

	Vector3 distanceVector() //returns the distance vector
	{
		distanceVec = (p1Transform.position + p2Transform.position) / 2;
		return distanceVec;
	}

	public void Shake()
	{	
		iTween.ShakePosition(childTransform.gameObject, iTween.Hash("x", shakeAmount, "time", shakeDuration, "easetype", easeType, "islocal", true));
	}

	public void ShakeSlam()
	{	
		iTween.ShakePosition(childTransform.gameObject, iTween.Hash("y", shakeAmount, "time", shakeDuration, "easetype", easeType, "islocal", true));
	}
}
