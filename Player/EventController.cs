using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Events;
using Actions;
using Events.Player;

namespace Player
{
    using PlayerEvent = PlayerEvents.PlayerEvent;
    public class EventController : MonoBehaviour
    {
        private EventHandler eventHandler;
        private ActionController actionController;
        private PlayerEvent currentEvent;

        void Awake()
        {
            eventHandler = GetComponent<EventHandler>();
            actionController = GetComponent<ActionController>();
        }
        void Start()
        {
            AddNormalEvents();
            actionController.onActionFinish += ResetEvent;
        }
        void AddNormalEvents()
        {
            AddEvent<PlayerEvents.Move>();
            AddEvent<PlayerEvents.AutoCraft>();
            AddEvent<PlayerEvents.Work>();
            AddEvent<PlayerEvents.Pick>();
            AddEvent<PlayerEvents.AutoAttack>();
            AddEvent<PlayerEvents.Attack>();
            AddEvent<PlayerEvents.GetHit>();
            AddEvent<PlayerEvents.Drop>();
            AddEvent<PlayerEvents.UnEquip>();
        }
        void AddEvent<T>() where T : PlayerEvent, new()
        {
            T e = new T();
            eventHandler.ListenEvent(e.name, (object[] args) => OnEvent(e, args));
        }
        void OnEvent(PlayerEvent e, params object[] args)
        {
            if(currentEvent != null && e.Level < currentEvent.Level) return;
            e.fn(gameObject, args);
            currentEvent = e;
        }
        public void ResetEvent(bool finish)
        {
            if(finish) currentEvent = null;
        }
    }
}