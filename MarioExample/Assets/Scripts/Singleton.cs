using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    [Header("Singleton Settings")]
    [SerializeField] private bool _persist = false;

    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject newInstance = new GameObject($"{_instance.GetComponent<T>().ToString()} Instance");
                    _instance = newInstance.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this as T;

        if (_persist) DontDestroyOnLoad(this);
    }
}