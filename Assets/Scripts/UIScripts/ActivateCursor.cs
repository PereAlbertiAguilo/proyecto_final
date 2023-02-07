using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCursor : MonoBehaviour
{
    private void Start()
    {
        //Confines the cursor to the screen size
        Cursor.lockState = CursorLockMode.Confined;
    }
}
