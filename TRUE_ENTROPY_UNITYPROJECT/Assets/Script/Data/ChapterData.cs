using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Chapter")]
public class ChapterData : ScriptableObject
{
    [Header("General")]
    public string Name;
    public int Number;
    public CharacterData StartCharacter;

    [Header("Specific")]
    public List<Conditions> conditions = new List<Conditions>();
    public List<Area> areas = new List<Area>();

    [Header("Inventory")]
    public Item[] items;
}
