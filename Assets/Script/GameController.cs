using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	int nWidth;						// 关卡宽度
	int nHeight;					// 关卡高度
	int[,] MissionData;				// 关卡数据，例：[1, 2]为第一行第二列的颜色号，方向为左下到右上
	int[,] AnswerData;				// 玩家解答数据

	List<Vector2>[] RowNums;		// 从关卡数据分析出的每行的色块数量
	List<Vector2>[] ColumnNums;		// 从关卡数据分析出的每列的色块数量

	// Use this for initialization
	void Start () {
		LoadMission (0);
		DrawCells (nWidth, nHeight);
		DrawGridNum ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void DrawCells(int wCount, int hCount){
		//画出背景格子
		DrawGrid (new Vector3 (-2, -2, 0), 0.5f, wCount, hCount, 0.03f);
		DrawGrid (new Vector3 (-2, -2, 0), 2.5f, wCount / 5, hCount / 5, 0.06f);
	}

	void DrawCellButton(){
		//画出用户点击的格子对象
	}

	void DrawGridNum(){
		//画出格子旁边的关卡数字
		GameObject NumImage = GameObject.Find("NumImage");
		GameObject num = GameObject.Instantiate(NumImage, NumImage.transform);
		num.transform.parent = NumImage.transform.parent;
		num.transform.position = new Vector3(NumImage.transform.position.x - 0.5f, NumImage.transform.position.y, NumImage.transform.position.z);
		num.transform.localScale = new Vector3(0.25f, 0.25f, 1);
	}

	void LoadMission(int MissionID){
		//读取关卡
		nWidth = 10;
		nHeight = 10;
		MissionData = new int[nHeight, nWidth];
		for (int i = 0; i < nHeight; i++) {
			for (int j = 0; j < nWidth; j++) {
				MissionData [i, j] = UnityEngine.Random.Range (1, 4);
			}
		}

		RowNums = new List<Vector2>[nHeight];
		for (int row = 0; row < nHeight; row++) {
			RowNums [row] = new List<Vector2>();
			List<Vector2> rowData = RowNums [row];
			int nLastColor = 0;
			for (int col = 0; col < nWidth; col++) {
				int nIdx = rowData.Count - 1;
				if (nIdx >= 0 && rowData [nIdx].x == MissionData[row, col]) {
					rowData [nIdx] = new Vector2 (nLastColor, rowData [nIdx].y + 1);
				} else {
					rowData.Add (new Vector2 (MissionData[row, col], 1));
				}
			}
		}

		ColumnNums = new List<Vector2>[nHeight];
		for (int col = 0; col < nHeight; col++) {
			ColumnNums [col] = new List<Vector2>();
			List<Vector2> colData = ColumnNums [col];
			int nLastColor = 0;
			for (int row = 0; row < nWidth; row++) {
				int nIdx = colData.Count - 1;
				if (nIdx >= 0 && colData [nIdx].x == MissionData[row, col]) {
					colData [nIdx] = new Vector2 (nLastColor, colData [nIdx].y + 1);
				} else {
					colData.Add (new Vector2 (MissionData[row, col], 1));
				}
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

	void DrawRect(Vector3 leftBottom, float fLength) {
		GameObject line = new GameObject();
		line.AddComponent<LineRenderer>();
		LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
		lineRenderer.startColor = Color.black;
		lineRenderer.endColor = Color.black;
		lineRenderer.useWorldSpace = true;
		lineRenderer.positionCount = 5;
		Vector3[] pos = new Vector3[5];
		pos [0] = leftBottom;
		pos [1] = new Vector3(leftBottom.x + fLength, leftBottom.y, leftBottom.z);
		pos [2] = new Vector3(leftBottom.x + fLength, leftBottom.y + fLength, leftBottom.z);
		pos [3] = new Vector3(leftBottom.x, leftBottom.y+ fLength, leftBottom.z);
		pos [4] = leftBottom;
		lineRenderer.SetPositions (pos);
		lineRenderer.startWidth = 0.03f;
		lineRenderer.endWidth = 0.03f;
	}

	void DrawGrid(Vector3 startPos, float fGridLengh, int nWCount, int nHCount, float fLintWidth) {
		for (int i = 0; i <= nWCount; i++) {
			GameObject line = new GameObject();
			line.AddComponent<LineRenderer>();
			LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
			lineRenderer.startColor = Color.black;
			lineRenderer.endColor = Color.black;
			lineRenderer.useWorldSpace = true;
			lineRenderer.positionCount = 2;
			Vector3[] pos = new Vector3[5];
			pos [0] = new Vector3(startPos.x, startPos.y + i * fGridLengh, startPos.z);
			pos [1] = new Vector3(startPos.x + nWCount * fGridLengh, startPos.y + i * fGridLengh, startPos.z);
			lineRenderer.SetPositions (pos);
			lineRenderer.startWidth = fLintWidth;
			lineRenderer.endWidth = fLintWidth;
		}

		for (int i = 0; i <= nHCount; i++) {
			GameObject line = new GameObject();
			line.AddComponent<LineRenderer>();
			LineRenderer lineRenderer = line.GetComponent<LineRenderer>();
			lineRenderer.startColor = Color.black;
			lineRenderer.endColor = Color.black;
			lineRenderer.useWorldSpace = true;
			lineRenderer.positionCount = 2;
			Vector3[] pos = new Vector3[5];
			pos [0] = new Vector3(startPos.x + i * fGridLengh, startPos.y, startPos.z);
			pos [1] = new Vector3(startPos.x + + i * fGridLengh, startPos.y + nHCount * fGridLengh, startPos.z);
			lineRenderer.SetPositions (pos);
			lineRenderer.startWidth = fLintWidth;
			lineRenderer.endWidth = fLintWidth;
		}
	}
}
