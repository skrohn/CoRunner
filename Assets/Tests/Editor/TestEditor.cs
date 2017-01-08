using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestEditor
{
	[MenuItem("Wooga/CoRunner/Test")]
	public static void TestBasic ()
	{
		CoRunner.Start(new Tests().coBasic());
	}
}
