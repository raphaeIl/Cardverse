using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameSetting", menuName = "Game/Game Setting")]
public class GameSetting : ScriptableObject {

    [Header("Controls")]
    public bool AutoHideCursor; // when not in play mode

    public Serializedictionary<ControlType, KeyCode> Controls;

    [Header("Video")]
    public FullScreenMode FullScreenMode;
    
    public int ResolutionIndex;
    [SerializeField] private Vector2 Resolution;

    [Header("Audio")]
    [Range(0, 1)] public float MasterVolume;
    [Range(0, 1)] public float MusicVolume;
    [Range(0, 1)] public float SFXVolume;

    [Header("Graphics")]
    public bool Bloom;

    public void OnValidate() {
        ResolutionIndex = Mathf.Clamp(ResolutionIndex, 0, Screen.resolutions.Length - 1);
        Resolution = new Vector2(Screen.resolutions[ResolutionIndex].width, Screen.resolutions[ResolutionIndex].height);
    }
}


[System.Serializable]
public class Serializedictionary<K, V> {

    [SerializeField] public List<SerializedictionaryWrapper> serializedictionary;

    public V this[K key] {
        get { return serializedictionary.Where(serializedictionary => serializedictionary.Key.Equals(key)).Single().Value; }
    }

    [System.Serializable]
    public class SerializedictionaryWrapper {
        public K Key;
        public V Value;
    }
}

public enum SettingTabType {
    Controls,
    Video,
    Audio,
    Graphics
}

public enum ControlType {
    Move_Up,
    Move_Down,
    Move_Left,
    Move_Right,
    Sprint,
    Interact,
    Show_Cursor,
    Character_Menu,
    Equipment_Panel,
    Inventory
}
