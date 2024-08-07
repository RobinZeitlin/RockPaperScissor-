using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [Header("Player Values")]
    [Range(50,550)]
    [SerializeField] private float mouseSensitivity = 100f;
    [Range(1,50)]
    [SerializeField] private float moveSpeed;

    [Header("References")]
    [SerializeField] private Camera pCamera;
    [SerializeField] private GameObject pMesh;
    [SerializeField] private GameObject canvas;

    [SerializeField] private GameObject arms;

    private Rigidbody rb;
    private Transform target;

    private float xRotation = 0f;
    private float yRotation = 0f;

    private RectTransform rectTransform;
    private Vector3 originalPosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
       
    }

    private void Start()
    {
        rectTransform = arms.GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    public void Die()
    {
        transform.position = new Vector3(0, 0, 0);

        Debug.Log("Player Died");
    }
    private void Update()
    {
        if (!IsOwner)
        {
            pCamera.enabled = false;
            canvas.SetActive(false);
            return;
        }

        if(pMesh != null)
            pMesh.SetActive(false);

        Movement();
        Camera();

        float yOffset = Mathf.Sin(Time.time * 5) * 10f;
        rectTransform.anchoredPosition = new Vector2(originalPosition.x, originalPosition.y + 50f + yOffset);
    }

    void Movement()
    {
        float posX = Input.GetAxis("Horizontal");
        float posZ = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(posX, 0, posZ);

        if (move.magnitude > 0)
        {
            Vector3 moveDirection = pCamera.transform.forward * move.z + pCamera.transform.right * move.x;
            moveDirection.y = 0;

            rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    void Camera()
    {
        if(target != null)
        {
            pCamera.transform.LookAt(target.position);
        }
        else
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            yRotation -= mouseY;
            yRotation = Mathf.Clamp(yRotation, -90f, 90f);

            xRotation -= mouseX;

            pCamera.transform.localRotation = Quaternion.Euler(yRotation, -xRotation, 0f);
            transform.localRotation = Quaternion.Euler(0, -xRotation, 0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawSphere(pCamera.transform.position, .25f);
        Gizmos.DrawRay(pCamera.transform.position, pCamera.transform.forward.normalized * 3);
    }
}
