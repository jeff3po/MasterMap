using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterDrawer : InfoDrawer 
{
	public Text characterNameField;
	public Text playerNameField;
	public StatField statFieldTemplate;
	public StatField statFieldOtherTemplate;

	public Dropdown attackDropdown;

	CharacterStats stats;

	void Start()
	{
		statFieldTemplate.gameObject.SetActive ( false );
		statFieldOtherTemplate.gameObject.SetActive ( false );
	}

	public void Setup ( CharacterStats s )
	{
		stats = s;
		playerNameField.text = stats.playerName;
		characterNameField.text = stats.characterName;

		foreach ( string nm in stats.abilities.Keys )
		{
			int val = 0;
			stats.abilities.TryGetValue ( nm, out val );
			AddStat ( nm, val.ToString());
		}

		AddOtherStat ( "Level", stats.level.ToString() );
		AddOtherStat ( "AC", stats.armorClass.ToString() );
		AddOtherStat ( "Spd", stats.speed.ToString() );
		string hp = string.Format ( "{0}/{1}", stats.hitPoints_Current, stats.hitPoints_Max );
		AddOtherStat ( "HP", hp );

		List<Dropdown.OptionData> data = new List<Dropdown.OptionData>();

		data.Add ( new Dropdown.OptionData ("Attack") );

		foreach ( string key in stats.attacks.Keys )
		{
			data.Add ( new Dropdown.OptionData ( key ) );
		}

		attackDropdown.AddOptions ( data );
	}

	/// <summary>
	/// Stat template is already in a grid layout group, so no positioning required
	/// </summary>
	public void AddStat ( string nm, string val )
	{
		_addStat ( nm, val, statFieldTemplate );
	}

	public void AddOtherStat ( string nm, string val )
	{
		_addStat ( nm, val, statFieldOtherTemplate );
	}

	void _addStat ( string nm, string val, StatField template )
	{
		StatField field = Instantiate ( template );
		field.gameObject.SetActive ( true );
		field.transform.SetParent ( template.transform.parent );
		field.transform.localScale = Vector3.one;
		field.Setup ( nm, val);
	}

	public void ChooseAttack()
	{
		string attack = attackDropdown.captionText.text;

		if ( attack == "Attack" ) 
		{
			// Empty slot at start of list
			return;
		}

		Debug.Log ( "Attack: "+attack);

		stats.attacks.TryGetValue ( attack, out currentAttack );

		if ( currentAttack == null )
		{
			Debug.LogError ( string.Format ("Can't find attack {0} in attack list for {1}", attack, stats.characterName ));
			return;
		}

		// TODO: AC from target
		targetRoll = 12;

		int statMod = stats.SkillModifier ( currentAttack.abilityForRoll );
		statMod += currentAttack.plusToHit;

		SpinTheWheel ( new Dice ( 1,20,statMod), ResultOfAttackRoll, targetRoll, "to hit" );

	}

	void SpinTheWheel ( Dice dice, System.Action<int> callback, int target, string action )
	{
		Spinner spin = Instantiate ( map.spinnerTemplateOnDisk );
		spin.transform.SetParent ( map.spinnerLayer );
		spin.transform.localScale = Vector3.one;
		spin.transform.localPosition = Vector3.zero;
		spin.Setup ( dice, callback, target, action );
	}

	int targetRoll = 0;
	Attack currentAttack = null;

	void ResultOfAttackRoll ( int result )
	{
		if ( result >= targetRoll )
		{
			Debug.Log ( "Success!");
			SpinTheWheel ( currentAttack.damageDice, ResultOfDamageRoll, -999, "damage" );
		}
	}

	void ResultOfDamageRoll ( int result )
	{
		Debug.Log ( "Damage: "+result );
	}
}
