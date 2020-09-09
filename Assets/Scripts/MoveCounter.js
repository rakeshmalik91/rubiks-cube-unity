#pragma strict

private var text: UI.Text;

function Start () {
	text = GetComponent.<UI.Text>();
}

function Update () {
	text.text = GameData.moves.ToString("000000");
}