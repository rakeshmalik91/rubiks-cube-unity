#pragma strict

var speed = 1;
var smoothness = 0.1f;
private var targetRot: Vector2 = Vector2();
private var dragging = false;

private var sliderSensitivity: UnityEngine.UI.Slider = null;

function Start () {
	if(PlayerPrefs.HasKey("sensitivity")) {
		speed = PlayerPrefs.GetFloat("sensitivity");
	}
}

function OnMouseDrag () {
	targetRot = Vector2.Lerp(targetRot, Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), 1 - smoothness);
}

function Update() {
	if(!dragging) {
		targetRot = Vector2.Lerp(targetRot, Vector2(), 1 - smoothness);
	}
	if(!GameData.lockRotate) {
		transform.Rotate(Vector3(-targetRot.y, targetRot.x, 0) * Time.deltaTime * speed);
	}
}

function OnMouseDown () {
	dragging = true;
}

function OnMouseUp () {
	dragging = false;
}

function InitSlider() {
	if(sliderSensitivity == null) {
		sliderSensitivity = GameObject.Find("Slider Sensitivity").GetComponent.<UnityEngine.UI.Slider>();
		if(PlayerPrefs.HasKey("sensitivity")) {
			sliderSensitivity.value = speed;
		}
	}
}

function SetSpeed() {
	speed = sliderSensitivity.value;
	PlayerPrefs.SetFloat("sensitivity", speed);
	PlayerPrefs.Save();
}
