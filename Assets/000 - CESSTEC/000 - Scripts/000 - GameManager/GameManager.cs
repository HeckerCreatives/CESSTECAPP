using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [field: Header("DEBUGGER")]
    [field: ReadOnly] [field: SerializeField] public bool CanUseButtons { get; set; }
}
