using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MZhen_YWPM : Game_MZhen {

	//炎威破魔阵

	//被击中掉落火种的几率
	private int rate=10;
	public int Rate{
		get{ return rate; }
		set{ rate=value; }
	}
	//被击中掉落的火种预制体
	private GameObject fire_seed;

	public void Func_After_Defend(Game_Battler b){
		if(this.rate>=Random.Range(0,50)){
			Instantiate(fire_seed,transform.position,transform.rotation);
		}
	}

	public void Func_Spe_Eff(Game_Skill s){
		//测试用待修改
		s.SKD.Atkf+=100f;
	}

	public void Func_Spe_Pre_Move(Game_Skill s){
		//测试用待修改
		s.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(1,1));
	}

	// Use this for initialization
	void Start () {
		fire_seed=(GameObject)Resources.Load("Prefabs/MZhen/YWPM/fire_seed");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public new void ApplyInBattler(Game_Battler[] gbs){
		for(int i=0;i<gbs.Length;i++){
			gbs[i].After_Defend+=Func_After_Defend;
			//测试用待修改
			gbs[i].Spe_Eff+=Func_Spe_Eff;
			gbs[i].Spe_Pre_Move+=Func_Spe_Pre_Move;
		}
	}
}
