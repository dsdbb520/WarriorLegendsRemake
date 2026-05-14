using UnityEngine;

/// <summary>
/// Generic MonoBehaviour singleton base. Subclasses that need DontDestroyOnLoad
/// should call it explicitly in their own Awake after base.Awake().
/// </summary>
public abstract class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance => _instance;

    protected virtual void Awake()
    {
        if (_instance == null)
            _instance = this as T;
        else if (_instance != this)
            Destroy(gameObject);
    }

    protected virtual void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
