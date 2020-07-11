using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameManager : MonoBehaviour
{
    //public Text[] guides;
    public MainController mainChar;
    public Image deckImage;
    public Image placeholder;
    public Text deckNumber;

    public Cards[] kindOfCards;

    public int maxDeckSize;
    public int maxHandSize;



    private List<Cards> deck;
    private List<Cards> hand;
    private int currentDeckSize = 0;
    private List<bool> availability;
    private List<bool> isUsing;
    private List<bool> activity;
    private List<float> activeTime;

    private void Start()
    {
        mainChar = GameObject.Find("Player").GetComponent<MainController>();
        deck = new List<Cards>();
        hand = new List<Cards>(maxHandSize);
    }

    public void initDeck()
    {
        List<Cards> movement = new List<Cards>();
        List<Cards> jump = new List<Cards>();
        List<Cards> other = new List<Cards>();
        for (int i = 0; i < kindOfCards.Length; ++i)
        {
            switch (kindOfCards[i].cardType)
            {
                case CardType.move:
                    movement.Add(kindOfCards[i]);
                    break;
                case CardType.jump:
                    jump.Add(kindOfCards[i]);
                    break;
                default:
                    other.Add(kindOfCards[i]);
                    break;
            }
        }
        while (deck.Count < maxDeckSize / 2)
        {
            int randMove = Random.Range(0, movement.Count);
            deck.Add(movement[randMove]);
            currentDeckSize++;
        }
        while (deck.Count < maxDeckSize / 3)
        {
            int randJump = Random.Range(0, jump.Count);
            deck.Add(jump[randJump]);
            currentDeckSize++;
        }
        while (deck.Count < maxDeckSize)
        {
            int randOther = Random.Range(0, other.Count);
            deck.Add(other[randOther]);
            currentDeckSize++;
        }
        availability = new List<bool>(maxHandSize);
        activeTime = new List<float>(maxHandSize);
        activity = new List<bool>(maxHandSize);
        for (int i = 0; i < maxHandSize; ++i)
        {
            availability[i] = false;
            activeTime[i] = 0f;
            activity[i] = false;
        }
    }

    public void drawNewCard(int slot)
    {
        if (availability[slot]) return;
        Cards card = deck[currentDeckSize];
        hand[slot] = card;
        currentDeckSize--;
        Image img = GameObject.Find("card" + slot.ToString()).GetComponent<Image>();
        img.sprite = card.artwork;
        img.GetComponent<Button_UI>().MouseOverOnceFunc = () => Tooltip.ShowTooltip_Static(card.name + ": " + card.description);
        img.GetComponent<Button_UI>().MouseOutOnceFunc = () => Tooltip.HideTooltip_Static();
        availability[slot] = true;
        activity[slot] = false;

    }

    public void useCard(int slot)
    {
        if (!availability[slot]) return null;
        else if (!activity[slot])
        {
            switch (hand[slot].cardType)
            {
                case CardType.attack:
                    mainChar.triggerAttack(hand[slot].direction, hand[slot].power);
                    break;
                case CardType.jump:
                    mainChar.triggerJump(hand[slot].power);
                    break;
                case CardType.move:
                    mainChar.triggerMove(hand[slot].direction, hand[slot].power);
                    break;
                case CardType.protect:
                case CardType.shuffle:
                    shuffleDeck();
                    break;
                case CardType.recover:
                default:
                    break;
            }
            yield return new WaitForSeconds(hand[slot].duration);
            switch (hand[slot].cardType)
            {
                case CardType.attack:
                    mainChar.isAttacking = false;
                    break;
                case CardType.jump:
                    mainChar.isJumping = false;
                    break;
                case CardType.move:
                    mainChar.isMovingLeft = mainChar.isMovingRight = false;
                    break;
                case CardType.protect:
                    mainChar.isProtecting = false;
                    break;
                case CardType.shuffle:
                    break;
                case CardType.recover:
                default:
                    break;
            }
        }
    }

    public void removeCardFromHand (int slot)
    {
        if (!availability[slot]) return;
        Image img = GameObject.Find("card" + slot.ToString()).GetComponent<Image>();
        img.color = new Color(0, 0, 0, 1);
        img.GetComponent<Button_UI>().MouseOverOnceFunc = () => Tooltip.ShowTooltip_Static("Empty");
        img.GetComponent<Button_UI>().MouseOutOnceFunc = () => Tooltip.HideTooltip_Static();
        availability[slot] = false;
        if (currentDeckSize > 0) drawNewCard(slot);
    }

    public void shuffleDeck()
    {
        
    }

    public void pointToGoal()
    {

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {

        }
        else if (Input.GetKeyDown(KeyCode.X))
        {

        }
        else if (Input.GetKeyDown(KeyCode.C))
        {

        }
        else if (Input.GetKeyDown(KeyCode.A))
        {

        }
        else if (Input.GetKeyDown(KeyCode.S))
        {

        }
        else if (Input.GetKeyDown(KeyCode.D))
        {

        }
    }

}
