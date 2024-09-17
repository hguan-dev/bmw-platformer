using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MinusThree : MonoBehaviour
{

    public TextMeshPro text;
    void Start()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }

        void Update()
    {
        transform.position += new Vector3(0, 2) * Time.deltaTime;
        text.color -= new Color(0,0,0,0.5f) * Time.deltaTime;

    }
}
