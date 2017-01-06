using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WooRoutine
{

}

public static class CoRunner
{
	private static readonly HashSet<IEnumerator> routines = new HashSet<IEnumerator>();
	private static readonly List<IEnumerator> routinesToAdd = new List<IEnumerator>();

	private static UniProxy proxy;
	private static bool isInitialized;

	private static void Init ()
	{
		if (!proxy) {
			proxy = new GameObject("UniProxy").AddComponent<UniProxy>();
		}
		proxy.onUpdate += Update;
		proxy.onLateUpdate += LateUpdate;
		isInitialized = true;
	}

	public static IEnumerator Start (IEnumerator routine)
	{
		if (!isInitialized) Init();

		bool hasNext;
		do {
			hasNext = routine.MoveNext();
		} while (hasNext && routine.Current is IEnumerator && IsFinished((IEnumerator)routine.Current));

		if (hasNext) {
			routinesToAdd.Add(routine);
		}
		return routine;
	}

	public static void Update ()
	{
		routines.RemoveWhere(Process);
	}

	static bool Process (IEnumerator item) {
//		Debug.Log("update current: " + item.Current + " count: " + routines.Count);
		if (ShouldCall(item.Current)) {
			var result = item.MoveNext();
			Debug.Log(!result);
			return !result;
		}
		return false;
	}

	public static void LateUpdate ()
	{
		foreach (var r in routinesToAdd) {
			routines.Add(r);
		}
		routinesToAdd.Clear();
	}

	static bool IsFinished (IEnumerator enumerator)
	{
		return !routines.Contains(enumerator) && !routinesToAdd.Contains(enumerator);
	}

	private static bool ShouldCall (object obj)
	{
		if (obj == null) return true;

		CustomYieldInstruction inst = obj as CustomYieldInstruction;
		if (inst != null) return !inst.keepWaiting;

		if (obj is WWW) {
			return ((WWW) obj).isDone;
		}

		if (obj is IEnumerator) {
			return IsFinished((IEnumerator) obj);
		}

		Debug.LogWarning("Didn't know what to do with " + obj.GetType());
		return true;
	}

}
