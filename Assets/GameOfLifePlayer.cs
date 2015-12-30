using UnityEngine;
using System.Collections;

public class GameOfLifePlayer : MonoBehaviour {

    public GameOfLife3DRenderer gol;

	void Update () {
        if (gol == null)
            return;

	    if(Input.GetMouseButtonUp(0))
        {
            Vector3 position = Input.mousePosition;
            position.z = 10.0f;

            Ray ray = Camera.main.ScreenPointToRay(position);
            gol.Raycast(ray);
        }
	}
}
