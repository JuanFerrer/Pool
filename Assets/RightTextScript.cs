using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightTextScript : MonoBehaviour {

    private Text Right;

    public void MyText(string Player)
    {
        Right = GetComponentInChildren<Text>();
        Right.text = Player;
    }
}
