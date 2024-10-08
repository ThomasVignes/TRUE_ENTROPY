using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D BaseCursor, MoveCursor, LookCursor, AimCursor;

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

        if (type != CursorType.Invisible)
            Cursor.visible = true;


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

            case CursorType.Aim:
                Cursor.SetCursor(AimCursor, new Vector2(25, 25), CursorMode.Auto);
                break;

            case CursorType.Invisible:
                Cursor.visible = false;
                break;
        }
    }
}

public enum CursorType
{
    Base,
    Move,
    Look,
    Aim,
    Invisible
}
