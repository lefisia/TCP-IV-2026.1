using UnityEngine;

public class InputManager : MonoBehaviour
{
    enum State { None, PuzzlePiece }
    private State state;

    [Header(" Elements ")]
    [SerializeField] private PuzzleController puzzleController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = State.None;
    }

    // Update is called once per frame
    void Update()
    {
        ManageInput();
    }

    private void ManageInput()
    {
        if (Input.touchCount == 1)
        {
            ManageSingleInput();
        }
    }

    private void ManageSingleInput()
    {
        Vector2 touchPos = Input.touches[0].position;

        Vector3 worldTouchPos = Camera.main.ScreenToWorldPoint(touchPos);
        worldTouchPos.z = 0;

        TouchPhase touchPhase = Input.touches[0].phase;

        switch(touchPhase)
        {
            case TouchPhase.Began:

                // checar se detecta peça ou nn com o puzzleController

                if (puzzleController.SingleTouchBeganCallback(worldTouchPos))
                {
                    //piece detected
                    state = State.PuzzlePiece;

                    return;
                }

                break;

            case TouchPhase.Moved:

                if (state == State.PuzzlePiece)
                    puzzleController.SingleTouchDrag(worldTouchPos);

                break;

            case TouchPhase.Stationary:
                
                if (state == State.PuzzlePiece)
                    puzzleController.SingleTouchDrag(worldTouchPos);

                break;

            case TouchPhase.Ended:

                if (state == State.PuzzlePiece)
                    puzzleController.SingleTouchEnded();

                break;

            default:
                break;
        }
    }
}
