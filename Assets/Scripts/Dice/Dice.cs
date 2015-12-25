using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// The definition of a set of dice to roll. Convenient to store/pass around
/// </summary>
public class Dice
{
	public int numberToRoll = 1;
	public int numberOfFaces = 6;
	public int modifier = 0;

	public Dice ( int num, int face, int mod )
	{
		numberOfFaces = face;
		numberToRoll = num;
		modifier = mod;
	}

	public Dice ( JSONNode data, int tokenIndex, int attackIndex )
	{
		numberToRoll = data [ "Dice" ] [ tokenIndex ] [ "rolls" ] [ attackIndex ].AsInt;
		numberOfFaces = data [ "Dice" ] [ tokenIndex ] [ "faces" ] [ attackIndex ].AsInt;
		modifier = data [ "Dice" ] [ tokenIndex ] [ "mod" ] [ attackIndex ].AsInt;
	}

	public void Export ( ref JSONNode data, int tokenIndex, int attackIndex )
	{
		data [ "Dice" ] [ tokenIndex ] [ "rolls" ] [ attackIndex ].AsInt = numberToRoll;
		data [ "Dice" ] [ tokenIndex ] [ "faces" ] [ attackIndex ].AsInt = numberOfFaces;
		data [ "Dice" ] [ tokenIndex ] [ "mod" ] [ attackIndex ].AsInt = modifier;

	}
}
