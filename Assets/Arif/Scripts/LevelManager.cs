﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arif.Scripts
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Manager;

        public enum LevelStates
        {
            Prepare,
            MainGame,
            War,
            Finish
        }

        public LevelStates currentLevelState;

        public PlayerController playerController;
        
        [HideInInspector]public List<CollectableImage> collectedImageList = new List<CollectableImage>();

        public RectTransform bagContentTransform;
        public CollectableImage collectableImagePrefab;

        public int maxItemCount=10;
        public Canvas mainCanvas;
        
        private void Awake()
        {
            Manager = this;
        }

        private void Start()
        {
            currentLevelState = LevelStates.MainGame;
        }

        private void Update()
        {
            switch (currentLevelState)
            {
                case LevelStates.Prepare:
                    break;
                case LevelStates.MainGame:
                    MovePlayer();
                    SelectObject();
                    break;
                case LevelStates.Finish:
                    break;
                case LevelStates.War:
                    
                   SelectAtWar();
                    
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SelectAtWar()
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                var ray = GameManager.Manager.overlayCam.ScreenPointToRay(Input.mousePosition);
               
                if (Physics.Raycast(ray,out hit))
                {
                   
                    var collectableObject = hit.collider.GetComponent<CollectableObject>();
                    if (collectableObject)
                    {
                        CollectObject(collectableObject);
                    }
                }
            }
        }

        public void MovePlayer()
        {
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;

                var ray = GameManager.Manager.mainCam.ScreenPointToRay(Input.mousePosition);
                
                if (Physics.Raycast(ray,out hit))
                {
                    var ground = hit.collider.GetComponent<Ground>();
                    if (ground)
                    {
                        playerController.playerAgent.SetDestination(hit.point);
                    }
                }
            }
        }
        

        public void SelectObject()
        {
            if (Input.GetMouseButtonDown(0))
            {

                RaycastHit hit;
                var ray = GameManager.Manager.mainCam.ScreenPointToRay(Input.mousePosition);
               
                if (Physics.Raycast(ray,out hit))
                {
                   
                    var collectableObject = hit.collider.GetComponent<CollectableObject>();
                    if (collectableObject)
                    {
                        if (collectableObject.canCollect)
                        {
                            CollectObject(collectableObject);
                            return;
                        }
                    }

                    var interactiveObject = hit.collider.GetComponent<InteractiveObjects>();
                    if (interactiveObject)
                    {
                        if (interactiveObject.canInteract)
                        {
                            interactiveObject.OnInteract();
                        }
                    }
                }
            }
        }
        
        
       
        public void CollectObject(CollectableObject collectableObject)
        {

            if (collectedImageList.Count>=maxItemCount)
            {
                
                Debug.Log("Doldu");
                return;
            }
            var cloneObject = Instantiate(collectableImagePrefab,bagContentTransform);

            cloneObject.myImage.sprite = collectableObject.mySprite;
            collectedImageList.Add(cloneObject);
            cloneObject.myObject = Instantiate(collectableObject);
            cloneObject.myObject.gameObject.SetActive(false);
            Destroy(collectableObject.gameObject);
        }
        
    }
}
