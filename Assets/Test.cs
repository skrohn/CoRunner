using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		CoRunner.Start(coBasic());
//		CoRunner.Start(coWWW());
		CoRunner.Start(coIEnumerator());
	}

	IEnumerator coBasic ()
	{
		Debug.Log(Time.frameCount);
		yield return null;
		Debug.Log(Time.frameCount);
	}

	IEnumerator coWWW ()
	{
		Debug.Log(Time.frameCount);
		yield return new WWW("http://www.google.com");
		Debug.Log(Time.frameCount);
	}

	IEnumerator coIEnumerator ()
	{
		Debug.Log(Time.frameCount);
		yield return CoRunner.Start(coBasic());
		Debug.Log(Time.frameCount);
	}
}
