using UnityEngine;

public class ObjectContact : MonoBehaviour
{
    [SerializeField]
    private GameObject contactPrefab;

    private PlayerNetwork playerNetwork;
    private GameObject instantiated;
    private Vector3 startPosition;
    private float startTime;
    private float endTime;
    private GameObject draggedObject;
    private bool dragging;
    private bool isRelevant;

    private void Awake()
    {
        playerNetwork = GetComponent<PlayerNetwork>();
    }

    private void OnEnable()
    {
        playerNetwork.OnStartTouch += DragStart;
        playerNetwork.OnDuringTouch += DragPeriod;
        playerNetwork.OnEndTouch += DragEnd;
        GameStates.Instance.OnStateChangedToInitial += CreateContactArea;
        GameStates.Instance.OnStateChangedToStart += RotateContactArea;
        GameStates.Instance.OnStateChangedToPlayer2 += ShowContactArea;
        GameStates.Instance.OnStateChangedToOpponent1 += HideContactArea;
    }

    private void OnDisable()
    {
        playerNetwork.OnStartTouch -= DragStart;
        playerNetwork.OnDuringTouch -= DragPeriod;
        playerNetwork.OnEndTouch -= DragEnd;
        GameStates.Instance.OnStateChangedToInitial -= CreateContactArea;
        GameStates.Instance.OnStateChangedToStart -= RotateContactArea;
        GameStates.Instance.OnStateChangedToPlayer2 -= ShowContactArea;
        GameStates.Instance.OnStateChangedToOpponent1 -= HideContactArea;
    }

    private void CreateContactArea()
    {
        instantiated = Instantiate(contactPrefab, transform.position, Quaternion.identity);
    }

    private void RotateContactArea()
    {
        instantiated.transform.Rotate(new Vector3(0, 180, 0));
    }

    private void ShowContactArea()
    {
        instantiated.SetActive(true);
    }

    private void HideContactArea()
    {
        instantiated.SetActive(false);
    }

    private void DragStart(Ray ray, float time)
    {
        RaycastHit checkHit;
        if (Physics.Raycast(ray, out checkHit, Mathf.Infinity, LayerMask.GetMask("PlayerCard")))
        {
            draggedObject = checkHit.transform.gameObject;
            startPosition = draggedObject.transform.position;
            startTime = time;
            dragging = true;
        }
    }
    
    private void DragPeriod(Ray ray, float time)
    {
        RaycastHit checkHit;
        if (draggedObject != null)
        {
            if (draggedObject.CompareTag("Object1") || draggedObject.CompareTag("Object2") || draggedObject.CompareTag("Object3"))
            {
                if (Physics.Raycast(ray, out checkHit, Mathf.Infinity, LayerMask.GetMask("Plane1")))
                {
                    isRelevant = true;
                }
                else
                {
                    isRelevant = false;
                }
            }
            else
            {
                isRelevant = false;
            }

            if (Physics.Raycast(ray, out checkHit, Mathf.Infinity, LayerMask.GetMask("TablePlane")))
            {
                draggedObject.transform.position = checkHit.point;
            }
            else
            {
                endTime = time;
                draggedObject.transform.position = startPosition;
                draggedObject = null;
                dragging = false;
            }
        }
        else
        {
            isRelevant = false;

        }
    }
    
    private void DragEnd(Ray ray, float time)
    {
        if (draggedObject == null) return;
        RaycastHit checkHit;
        if (Physics.Raycast(ray, out checkHit, Mathf.Infinity, LayerMask.GetMask("Plane1")))
        {
            if (draggedObject.CompareTag("Object1") || draggedObject.CompareTag("Object2") || draggedObject.CompareTag("Object3"))
            {
                Destroy(draggedObject);
                draggedObject = null;
            }
        }
        if (isRelevant == true) return;
        endTime = time;
        draggedObject.transform.position = startPosition;
        draggedObject = null;
        dragging = false;
    }
}
