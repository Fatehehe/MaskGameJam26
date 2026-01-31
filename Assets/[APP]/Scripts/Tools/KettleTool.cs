using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class KettleTool : InteractableObject
{
    private enum KettleState { Idle, OnStove, Boiling, Boiled }

    [Header("Boiling Settings")]
    [SerializeField] private float boilingTime = 3f;

    private KettleState state = KettleState.Idle;
    private Coroutine boilingRoutine;
    private StoveTool currentStove;

    private Vector2 originalPos;
    private Transform originalParent;

    protected override void Awake()
    {
        base.Awake();
        CacheOriginalTransform();
    }

    private void CacheOriginalTransform()
    {
        originalPos = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        StoveTool stove = eventData.pointerEnter?.GetComponentInParent<StoveTool>();
        if (stove == null) return false;

        AttachToStove(stove);
        return true;
    }

    private void AttachToStove(StoveTool stove)
    {
        currentStove = stove;
        transform.SetParent(stove.snapPoint);
        rectTransform.anchoredPosition = Vector2.zero;

        if (state == KettleState.Boiled)
        {
            state = KettleState.OnStove;
            return;
        }

        StartBoiling();
    }

    private void StartBoiling()
    {
        StopBoilingRoutine();

        state = KettleState.Boiling;
        boilingRoutine = StartCoroutine(BoilingProcess());
        Debug.Log("Kettle started boiling...");
    }

    private IEnumerator BoilingProcess()
    {
        yield return new WaitForSeconds(boilingTime);
        boilingRoutine = null;
        state = KettleState.Boiled;
        Debug.Log("Kettle finished boiling!");
    }

    private void CancelBoiling()
    {
        StopBoilingRoutine();
        state = KettleState.Idle;
        currentStove = null;
        Debug.Log("Boiling cancelled!");
    }

    private void StopBoilingRoutine()
    {
        if (boilingRoutine != null)
        {
            StopCoroutine(boilingRoutine);
            boilingRoutine = null;
        }
    }

    protected override void OnDragStart(PointerEventData eventData)
    {
        if (state == KettleState.Boiling)
            CancelBoiling();

        if (currentStove != null)
        {
            currentStove = null;
            DetachToOrigin();
        }
    }

    protected override void OnDragEnd(PointerEventData eventData, bool success)
    {
        if (!success)
            ReturnToOriginal();
    }

    private void ReturnToOriginal()
    {
        transform.SetParent(originalParent);
        rectTransform.anchoredPosition = originalPos;
    }
}
