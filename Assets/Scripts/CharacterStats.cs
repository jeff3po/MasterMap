using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Base stats, derived stats and helper functions to perform dice rolls for a character
/// </summary>
public class CharacterStats
{
	public string characterName = "";
	public string playerName = "";

	Dictionary<string,int> abilities = new Dictionary<string, int>();

	public static string[] abilityNames = new string[]{"STR","DEX","CON","INT","WIS","CHA"};

	public int Level = 1;
	public int ArmorClass = 10;
	public int HitPoints_Max = 10;
	public int HitPoints_Current = 10;
	public int HitPoints_Temporary = 0;

	public CharacterStats ( string nm, string player, int[] abs, int maxHP )
	{
		characterName = nm;
		playerName = player;
		SetAbilities ( abs );
		SetHP ( maxHP, 0, maxHP);
	}

	public void SetHP ( int current, int temp=-1, int max=-1)
	{
		HitPoints_Current = current;
		if ( temp >= 0 ) { HitPoints_Temporary = temp; }
		if ( max >= 0 ) { HitPoints_Max = max; }
	}

	public void SetAbilities ( int[] abs )
	{
		if ( abs.Length != 6 )
		{
			Debug.LogError ( "Must pass all six stats in order" );
			return;
		}

		for ( int i=0;i<abs.Length;i++ )
		{
			string nm = abilityNames[i];
			abilities.Add ( nm, abs[i]);
		}
	}

	public int SkillModifier ( string abilityName )
	{
		int ability = 10;
		abilities.TryGetValue ( abilityName, out ability );
		int delta = ability - 10;
		return delta/2;
	}
}
