using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterGenerator : SingletonMonoBehaviour<CharacterGenerator> 
{
	bool spawnNew = true;
	public Image frame;
	public CharGen_Ability abilityButtonTemplate;

	public InputField characterNameField;
	public InputField playerNameField;

	public AttackDefinition attackDefinitionTemplate;
	List<AttackDefinition> attackDefinitions = new List<AttackDefinition>();

	CharacterStats stats;

	List<CharGen_Ability> abilitySlots = new List<CharGen_Ability>();

	bool initialized = false;
	void Start()
	{
		if ( initialized ) { return; }
		initialized = true;
		attackDefinitionTemplate.gameObject.SetActive ( false );
		abilityButtonTemplate.gameObject.SetActive ( false );
		foreach ( string s in CharacterStats.abilityNames )
		{
			AddAbilityButton ( s, 10, 1, 20, 1 );
		}

		AddAbilityButton ( "HP", 10, 1, 40, 1 );
		AddAbilityButton ( "AC", 10, 1, 20, 1 );
		AddAbilityButton ( "Spd", 20, 5, 50, 5 );
		AddAbilityButton ( "Lvl", 1, 1, 20, 1 );
		gameObject.SetActive( false );
	}

	public void Init ( CharacterStats s )
	{
		Start();
		spawnNew = false;
		gameObject.SetActive ( true );
		stats = s;
		characterNameField.text = stats.characterName;
		playerNameField.text = stats.playerName;
		foreach ( Attack a in stats.attacks.Values )
		{
			AddAttackDefinition ( a );
		}

		foreach ( CharGen_Ability stat in abilitySlots )
		{
			stats.abilities.TryGetValue ( stat.title.text, out stat.abilityValue);
			stat.value.currentValue = stat.abilityValue;
			stat.value.SetDisplayedValue();
		}
	}

	public void Init()
	{
		spawnNew = true;
		gameObject.SetActive ( true );
		characterNameField.text = "";
		playerNameField.text = "";
		stats = null;
	}

	void AddAbilityButton ( string nm, int val, int min, int max, int step )
	{
		CharGen_Ability abBut = Instantiate ( abilityButtonTemplate );
		abBut.gameObject.SetActive ( true );
		abBut.transform.SetParent ( abilityButtonTemplate.transform.parent );
		abBut.transform.localScale = Vector3.one;
		abBut.Setup ( nm, val, min, max, step );
		abilitySlots.Add ( abBut );
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

		// Shove it all into a dictionary
		Dictionary<string,int> statlist = new Dictionary<string, int>();

		foreach ( CharGen_Ability slot in abilitySlots )
		{
			int val = int.Parse(slot.value.value.text);
			statlist.Add ( slot.title.text, val );
		}

		// Abilities
		int[] abilities = new int[6];
		int i=0;
		foreach ( string s in CharacterStats.abilityNames )
		{
			statlist.TryGetValue ( s, out abilities[i]);
			i++;
		}

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

		CloseGenerator();
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
