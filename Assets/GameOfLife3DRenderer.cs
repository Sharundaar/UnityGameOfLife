using UnityEngine;
using System.Collections;

[RequireComponent(typeof(GameOfLife))]
public class GameOfLife3DRenderer : MonoBehaviour {
    GameOfLife m_GameOfLife;

    [SerializeField]
    GameObject m_CellPrefab;

    [SerializeField]
    float m_Padding = 0.1f;

    bool m_bGoodToGo = false;

    GameObject[] m_Cells;

    [SerializeField]
    Color m_DeadColor = Color.grey;

    [SerializeField]
    Color m_AliveColor = Color.yellow;

    [SerializeField]
    float m_Width = 10.0f;

    [SerializeField]
    float m_Height = 10.0f;

	// Use this for initialization
	void Start () {
        m_GameOfLife = GetComponent<GameOfLife>();
        if (!m_GameOfLife.Started)
            m_GameOfLife.Initialize();

        if (m_GameOfLife != null && m_CellPrefab != null)
        {
            m_Cells = new GameObject[m_GameOfLife.Width * m_GameOfLife.Height];
            InitializeCells();
            m_GameOfLife.CellStateChanged += OnCellStateChanged;
            m_bGoodToGo = true;
        }
	}

    void InitializeCells()
    {
        Vector2 cellSize = ComputeCellSize();
        Vector3 position = new Vector3(-m_Width / 2.0f, -m_Height / 2.0f);

        for(int i=0; i<m_Cells.Length; ++i)
        {
            if (i % m_GameOfLife.Width == 0 && i > 0)
            {
                position.x = -m_Width / 2.0f;
                position.y += m_Padding + cellSize.y;
            }

            m_Cells[i] = Instantiate<GameObject>(m_CellPrefab);
            m_Cells[i].transform.parent = transform;
            m_Cells[i].transform.position = position;
            m_Cells[i].transform.localScale = new Vector3(cellSize.x, cellSize.y, cellSize.x);
            m_Cells[i].name = "Cell_" + i;
            m_Cells[i].GetComponent<Renderer>().material.color = m_GameOfLife.IsCellAlive(i) ? m_AliveColor : m_DeadColor;

            position.x += m_Padding + cellSize.x;
        }
    }

    public bool Raycast(Ray ray)
    {
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit))
        {
            for(int i=0; i<m_Cells.Length; ++i)
            {
                if(m_Cells[i] == hit.collider.gameObject)
                {
                    m_GameOfLife.ToggleCellState(i);
                    return true;
                }
            }
        }

        return false;
    }

    Vector2 ComputeCellSize()
    {
        Vector2 size = new Vector2();

        float width_no_padding = m_Width - m_Padding * (m_GameOfLife.Width - 1);
        float height_no_padding = m_Height - m_Padding * (m_GameOfLife.Height - 1);

        size.x = width_no_padding / m_GameOfLife.Width;
        size.y = height_no_padding / m_GameOfLife.Height;

        return size;
    }

    void OnCellStateChanged(object sender, int cell, bool oldState, bool newState)
    {
        if((GameOfLife)sender == m_GameOfLife)
            m_Cells[cell].GetComponent<Renderer>().material.color = newState ? m_AliveColor : m_DeadColor;
    }
}
