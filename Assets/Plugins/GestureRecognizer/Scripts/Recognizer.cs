﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;



namespace GestureRecognizer {

	/// <summary>
	/// Classe to hold the result score of a comparison between two gestures.
	/// The score gives preference to the position of each line.
	/// </summary>
	public struct Score{
		public float positionDistance;
		public float curvatureDistance;
		public float angleDistance;
		public float score {
			get{ 
				float posScore = Mathf.Clamp01 (1f - positionDistance / 50);
				float curvScore = Mathf.Clamp01 (1f - curvatureDistance / 50);
				float angleScore = Mathf.Clamp01 (1f - angleDistance / 50);
				return Mathf.Clamp01 ((4 * posScore + 1 * curvScore + 1 * angleScore) / 6);
			}
		}

		public void InitMax(){
			positionDistance = curvatureDistance = angleDistance = float.MaxValue;
		}

		public static Score MaxDistance {
			get{
				var result = new Score ();
				result.InitMax ();
				return result;
			}
		}

		public static bool operator >(Score s1, Score s2){
			return s1.score > s2.score;
		}
		public static bool operator <(Score s1, Score s2){
			return s1.score < s2.score;
		}
		public static bool operator >=(Score s1, Score s2){
			return s1.score >= s2.score;
		}
		public static bool operator <=(Score s1, Score s2){
			return s1.score <= s2.score;
		}
	}

	/// <summary>
	/// Class to hold the recognition result, with the found gesture and score.
	/// </summary>
	public class RecognitionResult{
		public GesturePattern gesture;
		public Score score;

        //Added: Stijn
        public GestureData gestureData;

		private static RecognitionResult empty = new RecognitionResult ();
		public static RecognitionResult Empty { get { return empty; } }
	}

	/// <summary>
	/// Class responsible for recognize a drawing in a list of gestures patterns.
	/// </summary>
	public class Recognizer : MonoBehaviour {

		private const int Detail = 100;

		public List<GesturePattern> patterns;

		public RecognitionResult Recognize(GestureData data){

			var normData = NormalizeData (data);

			var found = findPattern(normData);

			return found;
		}

		private GestureData NormalizeData(GestureData data){
			return NormalizeDistribution (NormalizeScale (data), Detail);
		}

		private static List<List<int>> GenPermutations(List<int> list, int low = 0){

			System.Action<int,int> swap = (int a, int b) => {
				var temp = list[a];
				list[a] = list[b];
				list[b] = temp;
			};

			var result = new List<List<int>> ();

			if (low + 1 >= list.Count) {
				result.Add (new List<int>(list));
			} else {
				foreach (var p in GenPermutations(list, low + 1)) {
					result.Add (new List<int>(p));
				}
				for (int i = low + 1; i < list.Count; i++) {
					swap (low, i);
					foreach (var p in GenPermutations(list, low + 1)) {
						result.Add (new List<int>(p));
					}
					swap (low, i);
				}
			}
			return result;
		}

		private GestureData makePermutation(List<int> indexes, GestureData data){
			return new GestureData (){ 
				lines = indexes.Select( e => data.lines[e] ).ToList()
			};
		}

		private RecognitionResult findPattern(GestureData queryData){

			var bestGesture = default(GesturePattern);
			var bestScore = Score.MaxDistance;

			var indexes = Enumerable.Range (0, queryData.lines.Count).ToList();
			List<List<int>> permutIndexes = GenPermutations (indexes);

			var permutations = permutIndexes.Select (e => makePermutation (e, queryData)).ToList ();

			for (int i = 0; i < patterns.Count; i++) {

				var gestureAsset = patterns [i];
				var assetData = NormalizeData (gestureAsset.gesture);

				if (assetData.lines.Count != queryData.lines.Count) {
					//ignore wrong sizes
					continue;
				}

				foreach (var data in permutations) {

					var permutScore = CalcScore (assetData, data);

					float pd = permutScore.positionDistance;
					float cd = permutScore.curvatureDistance;
					float ad = permutScore.angleDistance;

					if (permutScore > bestScore) {
						bestScore = permutScore;
						bestGesture = gestureAsset;
					}
				}


			}

			return new RecognitionResult(){gesture = bestGesture, score = bestScore};
		}

		private Score CalcScore(GestureData data1, GestureData data2){

			if (data1.lines.Count == data2.lines.Count) {

				var lineScores = new List<Score> ();

				for (int i = 0; i < data1.lines.Count; i++) {

					var line1 = data1.lines [i].points;

					var line2A = data2.lines [i].points;//forward
					var line2B = line2A.AsEnumerable ().Reverse ().ToList ();//reversed

					var scoreA = CalcScore (line1, line2A);
					var scoreB = CalcScore (line1, line2B);

					//gets best score between forward and reversed
					var score = scoreA > scoreB ? scoreA : scoreB;
					lineScores.Add (score);
				}

				//mean score from lines
				return new Score (){
					positionDistance = lineScores.Select( e => e.positionDistance).Sum() / lineScores.Count,
					angleDistance = lineScores.Select( e => e.angleDistance).Sum() / lineScores.Count,
					curvatureDistance = lineScores.Select( e => e.curvatureDistance).Sum() / lineScores.Count
				};

			} else {
				return Score.MaxDistance;
			}
		}


