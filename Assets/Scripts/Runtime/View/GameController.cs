using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] 
    private UnitController UnitController;
    [SerializeField] 
    private GridController GridController;
    [SerializeField] 
    private CameraController CameraController;
    [SerializeField] 
    private InteractionController InteractionController;

    void Start()
    {
        RunGame();
    }

    private void RunGame()
    {
        GridController.GenerateLevel(UnitController);
        CameraController.SetBounds(GridController.MinPosition, GridController.MaxPosition);
        InteractionController.Initialize(CameraController);
    }
}
