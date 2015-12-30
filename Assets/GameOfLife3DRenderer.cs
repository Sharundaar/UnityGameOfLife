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
    Color m_AliveColor1 = Color.yellow;

    [SerializeField]
    Color m_AliveColor2 = Color.magenta;

    [SerializeField]
    float m_Width = 10.0f;

    [SerializeField]
    float m_Height = 10.0f;

    private UnityEngine.UI.Button PlayPauseButton = null;

	// Use this for initialization
	void Start () {
        m_GameOfLife = GetComponent<GameOfLife>();
        if (m_GameOfLife.State == GameOfLife.GameState.UNINITIALIZE)
            m_GameOfLife.Initialize();

        if (m_GameOfLife != null && m_CellPrefab != null)
        {
            m_Cells = new GameObject[m_GameOfLife.Width * m_GameOfLife.Height];
            InitializeCells();
            m_GameOfLife.CellStateChanged += OnCellStateChanged;
            m_GameOfLife.GameStateChanged += OnGameStateChanged;
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
            m_Cells[i].GetComponent<Renderer>().material.color = m_GameOfLife.GetCellState(i) == 1 ? m_AliveColor1 : m_GameOfLife.GetCellState(i) == 2 ? m_AliveColor2 : m_DeadColor;

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

    void OnCellStateChanged(object sender, int cell, int oldState, int newState)
    {
        if((GameOfLife)sender == m_GameOfLife)
        {
            m_Cells[cell].GetComponent<Renderer>().material.color = newState == 1 ? m_AliveColor1 : newState == 2 ? m_AliveColor2 : m_DeadColor;
        }
    }

    void OnGameStateChanged(object sender, GameOfLife.GameState oldState, GameOfLife.GameState newState)
    {
        if (PlayPauseButton == null)
            return;

        if((GameOfLife)sender == m_GameOfLife)
        {
            switch(newState)
            {
                case GameOfLife.GameState.PLAY:
                    PlayPauseButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Pause";
                    break;
                case GameOfLife.GameState.PAUSE:
                    PlayPauseButton.GetComponentInChildren<UnityEngine.UI.Text>().text = "Play";
                    break;

                default:
                    break;
            }
        }
    }

    public void OnPlayPauseButtonClicked(UnityEngine.UI.Button PlayPauseButton)
    {
        if (PlayPauseButton != this.PlayPauseButton)
        {
            this.PlayPauseButton = PlayPauseButton;
        }
        
        switch(m_GameOfLife.State)
        {
            case GameOfLife.GameState.PAUSE:
                m_GameOfLife.Play();
                break;

            case GameOfLife.GameState.PLAY:
                m_GameOfLife.Pause();
                break;

            default:
                break;
        }
    }
}
