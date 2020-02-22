using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class MapInit : MonoBehaviour {

	public int W=100;
	public int H=100;
	public int RoomNum=15;
	private int cx,cy;
	private int[,] map;
	private List<Room> rooms=new List<Room>();
	private int currentRoom=0;
	public int CurrentRoom{
		set{
			currentRoom=value;
		}
	}
	public List<Room> Rooms{
		get{
			return rooms;
		}
	}
	public static int passWidth=4;


	//获取当前房间
	public Room GetCurrentRoom(){
		return rooms[currentRoom];
	}

	public static MapInit Map(){
		MapInit[] arr=(MapInit[])FindObjectsOfType(typeof(MapInit));
		return arr[0];
	}

	// Use this for initialization
	void Start () {
		this.map=new int[W,H];
		cx=W/2;
		cy=H/2;
		Room st=new Room(cx,cy,0);
		st.Fill(map);
		rooms.Add(st);
		while(rooms.Count<RoomNum){
			Room rm=CreateRoom(rooms[Random.Range(0,rooms.Count-1)]);
			if(rm!=null){
				rooms.Add(rm);
			}
		}
		for(int i=0;i<rooms.Count;i++){
			GameObject n=rooms[i].DrawRoom();
			n.transform.SetParent(transform.root,false);
		}
		PrintMap();
	}
	
	Room CreateRoom(Room lrm){
		List<int> dirList=new List<int>();
		Room rm;
		while(true){
			rm=new Room();
			int dir=-1;
			while(true){
				dir=Random.Range(0,4);
				if(dirList.IndexOf(dir)==-1)break;
				else{
					if(dirList.Count>=4){
						return null;
					}
				}
			}
			int ncx,ncy;
			switch(dir){
				case 0:
				//right
					ncx=lrm.R+rm.W/2+passWidth;
					ncy=lrm.Y;
					if(CheckXY(ncx,ncy)){
						rm.SetXY(ncx,ncy);
						if(rm.CheckRange(W,H)&&rm.CheckFull(map)){
							rm.Fill(map);
							cx=ncx;
							cy=ncy;
							return rm;
						}else{
							dirList.Add(dir);
						}
					}else{
						dirList.Add(dir);
					}
					break;
				case 1:
				//up
					ncx=lrm.X;
					ncy=lrm.T-rm.H/2-passWidth;
					if(CheckXY(ncx,ncy)){
						rm.SetXY(ncx,ncy);
						if(rm.CheckRange(W,H)&&rm.CheckFull(map)){
							rm.Fill(map);
							cx=ncx;
							cy=ncy;
							return rm;
						}else{
							dirList.Add(dir);
						}
					}else{
						dirList.Add(dir);
					}
					break;
				case 2:
				//left
					ncx=lrm.L-rm.W/2-passWidth;
					ncy=lrm.Y;
					if(CheckXY(ncx,ncy)){
						rm.SetXY(ncx,ncy);
						if(rm.CheckRange(W,H)&&rm.CheckFull(map)){
							rm.Fill(map);
							cx=ncx;
							cy=ncy;
							return rm;
						}else{
							dirList.Add(dir);
						}
					}else{
						dirList.Add(dir);
					}
					break;
				case 3:
				//down
					ncx=lrm.X;
					ncy=lrm.B+rm.H/2+passWidth;
					if(CheckXY(ncx,ncy)){
						rm.SetXY(ncx,ncy);
						if(rm.CheckRange(W,H)&&rm.CheckFull(map)){
							rm.Fill(map);
							cx=ncx;
							cy=ncy;
							return rm;
						}else{
							dirList.Add(dir);
						}
					}else{
						dirList.Add(dir);
					}
					break;
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	bool CheckXY(int x,int y){
		return x>=0&&x<W&&y>=0&&y<H;
	}

	void PrintMap(){
		FileStream fs=new FileStream(Application.dataPath+"//Resources/Map.txt",FileMode.Create);
		string str="";
		for(int i=0;i<map.GetLength(0);i++){
			for(int j=0;j<map.GetLength(1);j++){
				str+=map[i,j];
			}
			str+="\r\n";
		}
		byte[] bytes=Encoding.UTF8.GetBytes(str);
		fs.Write(bytes,0,bytes.Length);
		fs.Flush();
		fs.Close();
		fs.Dispose();
	}

}

public class Room{

	private GameObject tile;
	private GameObject wall;
	private int[] widths=new int[]{8,10,12,14};
	private int[] heights=new int[]{6,8,10,12};
	public int X,Y,W,H,L,R,T,B;

	public static int UnitSize;

	public void Init(){
		tile=Resources.Load("Prefabs/Tile")as GameObject;
		wall=Resources.Load("Prefabs/Wall")as GameObject;
		UnitSize=(int)tile.GetComponent<SpriteRenderer>().size[0];
	}

	public Room(){
		Init();
		int ind=Random.Range(0,widths.Length-1);
		this.W=widths[ind];
		this.H=heights[ind];
	}

	public Room(int x,int y){
		Init();
		this.X=x;
		this.Y=y;
		int ind=Random.Range(0,widths.Length-1);
		this.W=widths[ind];
		this.H=heights[ind];
		this.L=x-this.W/2;
		this.R=x+this.W/2;
		this.T=y-this.H/2;
		this.B=y+this.H/2;
	}

	public Room(int x,int y,int ind){
		Init();
		this.X=x;
		this.Y=y;
		this.W=widths[ind];
		this.H=heights[ind];
		this.L=x-this.W/2;
		this.R=x+this.W/2;
		this.T=y-this.H/2;
		this.B=y+this.H/2;
	}

	public void SetXY(int x,int y){
		this.X=x;
		this.Y=y;
		this.L=x-this.W/2;
		this.R=x+this.W/2;
		this.T=y-this.H/2;
		this.B=y+this.H/2;
	}

	public bool CheckRange(int w,int h){
		return this.L>=0&&this.R<w&&this.T>=0&&this.B<h;
	}

	public bool CheckFull(int[,] map){
		for(int i=L-MapInit.passWidth/2;i<=R+MapInit.passWidth/2;i++){
			for(int j=T-MapInit.passWidth/2;j<=B+MapInit.passWidth/2;j++){
				if(!Toolkit.IsInRange(i,0,map.GetLength(0))||!Toolkit.IsInRange(j,0,map.GetLength(1)))return false;
				if(map[i,j]!=0)return false;
			}
		}
		return true;
	}

	public void Fill(int[,] map){
		for(int i=L;i<=R;i++){
			for(int j=T;j<=B;j++){
				map[i,j]=1;
			}
		}
	}

	public GameObject DrawRoom(){
		GameObject empty=Resources.Load("Prefabs/Empty")as GameObject;
		GameObject n=GameObject.Instantiate(empty)as GameObject;
		for(int i=L-1;i<=R+1;i++){
			for(int j=T-1;j<=B+1;j++){
				if(i==L-1||i==R+1||j==T-1||j==B+1){
					GameObject w=GameObject.Instantiate(wall,new Vector3(i,j,0),Quaternion.identity);
					w.transform.SetParent(n.transform,false);
					continue;
				}
				GameObject p=GameObject.Instantiate(tile,new Vector3(i,j,0),Quaternion.identity);
				p.transform.SetParent(n.transform,false);
			}
		}
		return n;
	}

}