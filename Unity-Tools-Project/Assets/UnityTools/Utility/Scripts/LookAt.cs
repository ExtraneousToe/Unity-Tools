using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTools
{
    public class LookAt : BaseBehaviour
    {
        #region Variables
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private bool _returnToInitial = false;

        private Vector3 _initialLocalEuler;
        #endregion

        #region Mono
        protected override void Start()
        {
            base.Start();

            // store the inital angle
            _initialLocalEuler = transform.localEulerAngles;
        }

        protected void Update()
        {
            if (_target)
            {
                transform.LookAt(_target);
            }
            else if (_returnToInitial)
            {
                transform.localEulerAngles = _initialLocalEuler;
            }
        }
        #endregion
    }
}