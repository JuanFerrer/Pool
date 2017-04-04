using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FillSlider : MonoBehaviour {

    private Slider powerBar;

	// Use this for initialization
	void Start () {
        powerBar = GetComponentInChildren<Slider> ();
    }
	
	// Update is called once per frame
	void charge(float charge)
    {
        powerBar.value = charge;
    }
}
