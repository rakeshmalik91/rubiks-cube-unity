#pragma strict

var lerpSpeed: float = 0.01;
var targetTransform: Transform;

private var lastRotation: Quaternion;

function Start() {
	lastRotation = transform.localRotation;
}

function Update () {
	if(lastRotation != transform.localRotation) {
		enabled = false;
		return;
	}
	transform.localPosition = Vector3.Lerp(transform.localPosition, targetTransform.localPosition, lerpSpeed * Time.deltaTime);
	transform.localRotation = Quaternion.Lerp(transform.localRotation, targetTransform.localRotation, lerpSpeed * Time.deltaTime);
	lastRotation = transform.localRotation;
}

function Reset() {
	enabled = true;
	lastRotation = transform.localRotation;
}