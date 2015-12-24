using UnityEngine;
using System.Collections;

/// <summary>
/// A living, moving thing. Derived from Deco to inherit the Activity subset while adding a set of character stats.
/// </summary>
public class TokenCharacter : TokenDeco 
{
	public CharacterStats stats;

	public TokenCharacter ( CharacterStats s )
	{
		stats = s;
	}

	public override void Interact()
	{
		map.infoPanel.AddCharacterDrawer ( stats );
		base.Interact();
	}

	public override void Infopanel()
	{
		base.Infopanel();
		stats.AddAttack ( new Attack ( "Sword", "Dx", 5, Attack.AttackType.Melee, 1, 1, 6, 2, "slashing" ) );
		stats.AddAttack ( new Attack ( "Magic Missile", "In", 120, Attack.AttackType.Ranged, 1, 1, 4, 1, "radiant"));
		map.infoPanel.AddCharacterDrawer ( stats );

	}
}
