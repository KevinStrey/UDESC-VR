using System;
using System.Collections.Generic;
using UnityEngine;

public enum CellState { Empty, X, O }

public struct TurnData
{
    public Vector2Int PlayerMove;
    public Vector2Int BotMove;
    public bool BotDidMove;

    public TurnData(Vector2Int playerMove, Vector2Int botMove, bool botDidMove)
    {
        PlayerMove = playerMove;
        BotMove = botMove;
        BotDidMove = botDidMove;
    }
}

public class GameManager : MonoBehaviour
{
    [Header("Configurações do Tabuleiro")]
    private const int BOARD_SIZE = 5;
    private const int LINE_TO_WIN = 4;
    private CellState[,] board = new CellState[BOARD_SIZE, BOARD_SIZE];
    
    private Stack<TurnData> matchHistory = new Stack<TurnData>();
    private bool isGameOver = false;

    // Ações (Eventos) para a Camada Visual / Áudio escutar
    public static event Action<Vector2Int, CellState> OnCellUpdated;
    public static event Action OnGameWon;
    public static event Action OnGameLost;
    public static event Action OnGameDraw;
    public static event Action OnGameReset;

    private void Start()
    {
        ResetGame();
    }

    /// <summary>
    /// Chamado pela interface física (Botão Poke) quando o jogador interage.
    /// </summary>
    public bool RegisterPlayerMove(int row, int col)
    {
        if (isGameOver || board[row, col] != CellState.Empty) 
            return false;

        // 1. Executa a jogada do Jogador (X)
        UpdateCell(row, col, CellState.X);
        Vector2Int playerMove = new Vector2Int(row, col);

        // Verifica se o jogador venceu imediatamente
        if (CheckWinCondition(CellState.X))
        {
            EndGame(CellState.X);
            matchHistory.Push(new TurnData(playerMove, Vector2Int.zero, false));
            return true;
        }

        if (IsBoardFull())
        {
            EndGame(CellState.Empty); // Empate
            matchHistory.Push(new TurnData(playerMove, Vector2Int.zero, false));
            return true;
        }

        // 2. Executa a jogada do Bot (O)
        Vector2Int botMove = ExecuteBotTurn(playerMove);
        UpdateCell(botMove.x, botMove.y, CellState.O);

        // Grava o turno completo na pilha de Undo
        matchHistory.Push(new TurnData(playerMove, botMove, true));

        // Verifica se o bot venceu
        if (CheckWinCondition(CellState.O))
        {
            EndGame(CellState.O);
            return true;
        }

        if (IsBoardFull())
        {
            EndGame(CellState.Empty);
        }

        return true;
    }

    /// <summary>
    /// Desfaz o último turno completo (Jogador + Bot) simultaneamente.
    /// </summary>
    public void UndoLastTurn()
    {
        if (matchHistory.Count == 0) return;

        // Se o jogo tinha acabado, reativa o loop lógico
        isGameOver = false;

        TurnData lastTurn = matchHistory.Pop();

        // Limpa a jogada do Bot (se chegou a acontecer)
        if (lastTurn.BotDidMove)
        {
            UpdateCell(lastTurn.BotMove.x, lastTurn.BotMove.y, CellState.Empty);
        }

        // Limpa a jogada do Jogador
        UpdateCell(lastTurn.PlayerMove.x, lastTurn.PlayerMove.y, CellState.Empty);
    }

    /// <summary>
    /// Lógica de Decisão do Bot baseada em heurísticas de prioridade.
    /// </summary>
    private Vector2Int ExecuteBotTurn(Vector2Int lastPlayerMove)
    {
        Vector2Int targetMove;

        // 1. PRIORIDADE MÁXIMA: Tentar Vencer (Procura 3 'O's em uma linha de 4)
        if (TryFindAlignedMove(CellState.O, 3, out targetMove))
        {
            return targetMove;
        }

        // 2. PRIORIDADE SECUNDÁRIA: Bloqueio Crítico (Procura 3 'X's em uma linha de 4)
        if (TryFindAlignedMove(CellState.X, 3, out targetMove))
        {
            return targetMove;
        }

        // 3. PRIORIDADE TERCIÁRIA: Bloqueio Antecipado (Procura 2 'X's em uma linha de 4)
        if (TryFindAlignedMove(CellState.X, 2, out targetMove))
        {
            return targetMove;
        }

        // 4. PRIORIDADE QUATERNÁRIA: Jogar adjacente à última jogada do jogador
        return GetAdjacentMove(lastPlayerMove);
    }

