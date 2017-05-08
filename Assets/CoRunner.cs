using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Wooga.Coroutines;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class CoRunner
{
	private static readonly HashSet<IEnumerator> routines = new HashSet<IEnumerator>();
	private static readonly List<IEnumerator> routinesToAdd = new List<IEnumerator>();
	private static readonly List<IEnumerator> routinesToRemove = new List<IEnumerator>(10);

	private static UniProxy proxy;
	private static bool isInitialized;

	public static int FrameCount { get; private set; }

	private static void Init ()
	{
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			Debug.Log("hook up to editor.update");
			EditorApplication.update += Update;
		}
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
		StartInternal(routine);
		return routine;
	}

	public static Wooroutine<T> Start<T> (IEnumerator routine)
	{
		var wr = new Wooroutine<T>(routine);
		StartInternal(wr);
		return wr;
	}

	public static void Stop (IEnumerator routine)
	{
		routinesToRemove.Add(routine);
	}

	static void StartInternal (IEnumerator routine)
	{
		if (!isInitialized) Init();

		var hasNext = AdvanceEnumerator(routine);

		if (hasNext) {
			routinesToAdd.Add(routine);
		}
	}

	static bool AdvanceEnumerator (IEnumerator routine)
	{
		bool hasNext;
		do {
			hasNext = routine.MoveNext();
			var enumerator = routine.Current as IEnumerator;
			if (enumerator != null) {
				if (!enumerator.IsRunning()) {
					Start(enumerator);
				}
			}

		} while (hasNext && ShouldCall(routine.Current));
		return hasNext;
	}

	public static void Update ()
	{
		FrameCount++;
		AddRoutines();

		ProcessAll();

		while (routinesToRemove.Count > 0) {
			RemoveFinishedRoutines();
			ProcessAllNested();
		}
	}

	static void RemoveFinishedRoutines ()
	{
		foreach (var routine in routinesToRemove) {
			routines.Remove(routine);
		}
		routinesToRemove.Clear();
	}

	static void ProcessAll ()
	{
		foreach (var routine in routines) {
			if (Process(routine)) {
				routinesToRemove.Add(routine);
			}
		}
	}

	static void ProcessAllNested ()
	{
		foreach (var routine in routines) {
			if (ProcessOnlyNested(routine)) {
				routinesToRemove.Add(routine);
			}
		}
	}

	static bool Process (IEnumerator routine) {
		if (ShouldCall(routine.Current)) {
			var wr = routine as Wooroutine;
			if (wr != null && wr.Canceled) { return true;}
			return !AdvanceEnumerator(routine);
		}
		return false;
	}

	static bool ProcessOnlyNested (IEnumerator routine)
	{
		if (routine.Current is IEnumerator) {
			return Process(routine);
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
		var cyi = enumerator as CustomYieldInstruction;
		if (cyi != null) return !cyi.keepWaiting;
		return !routines.Contains(enumerator) && !routinesToAdd.Contains(enumerator);
	}


	private static bool ShouldCall (object obj)
	{
		CustomYieldInstruction inst = obj as CustomYieldInstruction;
		if (inst != null) {
			return !inst.keepWaiting;
		}

		if (obj is WWW) {
			return ((WWW) obj).isDone;
		}

		if (obj is AsyncOperation) {
			return ((AsyncOperation) obj).isDone;
		}

		if (obj is IEnumerator) {
			return IsFinished((IEnumerator) obj);
		}

		if (obj is YieldInstruction) {
			Debug.LogError("Didn't know what to do with " + obj.GetType());
		}
		return true;
	}

	public class WaitForFrames : CustomYieldInstruction
	{
		private int endFrame = -1;

		public WaitForFrames (int numFrames)
		{
			endFrame = Time.frameCount + numFrames;
		}

		public WaitForFrames WaitOne ()
		{
			endFrame = Time.frameCount + 1;
			return this;
		}

		public override bool keepWaiting {
			get {
				return Time.frameCount < endFrame;
			}
		}
	}
}

public class WaitForFrames : CoRunner.WaitForFrames
{
	public WaitForFrames (int numFrames) : base(numFrames) { }
}


public static class IEnumeratorExtensions
{
	public static bool IsRunning (this IEnumerator enumerator)
	{
		return GetState(enumerator) > 0;

	}

	public static bool IsFinished (IEnumerator enumerator)
	{
		return GetState(enumerator) == -1;
	}

	static int GetState (IEnumerator enumerator)
	{
		var type = enumerator.GetType();
		var field = type.GetField("$PC", BindingFlags.NonPublic | BindingFlags.Instance);
		if (field != null) {
			return (int)field.GetValue(enumerator);
		}
		return 1;
	}

	public static IEnumerator Start (this IEnumerator enumerator)
	{
		return CoRunner.Start(enumerator);
	}

	public static Wooroutine<T> Start<T> (this IEnumerator enumerator)
	{
		return CoRunner.Start<T>(enumerator);
	}
}
