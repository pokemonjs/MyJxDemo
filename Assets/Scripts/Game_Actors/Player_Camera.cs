using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour {

	private Vector3 offset;
	private Transform player;
	private float spd=5f;

	// Use this for initialization
	void Start () {
		if(player==null){
			player=GameObject.FindGameObjectWithTag("Player").transform;
		}
		offset=transform.position-player.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(player)transform.position=Vector3.Lerp(transform.position,player.position+offset,spd*Time.deltaTime);
	}
}