    /// <summary>
    /// Avalia o tabuleiro em busca de uma linha com o número exato de peças necessárias para ataque ou bloqueio.
    /// </summary>
    private bool TryFindAlignedMove(CellState stateToCheck, int requiredCount, out Vector2Int target)
    {
        target = Vector2Int.zero;

        int[] dr = { 0, 1, 1, -1 };
        int[] dc = { 1, 0, 1, 1 };

        for (int r = 0; r < BOARD_SIZE; r++)
        {
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if (r + (LINE_TO_WIN - 1) * dr[dir] >= 0 && r + (LINE_TO_WIN - 1) * dr[dir] < BOARD_SIZE &&
                        c + (LINE_TO_WIN - 1) * dc[dir] >= 0 && c + (LINE_TO_WIN - 1) * dc[dir] < BOARD_SIZE)
                    {
                        int countState = 0;
                        int countEmpty = 0;
                        List<Vector2Int> availableEmpties = new List<Vector2Int>();

                        for (int i = 0; i < LINE_TO_WIN; i++)
                        {
                            int currR = r + i * dr[dir];
                            int currC = c + i * dc[dir];

                            if (board[currR, currC] == stateToCheck) 
                            {
                                countState++;
                            }
                            else if (board[currR, currC] == CellState.Empty) 
                            {
                                countEmpty++;
                                availableEmpties.Add(new Vector2Int(currR, currC));
                            }
                        }

                        // Verifica se no segmento de 4 células, existem exatamente 'requiredCount' peças e o restante vazio
                        if (countState == requiredCount && countEmpty == LINE_TO_WIN - requiredCount)
                        {
                            target = availableEmpties[0];
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Retorna uma célula vazia aleatória que seja adjacente à última jogada do jogador.
    /// </summary>
    private Vector2Int GetAdjacentMove(Vector2Int targetPosition)
    {
        List<Vector2Int> adjacentEmpties = new List<Vector2Int>();
        List<Vector2Int> allEmpties = new List<Vector2Int>();

        for (int r = 0; r < BOARD_SIZE; r++)
        {
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                if (board[r, c] == CellState.Empty)
                {
                    Vector2Int emptyCell = new Vector2Int(r, c);
                    allEmpties.Add(emptyCell);

                    int rowDiff = Mathf.Abs(r - targetPosition.x);
                    int colDiff = Mathf.Abs(c - targetPosition.y);

                    if (rowDiff <= 1 && colDiff <= 1)
                    {
                        adjacentEmpties.Add(emptyCell);
                    }
                }
            }
        }

        if (allEmpties.Count == 0) return Vector2Int.zero;

        if (adjacentEmpties.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, adjacentEmpties.Count);
            return adjacentEmpties[randomIndex];
        }

        int fallbackIndex = UnityEngine.Random.Range(0, allEmpties.Count);
        return allEmpties[fallbackIndex];
    }

    private bool CheckWinCondition(CellState state)
    {
        int[] dr = { 0, 1, 1, -1 };
        int[] dc = { 1, 0, 1, 1 };

        for (int r = 0; r < BOARD_SIZE; r++)
        {
            for (int c = 0; c < BOARD_SIZE; c++)
            {
                for (int dir = 0; dir < 4; dir++)
                {
                    if (r + (LINE_TO_WIN - 1) * dr[dir] >= 0 && r + (LINE_TO_WIN - 1) * dr[dir] < BOARD_SIZE &&
                        c + (LINE_TO_WIN - 1) * dc[dir] >= 0 && c + (LINE_TO_WIN - 1) * dc[dir] < BOARD_SIZE)
                    {
                        int count = 0;
                        for (int i = 0; i < LINE_TO_WIN; i++)
                        {
                            if (board[r + i * dr[dir], c + i * dc[dir]] == state) count++;
                        }

                        if (count == LINE_TO_WIN) return true;
                    }
                }
            }
        }
        return false;
    }

    private void UpdateCell(int row, int col, CellState state)
    {
        board[row, col] = state;
        OnCellUpdated?.Invoke(new Vector2Int(row, col), state);
    }

    private bool IsBoardFull()
    {
        for (int r = 0; r < BOARD_SIZE; r++)
            for (int c = 0; c < BOARD_SIZE; c++)
                if (board[r, c] == CellState.Empty) return false;
        return true;
    }

    private void EndGame(CellState winner)
    {
        isGameOver = true;
        
        if (winner == CellState.X) 
        {
            OnGameWon?.Invoke();
        }
        else if (winner == CellState.O) 
        {
            OnGameLost?.Invoke();
        }
        else 
        {
            OnGameDraw?.Invoke();
            OnGameLost?.Invoke(); 
        }
    }

    public void ResetGame()
    {
        board = new CellState[BOARD_SIZE, BOARD_SIZE];
        matchHistory.Clear();
        isGameOver = false;
        OnGameReset?.Invoke();
    }
}