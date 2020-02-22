using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Skill : MonoBehaviour {
	//技能类
	//技能ID
	private int id;
	public int ID{
		get{
			return id;
		}
		set{
			id=value;
		}
	}
	//技能所有者
	protected Game_Battler owner;
	public Game_Battler Owner{
		get{return owner;}
		set{owner=value;}
	}
	//技能数据
	protected Data_Skill skd;
	//技能数据访问器
	public Data_Skill SKD{
		get{return skd;}
	}
	//法阵技能事件
	public event Game_MZhen.Skill_On_Create Spe_On_Create;
	public event Game_MZhen.Skill_On_Destroy Spe_On_Destroy;
	public event Game_MZhen.Skill_Pre_Move Spe_Pre_Move;
	public event Game_MZhen.Skill_After_Move Spe_After_Move;
	//事件调用方法
	protected void PreMove(){
		if(Spe_Pre_Move!=null)Spe_Pre_Move(this);
	}
	protected void AfterMove(){
		if(Spe_After_Move!=null)Spe_After_Move(this);
	}
	protected void OnCreate(){
		if(Spe_On_Create!=null)Spe_On_Create(this);
	}
	protected void MOnDestroy(){
		if(Spe_On_Destroy!=null)Spe_On_Destroy(this);
	}
	//技能对目标的影响
	public void Influence(Game_Battler ba){
		//增加状态
		for(int i=0;i<skd.plus_state_set.Length;i++){
			if(Random.Range(0,10f)<=skd.plus_state_rate[i]){
				ba.AddState(skd.plus_state_set[i]);
			}
		}
		//移除状态
		for(int i=0;i<skd.minus_state_set.Length;i++){
			if(Random.Range(0,10f)<=skd.minus_state_rate[i]){
				ba.RemoveState(skd.minus_state_set[i]);
			}
		}
	}
	public void Influence(){
		switch(skd.scope){
			case 3://己方单体
				Influence(this.owner);
				break;
			case 4://己方全体
				//待修改
				Influence(this.owner);
				break;
		}
	}
	//特殊效果,例如改变轨迹，追踪，分身等
	public void Effect(){

	}
	//初始化
	//因为技能大多是被创建的，所以Init在生成物体时由别的脚本显示调用，防止Start来不及执行
	public void Init(){
		OnCreate();
		skd=Toolkit.ReadJson<Data_Skill>("DataSetTXT/Skill",this.id);
		skd.Init();
	}
	//销毁时调用事件
	public virtual void OnDestroy(){
		MOnDestroy();
	}
	//固定帧更新,因为用刚体移动，所以移动也放这里.
	void FixedUpdate(){
		//可以不用virtual,因为很大几率会在子类中重写，不用继承这部分的,这个主要是做示范
		PreMove();
		//中间放移动代码,待修改
		//或者把移动和上下两个事件结合起来
		AfterMove();
	}
	// Use this for initialization
	void Start () {
		//调试中
		Destroy(gameObject,2.5f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
