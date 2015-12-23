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
}
