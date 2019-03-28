using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : ScriptableObject
{
    public int health;
    public string name;
    public int strength;
    public Item item1 = Item.Empty;
    public Item item2 = Item.Empty;
    public Item itemTemp = Item.Empty;

    public void InitializePlayer(string name, int hp, int str)
    {
        this.name = name;
        health = hp;
        strength = str;
    }

    public int CalculateDamage()
    {
        return strength;
    }

    public void AddItem(Item newItem)
    {
        if (item1 == Item.Empty)
        {
            item1 = newItem;
        }
        else if (item2 == Item.Empty)
        {
            item2 = newItem;
        }
    }

    public bool inventoryFull()
    {
        if (item1 == Item.Empty)
        {
            return false;
        }
        else if (item2 == Item.Empty)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void RemoveItem(int itemNum)
    {
        if (itemNum == 1)
        {
            if (item2 == Item.Empty)
            {
                item2 = itemTemp;
            }

            item1 = item2;
            item2 = itemTemp;
            itemTemp = Item.Empty;
        }
        else if (itemNum == 2)
        {
            item2 = itemTemp;
            itemTemp = Item.Empty;
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
    }
}
