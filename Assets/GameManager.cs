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
    public Cards placeholder;
    public Text deckNumber;
    public Transform destination;

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
        initDeck();
        Tooltip.HideTooltip_Static();
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
        while (deck.Count < 3 * maxDeckSize / 4 )
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
        availability = new List<bool>();
        activeTime = new List<float>();
        activity = new List<bool>();
        for (int i = 0; i < maxHandSize; ++i)
        {
            availability.Add(false);
            activeTime.Add(0f);
            activity.Add(false);
            hand.Add(placeholder);
        }
        shuffleDeck();
        for (int i = 0; i < maxHandSize; ++i) drawNewCard(i);
        
    }

    public void drawNewCard(int slot)
    {
        if (availability[slot] || currentDeckSize <= 0) return;
        Cards card = deck[currentDeckSize - 1];
        deck.Remove(deck[currentDeckSize - 1]);
        hand[slot] = card;
        currentDeckSize--;
        //Debug.Log(currentDeckSize);
        Image img = GameObject.Find("card" + (slot+1).ToString()).GetComponent<Image>();
        img.sprite = card.artwork;
        //img.GetComponent<Button_UI>().hoverBehaviour_Image = card.artwork;
        img.GetComponent<Button_UI>().MouseOverOnceTooltipFunc = () => Tooltip.ShowTooltip_Static(card.name + ": " + card.description);
        img.GetComponent<Button_UI>().MouseOutOnceTooltipFunc = () => Tooltip.HideTooltip_Static();
        LogSystem.SendMessageToChat_Static("Drew '" + card.name + "'.");
        availability[slot] = true;
        activity[slot] = false;
    }

    public void useCard(int slot)
    {
        if (!availability[slot]) return;
        else if (!activity[slot] && activeTime[slot] <= hand[slot].duration)
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
                    mainChar.isProtecting = true;
                    break;
                case CardType.shuffle:
                    shuffleDeck();
                    break;
                case CardType.recover:
                    mainChar.takeDamage(-hand[slot].power);
                    break;
                default:
                    break;
            }
            LogSystem.SendMessageToChat_Static("Used '" + hand[slot].name + "' - activated " + hand[slot].chatLog + ".");
            activity[slot] = true;
        }
        else if (activeTime[slot] <= hand[slot].duration)
        {
            deactivateCard(slot);
        }
    }

    public void deactivateCard(int slot)
    {
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
                break;
            default:
                break;
        }

        LogSystem.SendMessageToChat_Static("Negated '" + hand[slot].name + "' - stopped" + hand[slot].chatLog + ".");

        activity[slot] = false;
        //Debug.Log("Deactivated a card");
    }

    public void removeCardFromHand(int slot)
    {
        if (!availability[slot]) return;
        if (activity[slot]) deactivateCard(slot);
        Image img = GameObject.Find("card" + (slot+1).ToString()).GetComponent<Image>();
        Debug.Log("card" + slot.ToString());
        img.sprite = placeholder.artwork;
        
        img.GetComponent<Button_UI>().MouseOverOnceFunc = () => Tooltip.ShowTooltip_Static("Empty");
        img.GetComponent<Button_UI>().MouseOutOnceFunc = () => Tooltip.HideTooltip_Static();
        
        availability[slot] = false;
        activeTime[slot] = 0;
        LogSystem.SendMessageToChat_Static("'" + hand[slot].name + "' is out of control.");
        if (currentDeckSize > 0) drawNewCard(slot);
    }

    public void shuffleDeck()
    {
        List<Cards> newList = new List<Cards>();
        for (int i = 0; i < currentDeckSize; ++i)
        {
            newList.Add(deck[i]);
        }
        for (int i = 0; i < newList.Count/2; i++)
        {
            Cards temp = newList[i];
            int randomIndex = Random.Range(i, newList.Count);
            newList[i] = newList[randomIndex];
            newList[randomIndex] = temp;
        }
        deck.Clear();
        deck = new List<Cards>(newList);
        LogSystem.SendMessageToChat_Static("The deck has been shuffled.");
    }

    public void addToDeck(Cards other)
    {
        deck.Add(other);
        LogSystem.SendMessageToChat_Static("'" + other.name + "' has been added to the deck.");
    }

    public bool checkHand()
    {
        for (int i = 0; i < maxHandSize; ++i)
        {
            if (availability[i]) return true;
        }
        return false;
    }

    public void timer()
    {
        for (int i = 0; i < maxHandSize; ++i)
        {
            GameObject.Find("card" + (i + 1).ToString()).GetComponent<Image>().fillAmount = 
                Mathf.Clamp(1 - activeTime[i] / hand[i].duration, 0f, 1f);
            if (availability[i])
            {
                if (activity[i])
                activeTime[i] += Time.deltaTime;
                if (activeTime[i] >= hand[i].duration) removeCardFromHand(i);
            }
        }
    }

    private void Update()
    {
        
        timer();
        deckNumber.text = currentDeckSize.ToString() + "/" + maxDeckSize.ToString();
        if (currentDeckSize == 0 && !checkHand()) mainChar.isGameOver = true;

        if (Input.GetKeyDown(KeyCode.Z))
        {
            useCard(0);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            useCard(1);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            useCard(2);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            useCard(3);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            useCard(4);
        }
    }

}
