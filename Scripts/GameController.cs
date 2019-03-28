using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.ComponentModel;



public class GameController : MonoBehaviour
{
    //Player & main menu variables
    public GameObject mainMenuPanel;
    public List<Player> playerList = new List<Player>();

    const int BASIC_HEALTH = 30;
    const int BASIC_STRENGTH = 3;
    const int BASIC_HEAL = 3;

    private int playerIndex = 0;
    private int numPlayers;
    private const int MAX_PLAYERS = 5;

    private string mainMenuPlayerDisplay;
    public Text mainMenuPlayersText;
    public InputField mainMenuPlayerInputField;
    public Button addPlayerButton;

    //Player panel variables
    public GameObject playerPanel;
    public Text playerNameText;
    public Text playerHealthText;
    public Text playerStrengthText;
    public Text playerItem1Text;
    public Text playerItem2Text;

    public GameObject attackMenu;
    public List<Button> attackButtons = new List<Button>();
    public List<GameObject> attackButtonTexts = new List<GameObject>();

    public Button continueButton;


    //Basic event panel variables
    public GameObject eventPanel;
    public Text eventPanelText;

    //Choice event panel variables
    public GameObject choiceEventPanel;
    public Button choiceEventButton1;
    public Button choiceEventButton2;
    public Text choiceEventButton1Text;
    public Text choiceEventButton2Text;
    private int cursedTomeDamage;
    private int cursedTomeChance;

    public Text choiceEventText;
    private ChoiceEventType choiceEventType;

