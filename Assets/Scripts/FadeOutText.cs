using UnityEngine;
using System.Collections;

public class FadeOutText : MonoBehaviour {

	public float fadeSpeed = 0.1f;
	
	// Update is called once per frame
	void Update () {
		UnityEngine.UI.Text text = GetComponent <UnityEngine.UI.Text>();
		text.color = new Color (text.color.r, text.color.g, text.color.b, Mathf.Clamp(text.color.a - Time.deltaTime * fadeSpeed, 0f, 1f));
	}

	public void Show() {
		UnityEngine.UI.Text text = GetComponent <UnityEngine.UI.Text>();
		text.color = new Color (text.color.r, text.color.g, text.color.b, 1f);
	}

	public void Hide() {
		UnityEngine.UI.Text text = GetComponent <UnityEngine.UI.Text>();
		text.color = new Color (text.color.r, text.color.g, text.color.b, 0f);
	}
}
