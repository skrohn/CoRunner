using System;
using UnityEngine;
using UnityEngine.AI;

public class UniProxy : MonoBehaviour
{
	public Action onUpdate = delegate { };
	public Action onLateUpdate = delegate { };

	// Update is called once per frame
	void Update ()
	{
		onUpdate();
	}

	void LateUpdate ()
	{
		onLateUpdate();
	}
}
