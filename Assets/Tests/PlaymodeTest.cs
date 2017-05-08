using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;

public class PlaymodeTest
{
	private Tests tests = new Tests();
	private bool isFinished;

	CustomYieldInstruction WaitTillFinished;

	[InitializeOnLoadMethod]
	public void Setup ()
	{
		Debug.Log("Setup");
		WaitTillFinished = new WaitUntil(() => isFinished);
	}

	[UnityTest]
	public IEnumerator Basic ()
	{
		Debug.Log("start coBasic " + Time.frameCount);
		CoRunner.Start(coRunTest(tests.coBasic(), 1));
		yield return Await();
	}

	[UnityTest]
	public IEnumerator IEnumerator ()
	{
		Debug.Log("start coIEnumerator " + Time.frameCount);
		CoRunner.Start(coRunTest(tests.coIEnumerator(), 1));
		yield return Await();
	}

	[UnityTest]
	public IEnumerator RunImmediate ()
	{
		Debug.Log("start coRunImmediate " + Time.frameCount);
		CoRunner.Start(coRunTest(tests.coRunImmediate(), 0));
		yield return Await();
	}

	[UnityTest]
	public IEnumerator MultiYield ()
	{
		Debug.Log("start coMultiYield " + Time.frameCount);
		CoRunner.Start(coRunTest(tests.coMultiYield(), 5));
		yield return Await();
	}

	[UnityTest]
	public IEnumerator AwaitResult  ()
	{
		Debug.Log("start coReturnInt " + Time.frameCount);
		var result = 10;
		CoRunner.Start(coRunTest(tests.coReturnInt(result), 0, result));
		yield return Await();
	}

	[UnityTest]
	public IEnumerator AwaitSpiegel ()
	{
		Debug.Log("start coReturnWWW " + Time.frameCount);
		var url = "http://www.spiegel.de";
		Predicate<string> assertion = s => !string.IsNullOrEmpty(s);

		CoRunner.Start(coRunTest(tests.coReturnWWW(url), assertion));
		yield return Await();
	}

	[UnityTest]
	public IEnumerator Abort ()
	{
		var url = "http://www.spiegel.de";
		var en = tests.coReturnWWW(url);
		CoRunner.Start(coRunTest(en, 1));
		yield return null;
		Debug.Log("stop at " + Time.frameCount);
		CoRunner.Stop(en);
		yield return Await();
	}

	[UnityTest]
	public IEnumerator StartOnYield ()
	{
		var en = tests.coStartOnYield();
		CoRunner.Start(coRunTest(en, 3));
		yield return Await();
	}

// WebRequest is unable to fetch google...
//	[UnityTest]
//	public IEnumerator AwaitGoogle ()
//	{
//		Debug.Log("start coReturnWWW " + Time.frameCount);
//		var url = "http://www.google.de";
//		Predicate<string> assertion = s => !string.IsNullOrEmpty(s);
//
//		CoRunner.Start(coRunTest(tests.coReturnWWW(url), assertion));
//		yield return Await();
//	}

	IEnumerator coRunTest<T> (IEnumerator coTest, Predicate<T> assert)
	{
		isFinished = false;
		Debug.Log("start " + coTest + " " + Time.frameCount);
		var result = CoRunner.Start<T>(coTest);
		yield return result;
		Debug.Log("finished " + Time.frameCount + (result.ReturnValue == null));
		Assert.True(assert(result.ReturnValue), coTest.ToString());
		isFinished = true;
	}

	IEnumerator coRunTest (IEnumerator coTest, int expectedDuration)
	{
		isFinished = false;
		int startFrame = Time.frameCount;
		Debug.Log("start " + coTest + " " + Time.frameCount);
		yield return CoRunner.Start(coTest);
		Debug.Log("finished " + Time.frameCount);
		Assert.AreEqual(startFrame + expectedDuration, Time.frameCount, coTest.ToString());
		isFinished = true;
	}

	IEnumerator coRunTest<T> (IEnumerator coTest, int expectedDuration, T expectedResult)
	{
		isFinished = false;
		int startFrame = Time.frameCount;
		Debug.Log("start " + coTest + " " + Time.frameCount);
		var result = CoRunner.Start<T>(coTest);
		Debug.Log(result);
		yield return result;
		Debug.Log("finished at " + Time.frameCount);
		Assert.AreEqual(startFrame + expectedDuration, Time.frameCount, coTest.ToString());
		Assert.AreEqual(expectedResult, result.ReturnValue);
		isFinished = true;
	}

	CustomYieldInstruction Await ()
	{
		return new WaitUntil(() => isFinished);
	}
}
