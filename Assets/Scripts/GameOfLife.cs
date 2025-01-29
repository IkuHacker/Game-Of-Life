using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using TMPro;
using UnityEngine.InputSystem; // Import du nouveau système d'entrée
using UnityEngine.UI;

public class GameOfLife : MonoBehaviour
{
    [Header("Références publiques")]

    [SerializeField] private Tilemap gridTileMap;
    [SerializeField] private Tile tile;
    [SerializeField] private Color aliveTileColor;
    [SerializeField] private Color deathTileColor;

    [SerializeField] private Transform cam;

    [Header("Paramètres de la grille")]
    [SerializeField] private Vector2Int gridSize;
    [SerializeField] private int[,] grid;
    [SerializeField] private int[,] nextGrid;

    [Header("Contrôle du jeu")]
    [SerializeField] private bool isGameStopped;
    [SerializeField] private bool isEditMode; // Nouveau booléen pour le mode édition
    [SerializeField] private float speed;
    [SerializeField] private float minSpeed = 0.1f; // Vitesse minimale autorisée
    [SerializeField] private float maxSpeed = 10f; // Vitesse maximale autorisée
    [SerializeField] private int generation;
    [SerializeField] private int aliveCellCount;
    [SerializeField] private int deathCellCount;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI generationText;
    [SerializeField] private TextMeshProUGUI aliveCellCountText;
    [SerializeField] private TextMeshProUGUI deathCellCountText;

    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;

    [SerializeField] private Image buttonImage;

    [SerializeField] private Image editButtonImage;
    [SerializeField] private Image editButtonImageSprite;



    [SerializeField] private TMP_InputField inputFieldX;
    [SerializeField] private TMP_InputField inputFieldY;

    [SerializeField] private TMP_InputField inputSpeed;

    [SerializeField] private Camera mainCamera;
    [SerializeField]  private PlayerInput playerInput; // Référence au système d'entrée
    private InputAction rightClickAction; // Action pour gérer les clics
    private InputAction leftClickAction; // Action pour gérer les clics


    void Awake()
    {
        rightClickAction = playerInput.actions["RightClick"];
        leftClickAction = playerInput.actions["LeftClick"];
    }

    void OnEnable()
    {
        leftClickAction.Enable();
        rightClickAction.Enable();

    }

    void OnDisable()
    {
        leftClickAction.Disable();
        rightClickAction.Enable();

    }

    void Start()
    {
        inputSpeed.text = speed.ToString();
        inputFieldX.text = gridSize.x.ToString();
        inputFieldY.text = gridSize.y.ToString();

        grid = new int[gridSize.x, gridSize.y];
        nextGrid = new int[gridSize.x, gridSize.y];
        PlaceCam();
        GenerateGrid();
        StartCoroutine(GameLoop());
    }

