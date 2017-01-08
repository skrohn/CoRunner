using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEditor;

[UnityPlatform(TestPlatform.PlayMode)]
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

	IEnumerator coRunTest (IEnumerator coTest, int expectedDuration)
	{
		isFinished = false;
		int startFrame = Time.frameCount;
		Debug.Log("start " + coTest + " " + Time.frameCount);
		yield return CoRunner.Start(coTest);
		Debug.Log(Time.frameCount);
		Assert.AreEqual(startFrame + expectedDuration, Time.frameCount, coTest.ToString());
		isFinished = true;
	}

	CustomYieldInstruction Await ()
	{
		return new WaitUntil(() => isFinished);
	}
}
