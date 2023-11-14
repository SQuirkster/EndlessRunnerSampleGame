using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Serialization;

namespace Netflix
{
    internal interface IResponseDispatcher
    {
        void DispatchResponse();
    }

    internal class ResponseDispatcher : IResponseDispatcher
    {
        private Action _action;

        public ResponseDispatcher(Action action)
        {
            _action = action;
        }

        public void DispatchResponse()
        {
            _action?.Invoke();
        }
    }
}