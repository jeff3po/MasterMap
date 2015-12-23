using UnityEngine;
using System.Collections;

/// <summary>
/// A definition of a single damage-producing effect
/// </summary>
public class Attack
{
	/// <summary>
	/// Static list for use throughout game
	/// </summary>
	public static string[] DamageType = new string[]{"acid","bludgeoning","cold","fire","lightning","necrotic","piercing","poison","psychic","radiant","slashing","thunder"};

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

	public Attack ( string tit, string ab, int r, AttackType atype, int plusToHit, int damDiceNumber, int damDiceFace, int damDiceMod, string dType )
	{
		title = tit;
		abilityForRoll = ab;
		range = r;
		attackType = atype;
		damageType = dType;
		damageDice = new Dice ( damDiceNumber, damDiceFace, damDiceMod );
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
}
