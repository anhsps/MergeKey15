using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveArrow : MonoBehaviour
{
    private Player15 player15;
    private float duration;
    private Vector3 gridPos;
    private Vector3 direction;
    [SerializeField] private bool down, left, right;

    // Start is called before the first frame update
    void Start()
    {
        player15 = FindObjectOfType<Player15>();
        duration = player15.duration * 2;
        gridPos = transform.position;
        direction = down ? Vector3.down : (left ? Vector3.left : (right ? Vector3.right : Vector3.up));
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdatePos()
    {
        Vector3 nextPos = gridPos + direction;

        while (!CheckBlock(nextPos))
        {
            gridPos = nextPos;
            nextPos = gridPos + direction;
        }

        if (gridPos != transform.position)
            StartCoroutine(player15.Animate(transform, gridPos, duration));

        StartCoroutine(Changes());
    }

    private bool CheckBlock(Vector2 nextPos)
    {
        return Physics2D.OverlapPoint(nextPos, LayerMask.GetMask("Ground", "Box", "Player", "Wall"));
    }

    private IEnumerator Changes()
    {
        SoundManager15.Instance.PlaySound(6);
        yield return new WaitForSeconds(duration);

        direction = -direction;
        Vector3 newScale = transform.localScale;
        if (direction == Vector3.up || direction == Vector3.down)
            newScale.y *= -1;
        else if (direction == Vector3.left || direction == Vector3.right)
            newScale.x *= -1;
        transform.localScale = newScale;
    }
}
