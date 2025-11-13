using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchRotate : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnMouseDown() 
    {
        // CHANGED LINE ↓ (add !PuzzleControl.timeUp)
        if (!PuzzleControl.youWin && !PuzzleControl.timeUp)
        {
            transform.Rotate(0f, 0f, 90f);
        }
    }
}


