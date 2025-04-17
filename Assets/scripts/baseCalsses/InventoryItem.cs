using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class InventoryItem : MonoBehaviour
{
    public abstract string getInventoryName();

    public abstract Sprite getInventorySprite();
}
