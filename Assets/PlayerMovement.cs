using System;
using UnityEngine;
using UnityEngine.SceneManagement; // Necessary for using the SceneManager class

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Player movement speed, adjustable in Unity Inspector

    private Rigidbody _rigidbody; // Reference to the Rigidbody component for controlling physics

    private void Awake() // Called when the script instance is being loaded
    {
        // Get and store the Rigidbody component attached to the player object
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() // Called once per frame
    {
        // Get the player's movement direction from input and normalize it
        var moveDirection = GetInput().normalized;

        // Update the Rigidbody's velocity based on the movement direction and speed
        _rigidbody.velocity = moveDirection * moveSpeed;
    }

    private Vector3 GetInput() // Method to retrieve player input from keyboard or controller
    {
        // Input.GetAxis returns values between -1 and 1 for movement keys (WASD or Arrow Keys)
        var moveX = Input.GetAxis("Horizontal"); // Horizontal movement (A/D or Left/Right)
        var moveZ = Input.GetAxis("Vertical"); // Vertical movement (W/S or Up/Down)

        // Combine horizontal and vertical input into a 3D vector
        return new Vector3(moveX, 0, moveZ); // Y-axis remains 0 as movement is on a flat plane
    }

    private void OnCollisionEnter(Collision collision) // Called when the player collides with another object
    {
        // There is indeed a collision with enemy, but the method being implemented in PlayerMovement class,
        // doesn't mean that the player is the one that is intended to collide with the enemy
        if (collision.collider.CompareTag("Enemy")) // Check if the object collided with has the tag "Enemy"
        {
            // If we want to detect if the player is the main collider, we can track it by using a little hack
            // Calculate the vector difference between the enemy's position and the player's position
            var delta = collision.collider.transform.position - _rigidbody.position;
            delta.y = 0; // Ignore Y-axis as the collision is being checked in a flat plane
            
            // Get the player's velocity, ignoring any vertical component
            var playerVelocity = _rigidbody.velocity;
            playerVelocity.y = 0;
            
            // Getting the dot product of the inverted delta vector and the velocity vector of the player
            // (If the player is moving towards the enemy, this will return true)
            var isPlayer = Vector3.Dot(delta.normalized, playerVelocity.normalized) > 0;
            
            // Trace the collision
            // Log whether the player initiated the collision or the enemy did
            Debug.Log(isPlayer ? "Player collided with enemy" : "Enemy collided with player"); 
            LoadBattleScene(); // Load the battle scene if the player collides with an enemy
        }
    }

    private void LoadBattleScene() // Method to load the battle scene
    {
    // Use SceneManager to load a new scene called "BattleScene"
    SceneManager.LoadScene("BattleScene"); // Name of the scene to load
    }

}



