using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private Slider amountSlider;
    [SerializeField]
    private Slider speedSlider;
    [SerializeField]
    private Toggle follow;

    private void Awake()
    {
        SetAmount();
        SetSpeed();
        SetFollow();
    }

    public void SetAmount()
    {
        Fish.spawnCount = 250 + amountSlider.value * 50;
    }

    public void SetSpeed()
    {
        Fish.speed = speedSlider.value;
    }

    public void SetFollow()
    {
        FishSpawner.followTarget = follow.isOn;
    }
}
