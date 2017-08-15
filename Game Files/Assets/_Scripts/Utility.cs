﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utility //Place here all static methods that are generall use 
{
	public static List<GameObject> LoadAll(string path)
	{
		return (Resources.LoadAll(path, typeof(GameObject)).Cast<GameObject>()).ToList();
	}
	public static GameObject Load(string path)
	{
		return Resources.Load(path, typeof(GameObject)) as GameObject;
	}

	#region ExtensionMethods
	public static string RemoveAfter(this string s, char sign)
	{
		int index;
		for (int i = 0; i < s.Length; i++)
		{
			if (s[i] == sign)
			{
				index = i;
				char[] newString = new char[index];

				for (int j = 0; j < index; j++)
				{
					newString[j] = s[j];
				}
				string tmp = new string(newString);
				return tmp;
			}
		}
		return "0x0";
	}
	#endregion
}
