using UnityEngine;
using UnityEngine.SceneManagement;

public class Util : MonoBehaviour {

	public void  ToggleEnableDisable (){
		gameObject.SetActive(!gameObject.activeSelf);
	}

	public void  ReloadLevel (){
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}