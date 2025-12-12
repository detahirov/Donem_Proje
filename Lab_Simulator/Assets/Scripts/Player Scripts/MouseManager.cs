using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager Instance;

    public bool CursorLocked => Cursor.lockState == CursorLockMode.Locked;

    void Awake()
    {
        Instance = this;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
