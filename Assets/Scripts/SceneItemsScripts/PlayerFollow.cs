using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    private GameObject player;

    [Header("In which axis should this gameobject follow the player? \n")]
    [SerializeField] private bool X;
    [SerializeField] private bool Y;
    [SerializeField] private bool Z;


    private void Start()
    {
        player = GameObject.Find("Player");
    }

    private void Update()
    {
        transform.position = new Vector3(X ? player.transform.position.x : transform.position.x, 
            Y ? player.transform.position.y : transform.position.y, 
            Z ? player.transform.position.z : transform.position.z);
    }
}
