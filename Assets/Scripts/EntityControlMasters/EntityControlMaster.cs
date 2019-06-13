using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class EntityControlMaster : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] protected EntityController entityController;
    public EntityController EntityController { get { return entityController; } }
}
