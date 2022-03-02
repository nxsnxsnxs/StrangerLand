using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tools;

namespace Components
{
    public class Projectile : GameComponent
    {
        public Action<Combat.Config> onHitTarget;
        public float flySpeed;
        public float maxFlyDistance;
        private GameObject target;
        private Vector3 spawnPos;
        private bool settarget;
        private Combat.Config conf;
        public void SetTarget(Combat.Config _conf, GameObject _target)
        {
            target = _target;
            spawnPos = transform.position;
            conf = _conf;
            flySpeed = _conf.flySpeed;
            maxFlyDistance = _conf.maxFlyDistance;
            settarget = true;
        }

        void FixedUpdate()
        {
            if(!settarget) return;
            transform.position += transform.forward * Time.deltaTime * flySpeed;
            if(transform.position.PlanerDistance(spawnPos) >= maxFlyDistance) Finish();
        }

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject != target) return;
            onHitTarget?.Invoke(conf);
            Finish();
        }
        void Finish()
        {
            Destroy(gameObject);
        }
    }
}