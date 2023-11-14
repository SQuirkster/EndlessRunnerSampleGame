using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.Serialization;

namespace Netflix
{
    internal class ResponseManager
    {
        private static List<ResponseDispatcher> responseQueue = new List<ResponseDispatcher>();
        public static void DispatchResponseOnUiThread()
        {
            while (responseQueue.Count > 0)
            {
                ResponseDispatcher responseDispatcher = responseQueue[0];
                responseQueue.RemoveAt(0);
                responseDispatcher.DispatchResponse();
            }
        }


        public static void AddNewResponseFromAnyThread(ResponseDispatcher responseDispatcher)
        {
            responseQueue.Add(responseDispatcher);
        }
    }
 }