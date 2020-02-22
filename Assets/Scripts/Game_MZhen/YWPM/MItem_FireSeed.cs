using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MItem_FireSeed : Game_MItem {

	//炎威破魔阵掉落的火种物体

	//周围的火种
	private List<MItem_FireSeed> neighbors=new List<MItem_FireSeed>();
	//与该火种直接接触的火种
	private List<MItem_FireSeed> triggers=new List<MItem_FireSeed>();
	//火种存在的时间
	private float life_time=90f;
	//销毁时是否触发效果
	private bool flag=true;
	public bool Flag{
		get{return flag;}
		set{flag=value;}
	}
	//火种结合产物的预制体
	private GameObject[] animals=new GameObject[3];
	private int[] animals_num=new int[]{2,4,5};
	// Use this for initialization
	void Start () {
		Destroy(gameObject,life_time);
		triggers.Add(this);
		for(int i=0;i<animals.Length;i++){
			animals[i]=(GameObject)Resources.Load("Prefabs/MZhen/YWPM/ene_"+(i+1));
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnTriggerEnter2D(Collider2D collider){
		MItem_FireSeed m=collider.gameObject.GetComponent<MItem_FireSeed>();
		if(m!=null && !triggers.Contains(m)){
			neighbors.Add(m);
			triggers.Add(m);
		}
	}

	public void OnDestroy(){
		// if(flag && neighbors.Count>0){
		// 	MadeUp();
		// }
		foreach(MItem_FireSeed e in neighbors){
			if(e!=this){
				e.RemoveFromNeibors(this.triggers);
			}
		}
	}

	void MadeUp(){
		for(int i=0;i<neighbors.Count;i++){
			StartCoroutine(neighbors[i].MoveTo(transform.position));
			neighbors[i].Flag=false;
		}
		int num=Toolkit.CompareRange(neighbors.Count+1,animals_num);
		Instantiate(animals[num],transform.position,transform.rotation);
	}

	public IEnumerator MoveTo(Vector3 pos){
		Vector3 dt=(pos-transform.position)/20f;
		for(int i=0;i<20;i++){
			transform.Translate(dt);
			yield return i;
		}
	}

	public void AddToNeibors(List<MItem_FireSeed> list){
		foreach(MItem_FireSeed e in list){
			if(!neighbors.Contains(e)){
				neighbors.Add(e);
			}
		}
	}

	public void RemoveFromNeibors(List<MItem_FireSeed> list){
		foreach(MItem_FireSeed e in list){
			if(e!=this && !triggers.Contains(e)){
				neighbors.Remove(e);
			}
		}
	}
}
