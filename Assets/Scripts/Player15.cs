using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player15 : MonoBehaviour
{
    [SerializeField] private Sprite char2;
    private MatchObj matchObj;
    private SpriteRenderer sr;
    private bool changeType;
    private bool waiting;
    private Vector3 gridPos;
    private Button UpBtn, DownBtn, LeftBtn, RightBtn;

    [HideInInspector] public float duration = 0.1f;
    public GameObject explosionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        matchObj = FindObjectOfType<MatchObj>();
        sr = GetComponent<SpriteRenderer>();
        gridPos = transform.position;

        UpBtn = GameObject.Find("UpBtn").GetComponent<Button>();
        DownBtn = GameObject.Find("DownBtn").GetComponent<Button>();
        LeftBtn = GameObject.Find("LeftBtn").GetComponent<Button>();
        RightBtn = GameObject.Find("RightBtn").GetComponent<Button>();

        UpBtn.onClick.AddListener(() => UpdatePos(Vector3.up));
        DownBtn.onClick.AddListener(() => UpdatePos(Vector3.down));
        LeftBtn.onClick.AddListener(() => UpdatePos(Vector3.left));
        RightBtn.onClick.AddListener(() => UpdatePos(Vector3.right));
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
    }

    private void MoveInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) UpdatePos(Vector3.up);
        if (Input.GetKeyDown(KeyCode.DownArrow)) UpdatePos(Vector3.down);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) UpdatePos(Vector3.left);
        if (Input.GetKeyDown(KeyCode.RightArrow)) UpdatePos(Vector3.right);
    }

    private void UpdatePos(Vector3 direction)
    {
        Vector3 nextPos = gridPos + direction;
        if (waiting || CheckCollider2(nextPos, "Ground", "Wall")) return;

        Collider2D boxCol = CheckCollider(nextPos, "Box");

        if (boxCol)
        {
            if (CanPush(direction, nextPos))
            {
                gridPos = nextPos;
                MoveBox(direction, boxCol);
            }
            else return;
        }
        else gridPos = nextPos;

        if (gridPos != transform.position)
        {
            StartCoroutine(WaitForChanges());
            StartCoroutine(Animate(transform, gridPos, duration));

            if (CheckCollider(gridPos, "Finish")) StartCoroutine(Winner());
        }
    }

    private IEnumerator Winner()
    {
        yield return new WaitForSeconds(duration * 2);

        Debug.Log("Win");
        GameManager15.Instance.GameWin();
    }

    public Collider2D CheckCollider(Vector3 pos, string nameLayer)
    {
        return Physics2D.OverlapPoint(pos, LayerMask.GetMask(nameLayer));
    }
    public Collider2D CheckCollider2(Vector3 pos, string nameLayer, string nameLayer2)
    {
        return Physics2D.OverlapPoint(pos, LayerMask.GetMask(nameLayer, nameLayer2));
    }

    private bool CanPush(Vector3 direction, Vector3 pos)
    {
        Vector3 boxNextPos = pos + direction;
        if (CheckCollider2(boxNextPos, "Ground", "Wall")) return false;

        if (CheckCollider(boxNextPos, "Box"))
            return changeType ? CanPush(direction, boxNextPos) : false;

        return true;
    }

    private void MoveBox(Vector3 direction, Collider2D boxCol)
    {
        if (!boxCol) return;//

        Vector3 boxNextPos = boxCol.transform.position + direction;

        StartCoroutine(Animate(boxCol.transform, boxNextPos, duration));
        StartCoroutine(CheckMatch(boxCol));

        if (changeType)
        {
            Collider2D boxNextCol = CheckCollider(boxNextPos, "Box");
            if (boxNextCol) MoveBox(direction, boxNextCol);
        }
    }

    private IEnumerator WaitForChanges()
    {
        SoundManager15.Instance.PlaySound(4);
        waiting = true;
        yield return new WaitForSeconds(duration * 2.5f);
        waiting = false;
    }

    public IEnumerator Animate(Transform trans, Vector3 to, float duration)
    {
        float elapsed = 0;
        Vector3 from = trans.position;

        while (elapsed < duration)
        {
            trans.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        trans.position = to;
    }

    private IEnumerator CheckMatch(Collider2D boxCol)
    {
        yield return new WaitForSeconds(duration);
        if (boxCol) matchObj.CheckMatch(boxCol.transform);
    }

    public void SetType()
    {
        SoundManager15.Instance.PlaySound(8);
        changeType = true;
        sr.sprite = char2;
    }
}
