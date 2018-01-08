using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

enum enColor
{
	white = 0,
	red,
	blue,
	magenta,
	green
};

public class GameController : MonoBehaviour {
	static Color[] colors = {
		Color.white,
		Color.red,
		Color.blue,
		Color.magenta,
		Color.green,
	};

	public static GameController gameController;

	public int nCurColor;

	int nRowCount;					// 关卡行数
	int nColCount;					// 关卡列数
	int[,] MissionData;				// 关卡数据，例：[1, 2]为第一行第二列的颜色号，方向为左上到右下
	int[,] AnswerData;				// 玩家解答数据
	GameObject[,] ColorGrids;		// 玩家点出的色块

	List<GameObject> listNumGrid = new List<GameObject>();
	int				 nNumGridUse = 0;

	List<Vector2>[] RowNums;		// 从关卡数据分析出的每行的色块数量
	List<Vector2>[] ColumnNums;		// 从关卡数据分析出的每列的色块数量

	Vector3 leftUpPos;				// 左上角坐标
	float	fGridLength;			// 小格长度

	bool	bWin = false;

	public Text txtWin;

	// Use this for initialization
	void Start () {
		txtWin.enabled = false;
		nCurColor = 0;
		leftUpPos = new Vector3 (-2.5f, 1, 0);
		fGridLength = 0.8f;
		LoadMission (0);
		DrawCells (nRowCount, nColCount);
		DrawGridNum ();
		// DrawRightAnswer ();
	}

	//获得实例
	void Awake(){
		gameController = this;
	}

	// Update is called once per frame
	void Update () {
		
	}

	// 画出背景格子
	void DrawCells(int wCount, int hCount){
		Vector3 startPos = leftUpPos;
		DrawGrid (startPos, fGridLength, wCount, hCount, 0.03f);
		DrawGrid (startPos, fGridLength * 5, wCount / 5, hCount / 5, 0.06f);
	}

	// 画出格子旁边的关卡数字
	void DrawGridNum() {
		nNumGridUse = 0;
		Vector3 rowOffSet = new Vector3(-fGridLength / 2 - 0.1f, -fGridLength / 2, 0);
		Vector3 startRowPos = leftUpPos + rowOffSet;
		DrawRowNum (startRowPos);
		Vector3 colOffSet = new Vector3(fGridLength / 2, fGridLength / 2 + 0.1f, 0);
		Vector3 startColPos = leftUpPos + colOffSet;
		DrawColNum (startColPos);
		for (int i = nNumGridUse; i < listNumGrid.Count; i++) {
			listNumGrid [i].SetActive (false);
		}
	}

	// 画出每行的数字
	void DrawRowNum(Vector3 startPos) {
		for (int x = 0; x < nRowCount; x++) {
			List<Vector2> rowData = RowNums [x];
			for (int y = rowData.Count - 1; y >= 0; y--) {
				Vector2 vec = rowData [y];
				Vector3 pos = new Vector3 (startPos.x - (rowData.Count - y - 1) * fGridLength, startPos.y - x * fGridLength, startPos.z);
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
				Vector3 pos = new Vector3 (startPos.x + x * fGridLength, startPos.y + (colData.Count - y - 1) * fGridLength, startPos.z);
				AddOneNum (pos, colors[(int)vec.x], (int)vec.y);
			}
		}
	}

	// 添加一个数字
	void AddOneNum(Vector3 pos, Color color, int nNum) {
		GameObject NumImage;
		if (nNumGridUse >= listNumGrid.Count) {
			GameObject Canvas = GameObject.Find ("Canvas");
			NumImage = (GameObject)Resources.Load ("Prefabs/Num");  
			NumImage = GameObject.Instantiate (NumImage);
			NumImage.transform.SetParent (Canvas.transform, false);
			listNumGrid.Add (NumImage);
			nNumGridUse++;
		} else {
			NumImage = listNumGrid [nNumGridUse];
			NumImage.SetActive (true);
			nNumGridUse++;
		}
		NumImage.transform.position = pos;
		NumImage.transform.localScale = new Vector3 (1, 1, 1);
		Text text = NumImage.GetComponentInChildren<Text> ();
		text.text = nNum.ToString ();
		Image img = NumImage.GetComponent<Image> ();
		img.color = color;
	}

