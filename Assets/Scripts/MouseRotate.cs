using UnityEngine;
using System.Collections;

public class MouseRotate : MonoBehaviour {


	public float speed = 1.0f;
	public float smoothness= 0.1f;
	private Vector2 targetRot = new Vector2();
	private bool dragging= false;

	private UnityEngine.UI.Slider sliderSensitivity = null;

	void  Start (){
		if(PlayerPrefs.HasKey("sensitivity")) {
			speed = PlayerPrefs.GetFloat("sensitivity");
		}
	}

	void  OnMouseDrag (){
		targetRot = Vector2.Lerp(targetRot, new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")), 1 - smoothness);
	}

	void  Update (){
		if(!dragging) {
			targetRot = Vector2.Lerp(targetRot, new Vector2(), 1 - smoothness);
		}
		if (!GameData.lockRotate)
		{
			transform.Rotate(new Vector3(-targetRot.y, targetRot.x, 0) * Time.deltaTime * speed);
		}
	}

	void  OnMouseDown (){
		dragging = true;
	}

	void  OnMouseUp (){
		dragging = false;
	}

	void  InitSlider (){
		if(sliderSensitivity == null) {
			sliderSensitivity = GameObject.Find("Slider Sensitivity").GetComponent<UnityEngine.UI.Slider>();
			if(PlayerPrefs.HasKey("sensitivity")) {
				sliderSensitivity.value = speed;
			}
		}
	}

	void  SetSpeed (){
		speed = sliderSensitivity.value;
		PlayerPrefs.SetFloat("sensitivity", speed);
		PlayerPrefs.Save();
	}

}