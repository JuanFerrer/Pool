using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftTextScript : MonoBehaviour {

    private Text Left;

    public void MyText(string Player)
    {
        Left = GetComponentInChildren<Text>();
        Left.text = Player;
    }
	
	
}
