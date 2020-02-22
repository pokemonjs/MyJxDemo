using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_MZhen : MonoBehaviour {

	public delegate void On_Create(Game_Battler b);
	public delegate void On_Destroy(Game_Battler b);

	public delegate void Pre_Attack(Game_Battler b);
	public delegate void Pre_Defend(Game_Battler b);
	public delegate void Pre_Move(Game_Battler b);

	public delegate void After_Attack(Game_Battler b);
	public delegate void After_Defend(Game_Battler b);
	public delegate void After_Move(Game_Battler b);

	public delegate void Skill_Effect(Game_Skill s);
	public delegate void Skill_On_Create(Game_Skill s);
	public delegate void Skill_On_Destroy(Game_Skill s);
	public delegate void Skill_Pre_Move(Game_Skill s);
	public delegate void Skill_After_Move(Game_Skill s);

	public void ApplyInBattler(Game_Battler[] gbs){}

}
