using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameOfLife))]
public class GameOfLifeRenderer : MonoBehaviour {
    GameOfLife gol;

    public float Width = 1.0f;
    public float Height = 1.0f;
    public float Padding = 0.1f;

	// Use this for initialization
	void Start () {
        gol = GetComponent<GameOfLife>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetMouseButtonUp(0))
        {
            RaycastHit2D hit;
            Vector3 pos = Input.mousePosition;
            pos.z = 1;
            var ray = Camera.main.ScreenPointToRay(pos);
            hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue);
            if(hit.collider != null)
            {
                float x = hit.point.x;
                float y = hit.point.y;

                int cx = (int) ((x + Width / 2.0f) / Width * gol.Width);
                int cy = (int) ((y + Height / 2.0f) / Height * gol.Height);

                gol.ToggleCellState(cx, cy);
            }
        }
	}

    void OnDrawGizmos()
    {
        if (gol == null)
            return;

        int widthCount = gol.Width;
        int heightCount = gol.Height;

        float cellWidth = (Width - Padding * (widthCount - 1.0f)) / widthCount;
        float cellHeight = (Height - Padding * (heightCount - 1.0f)) / heightCount;

        float x = 0, y = 0;
        for(int i=0; i<widthCount; ++i)
        {
            y = 0;
            for(int j=0; j<heightCount; ++j)
            {
                Gizmos.color = gol.IsCellAlive(i, j) ? Color.yellow : Color.grey;
                Vector3 pos = new Vector3(-Width / 2.0f + x + cellWidth / 2.0f, -Height / 2.0f + y + cellHeight / 2.0f, 0);
                Gizmos.DrawCube(pos, Vector3.one);

                y += cellHeight + Padding;
            }
            x += cellWidth + Padding;
        }
    }

    
}
