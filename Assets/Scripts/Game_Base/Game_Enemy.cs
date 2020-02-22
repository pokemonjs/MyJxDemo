using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Enemy : Game_Battler {
	
	protected Data_Action[] actions;
	protected List<Data_Action> available_actions=new List<Data_Action>();
	//boss，即阵眼
	protected Game_Enemy boss;
	public Game_Enemy Boss{
		get{return boss;}
		set{boss=value;}
	}
	//思考时间
	protected float thinking_time=0.1f;
	//思考时间计时器
	[SerializeField]protected float thinking_timer;
	//技能时间和计时器(包括普通攻击和防御等)
	[SerializeField]protected float skill_time=0.5f;
	[SerializeField]protected float skill_timer;
	public float SkillTime{
		get{
			return skill_time;
		}
		set{
			skill_time=value;
		}
	}
	public float SkillTimer{
		get{
			return skill_timer;
		}
		set{
			skill_timer=value;
		}
	}
	//目标列表
	protected List<Game_Battler> targets=new List<Game_Battler>();
	public List<Game_Battler> Targets{
		get{return targets;}
	}

	//可用行动排序，按照rate升序排列
	protected void SortActions(){
		for(int i=0;i<available_actions.Count-1;i++){
			int endsort=0;
			for(int j=0;j<available_actions.Count-i-1;j++){
				if(available_actions[i].rate>available_actions[i].rate){
					Data_Action da=available_actions[i];
					available_actions[i]=available_actions[i+1];
					available_actions[i+1]=da;
					endsort=1;
				}
			}
			if(endsort==0){
				break;
			}
		}
	}
	//检查并设置可用行动
	protected void CheckActions(){
		available_actions.Clear();
		for(int i=0;i<actions.Length;i++){
			Toolkit.Print<Data_Action_Condition>(actions[i].Condition);
			if(actions[i].CanAction(this))available_actions.Add(actions[i]);
		}
		// foreach(var act in available_actions){
		// 	Toolkit.Print<Data_Action>(act);
		// }
	}
	//选择行动
	protected void ChooseAction(){
		for(int i=0;i<available_actions.Count;i++){
			if(Random.Range(0,10f)<=available_actions[i].rate){
				current_action=available_actions[i];
				break;
			}
		}
	}
	//与Boss（阵眼）的距离
	public float DistanceFromBoss(){
		if(!boss)return -1f;
		return Vector3.Distance(transform.position,boss.transform.position);
	}
	//与目标的距离
	public float DistanceFromTarget(){
		if(!targets[0])return -1f;
		return Vector3.Distance(transform.position,targets[0].transform.position);
	}
	//重置思考计时器
	protected void ResetThinkingTimer(float n=1f){
		thinking_timer=thinking_time/n;
	}
	//判断是否思考完毕
	protected bool ThinkingReady(){
		return thinking_timer<=0;
	}
	//刷新思考计时器
	protected void UpdateThinkingTimer(){
		if(thinking_timer>0)thinking_timer-=Time.deltaTime;
	}
	//重置技能计时器
	protected void ResetSkillTimer(float n=1f){
		skill_timer=skill_time/n;
	}
	//判断技能是否准备完毕
	protected bool SkillReady(){
		return skill_timer<=0;
	}
	//刷新技能计时器
	protected void UpdateSkillTimer(){
		if(skill_timer>0)skill_timer-=Time.deltaTime;
	}
	//刷新计时器
	protected void UpdateTimer(){
		UpdateThinkingTimer();
		UpdateSkillTimer();
	}
	//短暂性动作————攻击，特技，物品，防御
	//攻击
	protected void Attack(Game_Battler ba){
		GameObject n=CreateSkill(0);
		LetLookAt(n.transform,ba.transform);
	}
	//防御
	protected void Defend(){

	}
	//延续性动作————逃跑，向目标移动
	//逃跑
	protected void Escape(Game_Battler ba){
		float d=Vector3.Distance(ba.transform.position,transform.position)/this.Agi;
        rig.AddForce((transform.position - ba.transform.position).normalized * this.Agi);
	}
	//向目标移动
	protected void MoveTowards(Game_Battler ba){
		//float d=Vector3.Distance(ba.transform.position,transform.position)/this.Agi;
  		//rig.AddForce((ba.transform.position - transform.position).normalized * this.Agi);
  		Vector3 pos=(ba.transform.position-transform.position).normalized*this.Agi;
  		rig.velocity=new Vector2(pos[0],pos[1]);
	}
	//执行行动
	protected void Todo(Data_Action act){
		if(act==null)return;
		//迷惑.jpg   为什么会为null？
		switch(act.kind){
			case 0://基本
				if(act.basic==0){//攻击
					PreAttack();
					Attack(targets[0]);
					ResetSkillTimer();
					AfterAttack();
				}
				if(act.basic==1){//防御
					PreDefend();
					Defend();
					ResetSkillTimer();
					AfterDefend();
				}
				if(act.basic==2){//逃跑
					PreMove();
					Escape(targets[0]);
					AfterMove();
				}
				if(act.basic==3){//向目标移动
					PreMove();
					MoveTowards(targets[0]);
					AfterMove();
				}
				break;
			case 1://特技
				PreAttack();
				UseSkill(targets[0],act.skill_id);
				ResetSkillTimer();
				AfterAttack();
				break;
		}
		ResetThinkingTimer();
		current_action=null;
	}

    //初始化函数
    new void Init(){
		Data_Enemy da=Toolkit.ReadJson<Data_Enemy>("DataSetTXT/Enemy",this.id);
		this.mname=da.mname;
		this.ability=da.ability;
		this.actions=da.actions;
		thinking_timer=thinking_time;
		skill_timer=skill_time;
	}
	// Use this for initialization
	public new void Start () {
        //最顶层的类用virtual即可，后面的子类全部用override
        //调用base时，父类的方法和属性需要是public的
        Init();
		this.rig=transform.root.gameObject.GetComponent<Rigidbody2D>();
		this.ani=transform.root.gameObject.GetComponent<Animator>();
		//测试用
		targets.Add(GameObject.FindGameObjectWithTag("Player").GetComponent<Game_Battler>());
		// SortActions();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateTimer();
		SortActions();
		if(ThinkingReady()){
			CheckActions();
			ChooseAction();
			//Toolkit.Print<Data_Action>(current_action);
			Todo(current_action);
		}
	}
}
