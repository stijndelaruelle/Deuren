using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GestureRecognizer {

	/// <summary>
	/// Classes to store gesture lines.
	/// </summary>

	[System.Serializable]
	public class GestureLine {
		public List<Vector2> points = new List<Vector2> ();

        //Added: Stijn
        public Vector2 FirstPoint
        {
            get
            {
                if (points.Count > 0) { return points[0]; }
                else { return Vector2.zero; }
            }
        }

        public Vector2 LastPoint
        {
            get
            {
                if (points.Count > 0) { return points[points.Count - 1]; }
                else { return Vector2.zero; }
            }
        }
    }

	[System.Serializable]
	public class GestureData {
		public List<GestureLine> lines = new List<GestureLine>();

        //Added: Stijn
        public GestureLine FirstLine
        {
            get
            {
                if (lines.Count > 0)
                    return lines[0];
                else
                    return null;
            }
        }

        public GestureLine LastLine
        {
            get
            {
                if (lines.Count > 0)
                    return lines[lines.Count - 1];
                else
                    return null;
            }
        }

        public Vector2 FirstPoint
        {
            get
            {
                GestureLine firstLine = FirstLine;
                if (firstLine != null) { return firstLine.FirstPoint; }
                else { return Vector2.zero; }
            }
        }

        public Vector2 LastPoint
        {
            get
            {
                GestureLine lastLine = LastLine;
                if (lastLine != null) { return lastLine.LastPoint; }
                else { return Vector2.zero; }
            }
        }

        public Vector2 GetCenter()
        {
            Vector2 averagePoint = Vector2.zero;
            int numPoints = 0;

            foreach(GestureLine line in lines)
            {
                foreach (Vector2 point in line.points)
                {
                    averagePoint += point;
                    ++numPoints;
                }
            }

            averagePoint.x /= numPoints;
            averagePoint.y /= numPoints;

            return averagePoint;
        }
    }

}