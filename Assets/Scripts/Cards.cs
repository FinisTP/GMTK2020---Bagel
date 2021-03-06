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
    recover,
    nullity,
    dash
}

public enum Direction
{
    up,
    down,
    left,
    right
}

[System.Serializable]
public class Specification
{
    public Cards card;
    public int amount;
}

[CreateAssetMenu(fileName = "New Card")]
public class Cards : ScriptableObject
{
    public string name;
    public string description;
    public string chatLog;

    public Sprite artwork;
    public CardType cardType;
    public float duration;
    public int power;
    public Direction direction;
    public int amount;

}
