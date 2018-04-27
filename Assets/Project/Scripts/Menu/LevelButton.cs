using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour {

    public Text label;

	// Use this for initialization
	void Start () {
        label.text = "Level " + (GetComponent<LevelSettings>().id +1);
	}
	
}
