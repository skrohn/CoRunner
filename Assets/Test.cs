using System.Collections;
using UnityEditor;
using UnityEngine;

public class Test : MonoBehaviour {

	Tests tests = new Tests();

	void Start ()
	{
//		CoRunner.Start(coBasic());
		CoRunner.Start(tests.coWWW());
//		CoRunner.Start(coIEnumerator());
//		CoRunner.Start(coRunImmediate());
//		CoRunner.Start(coMultiYield());
//		CoRunner.Start(coImmediateMultiYield());
	}
}

public class Tests
{
	public IEnumerator coImmediateMultiYield ()
	{
		var routine = CoRunner.Start(coBasic());
		var r2 = CoRunner.Start(coYieldOn(routine));
		Debug.Log("start " + CoRunner.FrameCount);
		yield return r2;
		yield return CoRunner.Start(coImmediate());
		yield return CoRunner.Start(coImmediate());
		Debug.Log("coImmediateMultiYield finished " + CoRunner.FrameCount);
	}

	public IEnumerator coBasic ()
	{
		Debug.Log(CoRunner.FrameCount);
		yield return null;
		Debug.Log("coBasic finished " + CoRunner.FrameCount);
	}

	public IEnumerator coWWW ()
	{
		Debug.Log("start www " + CoRunner.FrameCount);
		var www = new WWW("http://www.google.com");
		yield return www;
		Debug.Log(www.text);
		Debug.Log("coWWW finished " + CoRunner.FrameCount);
	}

	public IEnumerator coIEnumerator ()
	{
		Debug.Log(CoRunner.FrameCount);
		yield return CoRunner.Start(coBasic());
		Debug.Log("coIEnumerator finished " + CoRunner.FrameCount);
	}

	public IEnumerator coRunImmediate ()
	{
		Debug.Log(CoRunner.FrameCount);
		yield return CoRunner.Start(coImmediate());
		Debug.Log("coRunImmediate finished " + CoRunner.FrameCount);
	}

	public IEnumerator coImmediate ()
	{
		Debug.Log("start coImmediate " + CoRunner.FrameCount);
		if (Time.deltaTime < 0) {
			yield return null;
		}
		Debug.Log("coImmediate finished " + CoRunner.FrameCount);
	}

	public IEnumerator coYieldOn (IEnumerator enumerator)
	{
		yield return enumerator;
		Debug.Log("finished YieldOn " + CoRunner.FrameCount);
	}

	public IEnumerator coMultiYield ()
	{
		var routine = CoRunner.Start(coWWW());
		CoRunner.Start(coYieldOn(routine));
		yield return CoRunner.Start(coYieldOn(routine));
		Debug.Log("finished MultiYield " + CoRunner.FrameCount);
	}
}
