using Oculus.Interaction;
using UnityEngine;

public class MemoryCards : MonoBehaviour
{
    [Header("Card Match ID")]
    [SerializeField] private int myID;

    private MemoryGame memoryGame;
    private RayInteractable rayInteractable;
    public Animator myAnimator;

    private bool faceUp = false;
    private bool isMatched = false;

    public bool FaceUp
    {
        get { return faceUp; }
    }

    public bool IsMatched
    {
        get { return isMatched; }
    }

    private void Awake()
    {
        if (rayInteractable == null)
        {
            rayInteractable = GetComponentInChildren<RayInteractable>(false);
        }

        if (myAnimator == null)
        {
            myAnimator = GetComponentInChildren<Animator>();
        }

        memoryGame = FindFirstObjectByType<MemoryGame>();
    }

    //get reference to memory game script
    public void GetMemoryScript(MemoryGame script)
    {
        memoryGame = script;
    }

    //STUFF FROM HW 2 BASE SCRIPT
    private void OnEnable()
    {
        if (rayInteractable != null)
        {
            rayInteractable.WhenStateChanged += OnRayStateChanged;
        }
    }

    private void OnDisable()
    {
        if (rayInteractable != null)
        {
            rayInteractable.WhenStateChanged -= OnRayStateChanged;
        }
    }

    private void OnRayStateChanged(InteractableStateChangeArgs args)
    {
        if (args.NewState == InteractableState.Hover)
        {
            print("hovering");
        }
        else if (args.NewState == InteractableState.Select)
        {
            print("selecting");
            SelectThisCard();
        }
        else
        {
            //idk
        }
    }

    private void SelectThisCard()
    {
        if (isMatched || faceUp)
        {
            return;
        }

        if (memoryGame != null)
        {
            memoryGame.CardSelected(this);
        }
    }

    public void FlipUp()
    {
        faceUp = true;

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("ReadyToFlipUp");
        }
    }

    public void FlipDown()
    {
        faceUp = false;

        if (myAnimator != null)
        {
            myAnimator.SetTrigger("ReadyToFlipDown");
        }
    }

    public bool CheckIfMatches(MemoryCards otherCard)
    {
        if (otherCard == null)
        {
            return false;
        }

        return myID == otherCard.myID;
    }

    public void Match()
    {
        isMatched = true;
        faceUp = true;
    }
    //leave face up once matched
}
