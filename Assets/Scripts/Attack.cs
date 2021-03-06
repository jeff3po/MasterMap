﻿using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// A definition of a single damage-producing effect
/// </summary>
public class Attack
{
	/// <summary>
	/// Static list for use throughout game
	/// </summary>
	public static string[] DamageType = new string[]{"Acid","Bludgeon","Cold","Fire","Lightning","Necrotic","Piercing","Poison","Psychic","Radiant","Slashing","Thunder"};

	public enum AttackType { Undefined, Melee, Ranged };

	/// <summary>
	/// Name of the attack
	/// </summary>
	public string title;

	/// <summary>
	/// Which ability to roll against
	/// </summary>
	public string abilityForRoll;

	/// <summary>
	/// Melee or ranged
	/// </summary>
	public AttackType attackType = AttackType.Undefined;
		
	/// <summary>
	/// Range in feet
	/// </summary>
	public int range = 5;

	/// <summary>
	/// Additional toHit bonus native to the attack, regardless of attacker
	/// </summary>
	public int plusToHit = 0;

	/// <summary>
	/// Which dice to roll for damage
	/// </summary>
	public Dice damageDice = null;

	public string damageType = "None";

	public Attack ( string tit, string ab, int r, AttackType atype, int toHit, int damDiceNumber, int damDiceFace, int damDiceMod, string dType )
	{
		title = tit;
		abilityForRoll = ab;
		range = r;
		attackType = atype;
		plusToHit = toHit;
		damageDice = new Dice ( damDiceNumber, damDiceFace, damDiceMod );
		damageType = dType;
	}

	/// <summary>
	/// Rolls to hit. On success, rolls and returns damage amount (not applied directly to target).
	/// Caller can determine what to do with the resulting damage based on extenuating circumstances
	/// </summary>
	public int LaunchAttack ( CharacterStats attacker, int targetAC, out string resultString )
	{
		// resultString passes back a message than can be displayed in the UI
		resultString = "";

		// Crit is -1 on failure, +1 on success
		int crit = 0;

		// Based on skill type of attack
		int plusToHit = attacker.SkillModifier ( abilityForRoll );

		// The result of the roll
		int roll = DiceRoller.Roll ( 1, 20, plusToHit, out crit );

		// Accumulate damage on successful hit
		int damage = 0;

		// Check crits. resultString will include this text
		string critResult = "";
		if ( crit != 0 )
		{
			critResult = " - Critical";
			if ( crit < 0 ) { critResult += " Fail";}
			if ( crit > 0 ) { critResult += " Success";}
		}

		// Did that get past the armor?
		if ( roll >= targetAC )
		{
			resultString = "Hit";

			// Automatically roll the damage. Calling function elects what to do with the resulting value
			damage = DiceRoller.Roll ( damageDice );

			// Critical hit does double damage
			if ( crit > 0 )
			{
				damage *= 2;
			}
		}
		else
		{
			resultString = "Miss";
		}

		// Hit or miss plus critical (if applicable)
		resultString += critResult;

		return damage;
	}

	public Attack ( ref JSONNode data, int tokenIndex, int attackIndex )
	{
		title = data [ "Attack" ] [ tokenIndex ] [ "title" ] [ attackIndex ];
		abilityForRoll = data [ "Attack" ] [ tokenIndex ] [ "ability"] [ attackIndex ];
		string attackTypeName = data [ "Attack" ] [ tokenIndex ] [ "attackType"] [ attackIndex ];
		attackType = (AttackType) System.Enum.Parse ( typeof (AttackType), attackTypeName );
		range = data [ "Attack" ] [ tokenIndex ] [ "range"] [ attackIndex ].AsInt;
		plusToHit = data [ "Attack" ] [ tokenIndex ] [ "toHit"] [ attackIndex ].AsInt;
		damageType = data [ "Attack" ] [ tokenIndex ] [ "damage"] [ attackIndex ];
		damageDice = new Dice ( ref data, tokenIndex, attackIndex );
	}

	public void Export ( ref JSONNode data, int tokenIndex, int attackIndex )
	{
		data [ "Attack" ] [ tokenIndex ] [ "title" ] [ attackIndex ] = title;
		data [ "Attack" ] [ tokenIndex ] [ "ability"] [ attackIndex ] = abilityForRoll;
		data [ "Attack" ] [ tokenIndex ] [ "attackType"] [ attackIndex ] = attackType.ToString();
		data [ "Attack" ] [ tokenIndex ] [ "range"] [ attackIndex ].AsInt = range;
		data [ "Attack" ] [ tokenIndex ] [ "toHit"] [ attackIndex ].AsInt = plusToHit;
		data [ "Attack" ] [ tokenIndex ] [ "damage"] [ attackIndex ] = damageType;
		damageDice.Export ( ref data, tokenIndex, attackIndex );
	}

	public override string ToString()
	{
		string output = "\n"+title+" "+abilityForRoll+" "+attackType.ToString()+" "+range+"ft "+plusToHit+" to hit "+damageType+"  "+damageDice.numberToRoll+"d"+damageDice.numberOfFaces+"+"+damageDice.modifier;
		return output;
	}
}
