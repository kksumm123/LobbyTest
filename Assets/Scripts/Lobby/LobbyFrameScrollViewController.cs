using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LobbyFrameScrollViewController : MonoBehaviour, IEndDragHandler
{
    private bool _isInitialized = false;

    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Scrollbar horizontalScrollBar;
    [SerializeField] private float lerpValue = 0.15f;

    private int _orderCount;
    private float _orderValue;
    private int _orderIndex;
    private List<float> _orderValues = new List<float>();
    private List<float> _compensationValues = new List<float>();

    private float _targetOrderValue;

    private bool _applyScrollMove = false;
    private Coroutine _moveCoHandle;

    public void Initialize()
    {
        InitializeVariables();

        _moveCoHandle = this.StopAndStartCo(
            _moveCoHandle, ScrollBarMoveCo());

        _isInitialized = true;
    }

    private void InitializeVariables()
    {
        if (scrollRect.content.childCount < 2)
        {
            Debug.LogError("로비 UI의 프레임 개수가 2개 미만이면 제대로 동작할 수 없습니다"); return;
        }
        _orderCount = scrollRect.content.childCount - 1;
        _orderValue = 1f / _orderCount;

        _orderValues.Clear();
        for (int i = 0; i < scrollRect.content.childCount; i++)
        {
            _orderValues.Add(i * _orderValue);
        }

        _compensationValues.Clear();
        for (int i = 0; i < _orderCount; i++)
        {
            _compensationValues.Add(
                (i * 2 + 1)
                * _orderValue * 0.5f);
        }
    }

    private IEnumerator ScrollBarMoveCo()
    {
        var isTrue = true;
        while (isTrue)
        {
            if (!_isInitialized || !_applyScrollMove)
            {
                yield return null;
                continue;
            }

            horizontalScrollBar.value = Mathf.Lerp(
                horizontalScrollBar.value,
                _targetOrderValue,
                lerpValue);

            if (Mathf.Approximately(horizontalScrollBar.value, _targetOrderValue))
            {
                _applyScrollMove = false;
            }
            yield return null;
        }
    }

    private void SetTargetOrderValue(float value)
    {
        _applyScrollMove = true;
        _targetOrderValue = value;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isInitialized) return;

        if (IsValueFirstUIFrame())
        {
            SetTargetOrderValue(value: 0);
        }
        else if (IsValueLastUIFrame())
        {
            SetTargetOrderValue(value: 1);
        }
        else
        {
            SetTargetOrderValue(_orderValues[GetOrderIndex()]);
        }
    }

    private bool IsValueFirstUIFrame()
    {
        return horizontalScrollBar.value < _compensationValues[0];
    }

    private bool IsValueLastUIFrame()
    {
        return horizontalScrollBar.value > _compensationValues.GetLastItem();
    }

    private int GetOrderIndex()
    {
        for (_orderIndex = 1; _orderIndex < _compensationValues.Count; _orderIndex++)
        {
            if (horizontalScrollBar.value < _compensationValues[_orderIndex])
            {
                break;
            }
        }
        return _orderIndex;
    }
}
