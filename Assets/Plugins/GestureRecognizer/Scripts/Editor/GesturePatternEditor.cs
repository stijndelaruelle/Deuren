﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace GestureRecognizer {
	
	/// <summary>
	/// Script to edit a "gesture".
	/// </summary>
	[CustomEditor(typeof(GesturePattern))]
	public class GesturePatternEditor : Editor{

		SerializedProperty gestures;

		void OnEnable(){
			gestures = serializedObject.FindProperty ("gesture").FindPropertyRelative ("lines");
		}

		int dragIndex;

		private static bool snap = false;

		int FindNear(SerializedProperty array, Vector2 pos){
			int nearIndex = 0;
			float minDist = Vector2.Distance(array.GetArrayElementAtIndex(0).vector2Value, pos);
			for (int i = 1; i < array.arraySize; i++) {
				var dist = Vector2.Distance(array.GetArrayElementAtIndex(i).vector2Value, pos);
				if (dist < minDist) {
					minDist = dist;
					nearIndex = i;
				}
			}
			return nearIndex;
		}

		const int RoundFactor = 16;

		public void RoundDrawing(SerializedProperty points){
			for (int i = 0; i < points.arraySize; i++) {
				var v = points.GetArrayElementAtIndex (i).vector2Value;
				v.x = Mathf.Round(v.x * RoundFactor) / RoundFactor;
				v.y = Mathf.Round (v.y * RoundFactor) / RoundFactor;
				points.GetArrayElementAtIndex (i).vector2Value = v;
			}
		}
		public void NegYDrawing(SerializedProperty points){
			for (int i = 0; i < points.arraySize; i++) {
				var e = points.GetArrayElementAtIndex (i);
				var v = e.vector2Value;
				v.y = 1f-v.y;
				e.vector2Value = v;
			}
		}
		public void NegXDrawing(SerializedProperty points){
			for (int i = 0; i < points.arraySize; i++) {
				var e = points.GetArrayElementAtIndex (i);
				var v = e.vector2Value;
				v.x = 1f-v.x;
				e.vector2Value = v;
			}
		}
		public void NormalizeDrawing(SerializedProperty points){
			Vector2 min = points.GetArrayElementAtIndex (0).vector2Value;
			Vector2 max = min;
			for (int i = 1; i < points.arraySize; i++) {
				var v = points.GetArrayElementAtIndex (i).vector2Value;
				min.x = Mathf.Min (min.x, v.x);
				min.y = Mathf.Min (min.y, v.y);
				max.x = Mathf.Max (max.x, v.x);
				max.y = Mathf.Max (max.y, v.y);
			}

			Rect rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

			float rectsize = Mathf.Max (rect.width, rect.height);
			rect = new Rect (rect.center - new Vector2 (rectsize / 2, rectsize / 2), new Vector2 (rectsize, rectsize));

			for (int i = 0; i < points.arraySize; i++) {
				var element = points.GetArrayElementAtIndex (i);
				element.vector2Value = Rect.PointToNormalized (rect, element.vector2Value);
			}
		}

		Vector2 NegY1(Vector2 v){
			return new Vector2 (v.x, 1f - v.y);
		}

		int currentGestureIndex = 0;

		void DrawGrid(Rect rect, int divs){
			for (int i = 0; i <= divs; i++) {
				float t = Mathf.InverseLerp (0, divs, i);
				float ty = Mathf.Lerp (rect.yMin, rect.yMax, t);
				float tx = Mathf.Lerp (rect.xMin, rect.xMax, t);
				Handles.DrawLine (new Vector3(rect.xMin, ty), new Vector3(rect.xMax, ty));
				Handles.DrawLine (new Vector3(tx, rect.yMin), new Vector3(tx, rect.yMax));
			}
		}
		void DrawDots(Vector2 p1, Vector2 p2, float dist){
			float d = Vector2.Distance (p1, p2);
			int n = Mathf.Max (Mathf.RoundToInt (d / dist + 1), 2);
			for (int i = 0; i <= n; i++) {
				float t = Mathf.InverseLerp (0, n, i);
				var p = Vector2.Lerp(p1, p2, t);
				Handles.DrawSolidRectangleWithOutline (new Rect (p - Vector2.one, Vector2.one * 3), Handles.color, Color.clear);
			}
		}

		public override void OnInspectorGUI () {

			{
				var current = target as GesturePattern;
				if (current != null && current.points != null && current.points.Count > 0 && current.gesture != null && current.gesture.lines.Count == 0) {
					if (GUILayout.Button ("Convert to new GesturePattern",GUILayout.ExpandWidth(true),GUILayout.Height(100))) {

						this.serializedObject.Update ();

						//one line
						gestures.arraySize = 1;

						//points size
						var points = gestures.GetArrayElementAtIndex (0).FindPropertyRelative ("points");
						points.arraySize = current.points.Count;

						//fill points
						for (int i = 0; i < current.points.Count; i++) {
							points.GetArrayElementAtIndex (i).vector2Value = current.points [i];
						}

						serializedObject.FindProperty ("points").arraySize = 0;

						serializedObject.ApplyModifiedProperties ();
					}
					return;
				}
			}

			DrawDefaultInspector ();

			this.serializedObject.Update ();

			float w = EditorGUIUtility.currentViewWidth * 0.75f;
			w = 201;

			if(gestures.arraySize > 0){
				currentGestureIndex = Mathf.Clamp (currentGestureIndex, 0, gestures.arraySize - 1);
			}else{
				currentGestureIndex = 0;
			}


			EditorGUILayout.Space();

			//----------------- add remove -----------------
			GUILayout.BeginHorizontal (GUILayout.Width (w));
			GUILayout.Label ("Lines:");
			GUI.enabled = gestures.arraySize > 0;
			if (GUILayout.Button ("Remove")) {
				gestures.DeleteArrayElementAtIndex (currentGestureIndex);
				currentGestureIndex = Mathf.Clamp (currentGestureIndex - 1, 0, gestures.arraySize - 1);
				Repaint ();
			}
			GUI.enabled = true;
			if(GUILayout.Button("Add")){
				gestures.arraySize = gestures.arraySize + 1;
				gestures.GetArrayElementAtIndex (gestures.arraySize - 1).FindPropertyRelative ("points").arraySize = 0;
				currentGestureIndex = gestures.arraySize - 1;
				Repaint ();
			}
			GUILayout.EndHorizontal ();



			//-------------- navigation -----------------
			GUILayout.BeginHorizontal (GUILayout.Width(w));
			GUILayout.Label ("Current:");
			GUI.enabled = gestures.arraySize > 0 && currentGestureIndex > 0;
			if (GUILayout.Button ("<",GUILayout.MinWidth(w/4))) {
				currentGestureIndex = Mathf.Max (currentGestureIndex - 1, 0);
			}
			GUILayout.FlexibleSpace ();
			if (gestures.arraySize > 0) {
				GUILayout.Label (string.Format ("{0} of {1}", currentGestureIndex + 1, gestures.arraySize));
			}
			GUILayout.FlexibleSpace ();
			GUI.enabled = gestures.arraySize > 0 && currentGestureIndex < gestures.arraySize-1;
			if (GUILayout.Button (">",GUILayout.MinWidth(w/4))) {
				currentGestureIndex = Mathf.Min (currentGestureIndex + 1, gestures.arraySize - 1);
			}
			GUI.enabled = true;
			GUILayout.EndHorizontal ();



			//----------------- draw background -----------------

			EditorGUILayout.Space ();
			var rect = EditorGUILayout.GetControlRect (GUILayout.Width (w), GUILayout.Height (w));
			EditorGUILayout.Space ();
			//Debug.Log (rect);
			GUI.Box (rect, GUIContent.none);
			Handles.DrawSolidRectangleWithOutline (rect, Color.gray, Color.gray);

			Handles.color = new Color(0f, 0f, 0f, 0.1f);
			DrawGrid (rect, 16);
			Handles.color = new Color(0f, 0f, 0f, 0.2f);
			DrawGrid (rect, 8);
			DrawGrid (rect, 4);
			DrawGrid (rect, 2);
			DrawGrid (rect, 2);

			//----------------- draw -----------------

			if (gestures.arraySize > 0) {

				currentGestureIndex = Mathf.Clamp (currentGestureIndex, 0, gestures.arraySize-1);

				for (int gestureIndex = 0; gestureIndex < gestures.arraySize; gestureIndex++) {

					var points = gestures.GetArrayElementAtIndex (gestureIndex).FindPropertyRelative ("points");

					//----------- draw strokes --------------
					Handles.color = Color.cyan;
					for (int i = 0; i < points.arraySize-1; i++) {
						var v1 = points.GetArrayElementAtIndex (i).vector2Value;
						var v2 = points.GetArrayElementAtIndex (i+1).vector2Value;
						v1 = Rect.NormalizedToPoint (rect, NegY1(v1));
						v2 = Rect.NormalizedToPoint (rect, NegY1(v2));

						Handles.DrawLine (v1, v2);

						if (gestureIndex == currentGestureIndex) {
							Handles.DrawLine (v1 + Vector2.up, v2 + Vector2.up);
							Handles.DrawLine (v1 + Vector2.left, v2 + Vector2.left);
							Handles.DrawLine (v1 + Vector2.right, v2 + Vector2.right);
							Handles.DrawLine (v1 + Vector2.down, v2 + Vector2.down);
						}
					}

					//------------ stroke editor ---------
					if (gestureIndex == currentGestureIndex) {

						float border = 5;
						var borderRect = Rect.MinMaxRect (rect.xMin - border, rect.yMin - border, rect.xMax + border, rect.yMax + border);

						var mousePos = Event.current.mousePosition;
						var localMousePos = NegY1 (Rect.PointToNormalized (rect, mousePos));

						bool insideBorderRect = borderRect.Contains (Event.current.mousePosition);

						// snap
						if (snap && insideBorderRect) {
							localMousePos.x = Mathf.Round (localMousePos.x * RoundFactor) / RoundFactor;
							localMousePos.y = Mathf.Round (localMousePos.y * RoundFactor) / RoundFactor;
							mousePos = Rect.NormalizedToPoint (rect, NegY1 (localMousePos));
						}

						bool onePoint = points.arraySize >= 1;
						bool twoPoints = points.arraySize >= 2;

						var nearIndex = onePoint ? FindNear (points, localMousePos) : -1;
						var nearPos = onePoint ? points.GetArrayElementAtIndex (nearIndex).vector2Value : Vector2.zero;
						float nearDist = onePoint ? Vector2.Distance (nearPos, localMousePos) : 0;

						float nearDistLimit = 0.05f;
						bool isNearPoint = onePoint && nearDist < nearDistLimit;

						// ----------- paint ------------
						if (Event.current.type == EventType.Repaint) {
							if (insideBorderRect) {
								if (isNearPoint) {
									//---- will drag point ----
									var globalNearPos = Rect.NormalizedToPoint (rect, NegY1 (nearPos));
									if (Event.current.control) {
										Handles.color = Color.red;
									} else {
										Handles.color = Color.cyan;
									}
									Handles.DrawSolidDisc (globalNearPos, Vector3.forward, 5f);
								} else {
									//----- will crate new point ----
									Handles.color = Color.cyan;
									if (points.arraySize > 0) {
										DrawDots (mousePos, Rect.NormalizedToPoint (rect, NegY1 (points.GetArrayElementAtIndex (points.arraySize - 1).vector2Value)), 10);
									}
									Handles.DrawSolidDisc (mousePos, Vector3.forward, 5f);
								}
							}
							Repaint ();
						}

						// ------------------- MouseDown --------------
						if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && insideBorderRect) {
							if (isNearPoint) {
								if (Event.current.control) {
									//delete
									if (nearIndex >= 0) {
										if (points.arraySize == 2) {
											points.arraySize = 0;
											Repaint ();
										} else {
											points.DeleteArrayElementAtIndex (nearIndex);
											currentGestureIndex = Mathf.Clamp (currentGestureIndex, 0, gestures.arraySize - 1);
											Repaint ();
										}
									}
								} else {
									//drag
									dragIndex = nearIndex;
									points.GetArrayElementAtIndex (dragIndex).vector2Value = localMousePos;
								}
							} else {
								points.InsertArrayElementAtIndex (points.arraySize);
								points.GetArrayElementAtIndex (points.arraySize - 1).vector2Value = localMousePos;
							}
						}

						// ------------------- MouseDrag --------------
						if (Event.current.type == EventType.MouseDrag && Event.current.button == 0 && insideBorderRect) {
							points.GetArrayElementAtIndex (dragIndex).vector2Value = localMousePos;
						}

						// ------------------- MouseUp --------------
						if (Event.current.type == EventType.MouseUp && Event.current.button == 0) {
							dragIndex = -1;
						}

						snap = EditorGUILayout.Toggle("Snap", snap);

						//GUILayout.Label ("Hold [Shift] to snap.");
						GUILayout.Label ("Hold [Ctrl] to delete.");

						GUILayout.BeginHorizontal (GUILayout.Width (w));
						if (GUILayout.Button ("Clear")) {
							points.ClearArray ();
						}
						//if (GUILayout.Button ("Del Last")) { points.DeleteArrayElementAtIndex (points.arraySize - 1); }
						if (GUILayout.Button ("Snap All")) {
							RoundDrawing (points);
						}
						GUILayout.EndHorizontal ();
						GUILayout.BeginHorizontal (GUILayout.Width (w));
						if (GUILayout.Button ("Expand")) {
							NormalizeDrawing(points);
						}
						if (GUILayout.Button ("Flip Y")) {
							NegYDrawing (points);
						}
						if (GUILayout.Button ("Flip X")) {
							NegXDrawing (points);
						}
						GUILayout.EndHorizontal ();

						//------------ end stroke editor ---------
					}


				}

			}

			serializedObject.ApplyModifiedProperties ();

		}

	}
}
