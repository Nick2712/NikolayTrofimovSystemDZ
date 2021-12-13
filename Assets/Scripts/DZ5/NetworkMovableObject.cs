using System;
using UnityEngine;
using UnityEngine.Networking;


namespace NikolayTrofimovDZ5
{
#pragma warning disable 618
    public abstract class NetworkMovableObject : NetworkBehaviour
#pragma warning restore 618
    {
        protected abstract float Speed { get; }
        protected Action OnUpdateAction { get; set; }
        protected Action OnFixedUpdateAction { get; set; }
        protected Action OnLateUpdateAction { get; set; }
        protected Action OnPreRenderAction { get; set; }
        protected Action OnPostRenderAction { get; set; }

#pragma warning disable 618
        [SyncVar] protected Vector3 _serverPosition;
        [SyncVar] protected Vector3 _serverEuler;
#pragma warning restore 618

        public override void OnStartAuthority()
        {
            Initiate();
        }

        protected virtual void Initiate(UpdatePhase updatePhase = UpdatePhase.Update)
        {
            switch(updatePhase)
            {
                case UpdatePhase.Update:
                    OnUpdateAction += Movement;
                    break;
                case UpdatePhase.FixedUpdate:
                    OnFixedUpdateAction += Movement;
                    break;
                case UpdatePhase.LateUpdate:
                    OnLateUpdateAction += Movement;
                    break;
                case UpdatePhase.PostRender:
                    OnPostRenderAction += Movement;
                    break;
                case UpdatePhase.PreRender:
                    OnPreRenderAction += Movement;
                    break;
            }
        }

        private void Update()
        {
            OnUpdateAction?.Invoke();
        }

        private void FixedUpdate()
        {
            OnFixedUpdateAction?.Invoke();
        }

        private void LateUpdate()
        {
            OnLateUpdateAction?.Invoke();
        }

        private void OnPreRender()
        {
            OnPreRenderAction?.Invoke();
        }

        private void OnPostRender()
        {
            OnPostRenderAction?.Invoke();
        }

        protected virtual void Movement()
        {
            if(hasAuthority)
            {
                HasAutorityMovement();
            }
            else
            {
                FromServerUpdate();
            }
        }

        protected abstract void HasAutorityMovement();

        protected abstract void FromServerUpdate();

        protected abstract void SendToServer();
    }
}