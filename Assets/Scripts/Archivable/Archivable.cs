using UnityEngine;
using System.Collections;
using SimpleJSON;

public class Archivable : MonoBehaviour
{
	public string Category = "SubclassMustDefine";

	public string Name;

	public int ID = 0;

	public virtual void SetupArchive()
	{
		// Each subclass uses this init to set Category
		ID = SimpleID.NewID;
	}

	/// <summary>
	/// Unique ID is always concatenated from the archived int ID and some other detail of the derived class
	/// </summary>
	public virtual string UniqueID()
	{
		string uID = "Each derived class returns unique ID";
		return uID;
	}


	public virtual void PostInit()
	{
		// For cross-referencing, some stats aren't imported until all databases have been initially imported

	}

	public virtual void Init ( ref JSONNode data, int i )
	{
		if ( Category == "SubclassMustDefine")
		{
			Debug.LogError ( "Category not defined!");
			return;
		}
		Name = data [ Category ] [ i ] [ "name" ];
		ID = data [ Category ] [ i ] [ "ID" ].AsInt;
		SimpleID.ValidateID ( ID );
	}

	public virtual void Export ( ref JSONNode data, int i )
	{
		if ( Category == "SubclassMustDefine" )
		{
			Debug.LogError ( "Category undefined for "+Name );
			return;
		}
		data [ Category ] [ i ] [ "name" ] = Name;
		data [ Category ] [ i ] [ "ID" ].AsInt = ID;
	}
}

