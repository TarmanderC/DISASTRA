using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class TransitionMove : MonoBehaviour
{
    public Animator transition;

    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner confiner;

    public float transitionTime = 1f;

    public Vector3 NewScenePosition;

    private bool playerIsColliding = false;

    public KnightMovement player;

    public GameObject turnOffObject;

    public CutsceneManager cutsceneManager;

    private void Awake() {
        confiner = FindFirstObjectByType<CinemachineConfiner>();
        cutsceneManager = FindFirstObjectByType<CutsceneManager>();
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsColliding = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsColliding = false;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIsColliding)
        {
            doTransition();
        }
    }

    public void doTransition() {
        transition.SetBool("Start", true);

        player.canMove = false;

        Invoke("MovePlayer", 1f);
    }

    public void doTransition(GameObject turnOff) {
        transition.SetBool("Start", true);

        player.canMove = false;

        Invoke("MovePlayerCutscene", 1f);

        turnOffObject = turnOff;
    }

    void MovePlayerCutscene() {
        player.transform.position = NewScenePosition;
        confiner.m_BoundingShape2D = mapBoundry;
        turnOffObject.SetActive(false);

        Invoke("ShowScene", 1f);

        cutsceneManager.PlayCutscene();
    }

    void MovePlayer()
    {
        player.transform.position = NewScenePosition;
        confiner.m_BoundingShape2D = mapBoundry;

        Invoke("ShowScene", 1f);
    }

    void ShowScene()
    {
        transition.SetBool("Start", false);

        player.canMove = true;
    }
    
}