using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Wooga.Coroutines;

public class Test : MonoBehaviour {

	Tests tests = new Tests();

	IEnumerator Start ()
	{
		yield return null;
//		CoRunner.Start(coBasic());
//		CoRunner.Start(tests.coWWW());
//		CoRunner.Start(tests.coIEnumerator());
//		CoRunner.Start(coRunImmediate());
//		CoRunner.Start(coMultiYield());
//		CoRunner.Start(tests.coImmediateMultiYield());
//		CoRunner.Start(tests.coYieldForFrames(10));
//		CoRunner.Start(tests.coAwaitResult());
//		CoRunner.Start(tests.coAwaitCanceledResult());
		CoRunner.Start(tests.coAwaitNull());
	}
}

public class Tests
{
	public IEnumerator coAwaitResult ()
	{
		var x = CoRunner.Start<int>(coReturnInt(5));
		yield return x;
		Debug.Log(x.ReturnValue);
	}

	public IEnumerator coAwaitNull ()
	{
		var result = CoRunner.Start<int>(coReturnNull());
		yield return result;
		Debug.Log("done at " + Time.frameCount + " " + result.ReturnValue);
	}

	public IEnumerator coReturnNull ()
	{
		yield return null;
	}

	public IEnumerator coAwaitCanceledResult ()
	{
		var x = CoRunner.Start<int>(coReturnInt(5));
		CoRunner.Start(coCancel(1, x));
		yield return x;
		Debug.Log("done yielding " + Time.frameCount);
		Debug.Log(x.ReturnValue);
	}

	public IEnumerator coCancel<T> (int numFrames, Wooroutine<T> routine)
	{
//		Debug.Log("start stop routine " + Time.frameCount);
		yield return WaitForFrames.Wait(1);
//		Debug.Log("stop at " + Time.frameCount);
		routine.Stop();
	}

	public IEnumerator coReturnInt (int x)
	{
		yield return null;
		yield return null;
		yield return null;
		Debug.Log("return int at " + Time.frameCount);
		yield return x;
	}

	public IEnumerator coReturnWWW (string url)
	{
		Debug.Log(Time.frameCount);
		var req = UnityWebRequest.Get(url);
		yield return req.Send();
		Debug.Log(Time.frameCount);
		Debug.Log("error?: " + req.error + " " + req.isError);
		Debug.Log("text: " + req.downloadHandler.text);
		yield return req.downloadHandler.text;
	}

	public IEnumerator coImmediateMultiYield ()
	{
		var routine = CoRunner.Start(coBasic());
		var r2 = CoRunner.Start(coYieldOn(routine));
//		Debug.Log("start " + CoRunner.FrameCount);
		yield return r2;
		yield return CoRunner.Start(coImmediate());
		yield return CoRunner.Start(coImmediate());
//		Debug.Log("coImmediateMultiYield finished " + CoRunner.FrameCount);
	}

	public IEnumerator coBasic ()
	{
//		Debug.Log("coBasic started " + CoRunner.FrameCount + " " + Time.frameCount);
		yield return null;
//		Debug.Log("coBasic finished " + CoRunner.FrameCount + " " + Time.frameCount);
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
		Debug.Log("coIEnumerator start " + CoRunner.FrameCount);
		yield return CoRunner.Start(coBasic());
		Debug.Log("coIEnumerator finished " + CoRunner.FrameCount + " " + Time.frameCount);
	}

	public IEnumerator coRunImmediate ()
	{
		Debug.Log(CoRunner.FrameCount);
		yield return CoRunner.Start(coImmediate());
		Debug.Log("coRunImmediate finished " + CoRunner.FrameCount);
	}

	public IEnumerator coImmediate ()
	{
//		Debug.Log("start coImmediate " + CoRunner.FrameCount);
		if (Time.deltaTime < 0) {
			yield return null;
		}
//		Debug.Log("coImmediate finished " + CoRunner.FrameCount);
	}

	public IEnumerator coYieldForFrames (int numFrames)
	{
		for (int i = 0; i < numFrames; i++) {
			yield return null;
		}
		Debug.Log("coYieldForFrames finished " + Time.frameCount);
	}

	public IEnumerator coYieldOn (IEnumerator enumerator)
	{
		yield return enumerator;
//		Debug.Log("finished YieldOn " + CoRunner.FrameCount);
	}

	public IEnumerator coMultiYield ()
	{
		var routine = CoRunner.Start(coYieldForFrames(5));

		// make to routines yield on the same ienumerator
		CoRunner.Start(coYieldOn(routine));
		yield return CoRunner.Start(coYieldOn(routine));
		Debug.Log("finished MultiYield " + CoRunner.FrameCount);
	}
}

public class WaitForFrames : CustomYieldInstruction
{
	public static WaitForFrames Wait (int numFrames)
	{
		return new WaitForFrames(numFrames);
	}

	private int startFrame;
	private int endFrame;

	public WaitForFrames (int numFrames)
	{
		startFrame = Time.frameCount;
		endFrame = startFrame + numFrames;
//		Debug.Log("endframe " + endFrame);
	}

	public override bool keepWaiting {
		get {
//			Debug.Log(Time.frameCount + " " + endFrame + " " + (Time.frameCount < endFrame));
			return Time.frameCount < endFrame;
		}
	}
}
