using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterGenerator : SingletonMonoBehaviour<CharacterGenerator> 
{
	bool spawnNew = true;
	public Image frame;
	public CharGen_Ability abilityButtonTemplate;
	public CharGen_Stat statButtonTemplate;

	public InputField characterNameField;
	public InputField playerNameField;

	public AttackDefinition attackDefinitionTemplate;
	List<AttackDefinition> attackDefinitions = new List<AttackDefinition>();

	CharacterStats stats;

	string incomingCharacter = "";

	List<CharGen_Stat> abilitySlots = new List<CharGen_Stat>();

	bool initialized = false;
	void Start()
	{
		if ( initialized ) { return; }
		initialized = true;
		attackDefinitionTemplate.gameObject.SetActive ( false );
		abilityButtonTemplate.gameObject.SetActive ( false );
		statButtonTemplate.gameObject.SetActive ( false );
		foreach ( string s in CharacterStats.abilityNames )
		{
			AddAbilityButton ( s, 10 );
		}

		AddStatButton ( "HP", 10, 1, 40, 1 );
		AddStatButton ( "AC", 10, 1, 20, 1 );
		AddStatButton ( "Spd", 20, 5, 50, 5 );
		AddStatButton ( "Lvl", 1, 1, 20, 1 );
		gameObject.SetActive( false );
	}

	public void Init ( CharacterStats s )
	{
		Start();
		incomingCharacter = s.characterName;

		spawnNew = false;
		gameObject.SetActive ( true );
		stats = s;
		characterNameField.text = stats.characterName;
		playerNameField.text = stats.playerName;
		foreach ( Attack a in stats.attacks.Values )
		{
			AddAttackDefinition ( a );
		}

		foreach ( CharGen_Stat stat in abilitySlots )
		{
			stats.abilities.TryGetValue ( stat.title.text, out stat.abilityValue);
			stat.value.currentValue = stat.abilityValue;
			stat.value.SetDisplayedValue();
		}

		int hp = stats.hitPoints_Max;
		int spd = stats.speed;
		int ac = stats.armorClass;
		int level = stats.level;

		UpdateStatValue ( "HP", hp );
		UpdateStatValue ( "Spd", spd );
		UpdateStatValue ( "AC", ac );
		UpdateStatValue ( "Lvl", level );
	}

	public void Init()
	{
		spawnNew = true;
		gameObject.SetActive ( true );
		characterNameField.text = "";
		playerNameField.text = "";
		stats = null;
	}

	void AddAbilityButton ( string nm, int val )
	{
		CharGen_Ability abBut = Instantiate ( abilityButtonTemplate );
		abBut.gameObject.SetActive ( true );
		abBut.transform.SetParent ( abilityButtonTemplate.transform.parent );
		abBut.transform.localScale = Vector3.one;
		abBut.SetupAbility ( nm, val, 3, 24, 1 );
		abilitySlots.Add ( abBut );
	}

	void AddStatButton ( string nm, int val, int min, int max, int step )
	{
		CharGen_Stat statButton = Instantiate ( statButtonTemplate );
		statButton.gameObject.SetActive ( true );
		statButton.transform.SetParent ( abilityButtonTemplate.transform.parent );
		statButton.transform.localScale = Vector3.one;
		statButton.SetupStat ( nm, val, min, max, step );
		abilitySlots.Add ( statButton );
	}

	void AddAttackDefinition ( Attack a )
	{
		AttackDefinition adef = Instantiate ( attackDefinitionTemplate );
		InitDefinition ( adef, a );
	}

	public void AddAttackDefinition()
	{
		AttackDefinition adef = Instantiate ( attackDefinitionTemplate );
		InitDefinition ( adef );
	}

	public void DuplicateAttackDefinition ( AttackDefinition existingDef )
	{
		AttackDefinition adef = Instantiate ( existingDef );
		InitDefinition ( adef );
	}

	void InitDefinition ( AttackDefinition adef, Attack a=null )
	{
		adef.gameObject.SetActive ( true );
		adef.transform.SetParent ( attackDefinitionTemplate.transform.parent );
		adef.transform.localScale = Vector3.one;
		adef.Init(a);
		attackDefinitions.Add ( adef );
	}

	public void Save()
	{
		// Dump all the values into the character stats

		// Shove all the entries in the stats column into a single dictionary
		Dictionary<string,int> statlist = new Dictionary<string, int>();

		// First six are abilities
		foreach ( CharGen_Stat slot in abilitySlots )
		{
			int val = int.Parse(slot.value.value.text);
			statlist.Add ( slot.title.text, val );
		}

		// Extract abilities from that list
		int[] abilities = new int[6];
		int i=0;
		foreach ( string s in CharacterStats.abilityNames )
		{
			statlist.TryGetValue ( s, out abilities[i]);
			i++;
		}

		// Pick out the individual stats from the rest of the list
		int hp = 0;
		statlist.TryGetValue ( "HP", out hp );
		int spd = 0;
		statlist.TryGetValue ( "Spd", out spd );
		int ac = 0;
		statlist.TryGetValue ( "AC", out ac );
		int level = 0;
		statlist.TryGetValue ( "Lvl", out level );

		string charName = characterNameField.text;
		string playerName = playerNameField.text;

		stats = new CharacterStats ( charName, playerName, level, abilities, hp, ac, spd );

		foreach ( AttackDefinition adef in attackDefinitions )
		{
			stats.AddAttack ( adef.theAttack );
		}

		if ( spawnNew )
		{
			WorldMap.Instance.SpawnNewCharacter(stats);
		}
		else
		{
			// Replace the existing character with this new one
			for ( int tc=0;tc<WorldMap.Instance.characterTokens.Count;tc++)
			{
				TokenCharacter ch = WorldMap.Instance.characterTokens[tc];

				if ( ch.stats.characterName == incomingCharacter )
				{
					// Delete and re-create this one
					TokenCharacter newChar = WorldMap.Instance.SpawnNewCharacter ( stats );
					newChar.transform.position = ch.transform.position;
					newChar.FindNewHome();
					WorldMap.Instance.characterTokens.Remove ( ch );
					Destroy ( ch.gameObject );
					break;
				}
			}
		}

		CloseGenerator();
	}

	void UpdateStatValue ( string statName, int value )
	{
		foreach ( CharGen_Stat stat in abilitySlots )
		{
			if ( stat.title.text == statName )
			{
				stat.abilityValue = value;
				stat.value.currentValue = value;
				stat.value.SetDisplayedValue();
				return;
			}
		}
	}

	public void Cancel()
	{
		CloseGenerator();
	}

	void CloseGenerator()
	{
		foreach (AttackDefinition adef in attackDefinitions) { Destroy ( adef.gameObject ); }
		attackDefinitions.Clear();
		gameObject.SetActive ( false );
	}
}
