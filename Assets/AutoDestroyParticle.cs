using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroyParticle : MonoBehaviour
{
    private void OnPart()
    {
        Destroy(this.gameObject);
    }
}
