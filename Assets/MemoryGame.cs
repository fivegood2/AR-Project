using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGame : MonoBehaviour
{
    //private bool firstCardFlipped = false;
    public List<MemoryCards> cardsList = new List<MemoryCards>();
public List<MemoryCards> matchedCards = new List<MemoryCards>();

private bool checkingCards = false;
private bool firstFlipDone = false;
private int totalMatchedCards = 0;

public float delayTime;
    //if first card flip and store it
    //else flip second card, wait 1 second, check match

    void Start()
    {
        foreach (MemoryCards card in cardsList)
        {
            card.GetMemoryScript(this);
        }

        StartCoroutine(InitialDelay());
    }

    public void CardSelected(MemoryCards selectedCard)
    {
        if (checkingCards || selectedCard == null)
        {
            return;
        }

        selectedCard.FlipUp();
        matchedCards.Add(selectedCard);

        if (matchedCards.Count == 2)
        {
            StartCoroutine(CheckSelectedCards());
        }
    }

    private IEnumerator InitialDelay()
    {
        yield return new WaitForSeconds(delayTime);
        yield return StartCoroutine(StartingFlip());
    }
    
    private IEnumerator StartingFlip()
    {
        foreach (MemoryCards card in cardsList)
        {
            card.myAnimator.SetTrigger("ReadyToFlipUp");
        }

        yield return new WaitForSeconds(2f);

        foreach (MemoryCards card in cardsList)
        {
            card.myAnimator.SetTrigger("ReadyToFlipDown");
        }

        firstFlipDone = true;
    }

    private IEnumerator CheckSelectedCards()
    {
        checkingCards = true;
        
        yield return new WaitForSeconds(1f);

        //declare these to shorten things so i dont get confused
        MemoryCards firstCard = matchedCards[0];
        MemoryCards secondCard = matchedCards[1];

        if (firstCard.CheckIfMatches(secondCard))
        {
            firstCard.Match();
            secondCard.Match();

            totalMatchedCards += 2;

            if (totalMatchedCards >= cardsList.Count)
            {
                Debug.Log("all carsd matched");
            }
        }
        else
        {
            firstCard.FlipDown();
            secondCard.FlipDown();
        }

        matchedCards.Clear();
        checkingCards = false;
    }
    
    
}
