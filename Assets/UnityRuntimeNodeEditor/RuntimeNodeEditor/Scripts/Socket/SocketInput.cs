using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RuntimeNodeEditor
{
    public class SocketInput : Socket, IPointerClickHandler
    {
        [SerializeField] private Image socketImage;
        [SerializeField] private Color disconnectedColor;
        [SerializeField] private Color connectedColor;
        public List<Connection> Connections { get; private set; }

        public override void Setup()
        {
            Connections = new List<Connection>();
            if(socketImage)
                socketImage.color = disconnectedColor;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Events.InvokeInputSocketClick(this, eventData);
        }

        public void Connect(Connection conn)
        {
            Connections.Add(conn);
            if (socketImage)
                socketImage.color = connectedColor;
        }

        public void Disconnect(Connection conn)
        {
            Connections.Remove(conn);
            if (socketImage)
                socketImage.color = disconnectedColor;
        }

        public override bool HasConnection()
        {
            return Connections.Count > 0;
        }
    }
}