using UnityEngine;
using System.Collections;

public class AttackDefinition : MonoBehaviour 
{
	public CharGen_AbilitySlider abilityValue;
	public CharGen_AbilitySlider rangeValue;
	public CharGen_AbilitySlider toHitValue;

	public CharGen_AbilitySlider diceNum;
	public CharGen_AbilitySlider diceFace;
	public CharGen_AbilitySlider diceMod;
	public CharGen_AbilitySlider diceDam;

	string[] diceFaces = new string[] { "4", "6", "8", "10", "12", "20", "100" };

	void Start()
	{
		// TODO: String varant
		abilityValue.Setup ( 0, 0, 10, 1, CharacterStats.abilityNames );
		rangeValue.Setup ( 5, 5, 120, 5 );
		toHitValue.Setup ( 0, -10, 10, 1 );
		diceNum.Setup ( 1, 1, 10, 1 );
		diceFace.Setup ( 0, 0, 1, 1, diceFaces );
		diceMod.Setup ( 0, -10, 10, 1 );
		diceDam.Setup ( 0, 0, 1, 1, Attack.DamageType );
	}
}
