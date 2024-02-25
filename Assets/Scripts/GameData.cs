using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {

	public static int moves = 0;

	public void  ResetMoves (){
		moves = 0;
	}

	public static bool  lockRotate = false;
	public GameObject cameraRotater;

	public void  ToggleRotateLock (){
		lockRotate = !lockRotate;
		if(lockRotate) {
			cameraRotater.GetComponent<LerpToTransform>().Reset();
		}
	}

	public void  ResetRotateLock (){
		lockRotate = false;
	}


	public static bool  lockInput = false;

	public void  ToggleInputLock (){
		lockInput = !lockInput;
	}

	public void  ResetInputLock (){
		lockInput = false;
	}
}