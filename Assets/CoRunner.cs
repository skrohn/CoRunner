using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoRunner
{
	private static readonly List<IEnumerator> routines = new List<IEnumerator>();
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

		routinesToAdd.Add(routine);
		routine.MoveNext();
		return routine;
	}

	public static void Update ()
	{
		for (var i = 0; i < routines.Count; i++) {
			Debug.Log("update: " + routines[i].Current);
			if (ShouldCall(routines[i].Current)) {
				var keepRunning = routines[i].MoveNext();
				if (!keepRunning) {
					routines.RemoveAt(i--);
				}
			}
		}
	}

	public static void LateUpdate ()
	{
		routines.AddRange(routinesToAdd);
		routinesToAdd.Clear();
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
//			((IEnumerator)obj).
		}

		Debug.LogWarning("Didn't know what to do with " + obj.GetType());
		return true;
	}

}
