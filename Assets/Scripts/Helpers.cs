using UnityEngine;
using System.Collections;

public static class Helpers 
{
	public static float RemapToRange ( float minOrig, float maxOrig, float minNew, float maxNew, float origValue )
	{
		return ( origValue - minOrig ) * ( (maxNew-minNew) / (maxOrig-minOrig) ) + minNew;
	}
}
