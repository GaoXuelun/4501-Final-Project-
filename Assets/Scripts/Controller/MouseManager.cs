using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Events;
using System;

public class MouseManager : MonoBehaviour
{
    private LineRenderer line;  // LineRenderer to draw select box

    private RaycastHit hitInfo; // ray inspection
    private Ray ray;
    private bool isMouseDown = false;   // jude mouse status

    // four vertices of selection area, use to draw selection rectangle
    private Vector3 leftTopPoint;   
    private Vector3 rightBottomPoint;
    private Vector3 leftBottomPoint;
    private Vector3 rightTopPoint;

    // make a box collider to get obj in area
    private Vector3 beginWorldPos;  // point of mouse click down, also the "leftTopPoint"
    private Vector3 center; // center of box collider
    private Vector3 halfExtents;    // half range of box collider
    private List<CharacterController> characters = new List<CharacterController>();  // store objs in area

    private Vector3 frontPos = Vector3.zero;
    private float unitOffset = 2;   // distance between two units

    public static MouseManager Instance;
    // public event Action<Vector3> OnMouseClicked;

    [SerializeField] private Texture2D move, attack, arrow, interact;

    void Awake()    // initialization
    {
        line = this.GetComponent<LineRenderer>();   // get line component     

        if (Instance != null)
            Destroy(gameObject);
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        for (int i = 0; i < characters.Count && characters.Count != 0; i++)
        {
            if (characters[i].GetComponent<CharacterStats>().CurrentHP == 0)
                characters.Remove(characters[i]);
        }
        MouseSelection();
        MoveController();
        SetCursorIcon();

    }

