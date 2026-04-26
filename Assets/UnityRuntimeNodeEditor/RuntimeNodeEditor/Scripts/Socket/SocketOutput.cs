using System;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

namespace RuntimeNodeEditor
{
    public class SocketOutput : Socket, IOutput, IPointerClickHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image socketImage;
        [SerializeField] private Color disconnectedColor;
        [SerializeField] private Color connectedColor;

        public  Connection  connection;
        private object      _value;

        public override void Setup()
        {
            if (socketImage)
                socketImage.color = disconnectedColor;
        }

        public void SetValue(object value)
        {
            if (_value != value)
            {
                _value = value;
                ValueUpdated?.Invoke();
            }
        }

        public event Action ValueUpdated;

        public T GetValue<T>()
        {
            return (T)_value;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Events.InvokeOutputSocketClick(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Events.InvokeSocketDragFrom(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            foreach (var item in eventData.hovered)
            {
                if (item.TryGetComponent<SocketInput>(out var input))
                {
                    Events.InvokeOutputSocketDragDropTo(input);
                    return;
                }
            }

            Events.InvokeOutputSocketDragDropTo(null);
        }
    
        public void Connect(Connection conn)
        {
            connection = conn;
            if (socketImage)
                socketImage.color = connectedColor;
        }

        public void Disconnect()
        {
            connection = null;
            if (socketImage)
                socketImage.color = disconnectedColor;
        }

        public override bool HasConnection()
        {
            return connection != null;
        }
    }
}