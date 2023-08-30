using UnityEngine;

public interface I_BuildingState
{
    void UpdatePlacementState(Vector3Int gridPos);
    void ExitPlacementState();
    bool OnClickDetected(Vector3Int gridPos);
    void OnRotateKeyPressed();
    void OnInventoryKeyPressed();
}
