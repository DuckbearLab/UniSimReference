using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class VrLinkDISAgent : MonoBehaviour
{

    [DllImport("VrLinkDIS")]
    extern static int test(int a);

    // Use this for initialization
    void Start()
    {
        Debug.Log(test(5));
    }

    // Update is called once per frame
    void Update()
    {

    }
}