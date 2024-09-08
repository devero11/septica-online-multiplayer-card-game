using System.Collections;
using System.Collections.Generic;
using Nakama.TinyJson;
using UnityEngine;

public class DragCard : MonoBehaviour
{
    public int cardIndex = -1;
    public NakamaConnection nakamaConnection;
    private Vector3 originalPosition;
    private bool isDragging = false;
    private Vector3 offset;
    private Camera mainCamera;
    private bool placedOnTable = false;



    private void Awake()
    {
        if(nakamaConnection.Session.Username != nakamaConnection.turnUsername)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
        // Store the original position of the object
        originalPosition = transform.position;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if(nakamaConnection.Session.Username != nakamaConnection.turnUsername)
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,0.5f);
        else 
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1f);
        if (isDragging)
        {
            DragObject();
        }

        // Handle releasing the drag with mouse or touch
        if (isDragging && (Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)))
        {
            isDragging = false;
            StartCoroutine(SmoothReturn());
        }
    }

    private void OnMouseDown()
    {
        if(nakamaConnection.Session.Username == nakamaConnection.turnUsername)
            StartDragging();
    }

    private void OnMouseUp()
    {
        StopDragging();
    }

    private void StartDragging()
    {
        isDragging = true;

        // Calculate offset between object position and mouse/touch position
        Vector3 mousePosition = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            mousePosition = Input.GetTouch(0).position;
        }

        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);
        offset = transform.position - worldPosition;
    }
    void OnEnable()
    {
        if(originalPosition !=null)
        transform.position = originalPosition;
    }
    private void DragObject()
    {
        Vector3 mousePosition = Input.mousePosition;

        if (Input.touchCount > 0)
        {
            mousePosition = Input.GetTouch(0).position;
        }

        mousePosition.z = mainCamera.WorldToScreenPoint(transform.position).z;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        // Move the object to the new position with the offset
        transform.position = worldPosition + offset;
    }

    private void StopDragging()
    {
        isDragging = false;
        int[] x={cardIndex};

        //Sending placed card
        if(placedOnTable == true)
            nakamaConnection.Socket.SendMatchStateAsync(nakamaConnection.match.Id, 103,  x.ToJson());
        StartCoroutine(SmoothReturn());
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Card enters table");
       if(other.tag == "Table"){
            placedOnTable = true;
       }
    }
    void OnTriggerExit2D(Collider2D other)
{
    Debug.Log("Card exits table");
    if(other.tag == "Table"){
            placedOnTable = false;
       }
}
private IEnumerator SmoothReturn()
    {
        float delay = 0.3f; // Delay before starting the smooth return
        float elapsedTime = 0f;
        float duration = 0.5f; // Time to return to the original position
        Vector3 startPosition = transform.position;

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Smooth return logic
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, originalPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final position is exactly at originalPosition
        transform.position = originalPosition;
    }
}
