using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Game_Battler : MonoBehaviour {

	//法阵战斗者事件
	public event Game_MZhen.On_Create On_Create;
	public event Game_MZhen.On_Destroy On_Destroy;
	public event Game_MZhen.Pre_Attack Pre_Attack;
	public event Game_MZhen.Pre_Defend Pre_Defend;
	public event Game_MZhen.Pre_Move Pre_Move;
	public event Game_MZhen.After_Attack After_Attack;
	public event Game_MZhen.After_Defend After_Defend;
	public event Game_MZhen.After_Move After_Move;

	//法阵技能事件
	public event Game_MZhen.Skill_Effect Spe_Eff;
	public event Game_MZhen.Skill_On_Create Spe_On_Create;
	public event Game_MZhen.Skill_On_Destroy Spe_On_Destroy;
	public event Game_MZhen.Skill_Pre_Move Spe_Pre_Move;
	public event Game_MZhen.Skill_After_Move Spe_After_Move;
	
	//rigidbody2d
	protected Rigidbody2D rig;
	//id,name
	public int id;
	public string mname;
	//初始化函数
	protected void Init(){
		//Toolkit.ReadJson<Data_Actor>("",this.id);//用于Actor(Player)
		//Toolkit.ReadJson<Data_Enemy>("",this.id);//用于Enemy
	}
	protected bool locked=false;
	public bool Locked{
		get{
			return locked;
		}
	}
	protected int[] ability;
	public int MaxHP{
		get{return ability[0];}
		set{ability[0]=value;}
	}
	public int MaxSP{
		get{return ability[1];}
		set{ability[1]=value;}
	}
	public int Str{//力量
		get{return ability[2];}
		set{ability[2]=value;}
	}
	public int Dex{//灵巧
		get{return ability[3];}
		set{ability[3]=value;}
	}
	public int Agi{//速度
		get{return ability[4];}
		set{ability[4]=value;}
	}
	public int Int{//魔力
		get{return ability[5];}
		set{ability[5]=value;}
	}
	public int Atk{//攻击力
		get{return ability[6];}
		set{ability[6]=value;}
	}
	public int Pdef{//物理防御
		get{return ability[7];}
		set{ability[7]=value;}
	}
	public int Mdef{//魔法防御
		get{return ability[8];}
		set{ability[8]=value;}
	}
	public int Hit{//命中率
		get{return ability[9];}
		set{ability[9]=value;}
	}
	public int Eva{//回避修正
		get{return ability[10];}
		set{ability[10]=value;}
	}
	public int HP{
		get{return ability[11];}
		set{ability[11]=value>=0?value<=MaxHP?value:MaxHP:0;}
	}
	public int SP{
		get{return ability[12];}
		set{ability[12]=value>=0?value<=MaxSP?value:MaxSP:0;}
	}
	//时间和计时器
	protected Dictionary<string,float> time=new Dictionary<string,float>();
	protected Dictionary<string,float> timer=new Dictionary<string,float>();
	public float GetTimer(string ind){
		return timer[ind];
	}
	public void SetTimer(string ind,float val){
		timer[ind]=val;
	}
	//当前的攻击属性（无，阴，阳）
	protected int element_set=0;
	public int ElementSet{
		get{return element_set;}
		set{
			if(value>2)element_set=2;
			else if(value<0)element_set=0;
			else element_set=value;
		}
	}
	//状态
	protected List<Data_State> states=new List<Data_State>();
	//会心一击标志
	protected bool critital=false;
	//伤害
	protected float damage;
	//当前行动
	protected Data_Action current_action=new Data_Action();
	
	//调试中
	// //目标列表
	// protected List<Game_Battler> targets=new List<Game_Battler>();
	// public List<Game_Battler> Targets{
	// 	get{return targets;}
	// }

	//动画控制器
	protected Animator ani;

	//事件调用方法
	protected void PreAttack(){
		if(Pre_Attack!=null)Pre_Attack(this);
	}
	protected void AfterAttack(){
		if(After_Attack!=null)After_Attack(this);
	}
	protected void PreDefend(){
		if(Pre_Defend!=null)Pre_Defend(this);
	}
	protected void AfterDefend(){
		if(After_Defend!=null)After_Defend(this);
	}
	protected void PreMove(){
		if(Pre_Move!=null)Pre_Move(this);
	}
	protected void AfterMove(){
		if(After_Move!=null)After_Move(this);
	}
	protected void OnCreate(){
		if(On_Create!=null)On_Create(this);
	}
	protected void MOnDestroy(){
		if(On_Destroy!=null)On_Destroy(this);
	}

	//状态检查
	public bool HasState(int state_id){
		for(int i=0;i<states.Count;i++){
			if(states[i].id==state_id)return true;
		}
		return false;
	}
	//附加状态
	public void AddState(int state_id,bool force=false){
		//该状态不存在（无效）
		if(Data.States.Length<=state_id){
			return;
		}
		if(!force){
			for(int i=0;i<states.Count;i++){
				if(!Toolkit.IsIn(Data.States[state_id].minus_state_set,states[i].id)&&
					Toolkit.IsIn(states[i].minus_state_set,state_id)){
					return;
				}
			}
		}
		if(!HasState(state_id)){
			states.Add(Data.States[state_id]);
		}
	}
	//解除状态
	public void RemoveState(int state_id,bool force=false){
		if(HasState(state_id)){
			states.Remove(Data.States[state_id]);
		}
	}
	//可以使用特技的判定
	public bool SkillCanUse(int skill_id){
		if(Data.Skills[skill_id].sp_cost>this.SP){
			return false;
		}
		if(Dead()){
			return false;
		}
		//沉默状态下只能使用物理特技
		if(Data.Skills[skill_id].ability_f[0]==0 && this.Restriction()==1){
			return false;
		}
		return true;
	}
	//死亡判定
	public bool Dead(){
		return HP<=0;
	}
	//防御中判定
	public bool Guarding(){
		return (this.current_action.kind==0&&this.current_action.basic==1);
	}
	//获得限制restriction
	public int Restriction(){
		int res=0;
		for(int i=0;i<states.Count;i++){
			if(states[i].restriction>=res){
				res=states[i].restriction;
			}
		}
		return res;
	}
	//应用通常攻击效果
	public void AttackEffect(Game_Battler attacker){
		this.critital=false;
		bool hit_result=Random.Range(0,100f)<attacker.Hit;
		if(hit_result){
			//计算基本伤害
			float atk=Mathf.Max(attacker.Atk-this.Pdef/2f,0);
			this.damage=atk*(20+attacker.Str)/20f;
			//属性修正
			this.damage*=this.ElementsCorrect(attacker.ElementSet);
			this.damage/=100f;
			if(this.damage>0){
				//会心一击修正
				if(Random.Range(0,100f)<4f*attacker.Dex/this.Agi){
					this.damage*=2;
					this.critital=true;
				}
				//防御修正
				if(Guarding()){
					this.damage/=2f;
				}
			}
			//状态冲击解除
			RemoveStateShock();
			//HP的伤害计算
			this.HP-=Mathf.RoundToInt(this.damage);
			//状态变化，暂略
		}
	}
	//应用特技效果
	public void SkillInfluence(Game_Battler user,Data_Skill skill){
		//清除会心一击标志
		this.critital=false;
		//命中判定
		float hit=skill.Hit;
		if(skill.Atkf>0){
			hit*=user.Hit/100;
		}
		bool hit_result=(Random.Range(0,100f)<hit);
		if(hit_result){
			//计算威力
			float power=skill.power+user.Atk*skill.Atkf/100;
			if(power>0){
				power-=this.Pdef/200f;
				power-=this.Mdef/200f;
				power=Mathf.Max(power,0);
			}
			//计算倍率
			float rate=20f;
			rate+=(user.Str*skill.Strf/100);
			rate+=(user.Dex*skill.Dexf/100);
			rate+=(user.Agi*skill.Agif/100);
			rate+=(user.Int*skill.Intf/100);
			//计算基本伤害
			this.damage=power*rate/20;
			//属性修正
			this.damage*=ElementsCorrect(skill.element_set);
			this.damage/=100;
			if(this.damage>0){
				if(this.Guarding()){
					this.damage/=2;
				}
			}
		}
	}
	//获取属性修正值并且计算
	public float ElementsCorrect(int eid){
		float[] table=new float[]{0f,200f,150f,100f,50f,0f,-100f};
		float reslut=table[Data.Classes[this.ElementSet].element_ranks[eid]];
		for(int i=0;i<states.Count;i++){
			if(ArrayUtility.IndexOf(this.states[i].guard_element_set,eid)>=0){
				reslut/=2f;
			}
		}
		return reslut;
	}
	//状态冲击解除
	public void RemoveStateShock(){
		for(int i =0;i<states.Count;i++){
			if(Random.Range(0,100f)<Data.States[states[i].id].shock_release_prob){
				RemoveState(states[i].id);
			}
		}
	}
	//物体X轴朝向某物
	protected void LetLookAt(Transform go,Transform tar){
		go.LookAt(tar);
		//绕Y轴逆时针旋转90度
		go.Rotate(0,90,0);
	}
	//物体X轴朝向某物
	protected void LetLookAt(Transform go,Vector3 tarpos){
		go.LookAt(tarpos);
		//绕Y轴逆时针旋转90度
		go.Rotate(0,90,0);
	}	
	//特技
	protected GameObject CreateSkill(int sid){
		string full=sid<100?(sid<10?("00"+sid):"0"+sid):""+sid;
		GameObject n=Instantiate(Resources.Load("Prefabs/Skills/"+full),transform.position,transform.rotation)as GameObject;
		Game_Skill s=n.GetComponent<Game_Skill>();
		s.Owner=this;
		s.ID=sid;
		s.Init();
		//event only +=,-= not =
		if(Spe_Eff!=null)Spe_Eff(s);
		s.Spe_On_Create+=this.Spe_On_Create;
		s.Spe_On_Destroy+=this.Spe_On_Destroy;
		s.Spe_Pre_Move+=this.Spe_Pre_Move;
		s.Spe_After_Move+=this.Spe_After_Move;
		//技能范围，待修改
		if(Data.ScopeSelf(s.SKD.scope))s.Influence();
		return n;
	}
	//特技使用
	protected void UseSkill(Game_Battler ba,int sid){
		GameObject n=CreateSkill(sid);
		LetLookAt(n.transform,ba.transform);
	}
	//更新计时器
	protected void UpdateTimer(){
		foreach(string key in timer.Keys){
			if(timer[key]>0)timer[key]-=Time.deltaTime;
		}
	}
	//判断攻击方技能硬直
	public bool IsHard(){
		AnimatorStateInfo aniInfo=this.ani.GetCurrentAnimatorStateInfo(0);
		return aniInfo.normalizedTime<=1.0f && !aniInfo.IsName("idle");
	}
	//被攻击方击飞效果
	public void BlowUp(Vector2 vec,float t){
		this.rig.velocity+=vec;
		this.locked=true;
		//增加击飞状态，编号待定
		AddState(0);
		StartCoroutine(HealHard(t));
	}
	//从硬直/击飞/眩晕状态恢复
	IEnumerator HealHard(float t){
		yield return new WaitForSeconds(t);
		this.locked=false;
		//移除击飞状态，编号待定
		RemoveState(0);
	}

	// Use this for initialization
	public void Start () {
		Init();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public virtual void OnDestroy(){
		MOnDestroy();
	}
}