    void SetCursorIcon()
    {
        if (Physics.Raycast(ray, out hitInfo))
        {
            // change cursor based on differnt obj
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground":
                    Cursor.SetCursor(move, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Enemy":
                    Cursor.SetCursor(attack, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Barrack":
                    Cursor.SetCursor(interact, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Base":
                    Cursor.SetCursor(interact, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Shop":
                    Cursor.SetCursor(interact, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Gold":
                    Cursor.SetCursor(interact, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Gem":
                    Cursor.SetCursor(interact, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "Gas":
                    Cursor.SetCursor(interact, new Vector2(32, 32), CursorMode.Auto);
                    break;
                case "UI":
                    Cursor.SetCursor(arrow, new Vector2(32, 32), CursorMode.Auto);
                    break;
            }
        }
    }
    private void MouseSelection()
    {
        // mouse left button click ground as start point
        if (Input.GetMouseButtonDown(0))
        {
            leftTopPoint = Input.mousePosition; // mouse current position
            isMouseDown = true;
            // raycast to get the point on "Ground"
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Ground"))
                beginWorldPos = hitInfo.point;
        }
        // mouse left button up as end point
        else if (Input.GetMouseButtonUp(0))
        {            
            isMouseDown = false;
            line.positionCount = 0; // stop draw selection box

            // clear list and initialize objs
            for (int i = 0; i < characters.Count; i++)
                characters[i].SetSelected(false);
            characters.Clear();   

            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                center = new Vector3((hitInfo.point.x + beginWorldPos.x) * 0.5f, 1, (hitInfo.point.z + beginWorldPos.z) * 0.5f);  // center point of box collider
                halfExtents = new Vector3(Mathf.Abs(hitInfo.point.x - beginWorldPos.x) * 0.5f, 1, Mathf.Abs(hitInfo.point.z - beginWorldPos.z) * 0.5f);   // half range of box collider
                Collider[] colliders = Physics.OverlapBox(center, halfExtents); // get colliders in area

                for (int i = 0; i < colliders.Length && characters.Count < 12; i++) // can select up to 12 objs at once
                {
                    CharacterController obj = colliders[i].GetComponent<CharacterController>();   // get objs
                    if (obj != null)
                    {
                        obj.SetSelected(true);    // active the SelectedCircle
                        characters.Add(obj);
                    }
                }
                SortSoldiers(characters, 0);   // sort list with obj type
            }
        }
        DrawSelectionBox(); // draw select area
    }

    private void DrawSelectionBox()
    {
        // draw if mouse down
        if (isMouseDown)
        {
            // get four point of box
            leftTopPoint.z = 5;  
            rightBottomPoint = Input.mousePosition;
            rightBottomPoint.z = 5;
            rightTopPoint = new Vector3(rightBottomPoint.x, leftTopPoint.y, 5);
            leftBottomPoint = new Vector3(leftTopPoint.x, rightBottomPoint.y, 5);
            // draw rectangle
            line.positionCount = 4;
            line.SetPosition(0, Camera.main.ScreenToWorldPoint(leftTopPoint));
            line.SetPosition(1, Camera.main.ScreenToWorldPoint(rightTopPoint));
            line.SetPosition(2, Camera.main.ScreenToWorldPoint(rightBottomPoint));
            line.SetPosition(3, Camera.main.ScreenToWorldPoint(leftBottomPoint));
        }
    }

    private void MoveController()
    {
        // mouse right buttom click to move
        if (Input.GetMouseButtonDown(1))
        {
            if (characters.Count == 0) // if no obj selected
                return;
            // ray inspection to get target point
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);
                
                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].EventMove(newPos[i]);
                    // characters[i].OnMouseClicked?.Invoke(newPos[i]);
                // else if (hitInfo.collider.gameObject.CompareTag("Character"))   //check if tag is character [edit (else if)]
                // {
                //     Transform targetCharacter = hitInfo.collider.transform;
                //     for (int i = 0; i < characters.Count; i++)
                //     {
                //         characters[i].FollowMove(targetCharacter);  //Follow method
                //     }
                // }
            }
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Enemy"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);
                
                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].EventAttack(hitInfo.collider.gameObject);
            }
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Gold"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);

                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].EventGold(hitInfo.collider.gameObject);                    
            }
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Gem"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);

                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].EventGem(hitInfo.collider.gameObject);                    
            }
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Gas"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);

                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].EventGas(hitInfo.collider.gameObject);                    
            }
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Base"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);

                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].EventAttackBase(hitInfo.collider.gameObject);
            }
            if (Physics.Raycast(ray, out hitInfo) && hitInfo.collider.gameObject.CompareTag("Character"))
            {
                Vector3 oldForward = characters[0].transform.forward;
                Vector3 newForward = (hitInfo.point - characters[0].transform.position).normalized;
                // calculate angle between new forward and old forward
                if (Vector3.Angle(oldForward, newForward) > 60)
                    SortSoldiers(characters, 1);
                
                // get target pos and let obj move to target pos
                List<Vector3> newPos = GetTargetPos(hitInfo.point);
                Transform targetCharacter = hitInfo.collider.transform;
                for (int i = 0; i < characters.Count && characters.Count != 0; i++)
                    characters[i].FollowMove(targetCharacter);  //Follow method
                    //characters[i].MoveToFollowTarget(hitInfo.collider.gameObject);
            }
        }
    }

    private List<Vector3> GetTargetPos(Vector3 targetPos)
    {
        // current point's forward and right
        Vector3 curForward = Vector3.zero;
        Vector3 curRight = Vector3.zero;

        if (frontPos != Vector3.zero) // have last time point
            curForward = (targetPos - frontPos).normalized;
        else    // no last time point, use first obj's point to calculate 
            curForward = (targetPos - characters[0].transform.position).normalized;    
        curRight = Quaternion.Euler(9, 90, 0) * curForward; // rotate from forward to get right

        // calculate target pos based on the number of objs
        List<Vector3> newPos = new List<Vector3>();
        switch (characters.Count)
        {
            case 1:
                newPos.Add(targetPos);
                break;
            case 2:
                newPos.Add(targetPos + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curRight * unitOffset * 0.5f);
                break;
            case 3:
                newPos.Add(targetPos);
                newPos.Add(targetPos + curRight * unitOffset);
                newPos.Add(targetPos - curRight * unitOffset);
                break;
            case 4:
                newPos.Add(targetPos + curForward * unitOffset * 0.5f - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset * 0.5f + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f + curRight * unitOffset * 0.5f);
                break;
            case 5:
                newPos.Add(targetPos + curForward * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset * 0.5f - curRight * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset * 0.5f + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f - curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f + curRight * unitOffset);
                break;
            case 6:
                newPos.Add(targetPos + curForward * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset * 0.5f - curRight * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset * 0.5f + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f - curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset * 0.5f);
                break;
            case 7:
                newPos.Add(targetPos + curForward * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset);
                newPos.Add(targetPos - curRight * unitOffset);
                newPos.Add(targetPos);
                newPos.Add(targetPos + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset);
                break;
            case 8:
                newPos.Add(targetPos + curForward * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset);
                newPos.Add(targetPos - curRight * unitOffset);
                newPos.Add(targetPos);
                newPos.Add(targetPos + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset - curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset + curRight * unitOffset);
                break;
            case 9:
                newPos.Add(targetPos + curForward * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset);
                newPos.Add(targetPos - curRight * unitOffset);
                newPos.Add(targetPos);
                newPos.Add(targetPos + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset - curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset + curRight * unitOffset);
                newPos.Add(targetPos - curForward * unitOffset);
                break;
            case 10:
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset * 1.5f);

                newPos.Add(targetPos - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos + curRight * unitOffset * 1.5f);

                newPos.Add(targetPos - curForward * unitOffset - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos - curForward * unitOffset + curRight * unitOffset * 1.5f);
                break;
            case 11:
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset * 1.5f);

                newPos.Add(targetPos - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos + curRight * unitOffset * 1.5f);

                newPos.Add(targetPos - curForward * unitOffset - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos - curForward * unitOffset + curRight * unitOffset * 1.5f);
                newPos.Add(targetPos - curForward * unitOffset);
                break;
            case 12:
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curForward * unitOffset - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos + curForward * unitOffset + curRight * unitOffset * 1.5f);

                newPos.Add(targetPos - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos + curRight * unitOffset * 1.5f);

                newPos.Add(targetPos - curForward * unitOffset - curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curForward * unitOffset + curRight * unitOffset * 0.5f);
                newPos.Add(targetPos - curForward * unitOffset - curRight * unitOffset * 1.5f);
                newPos.Add(targetPos - curForward * unitOffset + curRight * unitOffset * 1.5f);
                break;
        }
        
        frontPos = targetPos;   // store current pos
        return newPos;
    }

    private void SortSoldiers(List<CharacterController> characters, int type)
    {   
        // sort objs with it's type
        characters.Sort((a, b) => {
            // objs with small number move forward
            if (a.type < b.type)
                return -1;
            else if (a.type == b.type)
            {
                if (type == 0)
                    return 0;
                else
                {
                    if (Vector3.Distance(a.transform.position, hitInfo.point) <= Vector3.Distance(b.transform.position, hitInfo.point))
                        return -1;
                    else 
                        return 1;
                }
            }
            else
                return 1;
        });
    }
}
