﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.Events;

namespace GestureRecognizer {

	/// <summary>
	/// Captures player drawing and call the Recognizer to discover which gesture player id.
	/// Calls 'OnRecognize' event when something is recognized.
	/// </summary>
	public class DrawDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler {
		
		public Recognizer recognizer;

		public UILineRenderer line;
		private List<UILineRenderer> lines;

		[Range(0f,1f)]
		public float scoreToAccept = 0.8f;

		[Range(1,10)]
		public int minLines = 1;
		public int MinLines { set { minLines = Mathf.Clamp (value, 1, 10); } }

		[Range(1,10)]
		public int maxLines = 2;
		public int MaxLines { set { maxLines = Mathf.Clamp (value, 1, 10); } }

		public enum RemoveStrategy { RemoveOld, ClearAll }
		public RemoveStrategy removeStrategy;

		public bool clearNotRecognizedLines;

		GestureData data = new GestureData();

		[System.Serializable]
		public class ResultEvent : UnityEvent<RecognitionResult> {}
		public ResultEvent OnRecognize;

		RectTransform rectTransform;

		void Start(){
			line.relativeSize = true;
			line.LineList = false;
			lines = new List<UILineRenderer> (){ line };
			rectTransform = transform as RectTransform;
			UpdateLines ();
		}

		void OnValidate(){
			maxLines = Mathf.Max (minLines, maxLines);
		}

		public void UpdateLines(){
			while (lines.Count < data.lines.Count) {
				var newLine = Instantiate (line, line.transform.parent);
				lines.Add (newLine);
			}
			for (int i = 0; i < lines.Count; i++) {
				lines [i].Points = new Vector2[]{ };
				lines [i].SetAllDirty ();
			}
			for (int i = 0; i < data.lines.Count; i++) {
				lines [i].Points = data.lines [i].points.Select (p => RealToLine (p)).ToArray ();
				lines [i].SetAllDirty ();
			}
		}

		Vector2 RealToLine(Vector2 position){
			var local = rectTransform.InverseTransformPoint (position);
			var normalized = Rect.PointToNormalized (rectTransform.rect, local);
			return normalized;
		}

		Vector2 FixedPosition(Vector2 position){
			return position;
			//var local = rectTransform.InverseTransformPoint (position);
			//var normalized = Rect.PointToNormalized (rectTransform.rect, local);
			//return normalized;
		}

		public void ClearLines(){
			data.lines.Clear ();
			UpdateLines ();
		}

		public void OnPointerClick (PointerEventData eventData) {

		}

		public void OnBeginDrag (PointerEventData eventData) {

			if (data.lines.Count >= maxLines) {
				switch (removeStrategy) {
				case RemoveStrategy.RemoveOld:
					data.lines.RemoveAt (0);
					break;
				case RemoveStrategy.ClearAll:
					data.lines.Clear ();
					break;
				}
			}

			data.lines.Add (new GestureLine ());

			data.LastLine.points.Add (FixedPosition(eventData.position));
			UpdateLines ();
		}

		public void OnDrag (PointerEventData eventData) {
			data.LastLine.points.Add (FixedPosition(eventData.position));
			UpdateLines ();
		}

		public void OnEndDrag (PointerEventData eventData)
		{
			data.LastLine.points.Add (FixedPosition(eventData.position));
			UpdateLines ();

			for (int size = data.lines.Count; size >= 1 && size >= minLines; size--) {
				//last [size] lines
				var sizedData = new GestureData () {
					lines = data.lines.GetRange (data.lines.Count - size, size)
				};

				var result = recognizer.Recognize(sizedData);

				if (result.gesture != null && result.score.score >= scoreToAccept) {
					OnRecognize.Invoke (result);
					if (clearNotRecognizedLines) {
						data = sizedData;
						UpdateLines ();
					}
					break;
				} else {
					OnRecognize.Invoke (RecognitionResult.Empty);
				}
			}
		}

	}

}