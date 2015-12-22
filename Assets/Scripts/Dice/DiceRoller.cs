using UnityEngine;
using System.Collections;

public static class DiceRoller 
{
	public static int Roll ( Dice dice )
	{
		return Roll ( dice.numberToRoll, dice.numberOfFaces, dice.modifier );
	}

	public static int Roll ( Dice dice, out int critical )
	{
		critical = 0;
		return Roll ( dice.numberToRoll, dice.numberOfFaces, dice.modifier, out critical );
	}

	public static int Roll ( int numberDice, int diceFace, int modifier=0)
	{
		int ignore=0;
		return Roll ( numberDice, diceFace, modifier, out ignore );
	}

	public static int Roll ( int numberDice, int diceFace, int modifier, out int critical )
	{
		critical = 0;
		int result = 0;
		for ( int i=0;i<numberDice;i++)
		{
			int aRoll = Random.Range(0,diceFace);
			// 0 inclusive, diceface exclusive, so bump up to human numbers
			aRoll += 1;
			result += aRoll;
		}

		// Check for crit success and failure
		if ( diceFace == 20 )
		{
			if ( result == 1 ) { critical = -1;}
			if ( result == 20 ) { critical = 1;}
		}

		result += modifier;

		return result;
	}
}
