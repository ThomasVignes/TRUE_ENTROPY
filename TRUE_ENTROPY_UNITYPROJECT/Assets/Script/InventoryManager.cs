using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public Item[] items;
    [SerializeField] private Transform iconParent;
    [SerializeField] private GameObject iconPrefab;

    public void Init(Item[] items)
    {
        this.items = items;

        foreach (var item in items)
        {
            item.Equipped = false;
        }
    }

    public void EquipItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || item.Equipped)
            return;

        GameObject go = Instantiate(iconPrefab, iconParent);
        go.GetComponent<Image>().sprite = item.Sprite;
        go.GetComponentInChildren<TextMeshProUGUI>().text = item.Name;
        item.Icon = go;
        item.Equipped = true;
    }

    public void RemoveItem(string name)
    {
        Item item = Array.Find(items, e => e.ID == name);

        if (item == null || !item.Equipped)
            return;

        Destroy(item.Icon);
        item.Equipped = false;
    }
}

[System.Serializable]
public class Item
{
    public string ID;
    public string Name;
    public Sprite Sprite;
    [HideInInspector] public bool Equipped;
    [HideInInspector] public GameObject Icon;
}
