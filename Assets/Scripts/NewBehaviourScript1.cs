using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript1 : MonoBehaviour {

    int ballsIn = 0;
    int ballNum = 1;
    

	// Update is called once per frame
	void Update ()
    {
		if (Input.GetKeyDown("o"))
        {
            DeleteBall(ballNum);
            ballsIn += 25;
            ballNum++;
        }
	}

    void DeleteBall(int ballChosen)
    {
        
        
    }
}
