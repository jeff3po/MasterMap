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

	public Dictionary<string,int> abilities = new Dictionary<string, int>();
	public Dictionary<string,Attack> attacks = new Dictionary<string, Attack>();

	public static string[] abilityNames = new string[]{"St","Dx","Cn","In","Ws","Ch"};

	public int level = 1;
	public int armorClass = 10;
	public int speed = 30;
	public int hitPoints_Max = 10;
	public int hitPoints_Current = 10;
	public int hitPoints_Temporary = 0;

	public CharacterStats ( string nm, string player, int lev, int[] abs, int maxHP, int ac, int spd )
	{
		characterName = nm;
		playerName = player;
		SetAbilities ( abs );
		SetHP ( maxHP, 0, maxHP);
		armorClass = ac;
		speed = spd;
	}

	public void AddAttack ( Attack a )
	{
		if ( attacks.ContainsKey ( a.title ) ) { return; }
		attacks.Add ( a.title, a );
	}

	public void SetHP ( int current, int temp=-1, int max=-1)
	{
		hitPoints_Current = current;
		if ( temp >= 0 ) { hitPoints_Temporary = temp; }
		if ( max >= 0 ) { hitPoints_Max = max; }
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
