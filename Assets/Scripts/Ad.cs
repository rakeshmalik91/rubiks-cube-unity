using UnityEngine;
using System.Collections;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class Ad : MonoBehaviour {

	public FadeOutText error;
	public FadeOutText thanks;

	// Use this for initialization
	void Start () {
#if UNITY_ADS
		Debug.Log("Unity Ads is available.");
#endif
	}

	// Update is called once per frame
	void Update () {
	
	}

	public void ShowAd() {
#if UNITY_ADS
		if (Advertisement.IsReady ("rewardedVideo")) {
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show ("rewardedVideo", options);
		} else {
			error.Show ();
			thanks.Hide ();
		}
#endif
	}

#if UNITY_ADS
	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			thanks.Show ();
			error.Hide ();
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}
#endif
}
