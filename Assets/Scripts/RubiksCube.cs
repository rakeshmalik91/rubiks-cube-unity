using UnityEngine;
using System;
using System.Collections.Generic; 

public class RubiksCube : MonoBehaviour {

	public class Side {
		public GameObject trigger;
		public Vector3 dirToRotate;
		public GameObject[] cubes = new GameObject[9];
		public Side(GameObject trigger, Vector3 dirToRotate, GameObject[] cubes) {
			this.trigger = trigger;
			this.dirToRotate = dirToRotate;
			this.cubes = cubes;
		}
	}

	public Side[] sides = new Side[6];

	public Transform rotater;
	public int code = 0;
	public List<int> queue = new List<int>();

	private UnityEngine.UI.Slider sliderAnimationSpeed = null;

	void  Start (){
		if(PlayerPrefs.HasKey("rotateSpeed")) {
			rotateSpeed = PlayerPrefs.GetFloat("rotateSpeed");
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		GetCubes();
	}

	void Awake() {
		sides = new Side[9] {
			new Side(GameObject.Find("left"), new Vector3(0, 0, -1), new GameObject[9]),
			new Side(GameObject.Find("front"), new Vector3(1, 0, 0), new GameObject[9]),
			new Side(GameObject.Find("back"), new Vector3(-1, 0, 0), new GameObject[9]),
			new Side(GameObject.Find("right"), new Vector3(0, 0, 1), new GameObject[9]),
			new Side(GameObject.Find("up"), new Vector3(0, 1, 0), new GameObject[9]),
			new Side(GameObject.Find("down"), new Vector3(0, -1, 0), new GameObject[9]),
			new Side(GameObject.Find("middleFB"), new Vector3(1, 0, 0), new GameObject[9]),
			new Side(GameObject.Find("middleLR"), new Vector3(0, 0, -1), new GameObject[9]),
			new Side(GameObject.Find("middleTD"), new Vector3(0, 1, 0), new GameObject[9])
		};
	}

	private bool rotating = false;
	private float rotated = 0;

	public float rotateSpeed = 360f;

	void  Update (){
		HandleInput();

		if(code != 0) {
			int index;
			bool  clockwise = true;
			if(code < 0) {
				clockwise = false;
				index = -code - 1;
			} else {
				index = code - 1;
			}
			rotating = true;
			for(int c= 0; c < sides[index].cubes.Length; c++) {
				sides[index].cubes[c].transform.SetParent(rotater);
			}
			float deltaTime = Time.deltaTime;
			Vector3 eulers = sides[index].dirToRotate * rotateSpeed * deltaTime;
			//Debug.Log("eulers: " + eulers);
			rotater.Rotate(clockwise ? eulers : -eulers);
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
				for(int s= 0; s < sides.Length; s++) {
					for(int c = 0; c < sides[s].cubes.Length; c++) {
						sides[s].cubes[c].transform.SetParent(transform);
					}
				}
				rotater.SetLocalPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
				rotating = false;
				GetCubes();
			}
		} else if(queue.Count > 0) {
			code = queue[0];
			queue.RemoveAt(0);
			GameData.moves++;
		}
	}

	void  GetCubes (){
		for(int s= 0; s < sides.Length; s++) {
			int c= 0;
			foreach(BoxCollider collider in transform.GetComponentsInChildren<BoxCollider>()) {
				if(!collider.isTrigger && sides[s].trigger.GetComponent<Collider>().bounds.Contains(collider.bounds.center)) {
					sides[s].cubes[c] = collider.gameObject;
					c++;
				}
			}
		}
	}

