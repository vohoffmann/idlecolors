using System;
using UnityEngine;

public class SaugRobbiContoller : MonoBehaviour
{
    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        InvokeRepeating(nameof(startRobbi), 30f, 45f);
    }

    private void startRobbi()
    {
        _animator.Play("saugrobbi", 0, 0);
    }
}