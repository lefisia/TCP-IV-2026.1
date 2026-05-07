using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleController : MonoBehaviour
{

    [Header(" Elements ")]
    private PuzzleGenerator puzzleGenerator;

    [Header(" Settings ")]
    private float detectionRadius;

    [Header(" Piece Movement ")]
    private Vector3 clickedPosition;
    private PuzzlePiece currentPiece;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Configure(PuzzleGenerator puzzleGenerator, float gridScale)
    {
        this.puzzleGenerator = puzzleGenerator;
        detectionRadius = gridScale / 2 * 1.5f;
    }
    public bool SingleTouchBeganCallback(Vector3 worldPosition)
    {
        //1. pegar as peças (Puzzle Generator)
        PuzzlePiece[] puzzlePieces = puzzleGenerator.GetPuzzlePieces();

        //2. pegar a mais próxima da World Position
        currentPiece = GetTopClosestPiece(puzzlePieces, worldPosition);

        if(currentPiece == null)
            return false;

        ManagePiecesOrder(puzzlePieces);

        //3. mover a peça

        clickedPosition = worldPosition;
        currentPiece.StartMoving();

        return true;
    }

    public void SingleTouchDrag(Vector3 worldPosition)
    {
        Vector3 moveDelta = worldPosition - clickedPosition;

        if(currentPiece != null)
            currentPiece.Move(moveDelta);
    }

    public void SingleTouchEnded()
    {
        if (currentPiece == null)
            return;

        currentPiece.StopMoving();
        currentPiece = null;
    }

    private void ManagePiecesOrder(PuzzlePiece[] pieces)
    {
        float highestZ = pieces.Length * Constants.pieceZOffset;
        float currentPieceZ = currentPiece.transform.position.z;

        currentPiece.transform.position = currentPiece.transform.position.With(z: -highestZ);

        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == currentPiece)
                continue;

            if (pieces[i].transform.position.z < currentPieceZ)
                pieces[i].transform.position += Vector3.forward * Constants.pieceZOffset;
        }
    }

    private PuzzlePiece GetTopClosestPiece(PuzzlePiece[] puzzlePieces, Vector3 worldPosition)
    {
        List<PuzzlePiece> potentialPieces = new List<PuzzlePiece>();

        for (int i = 0; i < puzzlePieces.Length; i++)
        {

            if (puzzlePieces[i].IsValid)
                continue;

            float distance = Vector3.Distance((Vector2)puzzlePieces[i].transform.position, worldPosition);

            if (distance > detectionRadius)
                continue;

            potentialPieces.Add(puzzlePieces[i]);
        }

        if (potentialPieces.Count <= 0)
            return null;

        potentialPieces.Sort();

        return potentialPieces[0];
    }
}
