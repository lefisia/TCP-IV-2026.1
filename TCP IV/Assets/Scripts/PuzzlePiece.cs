using System;
using UnityEngine;

public class PuzzlePiece : MonoBehaviour, IComparable<PuzzlePiece>
{
    [Header(" Elements ")]
    [SerializeField] private Renderer renderer;

    [Header(" Movement ")]
    private Vector3 startMovePos;

    [Header(" Validation ")]
    private Vector3 correctPosition;
    public bool IsValid { get; private set; }

    [Header(" Neighbors ")]
    private PuzzlePiece[] neighbors;

    public void Configure(float scale, Vector2 tiling, Vector2 offset, Vector3 correctPosition)
    {
        transform.localScale = Vector3.one * scale;

        renderer.material.mainTextureScale = tiling;
        renderer.material.mainTextureOffset = offset;

        this.correctPosition = correctPosition;
    }

    public void SetNeighbors(params PuzzlePiece[] puzzlePieces)
    {
        neighbors = puzzlePieces;
    }

    public void StartMoving()
    {
        startMovePos = transform.position;
    }

    public void Move(Vector3 moveDelta)
    {
        Vector3 targetPosition = startMovePos + moveDelta;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 60 * .3f);
    }

    public void StopMoving()
    {
        bool isValid = CheckForValidation();

        if (isValid)
            return;

        CheckForNeighbors();
    }

    private bool CheckForValidation()
    {
        if (IsCloseToCorrectPosition())
        {
            Validate();
            return true;
        }

        return false;
    }

    private bool IsCloseToCorrectPosition()
    {
        return Vector3.Distance((Vector2)transform.position, (Vector2)correctPosition) < GetMinValidDistance();
    }

    private float GetMinValidDistance()
    {
        return Mathf.Max(.05f, transform.localScale.x / 5);
    }

    private void CheckForNeighbors()
    {
        for (int i = 0; i < neighbors.Length; i++)
        {
            if (neighbors[i] == null)
                continue;
            if (neighbors[i].IsValid)
                continue;

            Vector3 correctLocalPosition = Quaternion.Euler(0, 0, -90 * i) * Vector3.right * transform.localScale.x;
        }
    }

    private void Validate()
    {
        correctPosition.z = 0;
        transform.position = correctPosition;

        IsValid = true;

        Debug.Log("Piece Placed Correctly : " + name);
    }

    public int CompareTo(PuzzlePiece otherPiece)
    {
        return transform.position.z.CompareTo(otherPiece.transform.position.z);
    }

    
}
