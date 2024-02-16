using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D BaseCursor, MoveCursor, LookCursor;

    private CursorType current;

    public void Init()
    {
        current = CursorType.Base;
    }

    public void SetCursorType(CursorType type)
    {
        if (current == type)
            return;

        current = type;

        switch (type) 
        {
            case CursorType.Base :
                Cursor.SetCursor(BaseCursor, new Vector2(18, 13), CursorMode.Auto);
                break;

            case CursorType.Move:
                Cursor.SetCursor(MoveCursor, new Vector2(18, 13), CursorMode.Auto);
                break;

            case CursorType.Look:
                Cursor.SetCursor(LookCursor, new Vector2(18, 13), CursorMode.Auto);
                break;
        
        }
    }
}

public enum CursorType
{
    Base,
    Move,
    Look
}
