using UnityEngine;
using System.Collections;

public class MoveCounter : MonoBehaviour {

	private UnityEngine.UI.Text text;

	void  Start (){
		text = GetComponent<UnityEngine.UI.Text>();
	}

	void  Update (){
		text.text = GameData.moves.ToString("000000");
	}
}