	// 读取关卡
	void LoadMission(int MissionID){
		// 随机布置一个关卡
		nRowCount = 5;
		nColCount = 5;
		MissionData = new int[nRowCount, nColCount];
		AnswerData = new int[nRowCount, nColCount];
		InitColorGrids ();
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
		for (int x = 0; x < nRowCount; x++) {
			for (int y = 0; y < nRowCount; y++) {
				if (MissionData [x, y] != AnswerData [x, y]) {
					return false;
				}
			}
		}
		return true;
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

	// 逻辑坐标添加一个图块
	void AddOneColor(Vector2 logicPos, Color color) {
		
	}

	void InitColorGrids() {
		if (ColorGrids != null) {
			for (int x = 0; x < nRowCount; x++) {
				for (int y = 0; y < nRowCount; y++) {
					if (ColorGrids [x, y] != null) {
						GameObject.Destroy (ColorGrids [x, y]);
					}
				}
			}
		}
		ColorGrids = new GameObject[nRowCount, nColCount];
		for (int x = 0; x < nRowCount; x++) {
			for (int y = 0; y < nRowCount; y++) {
				Vector3 gridPos = leftUpPos + new Vector3 (fGridLength / 2 + fGridLength * x, -fGridLength / 2 - fGridLength * y, 0);
				GameObject mUICanvas = GameObject.Find("Canvas");
				GameObject ColorImage = (GameObject)Resources.Load("Prefabs/ColorSquare");  
				ColorGrids[x,y] = GameObject.Instantiate(ColorImage);
				ColorGrids[x,y].transform.SetParent (mUICanvas.transform, false);
				ColorGrids[x,y].transform.position = gridPos;
				ColorGrids[x,y].transform.localScale = new Vector3(1, 1, 1);
				Image img = ColorGrids[x,y].GetComponent<Image> ();
				img.color = Color.white;
				ColorGrids [x, y].SetActive (false);
			}
		}
	}

	void ChangeGridColor(Vector2 LogicPos, int nColor) {
		int nRow = (int)LogicPos.x;
		int nCol = (int)LogicPos.y;
		AnswerData [nCol, nRow] = nColor;
		if (nColor == (int)enColor.white) {
			ColorGrids [nRow, nCol].SetActive (false);
		} else {
			ColorGrids [nRow, nCol].SetActive (true);
			Image img = ColorGrids [nRow, nCol].GetComponent<Image> ();
			img.color = colors [nColor];
		}
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

	public void OnWhite() {
		nCurColor = (int)enColor.white;
	}

	public void OnRed() {
		nCurColor = (int)enColor.red;
	}

	public void OnBlue() {
		nCurColor = (int)enColor.blue;
	}

	public void OnMagenta() {
		nCurColor = (int)enColor.magenta;
	}

	public void OnGreen() {
		nCurColor = (int)enColor.green;
	}

	public void OnClickGrid(Vector3 pos) {
		if (bWin) {
			bWin = false;
			txtWin.enabled = false;
			ClearMission ();
			return;
		}
		Vector3 gridPos = (pos - leftUpPos) / fGridLength;
		int nRow = (int)Mathf.Abs (gridPos.x);
		int nCol = (int)Mathf.Abs (gridPos.y);
		Vector2 gridLogicPos = new Vector2 (nRow, nCol);
		Debug.Log (gridLogicPos);
		if (gridLogicPos.x < 0 || gridLogicPos.x >= nColCount || gridLogicPos.y < 0 || gridLogicPos.y >= nRowCount) {
			return;
		}
		ChangeGridColor(gridLogicPos, nCurColor);
		if (CompareAnswer () == true) {
			txtWin.enabled = true;
			bWin = true;
		}
	}

	void ClearMission() {
		LoadMission (0);
		DrawGridNum ();
		for (int x = 0; x < nRowCount; x++) {
			for (int y = 0; y < nColCount; y++) {
				ChangeGridColor(new Vector2(x, y), 0);
			}
		}
	}
}
