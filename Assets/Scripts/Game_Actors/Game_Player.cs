using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Player : Game_Battler {

	//当前武器（普通攻击技能）id
	private int atk_id=0;
	//位移的速度（倍率）
	private float displaceRate=10f;
	//速度
	private Vector2 velocity;

    //初始化函数
    new void Init(){
		Data_Actor da=Toolkit.ReadJson<Data_Actor>("DataSetTXT/Actor",this.id);
		this.mname=da.mname;
		this.ability=da.ability;
		for(int i=0;i<da.timer_name.Length;i++){
			SetTimer(da.timer_name[i],da.timer_val[i]);
		}
	}
	//获取玩家输入
	void GetInput(){
		//右键切换攻击属性
		if(Input.GetMouseButtonUp(1)){
			ChangeElementSet();
		}
		//左键攻击指定方向
		if(Input.GetMouseButton(0)){
			Attack(Input.mousePosition);
		}
		//移动
		float h=Input.GetAxis("Horizontal");
		float v=Input.GetAxis("Vertical");
		Move(h,v);
		//位移
		if(Input.GetKeyDown(KeyCode.Space)){
			Displacement(h,v);
		}
	}
	//更改攻击属性
	void ChangeElementSet(){
		this.ElementSet+=1;
	}
	//攻击指定方向
	void Attack(Vector3 input){
		if(GetTimer("atk")>0)return;
		Vector3 worldPos=Toolkit.ScreenToWorldPoint(input);
		GameObject n=CreateSkill(this.atk_id);
		LetLookAt(n.transform,worldPos);
	}
	//移动
	void Move(float h,float v,float z=0f){
		this.velocity=new Vector2(h,v)*Agi;
	}
	//位移
	void Displacement(float h,float v){
		this.velocity=new Vector2(h,v)*Agi*displaceRate;
	}
	// Use this for initialization
	public virtual void Start () {
        //最顶层的类用virtual即可，后面的子类全部用override
        Init();
		this.rig=transform.root.gameObject.GetComponent<Rigidbody2D>();
		this.ani=transform.root.gameObject.GetComponent<Animator>();
		MapInit map=MapInit.Map();
		Room st=map.GetCurrentRoom();
		int unitSize=Room.UnitSize;
		transform.Translate(new Vector3(st.X,st.Y,0)*unitSize);
	}
	
	// Update is called once per frame
	void Update ()
    {
        GetInput();
    }

	void FixedUpdate(){
		//物理更新操作都放在这里
		if(rig!=null)rig.velocity=this.velocity;
	}
}