    void GenerateGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                int tileChoose = Random.Range(0, 2);
                if (tileChoose == 0)
                {
                    deathCellCount++;
                    gridTileMap.SetTileFlags(new Vector3Int(x, y), TileFlags.None);
                    gridTileMap.SetColor(new Vector3Int(x, y), deathTileColor);
                    gridTileMap.SetTile(new Vector3Int(x, y), tile);
                    grid[x, y] = 0;
                }
                else
                {
                    aliveCellCount++;
                    gridTileMap.SetTileFlags(new Vector3Int(x, y), TileFlags.None);
                    gridTileMap.SetColor(new Vector3Int(x, y), aliveTileColor);
                    gridTileMap.SetTile(new Vector3Int(x, y), tile);
                    grid[x, y] = 1;
                }
            }
        }

        aliveCellCountText.text = "Cellule vivante :  " + aliveCellCount.ToString();
        deathCellCountText.text = "Cellule morte :  " + deathCellCount.ToString();
    }

    void PlaceCam()
    {
        cam.position = new Vector3(gridSize.x / 2, gridSize.y / 2, -10);
    }

    private void Update()
    {
        if (isEditMode)
        {
            HandleEditMode();
        }
    }


    void HandleEditMode()
    {
        if (leftClickAction.IsPressed()) // Si le clic est effectué
        {
            // Lire la position de la souris
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;

            Vector3Int cellPosition = gridTileMap.WorldToCell(mouseWorldPos);

            if (cellPosition.x >= 0 && cellPosition.x < gridSize.x && cellPosition.y >= 0 && cellPosition.y < gridSize.y)
            {

                AddCell(cellPosition);
            }
        }

        if (rightClickAction.IsPressed()) // Si le clic est effectué
        {
            // Lire la position de la souris
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            mouseWorldPos.z = 0;

            Vector3Int cellPosition = gridTileMap.WorldToCell(mouseWorldPos);

            if (cellPosition.x >= 0 && cellPosition.x < gridSize.x && cellPosition.y >= 0 && cellPosition.y < gridSize.y)
            {

                RemoveCell(cellPosition);
            }
        }
    }

    void RemoveCell(Vector3Int cellPosition)
    {
        int x = cellPosition.x;
        int y = cellPosition.y;

        if (grid[x, y] == 1) // Cellule morte -> devient vivante
        {
            grid[x, y] = 0;
            nextGrid[x, y] = 0;
            gridTileMap.SetTileFlags(cellPosition, TileFlags.None);
            gridTileMap.SetColor(cellPosition, deathTileColor);
            gridTileMap.SetTile(cellPosition, tile);
            aliveCellCount--;
            deathCellCount++;
        }
    
        aliveCellCountText.text = "Cellule vivante :  " + aliveCellCount.ToString();
        deathCellCountText.text = "Cellule morte :  " + deathCellCount.ToString();
    }

    void AddCell(Vector3Int cellPosition)
    {
        int x = cellPosition.x;
        int y = cellPosition.y;

        if (grid[x, y] == 0) // Cellule morte -> devient vivante
        {
            grid[x, y] = 1;
            nextGrid[x, y] = 1;
            gridTileMap.SetTileFlags(cellPosition, TileFlags.None);
            gridTileMap.SetColor(cellPosition, aliveTileColor);
            gridTileMap.SetTile(cellPosition, tile);
            aliveCellCount++;
            deathCellCount--;
        }
   

        aliveCellCountText.text = "Cellule vivante :  " + aliveCellCount.ToString();
        deathCellCountText.text = "Cellule morte :  " + deathCellCount.ToString();
    }

    IEnumerator GameLoop()
    {
        while (true)
        {
            if (isGameStopped || isEditMode)
            {
                yield return null; // Attend jusqu'à la prochaine frame pour vérifier l'état
                continue;
            }

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    int neighborCount = GetNeighborCount(new Vector2Int(x, y));

                    // Règles du jeu de la vie de Conway
                    if (grid[x, y] == 1) // Cellule vivante
                    {
                        if (neighborCount < 2 || neighborCount > 3)
                        {
                            nextGrid[x, y] = 0; // La cellule meurt
                        }
                        else
                        {
                            nextGrid[x, y] = 1; // La cellule reste vivante
                        }
                    }
                    else // Cellule morte
                    {
                        if (neighborCount == 3)
                        {
                            nextGrid[x, y] = 1; // La cellule naît
                        }
                        else
                        {
                            nextGrid[x, y] = 0; // La cellule reste morte
                        }
                    }
                }
            }

            // Appliquer les modifications de nextGrid à grid et mettre à jour les tuiles
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    grid[x, y] = nextGrid[x, y]; // Mettre à jour grid avec nextGrid

                    // Mettre à jour les tuiles en fonction de la nouvelle valeur de grid
                    if (grid[x, y] == 0)
                    {
                        gridTileMap.SetTileFlags(new Vector3Int(x, y), TileFlags.None);
                        gridTileMap.SetColor(new Vector3Int(x, y), deathTileColor);
                        gridTileMap.SetTile(new Vector3Int(x, y), tile);
                    }
                    else
                    {
                        gridTileMap.SetTileFlags(new Vector3Int(x, y), TileFlags.None);
                        gridTileMap.SetColor(new Vector3Int(x, y), aliveTileColor);
                        gridTileMap.SetTile(new Vector3Int(x, y), tile);
                    }
                }
            }

            generation++;
            generationText.text = "Generation :  " + generation.ToString();

            deathCellCount = 0;
            aliveCellCount = 0;

            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (grid[x, y] == 0)
                    {
                        deathCellCount++;
                    }
                    else
                    {
                        aliveCellCount++;
                    }
                }
            }

            aliveCellCountText.text = "Cellule vivante :  " + aliveCellCount.ToString();
            deathCellCountText.text = "Cellule morte :  " + deathCellCount.ToString();
            yield return new WaitForSeconds(speed);
        }
    }

    int GetNeighborCount(Vector2Int pos)
    {
        int count = 0;
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;

                int neighborX = pos.x + x;
                int neighborY = pos.y + y;

                if (neighborX >= 0 && neighborX < gridSize.x && neighborY >= 0 && neighborY < gridSize.y)
                {
                    count += grid[neighborX, neighborY];
                }
            }
        }
        return count;
    }

    public void Play()
    {
        isGameStopped = !isGameStopped;
        buttonImage.sprite = isGameStopped ? pauseSprite : playSprite;
    }

    public void ToggleEditMode()
    {
        isEditMode = !isEditMode;
        editButtonImage.color = isEditMode ? new Color(0f, 0.65f, 1f) : new Color(1f, 1f, 1f);
        editButtonImageSprite.color = isEditMode ? new Color(0f, 0.65f, 1f) : new Color(0.469028f, 0.5f, 0.5345911f);

        isGameStopped = isEditMode; // Arrête le jeu si le mode édition est activé
    }

    public void ResetGame()
    {
        deathCellCount = 0;
        aliveCellCount = 0;
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
               
                    deathCellCount++;
                    gridTileMap.SetTileFlags(new Vector3Int(x, y), TileFlags.None);
                    gridTileMap.SetColor(new Vector3Int(x, y), deathTileColor);
                    gridTileMap.SetTile(new Vector3Int(x, y), tile);
                    grid[x, y] = 0;
                
                
            }
        }
    }

    public void RePlay()
    {
        generation = 0;
        StopAllCoroutines();
        grid = new int[gridSize.x, gridSize.y];
        nextGrid = new int[gridSize.x, gridSize.y];
        PlaceCam();

        for (int x = 0; x < 1000; x++)
        {
            for (int y = 0; y < 1000; y++)
            {
                gridTileMap.SetTile(new Vector3Int(x, y), null);
            }
        }

        GenerateGrid();
        StartCoroutine(GameLoop());
    }

    public void ChangeDimension()
    {
        gridSize.x = int.Parse(inputFieldX.text);
        gridSize.y = int.Parse(inputFieldY.text);
    }

    public void ChangeSpeed()
    {
        speed = float.Parse(inputSpeed.text);
    }

    public void MoreSlow()
    {
        speed *= 1.5f;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        speed = Mathf.Round(speed * 100f) / 100f;
        inputSpeed.text = speed.ToString();
    }

    public void MoreFast()
    {
        speed /= 1.5f;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        speed = Mathf.Round(speed * 100f) / 100f;
        inputSpeed.text = speed.ToString();
    }
}
