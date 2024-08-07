using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ReactionAnimation : NetworkBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float speed;

    [SerializeField] private AnimationCurve curve;

    private void Awake()
    {
        StartCoroutine(SetTransform());
    }

    public IEnumerator SetTransform()
    {
        float startTime = 0;

        while (startTime < duration)
        {
            transform.localScale = Vector3.one * curve.Evaluate(startTime / duration);

            transform.position += new Vector3(0, .1f, 0) * speed * Time.deltaTime;

            transform.eulerAngles += new Vector3(0, 10f, 0f) * speed * Time.deltaTime;

            startTime += Time.deltaTime;
            
            yield return null;
        }

        if (IsServer)
        {
            Destroy(gameObject);
        }
    }
}
