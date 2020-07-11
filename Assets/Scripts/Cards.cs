﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    move,
    jump,
    attack,
    protect,
    shuffle,
    recover
}

public enum Direction
{
    up,
    down,
    left,
    right
}

public class Cards : ScriptableObject
{
    public string name;
    public string description;

    public Sprite artwork;
    public CardType cardType;
    public float duration;
    public int power;
    public Direction direction;

}
