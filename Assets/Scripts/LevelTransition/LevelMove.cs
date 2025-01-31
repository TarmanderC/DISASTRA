using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class LevelMove : MonoBehaviour
{

    public KnightMovement player;

    [SerializeField] PolygonCollider2D mapBoundry;
    CinemachineConfiner confiner;
    [SerializeField] Direction direction;
    enum Direction { Up, Down, Left, Right }
    [SerializeField] int distance = 2;

    private void Awake() {
        confiner = FindFirstObjectByType<CinemachineConfiner>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            confiner.m_BoundingShape2D = mapBoundry;
            UpdatePlayerPosition(other.gameObject);
        }
    }

    private void UpdatePlayerPosition(GameObject player) {
        Vector3 newPos = player.transform.position;

        switch (direction) {
            case Direction.Up:
                newPos.y += distance;
                break;
            case Direction.Down:
                newPos.y -= distance;
                break;
            case Direction.Left:
                newPos.x -= distance;
                break;
            case Direction.Right:
                newPos.x += distance;
                break;
            
        }

        player.transform.position = newPos;
    }
}