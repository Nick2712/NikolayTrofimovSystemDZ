using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace NikolayTrofimovDZ4
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class Character : NetworkBehaviour
    {
        protected Action OnUpdateAction { get; set; }
        protected abstract FireAction _fireAction { get; set; }

        [SyncVar] protected Vector3 _serverPosition;
        [SyncVar] protected Quaternion _serverRotation;

        protected virtual void Initiate()
        {
            OnUpdateAction += Movement;
        }

        private void Update()
        {
            OnUpdateAction?.Invoke();
        }

        [Command]
        protected void CmdUpdatePosition(Vector3 position, Quaternion rotation)
        {
            _serverPosition = position;
            _serverRotation = rotation;
        }

        public abstract void Movement();

    }
}