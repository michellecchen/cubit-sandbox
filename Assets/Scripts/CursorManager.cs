using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    [SerializeField]
    private Texture2D _defaultPointer;

    [SerializeField]
    private Texture2D _clickedPointer;

    [SerializeField]
    private Texture2D _grab;

    [SerializeField]
    private Texture2D _place;

    [SerializeField]
    private Texture2D _lookAround;

    void Start() {
        UpdateCursor(_defaultPointer);
    }

    void Update() {

        CheckLMB();
    }

    private void CheckLMB() {

        if (Input.GetMouseButtonDown(0)) {      // when LMB is clicked
            UpdateCursor(_clickedPointer);
        }
        if (Input.GetMouseButtonUp(0)) {        // when LMB is released
            UpdateCursor(_defaultPointer);
        }
    }

    private void UpdateCursor(Texture2D updatedTexture) {
        Cursor.SetCursor(updatedTexture, Vector2.zero, CursorMode.Auto);
    }
}
