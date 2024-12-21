using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyPatrol : MonoBehaviour
{
    public enum PatrolType
    {
        Random, // Patrol in a random direction
        Horizontal, // Patrol horizontally
        Vertical // Patrol vertically
    }
    
    [SerializeField] private float bounds = 10f; // Maximum bounds for enemy patrol
    [SerializeField] private PatrolType patrolType = PatrolType.Random; // Current patrol type

    private Rigidbody _rigidbody; // Rigidbody component for movement control
    
    // Data
    private Vector3 _targetPosition; // Current target position for patrol
    private float _findNewTargetPositionTimer; // Timer to track when to find a new target
    private float GroundLevelY => _initialPosition.y; // Ground level (Y-axis) from the initial position

    [field: SerializeField]
    public float FindNewTargetDuration { get; set; } = 3f;
    [field: SerializeField]
    public float Speed { get; set; } = 2f;

    private Vector3 _initialPosition; // Initial position of the enemy

    private void Awake()
    {
        // Get the Rigidbody component attached to this game object
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Set the initial position and assign a random position within bounds
        _initialPosition = _rigidbody.position = GetRandomPosition();
        
        // Set the first target position for the patrol
        SetNewTargetPosition();
    }

    #region Update
    
    private void Update()
    {
         // Increment the timer for finding a new target
        _findNewTargetPositionTimer += Time.deltaTime;
        
        // Move towards the initiated target
        MoveTowardsTarget();
        
        // If the enemy reaches the target
        // Or if the target change timer is up, set a new target
        var reachedTarget = Vector3.Distance(_rigidbody.position, _targetPosition) < 0.1f;
        if (reachedTarget || IsFindNewTargetPositionTimerUp())
        {
            // Set a new target
            SetNewTargetPosition();
        }
        
        // NOTE: As the target position is already within the bounds, there is no need to clamp the position
    }

    private void MoveTowardsTarget()
    {
        // Calculate the movement direction toward the target position
        var movementDirection = (_targetPosition - _rigidbody.position).normalized;
        
        // If the enemy is close to the target, stop moving
        if(movementDirection.sqrMagnitude < 0.1f) movementDirection = Vector3.zero;
        
         // Update the Rigidbody velocity to move the enemy
        _rigidbody.velocity = movementDirection * Speed;
    }
    
    private bool IsFindNewTargetPositionTimerUp()
    {
        // Check if the timer has reached the duration for finding a new target
        return _findNewTargetPositionTimer >= FindNewTargetDuration;
    }
    
    #endregion
    
    #region Utilities
    
    private void SetNewTargetPosition()
    {
        // Assign a new target position based on the patrol type
        _targetPosition = GetRandomTargetPosition();
        // Reset the timer
        _findNewTargetPositionTimer = 0f;
        
        // Debug log to show the new target position
        Debug.Log("Setting new target position: " + _targetPosition);
    }

    // Gets a random position within game bounds, Y level is derived from the initial position of the player
    private Vector3 GetRandomPosition()
    {
        // Get a random position within the specified bounds (ground level is fixed)
        return new Vector3(Random.Range(-bounds, bounds), GroundLevelY, Random.Range(-bounds, bounds));
    }
    
    private Vector3 GetRandomTargetPosition()
    {
        // Determine the next target position based on the patrol type
        var targetPosition = patrolType switch
        {
            // CAREFUL: Vector3.right (1, 0, 0) and Vector3.forward (0, 0, 1) are the axes parallel to plane surface
            PatrolType.Horizontal => _initialPosition + Vector3.right * bounds, // Move horizontally
            PatrolType.Vertical => _initialPosition + Vector3.forward * bounds, // Move vertically
            // Random position within the bounds
            _ => GetRandomPosition()
        };
        
        return targetPosition;
    }
    
    #endregion

    private void OnCollisionEnter(Collision other)
    {
        // On collision with another enemy or player, set a new target
        if (other.collider.CompareTag("Enemy") || other.collider.CompareTag("Player"))
        {
            SetNewTargetPosition();
        }
    }
}

