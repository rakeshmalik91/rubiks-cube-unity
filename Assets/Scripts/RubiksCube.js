#pragma strict

class Side {
	var trigger: GameObject;
	var dirToRotate: Vector3;
	var cubes: GameObject[] = new GameObject[9];
}
public var sides: Side[] = new Side[6];

var rotater: Transform;
private var code = 0;
var queue = Array();

private var sliderAnimationSpeed: UnityEngine.UI.Slider = null;

function Start () {
	if(PlayerPrefs.HasKey("rotateSpeed")) {
		rotateSpeed = PlayerPrefs.GetFloat("rotateSpeed");
	}

	Screen.sleepTimeout = SleepTimeout.NeverSleep;
	GetCubes();
}

private var rotating = false;
private var rotated: float = 0;

var rotateSpeed = 10;

function Update () {
	HandleInput();

	if(code != 0) {
		var index;
		var clockwise: boolean = true;
		if(code < 0) {
			clockwise = false;
			index = -code - 1;
		} else {
			index = code - 1;
		}
		rotating = true;
		for(var c = 0; c < sides[index].cubes.Length; c++) {
			sides[index].cubes[c].transform.SetParent(rotater);
		}
		var deltaTime = Time.deltaTime;
		if(clockwise) {
			rotater.Rotate(sides[index].dirToRotate * rotateSpeed * deltaTime);
		} else {
			rotater.Rotate(-sides[index].dirToRotate * rotateSpeed * deltaTime);
		}
		rotated += rotateSpeed * deltaTime;
		if(rotated >= 90) {
			//rotate back extra rotation
			if(clockwise) {
				rotater.Rotate(-sides[index].dirToRotate * (rotated - 90));
			} else {
				rotater.Rotate(sides[index].dirToRotate * (rotated - 90));
			}
		
			rotated = 0;
			code = 0;
			for(var s = 0; s < sides.Length; s++) {
				for(c = 0; c < sides[s].cubes.Length; c++) {
					sides[s].cubes[c].transform.SetParent(transform);
				}
			}
			rotater.localEulerAngles.x = rotater.localEulerAngles.y = rotater.localEulerAngles.z = 0;
			rotating = false;
			GetCubes();
		}
	} else if(queue.length > 0) {
		code = queue[0];
		queue.RemoveAt(0);
		GameData.moves++;
	}
}

function GetCubes() {
	for(var s = 0; s < sides.Length; s++) {
		var c = 0;
		for(var collider: BoxCollider in transform.GetComponentsInChildren(BoxCollider)) {
			if(!collider.isTrigger && sides[s].trigger.GetComponent(Collider).bounds.Contains(collider.bounds.center)) {
				sides[s].cubes[c] = collider.gameObject;
				c++;
			}
		}
	}
}

function HandleInput() {
	if(Input.GetButton("AntiClockwise")) {
		if(Input.GetButtonUp("Left")) {
			AddInput(-1);
		} else if(Input.GetButtonUp("Front")) {
			AddInput(-2);
		} else if(Input.GetButtonUp("Back")) {
			AddInput(-3);
		} else if(Input.GetButtonUp("Right")) {
			AddInput(-4);
		} else if(Input.GetButtonUp("Up")) {
			AddInput(-5);
		} else if(Input.GetButtonUp("Down")) {
			AddInput(-6);
		} else if(Input.GetButtonUp("MiddleFB")) {
			AddInput(-7); //middle fb
		} else if(Input.GetButtonUp("MiddleLR")) {
			AddInput(-8); //middle lr
		} else if(Input.GetButtonUp("MiddleUD")) {
			AddInput(-9); //middle ud
		} 
	} else {
		if(Input.GetButtonUp("Left")) {
			AddInput(1); //left
		} else if(Input.GetButtonUp("Front")) {
			AddInput(2); //front
		} else if(Input.GetButtonUp("Back")) {
			AddInput(3); //back
		} else if(Input.GetButtonUp("Right")) {
			AddInput(4); //right
		} else if(Input.GetButtonUp("Up")) {
			AddInput(5); //up
		} else if(Input.GetButtonUp("Down")) {
			AddInput(6); //down
		} else if(Input.GetButtonUp("MiddleFB")) {
			AddInput(7); //middle fb
		} else if(Input.GetButtonUp("MiddleLR")) {
			AddInput(8); //middle lr
		} else if(Input.GetButtonUp("MiddleUD")) {
			AddInput(9); //middle ud
		}
	}
}

