using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using Models;
using Unity.Cinemachine;
using UnityEngine;

public class Field : MonoBehaviour
{
    [field: SerializeField] public CropSlot[] Crops { get; private set; } //expose crop slots
}