#define SINGLETON_LAZYINIT
using UnityEngine;
using System.Collections;

// A Lazy Singleton that can be created from a prefab if the static method exists.
public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	// A singleton (lazy) can only be created once.  This bool ensures that rule is enforced.
	// If a singleton GetInstance() is called when the game shutsdown, this can cause an issue
	protected static bool _inited = false; 
	protected static T _instance = default(T);
	protected static bool _error = false;
	
	/* To Load from a prefab, declare this
	public static string GetSingletonPrefab()
	{
		return "path/To/Prefab";
	}
	*/
	
	public static T Instance
	{
		private set {}
		get { return GetInstance(); }
	}
	
	public static T GetInstance() 
	{
		#if SINGLETON_LAZYINIT
		if ( _instance == null )
		{
			if ( !Application.isPlaying ) 
			{
				//if the game is not playing do not make an instance.
				return null;
			}
			if (!_inited)
			{
				_instance = CreateInstance();
			}
			else 
			{
				if ( !_error )
				{
					_error = true; //only warn once.
					Debug.LogWarning("Singleton cannot be created after it has been destroyed");
				}
				#if UNITY_EDITOR
				//_instance = CreateInstance();  // we cannot leave this in because people ignore it and don't fix the root of the problem.
				#endif
			}
		}
		#endif
		return _instance;
	}
	
	public static void RequestInstance()
	{
		if ( _instance == null )
		{			 
			_instance = CreateInstance();
		}
	}
	
	void OnApplicationQuit() 
	{
		if ( _instance != null )
		{			 
			GameObject.Destroy(this);
			_instance = null;
		}
	}
	
	protected static T CreateInstance()
	{
		//first try to get singleton from a prefab
		System.Type type = typeof(T);
		System.Reflection.MethodInfo mi = type.GetMethod("GetSingletonPrefab");
		if (mi != null)
		{
			string prefab = (string)mi.Invoke(null,null);
			if ( prefab != "" )
			{
				GameObject template = Resources.Load(prefab) as GameObject;
				if ( template != null )
				{
					GameObject objFromInstance = GameObject.Instantiate(template) as GameObject;
					T comp = objFromInstance.GetComponent<T>();
					if ( comp != default(T) )
					{	
						objFromInstance.name = "_" + comp.GetType().Name;
						_instance = comp;
						return _instance;
					}
					
					//After loading the prefab and not finding the Singleton component we try searching in the child objects.
					comp = objFromInstance.GetComponentInChildren<T>();
					if ( comp != default(T) )
					{	
						objFromInstance.name = "_" + comp.GetType().Name;
						_instance = comp;
						return _instance;
					}
				}
			}
		}
		
		//if no prefab, just create the object
		GameObject obj = new GameObject();
		if ( obj != null )
		{
			T comp = obj.AddComponent<T>();
			if ( comp != null )
			{
				GameObject.DontDestroyOnLoad(obj);
				obj.name = "_" + comp.GetType().Name;
				_instance = comp;
				return _instance;
			}
		}
		
		//failed
		Debug.LogError("Could not create singleton!");
		return null;
	}
	
	
	protected virtual bool IsPersistent()
	{
//		return true;
		// HACK! 
		return false;
	}
	
	protected virtual void Awake()
	{
		if ( !_inited )
		{
			if ( _instance == null ) 
			{
				_instance = this as T;
			}
			if ( IsPersistent() )
			{
				GameObject.DontDestroyOnLoad(gameObject);
			}
			_inited = true;
		}
		else
		{
			Debug.LogWarning("Trying to create singleton that has already been created:" + name);
			GameObject.Destroy(this);
		}
	}
	
	protected virtual void OnDestroy()
	{
		
		if ( _instance == this ) 
		{
			if ( !IsPersistent() )
			{
				_inited = false;
			}
			_instance = null;
		}
	}
}