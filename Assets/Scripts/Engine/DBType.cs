using UnityEngine;
using System.Collections;
using SimpleJSON;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Baseclass for all database entries
/// </summary>
[System.Serializable]
public class DBType
{
//	public string Name="Unnamed";
//	public string Description="Undescribed";
//	public ArchiveGUID GUID=null;
//	public string ID="";
//
//	string m_iconString = "Textures/Icons/_generic";
//
//	/// <summary>
//	/// Save off the path information for the icon so it can be reloaded at runtime
//	/// </summary>
//	/// <value>The icon.</value>
//	public Texture2D Icon
//	{
//		get
//		{
//			if ( m_iconString == "" )
//			{
//				//Debug.LogError ( "No icon string for "+Name );
//				return null;
//			}
//
//			// Load from resources
//			Texture2D t = Resources.Load ( m_iconString ) as Texture2D;
//			return t;
//		}
//
//		set
//		{
//			// Remove the initial path and extension since it isn't used by Resource.Load
//#if UNITY_EDITOR
//			m_iconString = AssetDatabase.GetAssetPath ( value ).Replace ( "Assets/Resources/", "" ).Replace ( ".png", "" ).Replace ( ".PNG", "" );
//#endif
//		}
//	}
//
//	public string m_iconColor="";
//	public Color color
//	{
//		get
//		{
//			return Helpers.GetColor ( m_iconColor );
//		}
//		set
//		{
//			m_iconColor = value.ToString();
//		}
//	}
//
//	/// <summary>
//	/// Current version
//	/// </summary>
//	public int CurrentVersion = 3;
//
//	/// <summary>
//	/// Version at which this entry was saved
//	/// </summary>
//	public int SavedVersion = -1;
//
//	/// <summary>
//	/// Game operates in three distinct Tiers
//	/// </summary>
//	public int Tier=0;
//
//	/// <summary>
//	/// Variations within the current tier
//	/// </summary>
//	public int Level=0;
//
//	/// <summary>
//	/// How many are currently in inventory? Note that this value doesn't save to the database.
//	/// It's calculated during play
//	/// </summary>
//	public int AmountOwned=0;
//	
//	/// <summary>
//	/// How many have been used in crafting? Note that this value doesn't save to the database.
//	/// It's calculated during play
//	/// </summary>
//	public int AmountSpent=0;
//	
//	/// <summary>
//	/// Prefix for Data entry
//	/// </summary>
//	public string Category = "";
//
//	// Master version number of database saved
//	static public int kVersionNumber = 5;
//
//	// Editor only
//	/// <summary>
//	/// For interacting with the editor's dropdown menu
//	/// </summary>
//	public int DBIndex = -1;
//
//	public int m_cost = -1;
//
//	/// <summary>
//	/// Auto-calculated based on tier/level or accumulated cost of components
//	/// </summary>
//	public virtual int CalcCost()
//	{
//		if ( m_cost != -1 ) { return m_cost; }
//
//		int mags = 1;
//		int levs = 0;
//		for ( int i=0;i<Tier;i++ )
//		{
//			// Order of magnitude per tier
//			mags *= 10;
//			// Level costs also apply scale based on tier
//			levs += Level * 10 * (i+1);
//		}
//		int cost = mags + levs;
//
//		m_cost = cost;
//		return cost;
//	}
//
//	public void RecalculateCost()
//	{
//		// Simply reset value so it recalculates next time
//		m_cost = -1;
//	}
//
//	// Editor-only
//	[HideInInspector]
//	public bool ShowDetail=false;
//
//	public virtual string GetRequirements()
//	{
//		// Not all db types have requirements
//		return "";
//	}
//
//	public virtual bool CanCraft()
//	{
//		// Base class always can craft. Subclasses have different rules
//		return true;
//	}
//
//	public virtual void PostInit ( ref JSONNode data, int i, int version )
//	{
//		// For cross-referencing, some stats aren't imported until all databases have been initially imported
//
//	}
//
//	public virtual void Init ( ref JSONNode data, int i, int version )
//	{
//		Name = data [ Category ] [ i ] [ "name" ];
//		Description = data [ Category ] [ i ] [ "desc" ];
//		Tier = data [ Category ] [ i ] [ "tier" ].AsInt;
//		Level = data [ Category ] [ i ] [ "level" ].AsInt;
//
//		ID = data [ Category ] [ i ] [ "id" ];
//		if ( string.IsNullOrEmpty(ID)) { ID = Category.Substring(0,1).ToLower()+"_"+Tier+"-"+Level;}
//
//		SavedVersion = data [ Category ] [ i ] [ "version" ].AsInt;
//		if ( SavedVersion > 0 )
//		{
//			// There should be an icon beyond the first version
//			m_iconString = data [ Category ] [ i ] [ "icon" ];
//			
//		}
//
//		m_iconColor = data [ Category ] [ i ] [ "iconColor" ];
//		if ( string.IsNullOrEmpty ( m_iconColor ) ) { m_iconColor = "RGBA(1.000, 1.000, 1.000, 1.000)"; }
//
//		string guid = data [ Category ] [ i ] [ "guid" ];
//		if ( string.IsNullOrEmpty ( guid ) )
//		{
//			GUID = new ArchiveGUID();
//		}
//		else
//		{
//			GUID = new ArchiveGUID ( guid );
//		}
//
//		DBIndex = i;
//	}
//
//	public virtual void Export ( ref JSONNode data, int i )
//	{
//		data [ Category ] [ i ] [ "name" ] = Name;
//		data [ Category ] [ i ] [ "desc" ] = Description;
//		data [ Category ] [ i ] [ "id" ] = ID;
//		data [ Category ] [ i ] [ "tier" ].AsInt = Tier;
//		data [ Category ] [ i ] [ "level" ].AsInt = Level;
//		data [ Category ] [ i ] [ "guid" ] = GUID._guidString;
//		data [ Category ] [ i ] [ "icon" ] = m_iconString;
//		data [ Category ] [ i ] [ "iconColor" ] = m_iconColor;
//		data [ Category ] [ i ] [ "version" ].AsInt = CurrentVersion;
//	}
}
