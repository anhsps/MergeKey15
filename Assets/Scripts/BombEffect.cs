using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffect : MonoBehaviour
{
    private Player15 player15;

    // Start is called before the first frame update
    void Start()
    {
        player15 = FindObjectOfType<Player15>();
        DestroyCols();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void DestroyCols()
    {
        Vector3 pos = transform.position;
        Collider2D groundCol = player15.CheckCollider(pos, "Wall");
        Collider2D boxCol = player15.CheckCollider(pos, "Box");
        if (groundCol) Destroy(groundCol.gameObject);
        if (boxCol) Destroy(boxCol.gameObject);
    }
}
