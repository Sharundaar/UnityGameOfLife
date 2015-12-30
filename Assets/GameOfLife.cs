using UnityEngine;
using System.Collections;

public class GameOfLife : MonoBehaviour {

    public int Width = 32;
    public int Height = 32;
    public bool WrapBorder = true;
    public bool OuterCellsAreDead = false;

    [Range(0, 1)]
    public float FillRate = 0.25f;

    public bool Running = false;
    private bool m_started = false;

    public bool Started {
        get
        {
            return m_started;
        }

        private set
        {
            m_started = value;
        }
    }

    [Range(0, 10)]
        public float StepRate = 1;    

    private bool[] m_cells;
    private int[] m_neighborsCount;

    private float m_timer;

    public void ToggleCellState(int x, int y)
    {
        m_cells[x + y * Width] = !m_cells[x + y * Width];
    }

    public void ToggleCellState(int i)
    {
        bool oldState = m_cells[i];
        m_cells[i] = !oldState;
        OnCellStateChanged(i, oldState, m_cells[i]);
    }

    public bool IsCellAlive(int x, int y)
    {
        return m_cells[x + y * Width];
    }

    public bool IsCellAlive(int i)
    {
        return m_cells[i];
    }

    public void Initialize()
    {
        Initialize(FillRate);
    }

    public void Initialize(float fillRate)
    {
        m_cells = new bool[Width * Height];
        for (int i = 0; i < m_cells.Length; ++i)
        {
            m_cells[i] = Random.value < fillRate ? true : false;
            OnCellStateChanged(i, false, m_cells[i]);
        }

        m_neighborsCount = new int[Width * Height];

        Started = true;

    }

	// Use this for initialization
	void Start () {
        if (Started)
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

            int ncount = CountActiveNeighbors(x, y);
            m_neighborsCount[i] = ncount;
        }

        for(int i=0; i<m_cells.Length; ++i)
        {
            bool oldState = m_cells[i];

            if (m_neighborsCount[i] < 2)
                m_cells[i] = false;
            else if (m_neighborsCount[i] > 3)
                m_cells[i] = false;
            else if (m_neighborsCount[i] == 3 && m_cells[i] == false)
                m_cells[i] = true;

            if (oldState != m_cells[i])
                OnCellStateChanged(i, oldState, m_cells[i]);
        }
    }

    public void Play()
    {
        Running = true;
    }

    public void Pause()
    {
        Running = false;
    }

    public void Clear()
    {
        Initialize(0);
    }

    public void Reset()
    {
        Initialize(FillRate);
    }

    private int CountActiveNeighbors(int x, int y)
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

                        if (m_cells[ni + nj * Width])
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
                    if (m_cells[i + j * Width])
                        count++;
                }
            }
        }

        return count;
    }

    public delegate void CellStateChangedEventHandler(object sender, int cell, bool oldState, bool newState);
    public event CellStateChangedEventHandler CellStateChanged;

    private void OnCellStateChanged(int cell, bool oldState, bool newState)
    {
        if (CellStateChanged != null)
            CellStateChanged(this, cell, oldState, newState);
    }
}
