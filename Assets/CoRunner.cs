using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CoRunner
{
	private static readonly HashSet<IEnumerator> routines = new HashSet<IEnumerator>();
	private static readonly List<IEnumerator> routinesToAdd = new List<IEnumerator>();

	private static UniProxy proxy;
	private static bool isInitialized;

	public static int FrameCount { get; private set; }

	private static void Init ()
	{
		#if UNITY_EDITOR
		EditorApplication.update += Update;
		#endif

		if (Application.isPlaying) {
			if (!proxy) {
				proxy = new GameObject("UniProxy").AddComponent<UniProxy>();
			}
			proxy.onUpdate += Update;
		}

		isInitialized = true;
	}

	public static IEnumerator Start (IEnumerator routine)
	{
		if (!isInitialized) Init();

		var hasNext = AdvanceEnumerator(routine);

		if (hasNext) {
			routinesToAdd.Add(routine);
		}
		return routine;
	}

	static bool AdvanceEnumerator (IEnumerator routine)
	{
		bool hasNext;
		do {
			hasNext = routine.MoveNext();
		} while (hasNext && routine.Current is IEnumerator && IsFinished((IEnumerator)routine.Current));
		return hasNext;
	}

	public static void Update ()
	{
		FrameCount++;
		AddRoutines();
		if (routines.Count > 0) Debug.Log("update count: " + routines.Count);
		routines.RemoveWhere(Process);
	}

	static bool Process (IEnumerator routine) {
		if (ShouldCall(routine.Current)) {
			return !AdvanceEnumerator(routine);
		}
		return false;
	}

	static void AddRoutines ()
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
