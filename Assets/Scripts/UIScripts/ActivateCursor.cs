using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCursor : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }
}
