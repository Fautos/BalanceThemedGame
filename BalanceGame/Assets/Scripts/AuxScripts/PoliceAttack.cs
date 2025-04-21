using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicemanArm : MonoBehaviour
{
    public float extendDistance = 1.5f;
    public float extendSpeed = 8f;
    public float retractSpeed = 6f;
    public LayerMask targetLayer;
    public bool isExtending = false;

    private Vector3 initialLocalPosition;
    private Coroutine currentCoroutine;

    private void Start()
    {
        initialLocalPosition = transform.localPosition;
    }

    public void ActivateArm()
    {
        if (currentCoroutine == null)
        {
            currentCoroutine = StartCoroutine(ExtendAndRetract());
        }
    }

    private IEnumerator ExtendAndRetract()
    {
        isExtending = true;
        Vector3 targetLocalPosition = initialLocalPosition + Vector3.right * extendDistance;

        while (Vector3.Distance(transform.localPosition, targetLocalPosition) > 0.05f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetLocalPosition, extendSpeed * Time.deltaTime);
            DetectCollision();
            yield return null;
        }

        isExtending = false;

        // Pequeña pausa si quieres
        yield return new WaitForSeconds(0.1f);

        while (Vector3.Distance(transform.localPosition, initialLocalPosition) > 0.05f)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, initialLocalPosition, retractSpeed * Time.deltaTime);
            yield return null;
        }

        transform.localPosition = initialLocalPosition;
        currentCoroutine = null;
    }

    private void DetectCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 0.5f, targetLayer);

        if (hit.collider != null)
        {
            Debug.Log("¡Brazo del policía atrapó a alguien!");
            // Aquí podrías llamar a un método para castigar directamente si quieres
            // También puedes mandar mensaje al PoliceController si quieres coordinar mejor
        }
    }
}