private var dragStart: Vector3;
private var dragEnd: Vector3;

function OnMouseDown() {
	var hit : RaycastHit;
	var ray: Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	if(Physics.Raycast(ray, hit)) {
		dragStart = hit.point;
	}
}

function OnMouseUp() {
	var hit : RaycastHit;
	var ray: Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
	if(Physics.Raycast(ray, hit)) {
		dragEnd = hit.point;
	}
	if(Vector3.Distance(dragStart, dragEnd) < 0.01) {
		return;
	}
	//Debug.Log(dragStart + " -> " + dragEnd);
	if(dragStart.y == dragEnd.y) { //up & down
		if(dragStart.y > 0) { //up
			if(dragStart.z <= -0.5 && dragEnd.z <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) > Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.x <= dragEnd.x) {
					AddInput(1);
				} else {
					AddInput(-1);
				}
			} else if(dragStart.x >= 0.5 && dragEnd.x >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.z <= dragEnd.z) {
					AddInput(2);
				} else {
					AddInput(-2);
				}
			} else if(dragStart.x <= -0.5 && dragEnd.x <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(3);
				} else {
					AddInput(-3);
				}
			} else if(dragStart.z >= 0.5 && dragEnd.z >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.x >= dragEnd.x) {
					AddInput(4);
				} else {
					AddInput(-4);
				}
			} else if(dragStart.x >= -0.5 && dragStart.x <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(-7);
				} else {
					AddInput(7);
				}
			} else if(dragStart.z >= -0.5 && dragStart.z <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.x >= dragEnd.x) {
					AddInput(-8);
				} else {
					AddInput(8);
				}
			}
		} else { //down
			if(dragStart.z <= -0.5 && dragEnd.z <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.x >= dragEnd.x) {
					AddInput(1);
				} else {
					AddInput(-1);
				}
			} else if(dragStart.x >= 0.5 && dragEnd.x >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(2);
				} else {
					AddInput(-2);
				}
			} else if(dragStart.x <= -0.5 && dragEnd.x <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.z <= dragEnd.z) {
					AddInput(3);
				} else {
					AddInput(-3);
				}
			} else if(dragStart.z >= 0.5 && dragEnd.z >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.x <= dragEnd.x) {
					AddInput(4);
				} else {
					AddInput(-4);
				}
			} else if(dragStart.x >= -0.5 && dragStart.x <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.z <= dragEnd.z) {
					AddInput(-7);
				} else {
					AddInput(7);
				}
			} else if(dragStart.z >= -0.5 && dragStart.z <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
				if(dragStart.x <= dragEnd.x) {
					AddInput(-8);
				} else {
					AddInput(8);
				}
			}
		}
	} else if(dragStart.x == dragEnd.x) { //front and back
		if(dragStart.x > 0) { //front
			if(dragStart.z <= -0.5 && dragEnd.z <= -0.5 && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y >= dragEnd.y) {
					AddInput(1);
				} else {
					AddInput(-1);
				}
			} else if(dragStart.z >= 0.5 && dragEnd.z >= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y <= dragEnd.y) {
					AddInput(4);
				} else {
					AddInput(-4);
				}
			} else if(dragStart.y >= 0.5 && dragEnd.y >= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(5);
				} else {
					AddInput(-5);
				}
			} else if(dragStart.y <= -0.5 && dragEnd.y <= -0.5 && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.z <= dragEnd.z) {
					AddInput(6);
				} else {
					AddInput(-6);
				}
			} else if(dragStart.z >= -0.5 && dragStart.z <= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y >= dragEnd.y) {
					AddInput(8);
				} else {
					AddInput(-8);
				}
			} else if(dragStart.y >= -0.5 && dragStart.y <= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(9);
				} else {
					AddInput(-9);
				}
			}
		} else { //back
			if(dragStart.z <= -0.5 && dragEnd.z <= -0.5 && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y <= dragEnd.y) {
					AddInput(1);
				} else {
					AddInput(-1);
				}
			} else if(dragStart.y >= 0.5 && dragEnd.y >= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.z <= dragEnd.z) {
					AddInput(5);
				} else {
					AddInput(-5);
				}
			} else if(dragStart.y <= -0.5 && dragEnd.y <= -0.5 && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(6);
				} else {
					AddInput(-6);
				}
			} else if(dragStart.z >= 0.5 && dragEnd.z >= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y >= dragEnd.y) {
					AddInput(4);
				} else {
					AddInput(-4);
				}
			} else if(dragStart.z >= -0.5 && dragStart.z <= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y <= dragEnd.y) {
					AddInput(8);
				} else {
					AddInput(-8);
				}
			} else if(dragStart.y >= -0.5 && dragStart.y <= 0.5 && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.z >= dragEnd.z) {
					AddInput(-9);
				} else {
					AddInput(9);
				}
			}
		}
	} else if(dragStart.z == dragEnd.z) { //left & right
		if(dragStart.z > 0) { //right
			if(dragStart.x <= -0.5 && dragEnd.x <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y <= dragEnd.y) {
					AddInput(3);
				} else {
					AddInput(-3);
				}
			} else if(dragStart.x >= 0.5 && dragEnd.x >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y >= dragEnd.y) {
					AddInput(2);
				} else {
					AddInput(-2);
				}
			} else if(dragStart.y >= 0.5 && dragEnd.y >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.x <= dragEnd.x) {
					AddInput(5);
				} else {
					AddInput(-5);
				}
			} else if(dragStart.y <= -0.5 && dragEnd.y <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.x >= dragEnd.x) {
					AddInput(6);
				} else {
					AddInput(-6);
				}
			} else if(dragStart.x >= -0.5 && dragStart.x <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y >= dragEnd.y) {
					AddInput(7);
				} else {
					AddInput(-7);
				}
			} else if(dragStart.y >= -0.5 && dragStart.y <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.x >= dragEnd.x) {
					AddInput(-9);
				} else {
					AddInput(9);
				}
			}
		} else { //left
			if(dragStart.x <= -0.5 && dragEnd.x <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y >= dragEnd.y) {
					AddInput(3);
				} else {
					AddInput(-3);
				}
			} else if(dragStart.x >= 0.5 && dragEnd.x >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y <= dragEnd.y) {
					AddInput(2);
				} else {
					AddInput(-2);
				}
			} else if(dragStart.y >= 0.5 && dragEnd.y >= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.x >= dragEnd.x) {
					AddInput(5);
				} else {
					AddInput(-5);
				}
			} else if(dragStart.y <= -0.5 && dragEnd.y <= -0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.x <= dragEnd.x) {
					AddInput(6);
				} else {
					AddInput(-6);
				}
			} else if(dragStart.x >= -0.5 && dragStart.x <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.y <= dragEnd.y) {
					AddInput(7);
				} else {
					AddInput(-7);
				}
			} else if(dragStart.y >= -0.5 && dragStart.y <= 0.5 && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
				if(dragStart.x <= dragEnd.x) {
					AddInput(-9);
				} else {
					AddInput(9);
				}
			}
		}
	}
}

function AddInput(input: int) {
	if(!GameData.lockInput) {
		queue.Add(input);
	}
}

function Randomize() {
	if(queue.length > 0)
		return;
	var moves: int = Random.Range(10, 30);
	for(var i = 0; i < moves; i++) {
		var code: int = (Random.Range(0f, 1f) > 0.5 ? 1 : -1) * Random.Range(1, 9);
		queue.Add(code);
	}
	GameData.moves = -moves;
}

function InitSlider() {
	if(sliderAnimationSpeed == null) {
		sliderAnimationSpeed = GameObject.Find("Slider AnimationSpeed").GetComponent.<UnityEngine.UI.Slider>();
		if(PlayerPrefs.HasKey("rotateSpeed")) {
			sliderAnimationSpeed.value = rotateSpeed;
		}
	}
}

function SetRotateSpeed() {
	rotateSpeed = sliderAnimationSpeed.value;
	PlayerPrefs.SetFloat("rotateSpeed", rotateSpeed);
	PlayerPrefs.Save();
}
