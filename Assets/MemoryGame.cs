using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryGame : MonoBehaviour
{
    private bool firstCardFlipped = false;
    public List<MemoryCards> cards = new List<MemoryCards>();

    private MemoryCards firstSelectedCard = null;
    private MemoryCards secondSelectedCard = null;
    private bool checkingCards = false;
    
    //if first card flip and store it
    //else flip second card, wait 1 second, check match

    void Start()
    {
        foreach (MemoryCards card in cards)
        {
            card.GetMemoryScript(this);
        }
    }

    public void CardSelected(MemoryCards selectedCard)
    {
        if (checkingCards || selectedCard == null)
        {
            return;
        }
        
    }
}
