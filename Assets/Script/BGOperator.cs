using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGOperator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseDown(){
		Vector3 pos;
		if (Input.touchCount > 0) 
		{
			pos = Input.GetTouch(0).position;
		} 
		else 
		{
			pos = Input.mousePosition;
		}
		GameController.gameController.OnClickGrid (Camera.main.ScreenToWorldPoint(pos));
	}
}