    private void Start()
    {
        playerPanel.SetActive(false);
        eventPanel.SetActive(false);
        choiceEventPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            OnAddButtonClick();
        }
    }

    public void OnAddButtonClick()
    {
        //Add players to list
        playerList.Add(ScriptableObject.CreateInstance<Player>());

        playerList[playerIndex].InitializePlayer(mainMenuPlayerInputField.textComponent.text, BASIC_HEALTH, BASIC_STRENGTH);
        playerIndex++;


        //Display players
        mainMenuPlayerDisplay = "";

        foreach (Player p in playerList)
        {
            mainMenuPlayerDisplay += p.name + "\r\n";
        }

        mainMenuPlayersText.text = mainMenuPlayerDisplay;
        mainMenuPlayerInputField.textComponent.text = "";

        //Limit to max players
        if (playerIndex == MAX_PLAYERS)
        {
            addPlayerButton.interactable = false;
        }

        //Select text in input field
        mainMenuPlayerInputField.Select();
        mainMenuPlayerInputField.ActivateInputField();
    }

    public void OnStartButtonClick()
    {
        //Set max players and display the first player's panel
        numPlayers = playerIndex;
        playerIndex = 0;
        SetupPlayerPanel(playerIndex);

        mainMenuPanel.SetActive(false);
        playerPanel.SetActive(true);

        //Initialize attack menu buttons
        for (int i = 0; i < playerList.Count; i++)
        {
            attackButtonTexts[i].GetComponent<Text>().text = playerList[i].name;
        }
    }

    public void OnAttackButtonClick()
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            attackButtons[i].gameObject.SetActive(true);
            attackButtons[playerIndex].gameObject.SetActive(false);
        }
        attackMenu.SetActive(true);
        continueButton.gameObject.SetActive(false);
    }

    //needs improvement
    public void OnPlayerAttackButtonClick()
    {

        for (int i = 0; i < playerList.Count; i++)
        {
            if (EventSystem.current.currentSelectedGameObject.name.Equals("P" + (i + 1).ToString() + "AttackButton"))
            {
                SetupEventPanel(playerList[playerIndex].name + " attacked " + playerList[i].name + " for " + playerList[playerIndex].strength + " damage!");
                TakeDamage(i);
            }
        }
        attackMenu.SetActive(false);
        continueButton.gameObject.SetActive(true);
    }

    //Change back to rand
    public void OnExploreButtonClick()
    {
        int randNum = Random.Range(0, 5);

        switch (5)
        {
            case 0:
                FindItem(Item.DingusRing);
                break;
            case 1:
                FindItem(Item.PickOfDestiny);
                break;
            case 2:
                FindItem(Item.SatchelOfSquanch);
                break;
            case 3:
                choiceEventType = ChoiceEventType.BasicChoice;
                SetupChoiceEventPanel(choiceEventType);
                break;
            case 4:
                choiceEventType = ChoiceEventType.Boss;
                SetupChoiceEventPanel(choiceEventType);
                break;
            case 5:
                choiceEventType = ChoiceEventType.CursedTome;
                SetupChoiceEventPanel(choiceEventType);
                break;
            default:
                SetupEventPanel("default case.  something fucked up");
                break;
        }
        Debug.Log(randNum);
    }

    public void OnResolveButtonClick()
    {

        eventPanel.SetActive(false);
        playerPanel.SetActive(true);
        SetupPlayerPanel(playerIndex);
    }

    public void OnRestButtonClick()
    {
        SetupEventPanel("You rest: gain 3 hp");
        playerPanel.SetActive(false);
        eventPanel.SetActive(true);
        playerList[playerIndex].health += BASIC_HEAL;
    }

    public void OnItem1ButtonClick()
    {
        UseItem(playerList[playerIndex].item1);
        playerList[playerIndex].RemoveItem(1);
    }

    public void OnItem2ButtonClick()
    {
        UseItem(playerList[playerIndex].item2);
        playerList[playerIndex].RemoveItem(2);
    }

    public void SetupEventPanel(string customText)
    {
        eventPanelText.text = customText;
        playerPanel.SetActive(false);
        choiceEventPanel.SetActive(false);
        eventPanel.SetActive(true);
    }

    public void SetupPlayerPanel(int index)
    {
        playerNameText.text = playerList[index].name;
        playerHealthText.text = "Health: " + playerList[index].health.ToString();
        playerStrengthText.text = "Strength: " + playerList[index].strength.ToString();

        //Display item1 text
        switch (playerList[index].item1)
        {
            case Item.Empty:
                playerItem1Text.text = "";
                break;
            case Item.PickOfDestiny:
                playerItem1Text.text = "Pick of Destiny";
                break;
            case Item.DingusRing:
                playerItem1Text.text = "Dingus's Ring";
                break;
            case Item.SatchelOfSquanch:
                playerItem1Text.text = "Satchel of Squanch";
                break;
            case Item.CursedTome:
                playerItem1Text.text = "Cursed Tome";
                break;
            default:
                playerItem1Text.text = "something went wrong...";
                break;
        }

        //Display item2 text
        switch (playerList[index].item2)
        {
            case Item.Empty:
                playerItem2Text.text = "";
                break;
            case Item.PickOfDestiny:
                playerItem2Text.text = "Pick of Destiny";
                break;
            case Item.DingusRing:
                playerItem2Text.text = "Dingus's Ring";
                break;
            case Item.SatchelOfSquanch:
                playerItem2Text.text = "Satchel of Squanch";
                break;
            case Item.CursedTome:
                playerItem2Text.text = "Cursed Tome";
                break;
            default:
                playerItem2Text.text = "something went wrong...";
                break;
        }
    }

    public Player GetActivePlayer()
    {
        return playerList[playerIndex];
    }

    public int NextPlayer()
    {
        if (playerIndex < numPlayers - 1)
        {
            playerIndex++;
        }
        else
        {
            playerIndex = 0;
        }

        return playerIndex;
    }

    public void SetupNextPlayerPanel()
    {
        attackMenu.SetActive(false);
        eventPanel.SetActive(false);
        mainMenuPanel.SetActive(false);

        playerPanel.SetActive(true);
        SetupPlayerPanel(NextPlayer());
    }

    public void TakeDamage(int index)
    {
        playerList[index].health -= playerList[playerIndex].CalculateDamage();
    }

    public void UseItem(Item item)
    {
        switch (item)
        {
            //Empty Item
            case Item.Empty:
                Debug.Log("Empty item");
                break;
            //Pick of Destiny
            case Item.PickOfDestiny:
                playerList[playerIndex].strength++;
                SetupEventPanel("You gained 1 strengh");
                break;
            //Dingus's Ring
            case Item.DingusRing:
                playerList[playerIndex].health += BASIC_HEAL;
                SetupEventPanel("You gained 3 hp!");
                break;
            //Satchel of Squanch
            case Item.SatchelOfSquanch:
                Item randItem = (Item)(Random.Range(0, 3) + 1);

                if (playerList[playerIndex].item2 == Item.Empty)
                {
                    playerList[playerIndex].item2 = randItem;
                }
                else
                {
                    playerList[playerIndex].itemTemp = randItem;
                }
                SetupEventPanel("You found " + GetItemName(randItem) + "!");
                break;
            case Item.CursedTome:
                GetActivePlayer().TakeDamage(5);
                GetActivePlayer().strength += 3;
                SetupEventPanel("You gained 3 str, but took 5 damage!");
                break;
            default:
                break;
        }
    }

    public string GetItemName(Item item)
    {
        switch (item)
        {
            case Item.Empty:
                return "Empty";
            case Item.DingusRing:
                return "Dingus's Ring";
            case Item.PickOfDestiny:
                return "the Pick of Destiny";
            case Item.SatchelOfSquanch:
                return "the Satchel of Squanch";
            case Item.CursedTome:
                return "the Cursed Tome";
            default:
                return "default";
        }
    }

    public void FindItem(Item foundItem)
    {
        string eventText = "";
        string inventoryFullMsg = "Inventory full";

        if (!playerList[playerIndex].inventoryFull())
        {
            playerList[playerIndex].AddItem(foundItem);
            eventText = "You found " + GetItemName(foundItem) + "!";
        }
        else
        {
            eventText = inventoryFullMsg;
        }

        SetupEventPanel(eventText);
    }

    public void SetupChoiceEventPanel(ChoiceEventType evtType)
    {
        switch (evtType)
        {
            case ChoiceEventType.BasicChoice:
                choiceEventButton1Text.text = "Basic choice 1";
                choiceEventButton2Text.text = "Basic choice 2";
                choiceEventText.text = "Basic choice event.  What do you choose?";
                break;
            case ChoiceEventType.Boss:
                choiceEventButton1Text.text = "Attack the boss!";
                choiceEventButton2Text.text = "Run away!";
                choiceEventText.text = "You run into the boss!  What do you do?";
                break;
            case ChoiceEventType.CursedTome:
                cursedTomeDamage = 1;
                cursedTomeChance = 0;
                choiceEventButton1Text.text = "Read";
                choiceEventButton2Text.text = "Run away!";
                choiceEventText.text = "You find the cursed tome!  Will you read it?";
                break;
            default:
                SetupEventPanel("Default case.  Something fucked up");
                break;
        }
        playerPanel.SetActive(false);
        eventPanel.SetActive(false);
        choiceEventPanel.SetActive(true);
    }

    public void OnEventChoice1Click()
    {
        switch (choiceEventType)
        {
            case ChoiceEventType.BasicChoice:
                SetupEventPanel("basic choice 1!");
                break;
            case ChoiceEventType.Boss:
                SetupEventPanel("Your attack does nothing...");
                break;
            case ChoiceEventType.CursedTome:
                int randNum = Random.Range(0, 100) + 1;

                if (randNum < cursedTomeChance)
                {
                    FindItem(Item.CursedTome);
                }
                else
                {
                    playerList[playerIndex].TakeDamage(cursedTomeDamage);
                    choiceEventText.text = "You took " + cursedTomeDamage + " damage!\n HP: " + GetActivePlayer().health;
                    choiceEventButton1Text.text = "Read more...";
                    cursedTomeDamage++;
                    cursedTomeChance += 7;
                }
                break;
        }
    }

    public void OnEventChoice2Click()
    {
        switch (choiceEventType)
        {
            case ChoiceEventType.BasicChoice:
                SetupEventPanel("basic choice 2!");
                break;
            case ChoiceEventType.Boss:
                SetupEventPanel("Pussy ass hoe");
                break;
            case ChoiceEventType.CursedTome:
                SetupEventPanel("You ran to safety...");
                break;
        }
    }
}
    //Items
    public enum Item
    {
        Empty,
        SatchelOfSquanch,
        PickOfDestiny,
        DingusRing,
        CursedTome
    };

    //ChoiceEventTypes
    public enum ChoiceEventType
    {
        BasicChoice,
        CursedTome,
        Boss
    };


