using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class Toolkit : MonoBehaviour {

	public static void Print<T>(T obj){
		foreach(System.Reflection.PropertyInfo p in obj.GetType().GetProperties()){
			string text="{0}:{1}";
			text=String.Format(text,p.Name,p.GetValue(obj,null));
			Debug.Log(text);
		}
	}

	public static void PrintArr<T>(T[] obj){
		foreach(T a in obj){
			Print<T>(a);
		}
	}

	public void ForExample(){
		//从json中读取数组数据的示例,保存参考用
		Data_Arr<Data_Action_Condition> da=Toolkit.ReadJson<Data_Arr<Data_Action_Condition>>("DataSetTXT/Enemy");
		foreach(Data_Action_Condition d in da.arr){
			Debug.Log(d.content);
		}
	}

	public static T ReadJson<T>(string file){
		TextAsset txt=Resources.Load(file)as TextAsset;
		T arr=JsonConvert.DeserializeObject<T>(txt.text);
		return arr;
	}

	public static T ReadJson<T>(string file,int ind){
		Data_Arr<T> da=Toolkit.ReadJson<Data_Arr<T>>(file);
		return da.arr[ind];
	}


	public static string ReadJson(string file,int ind){
		TextAsset tx=Resources.Load(file) as TextAsset;
		string txt=tx.text.Trim();
		List<string> list=new List<string>();
		int index=0;
		for(int i=0;i<txt.Length-2;i++){
			if(txt[i]=='}'&&txt[i+1]==','){
				list.Add(txt.Substring(index,i-index+1));
			}
		}
		return list[ind];
	}

	public static Dictionary<string,string> ReadJson(string file){
		Dictionary<string,string> content=new Dictionary<string,string>();
		TextAsset txas=Resources.Load(file) as TextAsset;
		string txt=txas.text.Trim();
		txt=txt.Substring(1,txt.Length-2);
		int ind=0;
		for(int i=1;i<txt.Length;i++){
			if(txt[i]==','&&txt[i-1]=='\"'){
				string cont=txt.Substring(ind,i-ind+1);
				string[] arr=cont.Split(':');
				content[arr[0]]=arr[1];
				ind=i+1;
			}
		}
		return content;
	}

	public static int CompareRange(int val,int[] arr,bool up=true){
		//用来寻找当前值在哪两个值之间，默认为升序排列
		int ind=0;
		for(int i=0;i<arr.Length;i++){
			if(up){
				if(val<arr[i]){break;}
			}else{
				if(val>arr[i]){break;}
			}
			ind+=1;
		}
		if(ind!=arr.Length-1)ind=ind-1<0?0:ind-1;
		return ind;
	}

	public static bool IsIn<T>(T[] array,T val){
		return Array.IndexOf<T>(array,val)>=0;
	}

	public static Vector3 ScreenToWorldPoint(Vector3 scr){
		return Camera.main.ScreenToWorldPoint(scr);
	}

	public static bool IsInRange(int val,int low,int high){
		return val<=high&&val>=low;
	}
}
