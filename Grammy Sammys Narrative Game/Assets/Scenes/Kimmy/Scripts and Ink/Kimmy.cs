﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Ink.Runtime;
using TMPro;

public class Kimmy : MonoBehaviour { 
    [SerializeField] private TextAsset _inkJsonAsset;
    [SerializeField] private Story _story;
    
    //text
    public TMP_Text textPrefab;
    public TMP_FontAsset dialogueFont;
    public Button buttonPrefab;
   
    //choice and dialogue panels will determine where the dialogue and buttons are intantiated 
    public GameObject choicePanel;
    public GameObject dialoguePanel;
    
    //sounds 
    public AudioSource audioSource;
    public AudioClip mapSound; 
    public AudioClip click; 
    public AudioClip storeBell;
    public AudioClip boughtSound;
    public AudioClip store;
    public AudioClip background;
    public AudioClip playground;
    
    //locations/scene indicators 
    private bool day1;
    private bool atPlayground;
    private bool atStore;
    private bool atHome;
    private bool openMap;
    //backgrounds
    public GameObject storeBackground;
    public GameObject playgroundBackground;
    public GameObject mapBackground;
    public GameObject houseBackground;
    
    //characters
    public GameObject momSprite;
    public GameObject danaSprite;
    public GameObject kimmySprite;
    public GameObject kimmyMomSprite;
    public GameObject deanSprite;
    public GameObject lindaSprite;
    public GameObject blytheSprite;
    public GameObject janeySprite;
    //speaking indicators 
    private bool withLinda;
    private bool withBlythe;
    private bool withJaney;
    private bool withDean;

    //start locations
    private Vector3 danaStartLocation;
    private Vector3 momStartLocation;
    private Vector3 kimmyMomStartLocation;
    private Vector3 kimmyStartLocation;
    
    

    private GameObject[] characters;

    private bool choiceClicked; 

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = background;

        audioSource.Play();
        _story = new Story(_inkJsonAsset.text);
        RemoveChildren();

        danaStartLocation = danaSprite.transform.position;
        momStartLocation = momSprite.transform.position;
        kimmyStartLocation = kimmySprite.transform.position;
        kimmyMomStartLocation = kimmyMomSprite.transform.position;

        if (_story.canContinue)
        {
            string text = _story.Continue();
            text = text.Trim();
            CreateContentView(text);
        }

        characters = new GameObject[]
            {momSprite, danaSprite, kimmySprite, kimmyMomSprite, deanSprite, lindaSprite, blytheSprite, janeySprite};

