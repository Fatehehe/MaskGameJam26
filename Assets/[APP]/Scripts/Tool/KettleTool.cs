using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System;

public class KettleTool : InteractableObject
{
    public enum KettleState { Idle, OnStove, Boiling, Boiled }

    [Header("Boiling Settings")]
    [SerializeField] private float boilingTime = 3f;

    public event Action<KettleState> OnStateChanged;

    private KettleState state = KettleState.Idle;
    public KettleState State => state;
    private Coroutine boilingRoutine;
    private StoveTool currentStove;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void ForceReset()
    {
        base.ForceReset();

        CancelBoiling();
        
        if (currentStove != null)
        {
            currentStove.RemoveKettle();
            currentStove = null;
        }

        Debug.Log("Kettle Reset to Shelf & Cooled Down.");
    }

    protected override bool TryHandleDrop(PointerEventData eventData)
    {
        StoveTool stove = eventData.pointerEnter?.GetComponentInParent<StoveTool>();
        if (stove != null)
        {
            AttachToStove(stove);
            return true;
        }

        GlassTool glass = eventData.pointerEnter?.GetComponent<GlassTool>();
        if (glass != null && State == KettleState.Boiled)
        {
            glass.AddHotWater();
            ChangeState(KettleState.Idle);

            DetachToOrigin();
            return false;
        }

        DetachToOrigin();
        return false;
    }

    private void AttachToStove(StoveTool stove)
    {
        currentStove = stove;
        ChangeState(KettleState.OnStove);

        transform.SetParent(stove.snapPoint);
        rectTransform.anchoredPosition = Vector2.zero;

        stove.PlaceKettle(this);
    }

    public void StartBoilingFromStove()
    {
        if (state == KettleState.OnStove)
            StartBoiling();
    }

    private void StartBoiling()
    {
        StopBoilingRoutine();
        ChangeState(KettleState.Boiling);
        boilingRoutine = StartCoroutine(BoilingProcess());
        Debug.Log("Kettle started boiling...");
    }

    private IEnumerator BoilingProcess()
    {
        yield return new WaitForSeconds(boilingTime);
        boilingRoutine = null;
        ChangeState(KettleState.Boiled);
        Debug.Log("Kettle finished boiling!");
    }

    private void CancelBoiling()
    {
        StopBoilingRoutine();
        ChangeState(KettleState.Idle);

        if (currentStove != null)
            currentStove.RemoveKettle();

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
            currentStove.RemoveKettle();
            currentStove = null;
        }
    }

    protected override void OnDragging(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    protected override void OnDragEnd(PointerEventData eventData, bool success)
    {
        if (!success)
            ReturnToStartPosition();
    }

    private void ChangeState(KettleState newState)
    {
        state = newState;
        OnStateChanged?.Invoke(state);
    }
}
