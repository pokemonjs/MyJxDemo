using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Data : MonoBehaviour {

	//数据库类
	public static Data_Enemy[] Enemies;
	public static Data_Skill[] Skills;
	public static Data_State[] States;
	public static Data_Class[] Classes;

	//常用属性
	protected static int[] scope_self=new int[]{3,4,5,6};

	//常用方法
	public static bool ScopeSelf(int val){
		return Array.IndexOf(scope_self,val)>=0;
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
