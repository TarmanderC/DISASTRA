using Unity.VisualScripting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class KnightMovement : MonoBehaviour
{
    public float speed;
    public float collisionOffset;
    public ContactFilter2D movementFilter;
    Vector2 movementInput;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();
    public bool canMove = true;


    public Animator anim;
    public SpriteRenderer sp;
    private Rigidbody2D rb;

    public string direction = "left";



    [SerializeField] private DialogueUI dialogueUI;

    public DialogueUI DialogueUI => dialogueUI;

    [SerializeField] public IInteractable Interactable { get; set; }
    public GameObject cutsceneDialogue;
    public bool interactableHere;

    public BattleManager bm;

    public GameObject interactKey;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void InteractCutscene() {
        Interactable = cutsceneDialogue.GetComponent<DialogueActivator>();
        Interactable?.Interact(this);
    }

    public void setCanMove(bool canMove) {
        this.canMove = canMove;
    }

    void Update() {
        if (dialogueUI.isOpen || bm.isBattleActive || !canMove) {
            
            return;
        }

        if (!cutsceneDialogue.activeSelf && !interactableHere) {
            Interactable = null;
        }

        if (Input.GetKeyDown(KeyCode.E)) {
            if (!cutsceneDialogue.activeSelf) {
                Interactable?.Interact(this);
            }
        }



        float diagMult = 1f;
        if(movementInput.x != 0 && movementInput.y != 0)
        {
            diagMult = 1.414214f;
        }
        if (canMove)
        {
            if (movementInput != Vector2.zero)
            {
                bool success = TryMove(movementInput, diagMult);

                if (!success)
                {
                    success = TryMove(new Vector2(movementInput.x, 0), diagMult);

                    if (!success)
                        success = TryMove(new Vector2(0, movementInput.y), diagMult);
                }

                if (success) {
                    anim.SetFloat("isMoving", 1);
                } else {
                    anim.SetFloat("isMoving", 0);
                }

            }
            else
            {
                anim.SetFloat("isMoving", 0);

            }

            // set direction of sprite to movement direction
            if (movementInput.x < 0)
            {
                sp.flipX = true;
                direction = "left";
                
            }
            else if (movementInput.x > 0)
            {
                sp.flipX = false;
                direction = "right";
            } 
        }


        if(movementInput != Vector2.zero) 
        {
            anim.SetFloat("isMoving", 1);
        }
    }

    private bool TryMove(Vector2 direction, float multiplier)
    {
        int count = rb.Cast(direction, movementFilter, castCollisions, speed * Time.fixedDeltaTime + collisionOffset);

        if (count == 0)
        {
            rb.MovePosition(rb.position + direction * speed * multiplier * Time.fixedDeltaTime);
            return true;
        } else
        {
            return false;
        }
    }

    void OnMove(InputValue moveVal)
    {
        movementInput = moveVal.Get<Vector2>();
    }
    
    void LateUpdate()
    {
        
        if (dialogueUI.isOpen || bm.isBattleActive || !cutsceneDialogue.GetComponent<DialogueActivator>().isActiveAndEnabled) {
            interactKey.SetActive(false);
            
        }
        else if (Interactable != null && !cutsceneDialogue.activeSelf) {
            interactKey.SetActive(true);
        }
        
        if (interactableHere) {
            interactKey.SetActive(true);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (dialogueUI.isOpen || bm.isBattleActive) {
            
            return;
        }
        if (collision.CompareTag("Interactable") && dialogueUI.isOpen == false) {
            interactKey.SetActive(true);
            interactableHere = true;
        }
    }
    
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable")) {
            interactableHere = false;
            interactKey.SetActive(false);
        }
    }
}
