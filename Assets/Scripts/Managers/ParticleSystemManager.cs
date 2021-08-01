using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemManager : MonoBehaviour
{
    [Header("Camera Shake")]
    [SerializeField] float duration;
    [SerializeField] float magnitude;

    private void Start()
    {
        if (duration > 0 || magnitude > 0)
        {
            StartCoroutine(CameraShaker.instance.Shake(duration, magnitude));
        }
    }
}
