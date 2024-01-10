using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class MouseManager : Singleton<MouseManager>
{
    public event Action<Vector3> OnMouseCliceked;
    public event Action<GameObject> OnEnemyCliceked;
    public Texture2D point, doorway, attack, target, arrow;
    RaycastHit hitInfo;

    private void Update()
    {
        SetCursorTexture();
        MouseContol();
    }

    private void SetCursorTexture()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo))
        {
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(target, new Vector2(16,16), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(16,16), CursorMode.Auto);
                    break;
                case "Portal":
                    Cursor.SetCursor(doorway, new Vector2(16,16), CursorMode.Auto);
                    break;
                // default: 
                //     Cursor.SetCursor(arrow, new Vector2(16,16), CursorMode.Auto);
                //     break;
            }
        }
    }

    private void MouseContol()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
                OnMouseCliceked?.Invoke(hitInfo.point);
            if (hitInfo.collider.gameObject.CompareTag("Enemy"))
                OnEnemyCliceked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Attackable"))
                OnEnemyCliceked?.Invoke(hitInfo.collider.gameObject);
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
                OnMouseCliceked?.Invoke(hitInfo.point);
        }
    }
}
