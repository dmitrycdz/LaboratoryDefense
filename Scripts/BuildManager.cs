﻿using System;
using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public TurretData MGTurretData;
    public TurretData PBTurretData;
    public TurretData CHTurretData;
    public TurretData SPTurretData;
    public TurretData STTurretData;
    public TurretData RLTurretData;
    public TurretData PMTurretData;
    public TurretData PSTurretData;
    public TurretData TFTurretData;
    public TurretData TDTurretData;
    public TurretData FTTurretData;
    public TurretData MCTurretData;
    public TurretData MWTurretData;
    private TurretData selectedTurretData;

    GameObject TGTurrets;
    GameObject TMG;
    GameObject TST;
    GameObject TPB;
    GameObject TCH;
    GameObject TSP;

    private MapCube selectedMapCube;
    private Player player;
    public Animator moneyAnimator;
    public GameObject upgradeCanvas;
    private Animator upgradeCanvasAnimator;
    public Button buttonUpgrade;
    private GameManager gameManager;

    public class SpendMoneyEventArgs: EventArgs
    {
        public int moneySpent;
        public SpendMoneyEventArgs(int moneySpent)
        {
            this.moneySpent = moneySpent;
        }
    }
    public delegate void SpendMoneyEventHandler(object sender, SpendMoneyEventArgs e);
    public event SpendMoneyEventHandler SpendMoneyEvent;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        TGTurrets = GameObject.Find("Canvas/TurretSwitch");
        TMG = GameObject.Find("Canvas/TurretSwitch/TMG");
        TST = GameObject.Find("Canvas/TurretSwitch/TST");
        TPB = GameObject.Find("Canvas/TurretSwitch/TPB");
        TCH = GameObject.Find("Canvas/TurretSwitch/TCH");
        TSP = GameObject.Find("Canvas/TurretSwitch/TSP");
    }
    void Start()
    {
        gameManager.SubscribeSpendMoney(this);

        upgradeCanvasAnimator = upgradeCanvas.GetComponent<Animator>();

        TGTurrets.GetComponent<ToggleGroup>().SetAllTogglesOff();

        ReadFromXml(MGTurretData, Turret.TurretName.MG, "Towers.xml");
        ReadFromXml(CHTurretData, Turret.TurretName.CH, "Towers.xml");
        ReadFromXml(PBTurretData, Turret.TurretName.PB, "Towers.xml");
        ReadFromXml(SPTurretData, Turret.TurretName.SP, "Towers.xml");
        ReadFromXml(STTurretData, Turret.TurretName.ST, "Towers.xml");
        ReadFromXml(RLTurretData, Turret.TurretName.RL, "Towers.xml");
        ReadFromXml(PMTurretData, Turret.TurretName.PM, "Towers.xml");
        ReadFromXml(PSTurretData, Turret.TurretName.PS, "Towers.xml");
        ReadFromXml(TFTurretData, Turret.TurretName.TF, "Towers.xml");
        ReadFromXml(TDTurretData, Turret.TurretName.TD, "Towers.xml");
        ReadFromXml(FTTurretData, Turret.TurretName.FT, "Towers.xml");
        ReadFromXml(MCTurretData, Turret.TurretName.MC, "Towers.xml");
        ReadFromXml(MWTurretData, Turret.TurretName.MW, "Towers.xml");
        //ReadFromXml(RLTurretData, Turret.TurretName.RL, "Towers.xml");
        //ReadFromXml(RLTurretData, Turret.TurretName.RL, "Towers.xml");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject() == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                bool isCollider = Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("MapCube"));
                if (isCollider)
                {
                    MapCube mapCube = hit.collider.GetComponent<MapCube>();
                    if (selectedTurretData != null && mapCube.turretGo == null)
                    {
                        if (gameManager.money >= selectedTurretData.cost[0])
                        {
                            SpendMoneyEvent(this, new SpendMoneyEventArgs(selectedTurretData.cost[0]));
                            mapCube.BuildTurret(selectedTurretData);
                        }
                        else
                        {
                            moneyAnimator.SetTrigger("Flicker");
                        }
                    }
                    else if (mapCube.turretGo != null)
                    {
                        if (mapCube == selectedMapCube && upgradeCanvas.activeInHierarchy)
                        {
                            StartCoroutine(HideUpgradeUI());
                        }
                        else
                        {
                            ShowUpgradeUI(mapCube.transform.position, mapCube.currentLevel);
                        }
                        selectedMapCube = mapCube;
                    }

                }
            }
        }
    }

    public void OnMGSelected(bool isOn)
    {
        if (isOn) selectedTurretData = MGTurretData;
    }
    public void OnPBSelected(bool isOn)
    {
        if (isOn) selectedTurretData = PBTurretData;
    }
    public void OnCHSelected(bool isOn)
    {
        if (isOn) selectedTurretData = CHTurretData;
    }
    public void OnSPSelected(bool isOn)
    {
        if (isOn) selectedTurretData = SPTurretData;
    }
    public void OnSTSelected(bool isOn)
    {
        if (isOn) selectedTurretData = STTurretData;
    }
    public void OnRLSelected()
    {
        selectedTurretData = RLTurretData;
    }
    public void OnPMSelected()
    {
        selectedTurretData = PMTurretData;
    }
    public void OnPSSelected()
    {
        selectedTurretData = PSTurretData;
    }
    public void OnTFSelected()
    {
        selectedTurretData = TFTurretData;
    }
    public void OnTDSelected()
    {
        selectedTurretData = TDTurretData;
    }

    void ShowUpgradeUI(Vector3 pos, int mapCurrentLevel)
    {
        StopCoroutine("HideUpgradeUI");
        upgradeCanvas.SetActive(false);
        upgradeCanvas.SetActive(true);
        upgradeCanvas.transform.position = pos;
        if (mapCurrentLevel == 3) 
            buttonUpgrade.gameObject.SetActive(false);
    }

    IEnumerator HideUpgradeUI()
    {
        upgradeCanvasAnimator.SetTrigger("Hide");
        yield return new WaitForSeconds(0.8f);
        upgradeCanvas.SetActive(false);
    }

    public void OnUpgradeButtonDown()
    {
        if (selectedMapCube.turretData != null)
        {
            switch(selectedMapCube.currentLevel)
            {
                case 0:
                case 3:
                    break;
                case 1:
                    if (gameManager.money >= selectedMapCube.turretData.cost[1])
                    {
                        SpendMoneyEvent(this, new SpendMoneyEventArgs(selectedMapCube.turretData.cost[1]));
                        selectedMapCube.UpgradeTurret();
                    }
                    else
                    {
                        moneyAnimator.SetTrigger("Flicker");
                    }
                    break;
                case 2:
                    if (gameManager.money >= selectedMapCube.turretData.cost[2])
                    {
                        SpendMoneyEvent(this, new SpendMoneyEventArgs(selectedMapCube.turretData.cost[2]));
                        selectedMapCube.UpgradeTurret();
                    }
                    else
                    {
                        moneyAnimator.SetTrigger("Flicker");
                    }
                    break;
            }
        }
        StartCoroutine(HideUpgradeUI());
    }
    public void OnDestroyButtonDown()
    {
        selectedMapCube.DestroyTurret();
        StartCoroutine(HideUpgradeUI());
    }
    
    public void ReadFromXml(TurretData td, Turret.TurretName turretName, string fileName = "Towers.xml")
    {
        string filePath = "fileName";
#if UNITY_EDITOR
        filePath = Application.dataPath + "/StreamingAssets/" + fileName;
#elif UNITY_IPHONE
        filePath = Application.dataPath +"/Raw/"+fileName;  
#elif UNITY_ANDROID
        filePath = "jar:file://" + Application.dataPath + "!/assets/"+fileName;  
#else
        filePath = Application.dataPath + "/StreamingAssets/" + fileName;
#endif

        XmlReaderSettings settings = new XmlReaderSettings
        {
            IgnoreComments = true
        };

        XmlReader turretReader = XmlReader.Create(filePath);
        XmlDocument turretDocument = new XmlDocument();
        turretDocument.Load(turretReader);

        XmlNode turretRootNode = turretDocument.SelectSingleNode("Towers");
        XmlNodeList turretLists = turretRootNode.ChildNodes;

        foreach(XmlElement turretElement in turretLists)
        {
            if (turretElement.GetAttribute("name") == turretName.ToString())
            {
                td.cost = new int[3];
                XmlElement level = (XmlElement)turretElement.ChildNodes.Item(0);
                td.cost[0] = System.Int32.Parse(level.GetAttribute("cost"));
                level = (XmlElement)turretElement.ChildNodes.Item(1);
                td.cost[1] = System.Int32.Parse(level.GetAttribute("cost"));
                level = (XmlElement)turretElement.ChildNodes.Item(2);
                td.cost[2] = System.Int32.Parse(level.GetAttribute("cost"));
                break;
            }
        }
    }
}
