using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Cinemachine;

[DefaultExecutionOrder(-2)]
public class Railroad : MonoBehaviour
{
    [SerializeField] private List<CinemachineSmoothPath> m_cmSmoothPaths = new();

    private List<RailroadSocket> m_mySockets = new();

    private void Awake()
    {
        createPathSockets();
    }

    private void createPathSockets()
    {
        for (int i = 0; i < m_cmSmoothPaths.Count; ++i)
        {
            var _path = m_cmSmoothPaths[i];

            var _startSocket = new RailroadSocket();
            _startSocket.WorldPosition = _path.transform.TransformPoint(_path.m_Waypoints[0].position);
            _startSocket.RailroadComponent = this;
            _startSocket.Path = _path;
            m_mySockets.Add(_startSocket);
            ObjectCollection<RailroadSocket>.RegisterObject(_startSocket);

            var _endSocket = new RailroadSocket();
            _endSocket.WorldPosition = _path.transform.TransformPoint(_path.m_Waypoints[_path.m_Waypoints.Length - 1].position);
            _endSocket.RailroadComponent = this;
            _endSocket.Path = _path;
            m_mySockets.Add(_endSocket);
            ObjectCollection<RailroadSocket>.RegisterObject(_endSocket);
        }
    }

    private void Start()
    {
        connectPathSockets();
    }

    private void connectPathSockets()
    {
        var _allSockets = ObjectCollection<RailroadSocket>.AllObjects;

        for (int i = 0; i < m_mySockets.Count; i++)
        {
            var _socket = m_mySockets[i];

            for (int ii = 0; ii < _allSockets.Count; ii++)
            {
                var _otherSocket = _allSockets[ii];

                if (m_mySockets.Contains(_otherSocket))
                    continue;

                float _distance = Vector3.Distance(_socket.WorldPosition, _otherSocket.WorldPosition);

                if (_distance > 0.1f)
                    continue;

                if (Application.isEditor)
                    Debug.DrawLine(_socket.RailroadComponent.transform.position, _otherSocket.RailroadComponent.transform.position, Color.green, 15.0f);

                if (_socket.ConnectedSockets.Contains(_otherSocket) == false)
                    _socket.ConnectedSockets.Add(_otherSocket);

                if (_otherSocket.ConnectedSockets.Contains(_socket) == false)
                    _otherSocket.ConnectedSockets.Add(_socket);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < m_mySockets.Count; i++)
        {
            ObjectCollection<RailroadSocket>.UnregisterObject(m_mySockets[i]);
        }

        m_mySockets.Clear();
    }

    public CinemachineSmoothPath GetOpenPath()
    {
        for (int i = 0; i < m_cmSmoothPaths.Count; i++)
        {
            var _path = m_cmSmoothPaths[i];

            if (_path.enabled)
                return _path;
        }

        return null;
    }

    public RailroadSocket GetClosestSocket(Vector3 point)
    {
        float _closestDistance = float.MaxValue;
        RailroadSocket _result = null;

        for (int i = 0; i < m_mySockets.Count; i++)
        {
            var _socket = m_mySockets[i];

            var _dist = Vector3.Distance(_socket.WorldPosition, point);

            if (_dist < _closestDistance)
            {
                _closestDistance = _dist;
                _result = _socket;
            }
        }

        return _result;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.TryGetComponent(out Minecart _minecart))
            _minecart.OnCollidedWithRailroad(this);
    }

    public class RailroadSocket
    {
        public Vector3 WorldPosition;
        public Railroad RailroadComponent = null;
        public CinemachineSmoothPath Path = null;
        public List<RailroadSocket> ConnectedSockets = new();
    }
}