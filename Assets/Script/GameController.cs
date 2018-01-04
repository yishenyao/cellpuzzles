using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
	static Color[] colors = {
		Color.white,
		Color.red,
		Color.blue,
		Color.magenta,
		Color.green,
	};

	int nRowCount;					// 关卡行数
	int nColCount;					// 关卡列数
	int[,] MissionData;				// 关卡数据，例：[1, 2]为第一行第二列的颜色号，方向为左上到右下
	int[,] AnswerData;				// 玩家解答数据

	List<Vector2>[] RowNums;		// 从关卡数据分析出的每行的色块数量
	List<Vector2>[] ColumnNums;		// 从关卡数据分析出的每列的色块数量

	Vector3 leftUpPos;

	// Use this for initialization
	void Start () {
		leftUpPos = new Vector3 (-2, 1, 0);
		LoadMission (0);
		DrawCells (nRowCount, nColCount);
		DrawGridNum ();
		// DrawRightAnswer ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// 画出背景格子
	void DrawCells(int wCount, int hCount){
		Vector3 startPos = leftUpPos;
		DrawGrid (startPos, 0.5f, wCount, hCount, 0.03f);
		DrawGrid (startPos, 2.5f, wCount / 5, hCount / 5, 0.06f);
	}

	void DrawCellButton(){
		//画出用户点击的格子对象
	}

	// 画出格子旁边的关卡数字
	void DrawGridNum() {
		Vector3 rowOffSet = new Vector3(-0.25f, -0.25f, 0);
		Vector3 startRowPos = leftUpPos + rowOffSet;
		DrawRowNum (startRowPos);
		Vector3 colOffSet = new Vector3(0.25f, 0.25f, 0);
		Vector3 startColPos = leftUpPos + colOffSet;
		DrawColNum (startColPos);
	}

	// 画出每行的数字
	void DrawRowNum(Vector3 startPos) {
		for (int x = 0; x < nRowCount; x++) {
			List<Vector2> rowData = RowNums [x];
			for (int y = rowData.Count - 1; y >= 0; y--) {
				Vector2 vec = rowData [y];
				Vector3 pos = new Vector3 (startPos.x - (rowData.Count - y - 1) * 0.5f, startPos.y - x * 0.5f, startPos.z);
				AddOneNum (pos, colors[(int)vec.x], (int)vec.y);
			}
		}
	}

	// 画出每列的数字
	void DrawColNum(Vector3 startPos) {
		for (int x = 0; x < nColCount; x++) {
			List<Vector2> colData = ColumnNums [x];
			for (int y = colData.Count - 1; y >= 0 ; y--) {
				Vector2 vec = colData [y];
				Vector3 pos = new Vector3 (startPos.x + x * 0.5f, startPos.y + (colData.Count - y - 1) * 0.5f, startPos.z);
				AddOneNum (pos, colors[(int)vec.x], (int)vec.y);
			}
		}
	}

	// 添加一个数字
	void AddOneNum(Vector3 pos, Color color, int nNum) {
		GameObject Canvas = GameObject.Find("Canvas");
		GameObject NumImage = (GameObject)Resources.Load("Prefabs/Num");  
		NumImage = GameObject.Instantiate(NumImage);
		NumImage.transform.SetParent (Canvas.transform);
		NumImage.transform.position = pos;
		NumImage.transform.localScale = new Vector3(0.25f, 0.25f, 1);
		Text text = NumImage.GetComponentInChildren<Text>();
		text.text = nNum.ToString();
		Image img = NumImage.GetComponent<Image> ();
		img.color = color;
	}

	// 读取关卡
	void LoadMission(int MissionID){
		// 随机布置一个关卡
		nRowCount = 10;
		nColCount = 10;
		MissionData = new int[nRowCount, nColCount];
		for (int i = 0; i < nRowCount; i++) {
			for (int j = 0; j < nRowCount; j++) {
				MissionData [i, j] = UnityEngine.Random.Range (0, 4);
			}
		}

		RowNums = new List<Vector2>[nRowCount];
		for (int row = 0; row < nRowCount; row++) {
			RowNums [row] = new List<Vector2>();
			List<Vector2> rowData = RowNums [row];
			int nLastColor = 0;
			for (int col = 0; col < nRowCount; col++) {
				int nIdx = rowData.Count - 1;
				if (MissionData[row, col] != 0) {
					if (nIdx >= 0 && MissionData[row, col] == nLastColor) {
						rowData [nIdx] = new Vector2 (nLastColor, rowData [nIdx].y + 1);
					} else {
						rowData.Add (new Vector2 (MissionData[row, col], 1));
					}
				}
				nLastColor = MissionData [row, col];
			}
		}

		ColumnNums = new List<Vector2>[nColCount];
		for (int col = 0; col < nColCount; col++) {
			ColumnNums [col] = new List<Vector2>();
			List<Vector2> colData = ColumnNums [col];
			int nLastColor = 0;
			for (int row = 0; row < nColCount; row++) {
				int nIdx = colData.Count - 1;
				if (MissionData[row, col] != 0) {
					if (nIdx >= 0 && MissionData[row, col] == nLastColor) {
						colData [nIdx] = new Vector2 (nLastColor, colData [nIdx].y + 1);
					} else {
						colData.Add (new Vector2 (MissionData[row, col], 1));
					}
				}
				nLastColor = MissionData [row, col];
			}
		}
	}

	bool CompareAnswer(){
		//比较用户答案与正确答案
		return false;
	}

	public void UpdateAnswer(int w,int h){
		//更新用户答案
	}

	// 画出正确答案
	public void DrawRightAnswer(){
		for (int row = 0; row < nRowCount; row++) {
			for (int col = 0; col < nRowCount; col++) {
				if (MissionData [row, col] > 0) {
					Vector3 pos = leftUpPos + new Vector3 (0.25f + 0.5f * col, -0.25f - 0.5f * row, 0);
					AddOneColor(pos, colors[MissionData [row, col]]);
				}
			}
		}
	}

	// 添加一个图块
	void AddOneColor(Vector3 pos, Color color) {
		GameObject mUICanvas = GameObject.Find("Canvas");
		GameObject ColorImage = (GameObject)Resources.Load("Prefabs/ColorSquare");  
		ColorImage = GameObject.Instantiate(ColorImage);
		ColorImage.transform.SetParent (mUICanvas.transform);
		ColorImage.transform.position = pos;
		ColorImage.transform.localScale = new Vector3(0.25f, 0.25f, 1);
		Image img = ColorImage.GetComponent<Image> ();
		img.color = color;
	}

	// 绘制格子, startPos为坐上角坐标
	void DrawGrid(Vector3 startPos, float fGridLengh, int nRow, int nCol, float fLintWidth) {
		for (int i = 0; i <= nCol; i++) {
			GameObject line = new GameObject();
			LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
			lineRenderer.material = new Material (Shader.Find("Unlit/Color"));
			lineRenderer.material.color = Color.gray;
			lineRenderer.useWorldSpace = true;
			lineRenderer.positionCount = 2;
			Vector3[] pos = new Vector3[5];
			pos [0] = new Vector3(startPos.x, startPos.y - i * fGridLengh, startPos.z);
			pos [1] = new Vector3(startPos.x + nCol * fGridLengh, startPos.y - i * fGridLengh, startPos.z);
			lineRenderer.SetPositions (pos);
			lineRenderer.startWidth = fLintWidth;
			lineRenderer.endWidth = fLintWidth;
		}

		for (int i = 0; i <= nRow; i++) {
			GameObject line = new GameObject();
			LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
			lineRenderer.material = new Material (Shader.Find("Unlit/Color"));
			lineRenderer.material.color = Color.gray;
			lineRenderer.useWorldSpace = true;
			lineRenderer.positionCount = 2;
			Vector3[] pos = new Vector3[5];
			pos [0] = new Vector3(startPos.x + i * fGridLengh, startPos.y, startPos.z);
			pos [1] = new Vector3(startPos.x + i * fGridLengh, startPos.y - nRow * fGridLengh, startPos.z);
			lineRenderer.SetPositions (pos);
			lineRenderer.startWidth = fLintWidth;
			lineRenderer.endWidth = fLintWidth;
		}
	}
}
