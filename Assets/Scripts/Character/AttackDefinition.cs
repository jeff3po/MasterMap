using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AttackDefinition : MonoBehaviour 
{
	public InputField attackName;

	public CharGen_AbilitySlider abilityValue;
	public CharGen_AbilitySlider rangeValue;
	public CharGen_AbilitySlider toHitValue;

	public Dropdown attackType;

	public CharGen_AbilitySlider diceNum;
	public CharGen_AbilitySlider diceFace;
	public CharGen_AbilitySlider diceMod;
	public CharGen_AbilitySlider diceDam;

	public Attack theAttack
	{
		get
		{
			string title = attackName.text;
			string ability = abilityValue.value.text;
			int range = int.Parse ( rangeValue.value.text );
			string type = attackType.options[attackType.value].text;
			Attack.AttackType atype = (Attack.AttackType) System.Enum.Parse ( typeof ( Attack.AttackType ), type );
			int toHit = int.Parse (toHitValue.value.text);
			int diceNumber = int.Parse ( diceNum.value.text );
			int diceFaces = int.Parse ( diceFace.value.text );
			int diceModifier = int.Parse ( diceMod.value.text );
			string diceDamage = diceDam.value.text;

			Attack a = new Attack ( title, ability, range, atype, toHit, diceNumber, diceFaces, diceModifier, diceDamage );
			return a;
		}
	}

	string[] diceFaces = new string[] { "4", "6", "8", "10", "12", "20", "100" };

	public void Init()
	{
		abilityValue.Setup ( 0, 0, 10, 1, CharacterStats.abilityNames );
		rangeValue.Setup ( 5, 5, 120, 5 );
		toHitValue.Setup ( 0, -10, 10, 1 );
		diceNum.Setup ( 1, 1, 10, 1 );
		diceFace.Setup ( 0, 0, 1, 1, diceFaces );
		diceMod.Setup ( 0, -10, 10, 1 );
		diceDam.Setup ( 0, 0, 1, 1, Attack.DamageType );
	}
}
