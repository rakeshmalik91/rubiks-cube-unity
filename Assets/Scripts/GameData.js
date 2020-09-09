#pragma strict

public static var moves: int = 0;

public function ResetMoves() {
	moves = 0;
}

public static var lockRotate: boolean = false;
public var cameraRotater: GameObject;

public function ToggleRotateLock() {
	lockRotate = !lockRotate;
	if(lockRotate) {
		cameraRotater.GetComponent.<LerpToTransform>().Reset();
	}
}

public function ResetRotateLock() {
	lockRotate = false;
}


public static var lockInput: boolean = false;

public function ToggleInputLock() {
	lockInput = !lockInput;
}

public function ResetInputLock() {
	lockInput = false;
}