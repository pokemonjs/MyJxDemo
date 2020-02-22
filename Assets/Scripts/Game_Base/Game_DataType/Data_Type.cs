using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

[Serializable]
public class Data_Arr<T>{
	public T[] arr;
}

[Serializable]
public class Data_Actor{
	public int id;
	public string mname;
	public int[] ability;//能力值数组
	public string[] timer_name;
	public float[] timer_val;
}
[Serializable]
public class Data_Action{

	public int id;
	public int kind=-1;
	public int basic;
	public int skill_id;
	public Data_Action_Condition condition;
	public float rate;

	public int ID{
		get{
			return id;
		}
	}
	public int Kind{
		get{
			return kind;
		}
	}
	public int Baisc{
		get{
			return basic;
		}
	}
	public int SkillID{
		get{
			return skill_id;
		}
	}
	public Data_Action_Condition Condition{
		get{
			return condition;
		}
	}
	public float Rate{
		get{
			return rate;
		}
	}

	public Data_Action(){}

	public Data_Action(string json){

	}

	public bool CanAction(Game_Enemy ene){
		return condition.CanAction(ene);
	}

	public void Reset(){
		this.kind=-1;
	}

}

[Serializable]
public class Data_Action_Condition{
	//敌人行动的条件类
	public int id;
	public Dictionary<string,float> content;
	//若不需要某个条件，则-1即可代表无效

	public bool CanAction(Game_Enemy ene){
		bool res=false;
		foreach(string k in content.Keys){
			switch(k){
				case "cd":
					if(ene.SkillTimer<=0){
						// ene.SkillTimer=content[k];
						res=true;
					}else{
						return false;
					}
					break;
				case "hp_h":
					if((ene.HP+0f)/ene.MaxHP>content[k]){
						res=true;
					}else{
						return false;
					}
					break;
				case "dist_boss_h":
					if(ene.DistanceFromBoss()>=0 && ene.DistanceFromBoss()>=content[k]){
						res=true;
					}else{
						return false;
					}
					break;
				case "dist_h":
					if(ene.DistanceFromTarget()>=0 && ene.DistanceFromTarget()>=content[k]){
						res=true;
					}else{
						return false;
					}
					break;
				case "dist_l":
					if(ene.DistanceFromTarget()>=0 && ene.DistanceFromTarget()<=content[k]){
						res=true;
					}else{
						return false;
					}
					break;
			}
		}
		return res;
	}
}

[Serializable]
public class Data_Enemy{

	public int id;
	public string mname;
	public int[] ability;//能力值数组
	public Data_Action[] actions;
	public string[] timer_name;
	public float[] timer_val;

}

[Serializable]
public class Data_State{
	//状态的数据类
	public int id;
	public string mname;
	//限制（0：无，1：不能使用魔法，2：普通攻击敌人，3：普通攻击同伴，4：不行动）。
	public int restriction;
	public float rating;
	//rates包括[hit_rate,maxhp_rate,maxsp_rate,str_rate,dex_rate,
	//agi_rate,int_rate,atk_rate,pdef_rate,mdef_rate,eva]
	public float[] rates;
	public float hold_turn;
	public float auto_release_prob;
	public float shock_release_prob;
	public int[] guard_element_set;
	public int[] plus_state_set;
	public int[] minus_state_set;
    public string[] effect_list;

    public delegate void Effect(Game_Battler gb);
    public Effect[] effects;

    public void Run(Game_Battler gb)
    {
        for (int i = 0; i < effect_list.Length; i++)
        {
            Type t = typeof(Data_State);
            MethodInfo mt = t.GetMethod("Add_"+effect_list[i]);
            if (mt != null)
            {
                mt.Invoke(this, new object[] { gb });
            }
        }
    }

    public void Remove(Game_Battler gb)
    {
        for (int i = 0; i < effect_list.Length; i++)
        {
            Type t = typeof(Data_State);
            MethodInfo mt = t.GetMethod("Remove_" + effect_list[i]);
            if (mt != null)
            {
                mt.Invoke(this, new object[] { gb });
            }
        }
    }

    public bool Over()
    {
        if (hold_turn > 0) hold_turn -= 1 * Time.deltaTime;
        return hold_turn <= 0;
    }

}

[Serializable]
public class Data_Skill{
	//特技的数据类
	public int id;
	public string mname;
	public string description;
	public int scope;
	public int sp_cost;
	public int power;
	//ability_f包括[atk_f,eva_f,str_f,dex_f,agi_f,int_f,,hit,pdef_f,mdef_f]
	public float[] ability_f;
	public int element_set;
	public int[] plus_state_set;
	public int[] plus_state_rate;
	public int[] minus_state_set;
	public int[] minus_state_rate;
	public float[] blow_up;//击飞效果的向量
	public float blow_up_time;//击飞/眩晕时间
	public string[] str_func;//技能特殊效果实现方法(字符串)
	[NonSerialized]protected List<MethodInfo> eff_func;//技能特殊效果实现方法(Method)
	[NonSerialized]protected Vector2 blow_up_vec;

	public void Init(){
		blow_up_vec=new Vector2(blow_up[0],blow_up[1]);
		InitEffFunc();
	}

	public void InitEffFunc(){
		for(int i=0;i<str_func.Length;i++){
			Type t=typeof(Data_Skill);
			MethodInfo m=t.GetMethod(str_func[i]);
			eff_func.Add(m);
		}
	}

	public float Atkf{
		get{return ability_f[0];}
		set{ability_f[0]=value;}
	}
	public float Evaf{
		get{return ability_f[1];}
		set{ability_f[1]=value;}
	}
	public float Strf{
		get{return ability_f[2];}
		set{ability_f[2]=value;}
	}
	public float Dexf{
		get{return ability_f[3];}
		set{ability_f[3]=value;}
	}
	public float Agif{
		get{return ability_f[4];}
		set{ability_f[4]=value;}
	}
	public float Intf{
		get{return ability_f[5];}
		set{ability_f[5]=value;}
	}
	public float Hit{
		get{return ability_f[6];}
		set{ability_f[6]=value;}
	}
	public float Pdeff{
		get{return ability_f[7];}
		set{ability_f[7]=value;}
	}
	public float Mdeff{
		get{return ability_f[8];}
		set{ability_f[8]=value;}
	}
}

[Serializable]
public class Data_Troop{
	//敌人队伍的数据类
	public int id;
	public int[] members;
}

[Serializable]
public class Data_Class{
	//职业的数据类,这里用作属性
	public int id;
	public int[] element_ranks;
	public int[] state_ranks;
}