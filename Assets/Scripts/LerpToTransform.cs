using UnityEngine;
using System.Collections;

public class LerpToTransform : MonoBehaviour {

	public float lerpSpeed = 0.01f;
	public Transform targetTransform;

	private Quaternion lastRotation;

	void  Start (){
		lastRotation = transform.localRotation;
	}

	void  Update (){
		if(lastRotation != transform.localRotation) {
			enabled = false;
			return;
		}
		transform.localPosition = Vector3.Lerp(transform.localPosition, targetTransform.localPosition, lerpSpeed * Time.deltaTime);
		transform.localRotation = Quaternion.Lerp(transform.localRotation, targetTransform.localRotation, lerpSpeed * Time.deltaTime);
		lastRotation = transform.localRotation;
	}

	public void  Reset (){
		enabled = true;
		lastRotation = transform.localRotation;
	}
}