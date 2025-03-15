using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchObj : MonoBehaviour
{
    private Player15 player15;
    private Transform itemTrans;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        player15 = FindObjectOfType<Player15>();
        timer = player15.duration;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CheckMatch(Transform itemTrans)
    {
        if (itemTrans.GetComponent<ItemID>())// fix bug push box truoc item
            this.itemTrans = itemTrans;
        else return;

        List<GameObject> horizontal = CheckDirection(Vector3.right);
        List<GameObject> vertical = CheckDirection(Vector3.up);

        if (horizontal.Count >= 3) StartCoroutine(DestroyObjs(horizontal));
        if (vertical.Count >= 3) StartCoroutine(DestroyObjs(vertical));
    }

    private List<GameObject> CheckDirection(Vector3 dir)
    {
        List<GameObject> objs = new List<GameObject> { itemTrans.gameObject };
        objs.AddRange(GetObjects(dir));
        objs.AddRange(GetObjects(-dir));
        return objs;
    }

    private List<GameObject> GetObjects(Vector3 dir)
    {
        List<GameObject> objs = new List<GameObject>();
        Vector3 nextPos = itemTrans.position + dir;
        Sprite sprite = itemTrans.GetComponent<SpriteRenderer>().sprite;

        while (true)
        {
            Collider2D col = player15.CheckCollider(nextPos, "Box");
            if (col && col.CompareTag("Item") && col.GetComponent<SpriteRenderer>().sprite == sprite)
            {
                objs.Add(col.gameObject);
                nextPos += dir;
            }
            else break;
        }

        return objs;
    }

    private IEnumerator DestroyObjs(List<GameObject> objs)
    {
        yield return new WaitForSeconds(timer);

        int itemID = objs[0].GetComponent<ItemID>().id;

        if (itemID == 0) player15.SetType();
        else if (itemID == -1)
        {
            MoveArrow[] moveArrows = FindObjectsOfType<MoveArrow>();
            foreach (var item in moveArrows)
                item.UpdatePos();
        }
        else if (itemID == -2)
        {// bomb
            foreach (var obj in objs)
            {
                Vector3 centerPos = obj.transform.position;
                Vector3[] directions = new Vector3[]
                {
                    Vector3.up, Vector3.down,
                    Vector3.left, Vector3.right,
                    Vector3.up + Vector3.left,
                    Vector3.up + Vector3.right,
                    Vector3.down + Vector3.left,
                    Vector3.down + Vector3.right
                };

                SpawnExplosion(centerPos);
                foreach (var dir in directions)
                    SpawnExplosion(centerPos + dir);
            }
            SoundManager15.Instance.PlaySound(7);
        }
        else DestroyLock(itemID);

        objs.ForEach(Destroy);
    }

    private void DestroyLock(int keyID)
    {
        GameObject[] locks = GameObject.FindGameObjectsWithTag("Lock");
        foreach (GameObject lockObj in locks)
        {
            var _lock = lockObj.GetComponent<ItemID>();
            if (_lock && _lock.id == keyID)
            {
                SoundManager15.Instance.PlaySound(5);
                StartCoroutine(Fade(lockObj));
            }
        }
    }

    private IEnumerator Fade(GameObject obj)
    {
        yield return new WaitForSeconds(timer);

        float elapsed = 0;
        float duration = 0.25f;
        SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
        Color color = sr.color;

        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(1f, 0, elapsed / duration);
            sr.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }

        color.a = 0;
        sr.color = color;
        Destroy(obj);

        yield return null;// wait destroy obj
        if (GameObject.FindGameObjectsWithTag("Lock").Length == 0)
            GameObject.FindGameObjectWithTag("Finish").GetComponent<Animator>().SetTrigger("Open");
    }

    private void SpawnExplosion(Vector3 pos)
    {
        var prefab = Instantiate(player15.explosionPrefab, pos, Quaternion.identity);
        StartCoroutine(ChangeScale(prefab.transform, 0.25f));
    }

    private IEnumerator ChangeScale(Transform trans, float t)
    {
        Vector3 from = trans.localScale;
        Vector3 to = trans.localScale * 0.5f;
        float elapsed = 0;

        while (elapsed < t)
        {
            trans.localScale = Vector3.Lerp(from, to, elapsed / t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        trans.localScale = to;
        Destroy(trans.gameObject);
    }
}
