using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public List<PlayableDirector> cutscenes = new List<PlayableDirector>();
    public KnightMovement knightMovement;
    public int currentIndex = 0;

    public int currentPriority = 0;

    public CinemachineVirtualCamera mainCamera;

    public void PlayCutscene()
    {
        cutscenes[currentIndex].Play();
        knightMovement.canMove = false;
        currentIndex++;
    }

    public void PlayCutscene(int index)
    {
        cutscenes[index].Play();
        knightMovement.canMove = false;
    }

    public void SetCameraNumber(int priority)
    {
        currentPriority = priority;
    }

    public void SetCameraCollider(PolygonCollider2D collider) {
        mainCamera.GetComponent<CinemachineConfiner>().m_BoundingShape2D = collider;
    }

    public void SetCameraPriority(CinemachineVirtualCamera cam)
    {
        if (cam != null)
        {
            cam.Priority = currentPriority;
        }
    }
}
