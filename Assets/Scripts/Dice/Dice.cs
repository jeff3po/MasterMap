using UnityEngine;
using System.Collections;

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
}
