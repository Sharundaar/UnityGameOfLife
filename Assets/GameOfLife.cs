using UnityEngine;
using System.Collections;

public class GameOfLife : MonoBehaviour {

    public enum GameState
    {
        UNINITIALIZE,
        PLAY,
        PAUSE,
    }

    private GameState m_state = GameState.UNINITIALIZE;
    public GameState State
    {
        get { return m_state; }
        private set {
            GameState oldState = m_state;
            m_state = value;
            OnGameStateChanged(oldState, m_state);
        }
    }

    public int Width = 32;
    public int Height = 32;
    public bool WrapBorder = true;
    public bool OuterCellsAreDead = false;

    [Range(0, 1)]
    public float FillRate = 0.25f;

    public bool Running = false;

    [Range(0, 10)]
        public float StepRate = 1;

    [Range(1, 2)]
    public int CellStateCount = 2;

    private int[] m_cells;
    private int[] m_neighborsCount;

    private float m_timer;

    public void SetFillRate(UnityEngine.UI.Slider slider)
    {
        FillRate = Mathf.Clamp01(slider.value);
    }

    public void ToggleCellState(int x, int y)
    {
        ToggleCellState(x + y * Width);
    }

    public void ToggleCellState(int i)
    {
        int oldState = m_cells[i];
        m_cells[i] = (m_cells[i] + 1) % (CellStateCount + 1);
        OnCellStateChanged(i, oldState, m_cells[i]);
    }

    public bool IsCellAlive(int x, int y)
    {
        return IsCellAlive(x + y * Width);
    }

    public bool IsCellAlive(int i)
    {
        return m_cells[i] > 0;
    }

    public int GetCellState(int i)
    {
        return m_cells[i];
    }

    IEnumerable GetAliveCells()
    {
        for(int i=0; i<m_cells.Length; ++i)
        {
            if (IsCellAlive(i))
                yield return m_cells[i];
        }
    }

    public void Initialize()
    {
        Initialize(FillRate);
    }

    public void Initialize(float fillRate)
    {
        m_cells = new int[Width * Height];
        for (int i = 0; i < m_cells.Length; ++i)
        {
            float rnd = Random.value;
            m_cells[i] = rnd < fillRate ? Mathf.FloorToInt((rnd / fillRate) * (CellStateCount + 1)): 0;
            OnCellStateChanged(i, 0, m_cells[i]);
        }

        m_neighborsCount = new int[Width * Height * CellStateCount];

        State = GameState.PAUSE;
    }

	// Use this for initialization
	void Start () {
        if (m_state != GameState.UNINITIALIZE)
            return;

        Initialize(FillRate);     
	}
	
	// Update is called once per frame
	void Update () {
	    if(Running)
        {
            m_timer += Time.deltaTime;
            if(m_timer >= StepRate)
            {
                m_timer -= StepRate;
                GameStep();
            }
        }
	}

    private void GameStep()
    {
        for(int i=0; i<m_cells.Length; ++i)
        {
            int x = i % Width;
            int y = i / Width;

            for(int j=0; j<CellStateCount; ++j)
            {
                int ncount = CountActiveNeighbors(x, y, j+1);
                m_neighborsCount[i + j * Width * Height] = ncount;
            }
        }

        for(int i=0; i<m_cells.Length; ++i)
        {
            int oldState = m_cells[i];
            int futureState = 0;
            
            for(int j=0; j< CellStateCount; ++j)
            {
                if ((m_neighborsCount[i + Width * Height * j] == 2 || m_neighborsCount[i + Width * Height * j] == 3) 
                    && m_cells[i] == j+1)
                    futureState = j+1;
                else if (m_neighborsCount[i + Width * Height * j] == 3 && m_cells[i] == 0)
                {
                    if(futureState == 0)
                        futureState = j+1;
                }
            }

            m_cells[i] = futureState;
            if (oldState != m_cells[i])
                OnCellStateChanged(i, oldState, m_cells[i]);
        }
    }

    public void Play()
    {
        Running = true;
        State = GameState.PLAY;
    }

    public void Pause()
    {
        Running = false;
        State = GameState.PAUSE;
    }

    public void Clear()
    {
        Initialize(0);
    }

    public void Reset()
    {
        Initialize(FillRate);
    }

    private int CountActiveNeighbors(int x, int y, int state)
    {
        int count = 0;
        for(int i=x-1; i<=x+1; ++i)
        {
            for(int j=y-1; j<=y+1; ++j)
            {
                if (i == x && j == y)
                    continue;

                if(i < 0 || j < 0 || i >= Width || j >= Height)
                {
                    if(WrapBorder)
                    {
                        int ni = 0, nj = 0;
                        if (i < 0)
                            ni = Width + i;
                        else if (i >= Width)
                            ni = i - Width;
                        else
                            ni = i;

                        if (j < 0)
                            nj = Height + j;
                        else if (j >= Height)
                            nj = j - Height;
                        else
                            nj = j;

                        if (m_cells[ni + nj * Width] == state)
                            count++;
                    }
                    else
                    {
                        if (!OuterCellsAreDead)
                            count++;
                    }
                }
                else
                {
                    if (m_cells[i + j * Width] == state)
                        count++;
                }
            }
        }

        return count;
    }

    #region Events

    public delegate void CellStateChangedEventHandler(object sender, int cell, int oldState, int newState);
    public event CellStateChangedEventHandler CellStateChanged;

    private void OnCellStateChanged(int cell, int oldState, int newState)
    {
        if (CellStateChanged != null)
            CellStateChanged(this, cell, oldState, newState);
    }

    public delegate void GameStateChangedEventHandler(object sender, GameState oldState, GameState newState);
    public event GameStateChangedEventHandler GameStateChanged;

    private void OnGameStateChanged(GameState oldState, GameState newState)
    {
        if (GameStateChanged != null)
            GameStateChanged(this, oldState, newState);
    }
    #endregion
}
