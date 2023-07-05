using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using DentedPixel;

public class PathSpline2d : MonoBehaviour {

	public List<Transform> cubes;

	public GameObject dude1;
	public GameObject dude2;

	private LTSpline visualizePath;

	void Start () {
		/*
		Vector3[] path = new Vector3[] {
			cubes[0].position,
			cubes[1].position,
			cubes[2].position,
			cubes[3].position,
			cubes[4].position,
			cubes[5].position
		};
		*/

		Vector3[] path = new Vector3[cubes.Count];
		for (int i = 0; i < path.Length; ++i) {
			path[i] = getPosition(cubes[i]);
		}

		visualizePath = new LTSpline( path );
		// move
		LeanTween.moveSpline(dude1, path, 10f).setOrientToPath2d(true).setSpeed(6f).setEase(LeanTweenType.easeInOutQuad);

		// move Local
		LeanTween.moveSplineLocal(dude2, path, 10f).setOrientToPath2d(true).setSpeed(2f);
	}

	Vector2 getPosition(Transform cube) {
		return new Vector3(cube.position.x, cube.position.y);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.red;
		if(visualizePath!=null)
			visualizePath.gizmoDraw();
	}
}