		private List<float> CalcAngles(List<Vector2> points){
			int step = 10;
			List<float> result = new List<float> ();

			for (int i = 0; i < points.Count; i++) {
				int i1 = Mathf.Max (i - step, 0);
				int i2 = Mathf.Min (i + step, points.Count - 1);
				var v1 = points [i1];
				var v2 = points [i2];
				var dir = v2 - v1;
				var angle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg;
				if (angle < 0)
					angle += 360;
				result.Add (angle);
			}
			return result;
		}

		private List<float> CalcCurvature(List<Vector2> points){
			int step = 10;
			List<float> result = new List<float> ();
			for (int i = 0; i < step; i++)
				result.Add (0);
			for (int i = step; i < points.Count-step; i++) {
				var p1 = points [i - step];
				var p2 = points [i];
				var p3 = points [i + step];
				var v1 = p2 - p1;
				var v2 = p3 - p2;
				var angle1 = Mathf.Atan2 (v1.y, v1.x) * Mathf.Rad2Deg;
				var angle2 = Mathf.Atan2 (v2.y, v2.x) * Mathf.Rad2Deg;
				var angle = Mathf.DeltaAngle (angle1, angle2);
				result.Add (angle);
			}
			for (int i = 0; i < step; i++)
				result.Add (0);
			return result;
		}

		private Score CalcScore(List<Vector2> points1, List<Vector2> points2){

			float posDist = CalcPositionDistance (points1, points2);
			float curvDist = CalcCurvatureDistance (points1, points2);
			float angleDist = CalcAngleDistance (points1, points2);

			return new Score (){ positionDistance = posDist, curvatureDistance = curvDist, angleDistance = angleDist };
		}

		private float CalcPositionDistance(List<Vector2> points1, List<Vector2> points2){
			float sqrt2 = Mathf.Sqrt (2);

			float sumDistance = 0;

			int n = points1.Count;
			for (int i = 0; i < n; i++) {
				float dif = Vector2.Distance (points1 [i], points2 [i]) / sqrt2;
				sumDistance += dif;
			}

			return sumDistance;
		}

		private float CalcCurvatureDistance(List<Vector2> points1, List<Vector2> points2){

			int n = points1.Count;

			var curv1 = CalcCurvature (points1);
			var curv2 = CalcCurvature (points2);

			float sumCurvDistance = 0;

			for (int i = 0; i < n; i++) {
				float dif = Mathf.Abs (curv1 [i] - curv2 [i]) / 360f;
				sumCurvDistance += dif;
			}

			return sumCurvDistance;
		}

		private float CalcAngleDistance(List<Vector2> points1, List<Vector2> points2){

			int n = points1.Count;

			var angles1 = CalcAngles (points1);
			var angles2 = CalcAngles (points2);

			float sumAngleDistance = 0;

			for (int i = 0; i < n; i++) {
				float dif = Mathf.Abs(Mathf.DeltaAngle (angles1 [i], angles2 [i])) / 360f;
				sumAngleDistance += dif;
			}

			return sumAngleDistance;
		}

		private Rect CalcRect(GestureData data){
			float minx, miny, maxx, maxy;
			minx = maxx = data.lines [0].points [0].x;
			miny = maxy = data.lines [0].points [0].y;

			for (int j = 0; j < data.lines.Count; j++) {
				var points = data.lines [j].points;

				for (int i = 0; i < points.Count; i++) {

					var p = points [i];
					minx = Mathf.Min (minx, p.x);
					maxx = Mathf.Max (maxx, p.x);
					miny = Mathf.Min (miny, p.y);
					maxy = Mathf.Max (maxy, p.y);
				}
			}
			Rect rect = Rect.MinMaxRect (minx, miny, maxx, maxy);
			float rectsize = Mathf.Max (rect.width, rect.height);
			rect = new Rect (rect.center - new Vector2 (rectsize / 2, rectsize / 2), new Vector2 (rectsize, rectsize));
			return rect;
		}

		private GestureData NormalizeScale(GestureData data){
			var rect = CalcRect (data);
			var result = new GestureData ();
			foreach (var line in data.lines) {
				result.lines.Add (new GestureLine () { 
					points = line.points.Select (e => Rect.PointToNormalized (rect, e)).ToList ()
				});
			}
			return result;
		}

		private Vector2 FindByNormalized(List<Vector2> vs, List<float> ts, float t){
			for (int i = 0; i < ts.Count-1; i++) {
				var t1 = ts [i];
				var t2 = ts [i + 1];
				if (t1 <= t && t <= t2) {
					var v1 = vs [i];
					var v2 = vs [i + 1];
					float tt = Mathf.InverseLerp (t1, t2, t);
					return Vector2.Lerp (v1, v2, tt);
				}
			}
			return t > 0.5f ? vs [vs.Count - 1] : vs [0];
		}

		private GestureData NormalizeDistribution(GestureData data, int n) {
			var result = new GestureData ();
			foreach (var line in data.lines) {
				result.lines.Add (new GestureLine (){ points = NormalizeDistribution (line.points, n) });
			}
			return result;
		}

		private List<Vector2> NormalizeDistribution(List<Vector2> path, int n) {

			List<float> realPos = new List<float> ();

			realPos.Add (0);
			for (int i = 1; i < path.Count; i++) {
				var v1 = path [i-1];
				var v2 = path [i];
				realPos.Add (realPos [i - 1] + Vector2.Distance (v1, v2));
			}

			float totalDist = realPos.Last ();

			var normPos = realPos.Select (e => e / totalDist).ToList ();

			var result = new List<Vector2> ();

			for (int ti = 0; ti <= n; ti++) {
				float t = (float)ti / n;
				result.Add(FindByNormalized(path, normPos, t));
			}

			return result;
		}

	}


}