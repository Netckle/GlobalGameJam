using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using Cinemachine;
using Unity.VisualScripting;

//using static UnityEditor.PlayerSettings;
using UnityEditor;

public class Root : MonoBehaviour
{
    public Tile seed;

    public Tile root;
    public Tile stem;
    public List<Tile> obstacles = new List<Tile>();

    private Vector2 startPosition = Vector2.zero;
    public List<Vector2> rootPositions  = new List<Vector2>();
    public List<Vector2> stemPositions  = new List<Vector2>();

    private bool canInstall = false;
    
    private GridLayout gridLayout;
    private Vector3Int cellPosition = Vector3Int.zero;

    public Transform childCam;
    public Tilemap rootMap;
    public List<Tilemap> obstacleMaps = new List<Tilemap>();

    [SerializeField] private int maxLimitCount;
    private int limitCount;
    [SerializeField] TMPro.TextMeshProUGUI limitTmp;

    [SerializeField] private GameObject rootEffect;
    [SerializeField] private GameObject stemEffect;
    public GameObject highlight;
    private void Start()
    {
        limitCount = maxLimitCount;
        startPosition = transform.position;

        gridLayout = rootMap.transform.parent.GetComponentInParent<GridLayout>();
        ChangeTile(seed, startPosition);


        rootPositions.Add(startPosition);
        stemPositions.Add(startPosition);
    }

    public bool IsTileCanInstall(Tile tile, Vector2 worldPos)
    {

        if (rootMap.GetTile<Tile>(gridLayout.WorldToCell(worldPos)) == tile)
            return false; // cannot path myself

        foreach (var map in obstacleMaps)
        {
            Tile target = map.GetTile<Tile>(gridLayout.WorldToCell(worldPos));

            

            foreach (var obstacle in obstacles)
            {
                if (target == obstacle)
                    return false; // cannot install new tile...!
            }
        }        
        return true;
    }

    public void ChangeTile(Tile tile, Vector2 worldPos)
    {
        cellPosition = gridLayout.WorldToCell(worldPos);
        rootMap.SetTile(cellPosition, tile);

        
    }

    public bool PressDown()     => Move(Vector2.down);

    public bool PressUp()       => Move(Vector2.up);

    public bool PressRight()    => Move(Vector2.right);

    public bool PressLeft()     => Move(Vector2.left);

    public void PressUndo()     => Undo();

    public void OpenUI() => ShowLimitUI(true);
    public void CloseUI()=> ShowLimitUI(false);

    private bool Move(Vector2 direction)
    {
        if (limitCount <= 0)
            return false;

        Vector2 rootPosition = rootPositions[rootPositions.Count - 1] + direction;
        Vector2 stemPosition = stemPositions[stemPositions.Count - 1] + direction * -1;

        canInstall = IsTileCanInstall(root, rootPosition);
        if (!canInstall)
            return false;

        stemPositions.Add(stemPosition);
        rootPositions.Add(rootPosition);

        ChangeTile(root, rootPosition);
        ChangeTile(stem, stemPosition);

        rootEffect.transform.position = gridLayout.WorldToCell(rootPosition ) + Vector3.one * 0.5f;
        rootEffect.gameObject.SetActive(false);
        rootEffect.gameObject.SetActive(true);
        stemEffect.transform.position = gridLayout.WorldToCell(stemPosition) + Vector3.one * 0.5f;
        stemEffect.gameObject.SetActive(false);
        stemEffect.gameObject.SetActive(true);
        limitCount--;

        limitTmp.text = "x " + limitCount.ToString();

        highlight.transform.position = rootPositions[rootPositions.Count - 1];

        return true;
    }

    private void Undo()
    {
        Vector2 rootPosition = rootPositions[rootPositions.Count - 1];
        Vector2 stemPosition = stemPositions[stemPositions.Count - 1];

        ChangeTile(null, rootPosition);
        ChangeTile(null, stemPosition);

        if (rootPositions.Count == 1 || stemPositions.Count == 1)
            return;

        stemPositions.RemoveAt(stemPositions.Count - 1);
        rootPositions.RemoveAt(rootPositions.Count - 1);

        limitCount++;

        limitTmp.text = "x " + limitCount.ToString();

        highlight.transform.position = rootPositions[rootPositions.Count - 1];
    }

    private void ShowLimitUI(bool isOpen)
    {

            limitTmp.transform.parent.gameObject.SetActive(isOpen);
        limitTmp.text = "x " + limitCount.ToString();
    }

    [SerializeField]private bool liftMode = false;
    public bool LiftMode { get { return liftMode; } set { liftMode = value; } }

    public void RefreshPlayerPositionOnLiftMode(Transform playerPos)
    {
        playerPos.position = stemPositions[stemPositions.Count - 1] + Vector2.up * 1.1f;
    }
}