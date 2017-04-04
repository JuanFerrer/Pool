using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseGameScript : MonoBehaviour {

    private Canvas CanvasObject; // Assign in inspector

    void Start()
    {
        CanvasObject = GetComponent<Canvas>();
        CanvasObject.enabled = false;

        StartCoroutine(PauseCoroutine());
    }

    IEnumerator PauseCoroutine()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CanvasObject.enabled = !CanvasObject.enabled;
                if (Time.timeScale == 0)
                {
                    Time.timeScale = 1;
                }
                else {
                    Time.timeScale = 0;
                }
            }
            yield return null;
        }
    }
}
