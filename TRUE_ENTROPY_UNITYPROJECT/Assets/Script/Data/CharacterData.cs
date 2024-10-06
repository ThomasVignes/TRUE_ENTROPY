using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Character")]
public class CharacterData : ScriptableObject
{
    public string Name;
    public string Nickname;
    public CharacterPronoun Pronoun;
    public CharacterSpecial Special;

    public GameObject ControllerPrefab;
    public Sprite Icon;
    public CharacterPuppet[] CharPuppets;
    
}

[System.Serializable]
public class CharacterPuppet
{
    public string ID;
    public GameObject Prefab;
}

public enum CharacterSpecial
{
    None,
    Shooting,
    Authority,
    Clairvoyance
}

public enum CharacterPronoun
{
    Her,
    Him,
    They
}
