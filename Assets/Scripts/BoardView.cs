using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoardView : MonoBehaviour
{
    [Header("Referências do Jogo")]
    [Tooltip("O Prefab do botão XR que será instanciado.")]
    public GameObject buttonPrefab;
    [Tooltip("Referência ao script que contém a lógica do jogo.")]
    public GameManager gameManager;

    [Header("Referências da Interface")]
    [Tooltip("O container do grid do tabuleiro (BoardGridContainer)")]
    public GameObject boardContainer;
    [Tooltip("O painel que contém o texto e imagem do tutorial")]
    public GameObject tutorialPanel;
    [Tooltip("O botão para iniciar o jogo após ler o tutorial")]
    public Button startButton;
    
    [Tooltip("O botão para reiniciar o jogo em caso de derrota ou empate")]
    public Button restartButton; 

    [Tooltip("O botão para desfazer a jogada")]
    public Button undoButton; // NOVO: Referência ao botão de desfazer

    [Header("Feedback Sensorial")]
    [Tooltip("AudioSource para tocar o som de clique")]
    public AudioSource audioSource;
    [Tooltip("O arquivo de som do clique")]
    public AudioClip clickSound;

    private GameObject[,] buttonGrid = new GameObject[5, 5];

    private void Start()
    {
        // Configuração inicial: Mostra tutorial, esconde tabuleiro
        tutorialPanel.SetActive(true);
        boardContainer.SetActive(false);

        // Inscreve o botão de começar
        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }

        // Configura o botão de reinício
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false); // Esconde no início
            restartButton.onClick.AddListener(() => gameManager.ResetGame()); // Chama o reset do GameManager
        }

        GenerateGrid();
        GameManager.OnGameReset += ResetBoardVisuals;
    }

    private void StartGame()
    {
        // Esconde tutorial, mostra tabuleiro
        tutorialPanel.SetActive(false);
        boardContainer.SetActive(true);
    }

    private void OnEnable()
    {
        GameManager.OnCellUpdated += UpdateButtonVisual;
        
        // Escuta os eventos de derrota/empate. 
        GameManager.OnGameLost += MostrarBotaoRestart;
    }

    private void OnDisable()
    {
        GameManager.OnCellUpdated -= UpdateButtonVisual;
        GameManager.OnGameReset -= ResetBoardVisuals;
        GameManager.OnGameLost -= MostrarBotaoRestart; // Limpa o evento
    }

    private void GenerateGrid()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int col = 0; col < 5; col++)
            {
                GameObject newBtn = Instantiate(buttonPrefab, boardContainer.transform);
                newBtn.name = $"Cell_{row}_{col}";

                int r = row;
                int c = col;

                Button uiButton = newBtn.GetComponent<Button>();
                if (uiButton != null)
                {
                    uiButton.onClick.AddListener(() => OnButtonClicked(r, c));
                }

                TextMeshProUGUI btnText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = "";

                buttonGrid[row, col] = newBtn;
            }
        }
    }

    private void OnButtonClicked(int row, int col)
    {
        // Toca o som de feedback
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        gameManager.RegisterPlayerMove(row, col);
    }

    private void UpdateButtonVisual(Vector2Int position, CellState state)
    {
        GameObject btnObj = buttonGrid[position.x, position.y];
        TextMeshProUGUI textMesh = btnObj.GetComponentInChildren<TextMeshProUGUI>();

        if (textMesh != null)
        {
            switch (state)
            {
                case CellState.X:
                    textMesh.text = "X";
                    textMesh.color = Color.blue;
                    break;
                case CellState.O:
                    textMesh.text = "O";
                    textMesh.color = Color.red;
                    break;
                case CellState.Empty:
                    textMesh.text = "";
                    break;
            }
        }
    }

    // Método que exibe o botão de restart e esconde o de desfazer
    private void MostrarBotaoRestart()
    {
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
        }

        // NOVO: Esconde o botão de desfazer
        if (undoButton != null)
        {
            undoButton.gameObject.SetActive(false);
        }
    }

    private void ResetBoardVisuals()
    {
        // Oculta o botão de restart novamente
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
        }

        // NOVO: Mostra o botão de desfazer novamente
        if (undoButton != null)
        {
            undoButton.gameObject.SetActive(true);
        }

        for (int r = 0; r < 5; r++)
        {
            for (int c = 0; c < 5; c++)
            {
                GameObject btnObj = buttonGrid[r, c];
                TextMeshProUGUI textMesh = btnObj.GetComponentInChildren<TextMeshProUGUI>();
                if (textMesh != null) textMesh.text = "";
            }
        }
    }
}