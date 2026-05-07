using System.Collections.Generic;
using UnityEngine;

public class ARPuzzleGenerator : MonoBehaviour
{
    [Header(" Elements ")]
    [SerializeField] private ARPuzzleController arPuzzleController;
    [SerializeField] private PuzzlePiece puzzlePiecePrefab;

    [Header(" Settings ")]
    [SerializeField] private int gridSize;
    private float gridScale;
    private List<PuzzlePiece> puzzlePieces = new List<PuzzlePiece>();

    void Start()
    {
        gridScale = Constants.puzzleWorldSize / gridSize;

        arPuzzleController.Configure(this, gridScale);

        GenerateGrid();
    }

    private void GenerateGrid()
    {
        puzzlePieces.Clear();

        Vector3 startPos = Vector2.left * (gridScale * gridSize) / 2 + Vector2.down * (gridScale * gridSize) / 2;

        startPos.x += gridScale / 2;
        startPos.y += gridScale / 2;

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                Vector3 correctPosition = startPos + new Vector3(x, y) * gridScale;
                correctPosition.z -= Constants.pieceZOffset * GridIndexFromPos(x, y);

                Vector3 randomPosition = Random.onUnitSphere * 2;
                randomPosition.z = correctPosition.z;

                PuzzlePiece puzzlePieceInstance = Instantiate(puzzlePiecePrefab, randomPosition, Quaternion.identity, transform);

                puzzlePieces.Add(puzzlePieceInstance);

                Vector2 tiling = new Vector2(1f / gridSize, 1f / gridSize);
                Vector2 offset = new Vector2((float)x / gridSize, (float)y / gridSize);

                puzzlePieceInstance.Configure(gridScale, tiling, offset, correctPosition);
            }
        }

        ConfigureNeighbors();
    }

    private void ConfigureNeighbors()
    {
        for (int i = 0; i < puzzlePieces.Count; i++)
            ConfigurePieceNeighbors(puzzlePieces[i], i);
    }

    private void ConfigurePieceNeighbors(PuzzlePiece piece, int index)
    {
        Vector2Int gridPos = IndexToGridPos(index);
        int x = gridPos.x;
        int y = gridPos.y;

        PuzzlePiece rightPiece = IsValidGridPos(x + 1, y) ? transform.GetChild(GridIndexFromPos(x + 1, y)).GetComponent<PuzzlePiece>() : null;
        PuzzlePiece bottomPiece = IsValidGridPos(x, y - 1) ? transform.GetChild(GridIndexFromPos(x, y - 1)).GetComponent<PuzzlePiece>() : null;
        PuzzlePiece leftPiece = IsValidGridPos(x - 1, y) ? transform.GetChild(GridIndexFromPos(x - 1, y)).GetComponent<PuzzlePiece>() : null;
        PuzzlePiece topPiece = IsValidGridPos(x, y + 1) ? transform.GetChild(GridIndexFromPos(x, y + 1)).GetComponent<PuzzlePiece>() : null;

        piece.SetNeighbors(rightPiece, bottomPiece, leftPiece, topPiece);
    }

    private bool IsValidGridPos(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    private int GridIndexFromPos(int x, int y)
    {
        return y + gridSize * x;
    }

    private Vector2Int IndexToGridPos(int index)
    {
        int x = index / gridSize;
        int y = (int)(((float)index) % gridSize);

        return new Vector2Int(x, y);
    }
    public PuzzlePiece[] GetPuzzlePieces()
    {
        return puzzlePieces.ToArray();
    }
}
