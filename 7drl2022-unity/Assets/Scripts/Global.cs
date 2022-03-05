using System;
using System.Collections.Generic;
using UnityEngine;

public class Global : MonoBehaviour {

    private static Global instance;

    public int InstanceID { get; private set; }
    public static bool IsDestructing { get; private set; }

    [SerializeField] private List<SingletonBehavior> serializedManagers = null;

    public InputManager Input => Get<InputManager>();
    public MapManager Maps => Get<MapManager>();
    public AudioManager Audio => Get<AudioManager>();
    public SerializationManager Serialization => Get<SerializationManager>();

    public GameData Data { get; private set; } = new GameData();

    private Dictionary<Type, SingletonBehavior> singletons = new Dictionary<Type, SingletonBehavior>();

    private UIAnchorPoint ui;
    public UIAnchorPoint UI {
        get {
            if (ui == null) {
                ui = FindObjectOfType<UIAnchorPoint>();
            }
            return ui;
        }
    }

    public static Global Instance() {
        if (instance == null) {
            instance = FindObjectOfType<Global>();
            if (instance == null) {
                instance = Instantiate(Resources.Load<Global>("Prefabs/Global"));
                instance.ReadSerializedManagers();
            }
        }
        return instance;
    }

    public void Awake() {
        if (instance == null) {
            instance = this;
        }
        instance.InstanceID = UnityEngine.Random.Range(0, int.MaxValue);
        instance.ReadSerializedManagers();
        DontDestroyOnLoad(gameObject);
        MoonSharp.Interpreter.UserData.RegisterAssembly();
    }

    public T Get<T>() where T : SingletonBehavior {
        var type = typeof(T);
        if (!singletons.ContainsKey(type)) {
            var instance = gameObject.AddComponent<T>();
            singletons[type] = instance;
        }
        return (T)singletons[type];
    }

    public void OnDestroy() {
        IsDestructing = true;
    }
    
    private void ReadSerializedManagers() {
        foreach (var manager in serializedManagers) {
            singletons[manager.GetType()] = manager;
        }
    }
}
