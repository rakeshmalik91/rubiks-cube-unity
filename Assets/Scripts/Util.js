#pragma strict

function ToggleEnableDisable () {
	gameObject.SetActive(!gameObject.activeSelf);
}

function ReloadLevel() {
	Application.LoadLevel(Application.loadedLevel);
}