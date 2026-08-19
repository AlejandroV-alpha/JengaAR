using UnityEngine;

public class Block : MonoBehaviour
{
    private Camera arCamera;
    private Rigidbody rb;

    private bool isDragging = false;
    private bool blockWasMoved = false;

    private Vector2 initialTouchPosition;
    private Vector3 initialBlockPosition;

    [SerializeField]
    private float movementScale = 0.001f;

    [SerializeField]
    private float minimumRemovalDistance = 0.05f;

    private int currentFloor;

    private void Start()
    {
        arCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.touchCount == 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                StartDragging(touch.position);
                break;

            case TouchPhase.Moved:
                if (isDragging)
                {
                    DragBlock(touch.position);
                }
                break;

            case TouchPhase.Ended:
                StopDragging();
                break;

            case TouchPhase.Canceled:
                StopDragging();
                break;
        }
    }

    private void StartDragging(Vector2 touchPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(touchPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.gameObject != gameObject)
            {
                return;
            }

            // Comprobar si este bloque pertenece al piso superior
            if (!JengaManager.Instance.CanRemoveBlock(currentFloor))
            {
                Debug.Log(
                    "NO SE PUEDE RETIRAR: " +
                    gameObject.name +
                    " | Piso: " +
                    currentFloor
                );

                return;
            }

            isDragging = true;
            blockWasMoved = false;

            initialTouchPosition = touchPosition;
            initialBlockPosition = transform.position;

            rb.isKinematic = true;

            Debug.Log(
                "ARRASTRANDO: " +
                gameObject.name +
                " | Piso: " +
                currentFloor
            );
        }
    }

    private void DragBlock(Vector2 currentTouchPosition)
    {
        Vector2 touchDelta =
            currentTouchPosition - initialTouchPosition;

        float horizontalMovement =
            touchDelta.x * movementScale;

        float forwardMovement =
            touchDelta.y * movementScale;

        Vector3 cameraRight = arCamera.transform.right;
        Vector3 cameraForward = arCamera.transform.forward;

        cameraRight.y = 0.0f;
        cameraForward.y = 0.0f;

        cameraRight.Normalize();
        cameraForward.Normalize();

        Vector3 movement = cameraRight * horizontalMovement + cameraForward * forwardMovement;

        movement.y = 0.0f;

        Vector3 targetPosition = initialBlockPosition + movement;

        MoveBlock(targetPosition);

        float distanceMoved = Vector3.Distance(initialBlockPosition, transform.position);

        if (distanceMoved >= minimumRemovalDistance)
        {
            blockWasMoved = true;
        }
    }

    private void MoveBlock(Vector3 targetPosition)
    {
        Vector3 currentPosition = transform.position;

        targetPosition.y = initialBlockPosition.y;
        currentPosition.y = initialBlockPosition.y;

        Vector3 direction = targetPosition - currentPosition;

        float distance = direction.magnitude;

        if (distance <= 0.0f)
        {
            return;
        }

        direction.y = 0.0f;
        direction.Normalize();

        if (rb.SweepTest(
            direction,
            out RaycastHit hit,
            distance,
            QueryTriggerInteraction.Ignore))
        {
            float safeDistance = Mathf.Max(0.0f, hit.distance - 0.001f);

            transform.position =
                new Vector3(
                    currentPosition.x +
                    direction.x * safeDistance,

                    initialBlockPosition.y,

                    currentPosition.z +
                    direction.z * safeDistance
                );

            return;
        }

        transform.position =
            new Vector3(
                targetPosition.x,
                initialBlockPosition.y,
                targetPosition.z
            );
    }

    private void StopDragging()
    {
        if (!isDragging)
        {
            return;
        }

        isDragging = false;

        rb.isKinematic = false;

        // Solo considerar retirado si realmente se movió
        if (blockWasMoved)
        {
            Debug.Log(
                "BLOQUE RETIRADO: " +
                gameObject.name
            );

            JengaManager.Instance.MoveBlockToTop(this);
        }
        else
        {
            Debug.Log(
                "BLOQUE NO RETIRADO: " +
                gameObject.name +
                " | No se movió suficiente."
            );
        }
    }

    public void SetFloor(int floor)
    {
        currentFloor = floor;
    }

    public int GetFloor()
    {
        return currentFloor;
    }
}