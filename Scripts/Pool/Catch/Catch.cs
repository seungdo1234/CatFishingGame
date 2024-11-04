using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Catch : PoolObject
{
    [SerializeField] private float pitchScale;
    private Vector3 initScale;
    private void Awake()
    {
        initScale = transform.localScale;
    }
    public void CatchEffectInit()
    {
        float random = Random.Range(0, pitchScale);
        transform.localScale = initScale + (initScale * random);
    }
    public void DeActivate()
    {
        gameObject.SetActive(false);
    }
}
