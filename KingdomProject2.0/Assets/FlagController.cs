using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    public GameObject flagPrefab;  // The flag prefab to be instantiated
    public GameObject capsulePrefab;  // The capsule prefab to be instantiated
    public float flagSpawnHeightOffset = -1.0f;  // Adjustable height offset for flag spawn
    public float moveSpeed = 5f;   // Speed at which the capsules move towards the flag
    public KeyCode flagControlKey = KeyCode.LeftShift;  // Key to activate flag placement mode
    public KeyCode spawnCapsuleKey = KeyCode.K;  // Key to spawn new capsules
    public float groundYPosition = 0f;  // Y position of the ground (adjust as needed)
    public float formationSpacing = 1.0f;  // Spacing between capsules in the formation
    public int currentGroupIndex = -1;  // The currently selected group (-1 means all groups)
    public float flagMoveSpeed = 5f;  // Speed at which the flag moves using arrow keys

    [SerializeField]  // Expose the list of groups to the Inspector
    public List<Group> groups = new List<Group>();

    private GameObject currentFlag = null;  // Store the flag that's currently placed
    private bool isPlacingFlag = false;
    private Vector3 lastFlagPosition;  // Store the last position of the flag

    void Start()
    {
        currentFlag = null;  // Ensure no flag exists at game start
        lastFlagPosition = new Vector3(transform.position.x, groundYPosition + flagSpawnHeightOffset, transform.position.z); // Default starting position
        Debug.Log("Selected Group: " + (currentGroupIndex == -1 ? "All Groups" : "Group " + currentGroupIndex));
    }

    void Update()
    {
        // Handle flag placement and movement for the current group
        HandleFlagPlacement();

        // Move units based on the selected group
        MoveSelectedGroupUnits();

        // Spawn a new capsule in the current group when 'K' is pressed
        /*
        if (Input.GetKeyDown(spawnCapsuleKey))
        {
            if (currentGroupIndex != -1)  // Only spawn in the selected group
            {
                SpawnNewUnitInCurrentGroup();
                Debug.Log("Spawned new unit in Group " + currentGroupIndex);
            }
        }
        */

        // Handle group selection
        HandleGroupSelection();
    }

    void HandleFlagPlacement()
    {
        // Start placing the flag when shift is pressed
        if (Input.GetKeyDown(flagControlKey))
        {
            StartFlagPlacement();
        }

        // End flag placement when shift is released and set the target position
        if (Input.GetKeyUp(flagControlKey))
        {
            EndFlagPlacement();
        }

        // Move the flag using arrow keys (only left and right) while placing
        if (isPlacingFlag && currentFlag != null)
        {
            MoveFlagLeftRight();
        }
    }

    void StartFlagPlacement()
    {
        isPlacingFlag = true;

        // Instantiate the flag if it doesn't exist, or make it visible if it was hidden
        if (currentFlag == null)
        {
            currentFlag = Instantiate(flagPrefab, lastFlagPosition, Quaternion.identity);
        }
        else
        {
            // Make sure the flag is active and visible
            currentFlag.transform.position = lastFlagPosition;
            currentFlag.SetActive(true);  // Re-enable the flag
        }
    }

    void EndFlagPlacement()
    {
        isPlacingFlag = false;

        // Set the target position for the current group or all groups and HIDE the flag after placement
        if (currentFlag != null)
        {
            SetTargetPositionForSelectedGroup();
            lastFlagPosition = currentFlag.transform.position;  // Store the flag's last position
            currentFlag.SetActive(false);  // Hide the flag (disable it visually but keep its position)
        }
    }

    void MoveFlagLeftRight()
    {
        Vector3 moveDirection = Vector3.zero;

        // Check for arrow key input to move left or right
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveDirection += Vector3.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            moveDirection += Vector3.right;
        }

        // Move the flag on the X-axis only, keeping Y position fixed to groundYPosition + offset
        currentFlag.transform.position += new Vector3(moveDirection.x * flagMoveSpeed * Time.deltaTime, 0, 0);
        currentFlag.transform.position = new Vector3(currentFlag.transform.position.x, groundYPosition + flagSpawnHeightOffset, currentFlag.transform.position.z);
    }

    void SetTargetPositionForSelectedGroup()
    {
        if (currentFlag != null)
        {
            Vector3 flagPosition = currentFlag.transform.position;

            if (currentGroupIndex == -1)
            {
                // Move all groups if "0" is pressed (currentGroupIndex = -1)
                foreach (Group group in groups)
                {
                    group.SetTargetPosition(flagPosition);
                }
                Debug.Log("All groups target position set at " + flagPosition);
            }
            else
            {
                // Move only the selected group
                groups[currentGroupIndex].SetTargetPosition(flagPosition);
                Debug.Log("Group " + currentGroupIndex + " target position set at " + flagPosition);
            }
        }
    }

    void MoveSelectedGroupUnits()
    {
        if (currentGroupIndex == -1)
        {
            // Move all groups if "0" is pressed (currentGroupIndex = -1)
            foreach (Group group in groups)
            {
                group.MoveUnits(moveSpeed);
            }
        }
        else
        {
            // Move only the selected group
            groups[currentGroupIndex].MoveUnits(moveSpeed);
        }
    }

    void SpawnNewUnitInCurrentGroup()
    {
        groups[currentGroupIndex].SpawnNewUnit(capsulePrefab, groundYPosition);
    }

    public void CreateNewGroup()
    {
        Group newGroup = new Group(groundYPosition); // Don't create a flag here
        newGroup.formationSpacing = formationSpacing;
        groups.Add(newGroup);
    }

    void HandleGroupSelection()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            currentGroupIndex = -1;  // Select all groups
            Debug.Log("Selected All Groups");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && groups.Count >= 1)
        {
            currentGroupIndex = 0;  // Select Group 1
            Debug.Log("Selected Group 1");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && groups.Count >= 2)
        {
            currentGroupIndex = 1;  // Select Group 2
            Debug.Log("Selected Group 2");
        }
        // Add more group selection keys as needed (Alpha3, Alpha4, etc.)
        // Ensure that the selected group number does not exceed the number of groups
        else if (Input.GetKeyDown(KeyCode.Alpha3) && groups.Count >= 3)
        {
            currentGroupIndex = 2;  // Select Group 3
            Debug.Log("Selected Group 3");
        }
    }
}