        choiceClicked = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SceneManager.LoadScene("Main Menu");
        }
        if ((Input.GetMouseButtonDown(0) || choiceClicked) && _story.canContinue)
        {
            audioSource.PlayOneShot(click, 0.1f);
            RemoveChildren();
            string text = _story.Continue();
            text = text.Trim();
            CreateContentView(text);
            if (_story.currentChoices.Count > 0)
            {
                foreach (Choice choice in _story.currentChoices)
                {
                    Button button = CreateChoiceView(choice.text.Trim());
                    Choice thisChoice = choice;
                    button.onClick.AddListener(delegate { OnClickChoiceButton(thisChoice); });
                }
            }

            choiceClicked = false;
        }

        if (openMap)
        {
            setMap();
        }
    }
    
    void RemoveChildren () {
        int dialogueChildCount = dialoguePanel.transform.childCount;
        for (int i = dialogueChildCount - 1; i >= 0; --i) {
            GameObject.Destroy (dialoguePanel.transform.GetChild (i).gameObject);
        }

        int choiceChildCount = choicePanel.transform.childCount;
        for (int i = choiceChildCount - 1; i >= 0; --i)
        {
            GameObject.Destroy(choicePanel.transform.GetChild(i).gameObject);
        }
    }
    
    Button CreateChoiceView (string text) {
        // Creates the button from a prefab
        Button choice = Instantiate(buttonPrefab) as Button;
        choice.transform.SetParent(choicePanel.transform, false);
		
        // Gets the text from the button prefab
        TMP_Text choiceText = choice.GetComponentInChildren<TMP_Text> ();
        choiceText.text = text;
        choiceText.font = dialogueFont;
        
        HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
        layoutGroup.childForceExpandHeight = false;
        
        return choice;
    }
    
    void OnClickChoiceButton(Choice choice)
    {
        audioSource.PlayOneShot(click, 0.1f);
        if (choice.text.Contains("Store"))
        {
            audioSource.PlayOneShot(storeBell, 0.6f);
            audioSource.clip = store;
            audioSource.Play();

            setStore();
        }
        
        if (choice.text.Contains("Map"))
        {
            setMap();
        }

        if (choice.text.Contains("Playground"))
        {
            setPlayground();
            audioSource.clip = playground;
            audioSource.Play();
        }

        if (choice.text.Contains("Head to Kimmy's house"))
        {
            intro();
        }

        if (choice.text.Contains("Linda"))
        {
            setLinda();
        }

        if (choice.text.Contains("Janey"))
        {
            setJaney();
        }

        if (choice.text.Contains("Blythe"))
        {
            setBlythe();
        }

        if (choice.text.Contains("chalk - 5"))
        {
            audioSource.PlayOneShot(boughtSound, 0.6f);
        }

        if (choice.text.Contains("Day 1"))
        {
            Debug.Log("moving into day 1");
            intro2();
        }

        if (choice.text.Contains("Quit"))
        {
            SceneManager.LoadScene("Main Menu");
        }

        choiceClicked = true;
        _story.ChooseChoiceIndex(choice.index);
        RemoveChildren();
    }
    
    void CreateContentView (string text) {
        TMP_Text storyText = Instantiate (textPrefab) as TMP_Text;
        storyText.text = text;
        storyText.font = dialogueFont;
       
        if (text.Contains("Dana:"))
        {
            danaSprite.transform.position = danaStartLocation + Vector3.up;
            kimmySprite.transform.position = kimmyStartLocation;
            momSprite.transform.position = momStartLocation;
            kimmyMomSprite.transform.position = kimmyMomStartLocation;
        }
        else if (text.Contains("Kimmy's Mom:"))
        {
            Debug.Log("Kimmy's mom is speaking");
            kimmyMomSprite.transform.position = kimmyMomStartLocation + Vector3.up;
            danaSprite.transform.position = danaStartLocation;
            kimmySprite.transform.position = kimmyStartLocation;
            momSprite.transform.position = momStartLocation;
        }
        else if (text.Contains("Mom:"))
        {
            momSprite.transform.position = momStartLocation + Vector3.up;
            danaSprite.transform.position = danaStartLocation;
            kimmySprite.transform.position = kimmyStartLocation;
            kimmyMomSprite.transform.position = kimmyMomStartLocation;
        }
        else if (text.Contains("Kimmy:"))
        {
            kimmySprite.transform.position = kimmyStartLocation + Vector3.up;
            danaSprite.transform.position = danaStartLocation;
            momSprite.transform.position = momStartLocation;
            kimmyMomSprite.transform.position = kimmyMomStartLocation;
        }
        else
        {
            resetAll();
        }

        storyText.transform.SetParent (dialoguePanel.transform, false);
    }

    void intro()
    {
        momSprite.SetActive(true);
        kimmyMomSprite.SetActive(true); 
    }

    void intro2()
    {
        momSprite.SetActive(false);
        kimmyMomSprite.SetActive(false);
    }

    void setBlythe()
    {
        Debug.Log("We're talking to Blythe");
        blytheSprite.SetActive(true);
        
        janeySprite.SetActive(false);
        lindaSprite.SetActive(false);
        setPlayground();
    }

    void setLinda()
    {
        Debug.Log("We're talking to Linda");
        lindaSprite.SetActive(true);
        
        Debug.Log("Linda status: " + lindaSprite.activeSelf);

        janeySprite.SetActive(false);
        blytheSprite.SetActive(false);
        setPlayground();
    }

    void setJaney()
    {
        Debug.Log("We're talking to Janey");
        janeySprite.SetActive(true);

        lindaSprite.SetActive(false);
        blytheSprite.SetActive(false);
        setPlayground();
    }

    void resetAll()
    {
        danaSprite.transform.position = danaStartLocation;
        momSprite.transform.position = momStartLocation;
        kimmyMomSprite.transform.position = kimmyMomStartLocation;
        kimmySprite.transform.position = kimmyStartLocation;
    }

    void setPlayground()
    {
        playgroundBackground.SetActive(true);
        storeBackground.SetActive(false);
        mapBackground.SetActive(false);
        houseBackground.SetActive(false);
        
        momSprite.SetActive(false);
        kimmyMomSprite.SetActive(false);
        deanSprite.SetActive(false);
    }

    void setStore()
    {
        storeBackground.SetActive(true);
        playgroundBackground.SetActive(false);
        mapBackground.SetActive(false);
        houseBackground.SetActive(false);
        
        deanSprite.SetActive(true);

        momSprite.SetActive(false);
        kimmyMomSprite.SetActive(false);
        lindaSprite.SetActive(false);
        blytheSprite.SetActive(false);
        janeySprite.SetActive(false);
    }

    void setMap()
    {
        mapBackground.SetActive(true);
        storeBackground.SetActive(false);
        playgroundBackground.SetActive(false);
        houseBackground.SetActive(false);
        
        audioSource.PlayOneShot(mapSound, 1.2f);
        
        deanSprite.SetActive(false);
        momSprite.SetActive(false);
        kimmyMomSprite.SetActive(false);
        lindaSprite.SetActive(false);
        blytheSprite.SetActive(false);
        janeySprite.SetActive(false);
    }
}