using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    private Vector3 _previousPosition;
    private const float MOVEMENT_THRESHOLD = 0.001f;
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _previousPosition = transform.position;
    }

    // Update is called once per frame

    void Update()
    {
        // Get current position
        Vector3 currentPosition = transform.position;

        // Check if X or Z position has changed significantly
        bool isMoving = Mathf.Abs(currentPosition.x - _previousPosition.x) > MOVEMENT_THRESHOLD ||
                        Mathf.Abs(currentPosition.z - _previousPosition.z) > MOVEMENT_THRESHOLD;

        // Set isWalking parameter in Animator
        _animator.SetBool("isWalking", isMoving);

        // Update previous position for next frame
        _previousPosition = currentPosition;
    }
}
