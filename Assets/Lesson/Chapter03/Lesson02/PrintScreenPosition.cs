using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintScreenPosition : MonoBehaviour
{
    // Start is called before the first frame update
    public uint a;
    public uint b;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var position = Camera.main.WorldToScreenPoint(transform.position);
        print(position);

        print((uint)b);
        print(a - b);
    }
}
