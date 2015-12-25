using UnityEngine;
using System.Collections;
using SimpleJSON;

/// <summary>
/// A living, moving thing. Derived from Deco to inherit the Activity subset while adding a set of character stats.
/// </summary>
public class TokenCharacter : TokenDeco 
{
	public CharacterStats stats;

	public TokenCharacter ( CharacterStats s )
	{
		stats = s;
		Category = "Character";
	}

	public override void Interact()
	{
		InfoPanel.Instance.AddCharacterDrawer ( stats );
		base.Interact();
	}

	public override void Infopanel()
	{
		base.Infopanel();
		InfoPanel.Instance.AddCharacterDrawer ( stats );
	}

	public override void PostInit()
	{
		base.PostInit();
		homeTile = WorldMap.Instance.roomManager.FindTileByID ( homeTileID );
		transform.position = homeTile.transform.position;
		FindNewHome();
	}

	public override void Init(JSONNode data, int tokenIndex)
	{
		base.Init(data, tokenIndex);
		stats = new CharacterStats ( data, tokenIndex );
	}

	public override void Export(ref JSONNode data, int tokenIndex)
	{
		base.Export(ref data, tokenIndex);

		stats.Export ( ref data, tokenIndex );
	}
}
