using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Draws a catmull-rom spline in the scene view,
// along the child objects of the transform of this component
public class CatmullRomSpline : MonoBehaviour {
	
	// Alpha : 0.5 for the centripetal spline
	[Range( 0, 1 )] public float alpha = 0.5f;
	
	// PonintCount is the number of points to include in this segement.
	int PointCount => transform.childCount;
	int SegmentCount => PointCount - 3;
	
	// For Test , Utilisateur entrer a set of points in order .
	Vector2 GetPoint( int i ) => transform.GetChild( i ).position;

	// Get one curve  from  the point i+1 to i+2
	CatmullRomCurve GetCurve( int i ) {
		return new CatmullRomCurve( GetPoint(i), GetPoint(i+1), GetPoint(i+2), GetPoint(i+3), alpha );
	}
	
	// Draw a combined curve with a chain of points
	void OnDrawGizmos() {
		for( int i = 0; i < SegmentCount; i++ )
			DrawCurveSegment( GetCurve( i ) );
			
	}
    
 	void DrawCurveSegment( CatmullRomCurve curve ) {
		const int detail = 32;
		Vector2 prev = curve.p1;
		for( int i = 1; i < detail; i++ ) {
			float t = i / ( detail - 1f );
			Vector2 pt = curve.GetPoint( t );
			Gizmos.DrawLine( prev, pt );
			prev = pt;
		}
	}
}