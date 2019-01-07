using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public UnityEngine.UI.Text scoreLabel;
    public GameObject winnerLabelObject;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        int count = GameObject.FindGameObjectsWithTag("Item").Length;
        scoreLabel.text = count.ToString();

        if(count == 0)
        {
            winnerLabelObject.SetActive(true);
        }
	}
}
