using System.Collections;
using UnityEngine;

public class Test : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		CoRunner.Start(coBasic());
//		CoRunner.Start(coWWW());
//		CoRunner.Start(coIEnumerator());
//		CoRunner.Start(coRunImmediate());
//		CoRunner.Start(coMultiYield());
	}

	IEnumerator coBasic ()
	{
		Debug.Log(Time.frameCount);
		yield return null;
		Debug.Log("coBasic finished " + Time.frameCount);
	}

	IEnumerator coWWW ()
	{
		Debug.Log("start www " + Time.frameCount);
		yield return new WWW("http://www.google.com");
		Debug.Log("coWWW finished " + Time.frameCount);
	}

	IEnumerator coIEnumerator ()
	{
		Debug.Log(Time.frameCount);
		yield return CoRunner.Start(coBasic());
		Debug.Log("coIEnumerator finished " + Time.frameCount);
	}

	IEnumerator coRunImmediate ()
	{
		Debug.Log(Time.frameCount);
		yield return CoRunner.Start(coImmediate());
		Debug.Log("coRunImmediate finished " + Time.frameCount);
	}

	IEnumerator coImmediate ()
	{
		if (Time.deltaTime < 0) {
			yield return null;
		}
		Debug.Log("coImmediate finished " + Time.frameCount);
	}

	IEnumerator coYieldOn (IEnumerator enumerator)
	{
		yield return enumerator;
		Debug.Log("finished YieldOn " + Time.frameCount);
	}
	IEnumerator coMultiYield ()
	{
		var routine = CoRunner.Start(coWWW());
		CoRunner.Start(coYieldOn(routine));
		yield return CoRunner.Start(coYieldOn(routine));
		Debug.Log("finished MultiYield " + Time.frameCount);
	}
}