	void  HandleInput (){
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

	private Vector3 dragStart;
	private Vector3 dragEnd;

	void  OnMouseDown (){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray.origin, ray.direction, out hit)) {
			dragStart = hit.point;
		}
	}

	void  OnMouseUp (){
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray.origin, ray.direction, out hit)) {
			dragEnd = hit.point;
		}
		if(Vector3.Distance(dragStart, dragEnd) < 0.01f) {
			return;
		}
		//Debug.Log(dragStart + " -> " + dragEnd);
		if(dragStart.y == dragEnd.y) { //up & down
			if(dragStart.y > 0) { //up
				if(dragStart.z <= -0.5f && dragEnd.z <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) > Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.x <= dragEnd.x) {
						AddInput(1);
					} else {
						AddInput(-1);
					}
				} else if(dragStart.x >= 0.5f && dragEnd.x >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.z <= dragEnd.z) {
						AddInput(2);
					} else {
						AddInput(-2);
					}
				} else if(dragStart.x <= -0.5f && dragEnd.x <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(3);
					} else {
						AddInput(-3);
					}
				} else if(dragStart.z >= 0.5f && dragEnd.z >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.x >= dragEnd.x) {
						AddInput(4);
					} else {
						AddInput(-4);
					}
				} else if(dragStart.x >= -0.5f && dragStart.x <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(-7);
					} else {
						AddInput(7);
					}
				} else if(dragStart.z >= -0.5f && dragStart.z <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.x >= dragEnd.x) {
						AddInput(-8);
					} else {
						AddInput(8);
					}
				}
			} else { //down
				if(dragStart.z <= -0.5f && dragEnd.z <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.x >= dragEnd.x) {
						AddInput(1);
					} else {
						AddInput(-1);
					}
				} else if(dragStart.x >= 0.5f && dragEnd.x >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(2);
					} else {
						AddInput(-2);
					}
				} else if(dragStart.x <= -0.5f && dragEnd.x <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.z <= dragEnd.z) {
						AddInput(3);
					} else {
						AddInput(-3);
					}
				} else if(dragStart.z >= 0.5f && dragEnd.z >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.x <= dragEnd.x) {
						AddInput(4);
					} else {
						AddInput(-4);
					}
				} else if(dragStart.x >= -0.5f && dragStart.x <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.z <= dragEnd.z) {
						AddInput(-7);
					} else {
						AddInput(7);
					}
				} else if(dragStart.z >= -0.5f && dragStart.z <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.z - dragEnd.z)) {
					if(dragStart.x <= dragEnd.x) {
						AddInput(-8);
					} else {
						AddInput(8);
					}
				}
			}
		} else if(dragStart.x == dragEnd.x) { //front and back
			if(dragStart.x > 0) { //front
				if(dragStart.z <= -0.5f && dragEnd.z <= -0.5f && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y >= dragEnd.y) {
						AddInput(1);
					} else {
						AddInput(-1);
					}
				} else if(dragStart.z >= 0.5f && dragEnd.z >= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y <= dragEnd.y) {
						AddInput(4);
					} else {
						AddInput(-4);
					}
				} else if(dragStart.y >= 0.5f && dragEnd.y >= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(5);
					} else {
						AddInput(-5);
					}
				} else if(dragStart.y <= -0.5f && dragEnd.y <= -0.5f && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.z <= dragEnd.z) {
						AddInput(6);
					} else {
						AddInput(-6);
					}
				} else if(dragStart.z >= -0.5f && dragStart.z <= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y >= dragEnd.y) {
						AddInput(8);
					} else {
						AddInput(-8);
					}
				} else if(dragStart.y >= -0.5f && dragStart.y <= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(9);
					} else {
						AddInput(-9);
					}
				}
			} else { //back
				if(dragStart.z <= -0.5f && dragEnd.z <= -0.5f && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y <= dragEnd.y) {
						AddInput(1);
					} else {
						AddInput(-1);
					}
				} else if(dragStart.y >= 0.5f && dragEnd.y >= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.z <= dragEnd.z) {
						AddInput(5);
					} else {
						AddInput(-5);
					}
				} else if(dragStart.y <= -0.5f && dragEnd.y <= -0.5f && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(6);
					} else {
						AddInput(-6);
					}
				} else if(dragStart.z >= 0.5f && dragEnd.z >= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y >= dragEnd.y) {
						AddInput(4);
					} else {
						AddInput(-4);
					}
				} else if(dragStart.z >= -0.5f && dragStart.z <= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y <= dragEnd.y) {
						AddInput(8);
					} else {
						AddInput(-8);
					}
				} else if(dragStart.y >= -0.5f && dragStart.y <= 0.5f && Mathf.Abs(dragStart.z - dragEnd.z) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.z >= dragEnd.z) {
						AddInput(-9);
					} else {
						AddInput(9);
					}
				}
			}
		} else if(dragStart.z == dragEnd.z) { //left & right
			if(dragStart.z > 0) { //right
				if(dragStart.x <= -0.5f && dragEnd.x <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y <= dragEnd.y) {
						AddInput(3);
					} else {
						AddInput(-3);
					}
				} else if(dragStart.x >= 0.5f && dragEnd.x >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y >= dragEnd.y) {
						AddInput(2);
					} else {
						AddInput(-2);
					}
				} else if(dragStart.y >= 0.5f && dragEnd.y >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.x <= dragEnd.x) {
						AddInput(5);
					} else {
						AddInput(-5);
					}
				} else if(dragStart.y <= -0.5f && dragEnd.y <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.x >= dragEnd.x) {
						AddInput(6);
					} else {
						AddInput(-6);
					}
				} else if(dragStart.x >= -0.5f && dragStart.x <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y >= dragEnd.y) {
						AddInput(7);
					} else {
						AddInput(-7);
					}
				} else if(dragStart.y >= -0.5f && dragStart.y <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.x >= dragEnd.x) {
						AddInput(-9);
					} else {
						AddInput(9);
					}
				}
			} else { //left
				if(dragStart.x <= -0.5f && dragEnd.x <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y >= dragEnd.y) {
						AddInput(3);
					} else {
						AddInput(-3);
					}
				} else if(dragStart.x >= 0.5f && dragEnd.x >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y <= dragEnd.y) {
						AddInput(2);
					} else {
						AddInput(-2);
					}
				} else if(dragStart.y >= 0.5f && dragEnd.y >= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.x >= dragEnd.x) {
						AddInput(5);
					} else {
						AddInput(-5);
					}
				} else if(dragStart.y <= -0.5f && dragEnd.y <= -0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.x <= dragEnd.x) {
						AddInput(6);
					} else {
						AddInput(-6);
					}
				} else if(dragStart.x >= -0.5f && dragStart.x <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) <= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.y <= dragEnd.y) {
						AddInput(7);
					} else {
						AddInput(-7);
					}
				} else if(dragStart.y >= -0.5f && dragStart.y <= 0.5f && Mathf.Abs(dragStart.x - dragEnd.x) >= Mathf.Abs(dragStart.y - dragEnd.y)) {
					if(dragStart.x <= dragEnd.x) {
						AddInput(-9);
					} else {
						AddInput(9);
					}
				}
			}
		}
	}

	void  AddInput ( int input  ){
		if(!GameData.lockInput) {
			queue.Add(input);
		}
	}

	void  Randomize (){
		if(queue.Count > 0)
			return;
		int moves = UnityEngine.Random.Range(10, 30);
		for(int i= 0; i < moves; i++) {
			int code = (UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1) * UnityEngine.Random.Range(1, 9);
			queue.Add(code);
		}
		GameData.moves = -moves;
	}

	void  InitSlider (){
		if(sliderAnimationSpeed == null) {
			sliderAnimationSpeed = GameObject.Find("Slider AnimationSpeed").GetComponent<UnityEngine.UI.Slider>();
			if(PlayerPrefs.HasKey("rotateSpeed")) {
				sliderAnimationSpeed.value = rotateSpeed;
			}
		}
	}

	void  SetRotateSpeed (){
		rotateSpeed = sliderAnimationSpeed.value;
		PlayerPrefs.SetFloat("rotateSpeed", rotateSpeed);
		PlayerPrefs.Save();
	}

}