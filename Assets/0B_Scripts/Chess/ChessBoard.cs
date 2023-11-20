using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChessBoard : MonoBehaviour
{
    public static Dictionary<string, bool> upgradeParts = new Dictionary<string, bool>();

    public Team team = Team.None;
    public Team curTeam = Team.White;

    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Transform _camTrm;
    [SerializeField] private Material _normalMat;
    [SerializeField] private Material _selectedMat;
    [SerializeField] private Material _selectableMat;
    [SerializeField] private Material _attackableMat;
    [SerializeField] private Material _promotableMat;

    private ChessTile[,] tiles = new ChessTile[8,8];
    private CheckPiece _checkPiece;

    private void Awake() {
        int count = 0;
        for(int y = 0; y < 8; y++) {
            for(int x = 0; x < 8; x++) {
                tiles[x, y] = transform.GetChild(count).GetComponent<ChessTile>();
                tiles[x, y].pos = new Vector2Int(x, y);
                count++;
            }
        }
        _checkPiece = GetComponent<CheckPiece>();

        upgradeParts["March"] = false;
        upgradeParts["Patriarchy"] = false;

        if ((team = RememberMe.Instance.team) == Team.Black) _camTrm.rotation = Quaternion.Euler(0, 180, 0);
    }

    private void Start() {
        TCPClient.EventListener["chess"] = ReceiveChessInfo;
        TCPClient.EventListener["disconnect.room"] = (LitJson.JsonData jsondata) => SceneManager.LoadScene(1);
    }

    public void ReceiveChessInfo(LitJson.JsonData jsondata) {
        ChessInfo data = LitJson.JsonMapper.ToObject<ChessInfo>(jsondata.ToJson());

        if(data.isMove) {
            if(data.promote)
                Promote(new Vector2Int(data.selectTile[0], data.selectTile[1]), new Vector2Int(data.moveTile[0], data.moveTile[1]), data.type);
            else
                Move(new Vector2Int(data.selectTile[0], data.selectTile[1]), new Vector2Int(data.moveTile[0], data.moveTile[1]), data.isAttack);
        }
        else
            Select(new Vector2Int(data.selectTile[0], data.selectTile[1]), data.type, data.team);
    }

    private void Promote(Vector2Int pos, Vector2Int targetPos, Type type) {
        Transform targetPiece = tiles[pos.x, pos.y].transform.GetChild(0);
        _checkPiece.icons.Remove(targetPiece.GetChild(targetPiece.childCount - 1).gameObject);
        Destroy(targetPiece.gameObject);
        int black = 0;
        if(team == Team.Black) black = 4;

        GameObject promoteObj = null;
        switch(type) {
            case Type.Knight:
                promoteObj = prefabs[1 + black];
                break;
            case Type.Bishop:
                promoteObj = prefabs[2 + black];
                break;
            case Type.Rook:
                promoteObj = prefabs[0 + black];
                break;
            case Type.Queen:
                promoteObj = prefabs[3 + black];
                break;
        }

        GameObject obj = Instantiate(promoteObj, tiles[targetPos.x, targetPos.y].transform);
        obj.transform.localPosition = new Vector3(0, 0.2f, 0);
        _checkPiece.icons.Add(obj.transform.GetChild(obj.transform.childCount - 1).gameObject);
        obj.transform.GetChild(obj.transform.childCount - 1).gameObject.SetActive(false);
        DeselectAll();
    }

    public void Select(Vector2Int pos, Type type, Team team) {
        DeselectAll();
        tiles[pos.x, pos.y].meshRender.material = _selectedMat;
        
        if(team == this.team) {
            switch(type) {
                case Type.Pawn:
                    Pawn(pos);
                    break;
                case Type.Rook:
                    Rook(pos);
                    break;
                case Type.Knight:
                    Knight(pos);
                    break;
                case Type.Bishop:
                    Bishop(pos);
                    break;
                case Type.King:
                    King(pos);
                    break;
                case Type.Queen:
                    Queen(pos);
                    break;
            }
        }
    }

    private void Pawn(Vector2Int pos) {
        if (team == Team.Black) {
            if (pos.x - 1 >= 0 && pos.y - 1 >= 0 && tiles[pos.x - 1, pos.y - 1].transform.childCount > 0 && team != tiles[pos.x - 1, pos.y - 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x - 1, pos.y - 1));
            }
            else if (pos.x + 1 < 8 && pos.y - 1 >= 0 && tiles[pos.x + 1, pos.y - 1].transform.childCount > 0 && team != tiles[pos.x + 1, pos.y - 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x + 1, pos.y - 1));
            }
        }
        else if (team == Team.White) {
            if (pos.x - 1 >= 0 && pos.y + 1 < 8 && tiles[pos.x - 1, pos.y + 1].transform.childCount > 0 && team != tiles[pos.x - 1, pos.y + 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x - 1, pos.y + 1));
            }
            else if (pos.x + 1 < 8 && pos.y + 1 < 8 && tiles[pos.x + 1, pos.y + 1].transform.childCount > 0 && team != tiles[pos.x + 1, pos.y + 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x + 1, pos.y + 1));
            }
        }

        if (tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove) {
            if (team == Team.Black)
                SetSelectedTile(new Vector2Int(pos.x, pos.y - 2), false);
            else
                SetSelectedTile(new Vector2Int(pos.x, pos.y + 2), false);

            if(upgradeParts["March"]) {
                if (team == Team.Black)
                    SetSelectedTile(new Vector2Int(pos.x, pos.y - 3), false);
                else
                    SetSelectedTile(new Vector2Int(pos.x, pos.y + 3), false);
            }
        }
        if (team == Team.Black)
            SetSelectedTile(new Vector2Int(pos.x, pos.y - 1), false);
        else
            SetSelectedTile(new Vector2Int(pos.x, pos.y + 1), false);

        if(upgradeParts["March"]) {
            if (team == Team.Black)
                SetSelectedTile(new Vector2Int(pos.x, pos.y - 2), false);
            else
                SetSelectedTile(new Vector2Int(pos.x, pos.y + 2), false);
        }
    }

    private void Rook(Vector2Int pos)
    {
        for(int i = 1; i < 8; i++) {
            if(pos.x + i < 8) {
                SetSelectedTile(new Vector2Int(pos.x + i, pos.y));
                if(tiles[pos.x + i, pos.y].transform.childCount > 0) break;
            }
            else break;
        }
        for(int i = 1; i < 8; i++) {
            if(pos.x - i >= 0) {
                SetSelectedTile(new Vector2Int(pos.x - i, pos.y));
                if(tiles[pos.x - i, pos.y].transform.childCount > 0) break;
            }
            else break;
        }
        for(int i = 1; i < 8; i++) {
            if(pos.y + i < 8) {
                SetSelectedTile(new Vector2Int(pos.x, pos.y + i));
                if(tiles[pos.x, pos.y + i].transform.childCount > 0) break;
            }
            else break;
        }
        for(int i = 1; i < 8; i++) {
            if(pos.y - i >= 0) {
                SetSelectedTile(new Vector2Int(pos.x, pos.y - i));
                if(tiles[pos.x, pos.y - i].transform.childCount > 0) break;
            }
            else break;
        }
    }

    private void Knight(Vector2Int pos)
    {
        if(pos.x + 2 >= 0 && pos.x + 2 < 8 && pos.y + 1 >= 0 && pos.y + 1 < 8)
            SetSelectedTile(new Vector2Int(pos.x + 2, pos.y + 1));
        if(pos.x + 2 >= 0 && pos.x + 2 < 8 && pos.y - 1 > 0 && pos.y - 1 < 8)
            SetSelectedTile(new Vector2Int(pos.x + 2, pos.y - 1));
        if(pos.x + 1 >= 0 && pos.x + 1 < 8 && pos.y + 2 >= 0 && pos.y + 2 < 8)
            SetSelectedTile(new Vector2Int(pos.x + 1, pos.y + 2));
        if(pos.x + 1 >= 0 && pos.x + 1 < 8 && pos.y - 2 >= 0 && pos.y - 2 < 8)
            SetSelectedTile(new Vector2Int(pos.x + 1, pos.y - 2));
        if(pos.x - 1 >= 0 && pos.x - 1 < 8 && pos.y + 2 >= 0 && pos.y + 2 < 8)
            SetSelectedTile(new Vector2Int(pos.x - 1, pos.y + 2));
        if(pos.x - 1 >= 0 && pos.x - 1 < 8 && pos.y - 2 >= 0 && pos.y - 2 < 8)
            SetSelectedTile(new Vector2Int(pos.x - 1, pos.y - 2));
        if(pos.x - 2 >= 0 && pos.x - 2 < 8 && pos.y + 1 >= 0 && pos.y + 1 < 8)
            SetSelectedTile(new Vector2Int(pos.x - 2, pos.y + 1));
        if(pos.x - 2 >= 0 && pos.x - 2 < 8 && pos.y - 1 >= 0 && pos.y - 1 < 8)
            SetSelectedTile(new Vector2Int(pos.x - 2, pos.y - 1));
    }

    private void Bishop(Vector2Int pos)
    {
        for(int i = 1; i < 8; i++) {
            if(pos.x + i < 8 && pos.y + i < 8) {
                SetSelectedTile(new Vector2Int(pos.x + i, pos.y + i));
                if(tiles[pos.x + i, pos.y + i].transform.childCount > 0) break;
            }
        }
        for(int i = 1; i < 8; i++) {
            if(pos.x - i >= 0 && pos.y + i < 8) {
                SetSelectedTile(new Vector2Int(pos.x - i, pos.y + i));
                if(tiles[pos.x - i, pos.y + i].transform.childCount > 0) break;
            }
        }
        for(int i = 1; i < 8; i++) {
            if(pos.x + i < 8 && pos.y - i >= 0) {
                SetSelectedTile(new Vector2Int(pos.x + i, pos.y - i));
                if(tiles[pos.x + i, pos.y - i].transform.childCount > 0) break;
            }
        }
        for(int i = 1; i < 8; i++) {
            if(pos.x - i >= 0 && pos.y - i >= 0) {
                SetSelectedTile(new Vector2Int(pos.x - i, pos.y - i));
                if(tiles[pos.x - i, pos.y - i].transform.childCount > 0) break;
            }
        }
    }

    private void King(Vector2Int pos, bool ignore = false) {
        if(upgradeParts["Patriarchy"] && !ignore) {
            Queen(pos, true);
            return;
        }

        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if((x | y) == 0 || !(pos.x + x >= 0 && pos.x + x < 8 && pos.y + y >= 0 && pos.y + y < 8)) continue;
                SetSelectedTile(new Vector2Int(pos.x + x, pos.y + y));
            }
        }

        if(tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove) {
            for(int i = pos.x + 1; i < 8; ++i)
                if(tiles[i, pos.y].transform.childCount > 0) {
                    if(tiles[i, pos.y].transform.GetChild(0).name.Contains("Rook"))
                        if(tiles[i, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove)
                            SetSelectedTile(new Vector2Int(pos.x + 2, pos.y));
                break;
                }
            for(int i = pos.x - 1; i >= 0; --i)
                if(tiles[i, pos.y].transform.childCount > 0) {
                    if(tiles[i, pos.y].transform.GetChild(0).name.Contains("Rook"))
                        if(tiles[i, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove)
                            SetSelectedTile(new Vector2Int(pos.x - 2, pos.y));
                break;
                }
        }
    }

    private void Queen(Vector2Int pos, bool ignore = false) {
        if(upgradeParts["Patriarchy"] && !ignore) {
            King(pos, true);
            return;
        }

        Rook(pos);
        Bishop(pos);
    }

    private void SetSelectedTile(Vector2Int pos, bool attack = true, bool promote = false) {
        if(promote && tiles[pos.x, pos.y].transform.childCount == 0 && (pos.y == 0 || pos.y == 7)) {
            tiles[pos.x, pos.y].meshRender.material = _promotableMat;
            tiles[pos.x, pos.y].gameObject.layer = 9;
        }
        else if(attack && tiles[pos.x, pos.y].transform.childCount > 0) {
            if(tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.team != team) {
                tiles[pos.x, pos.y].meshRender.material = _attackableMat;
                tiles[pos.x, pos.y].gameObject.layer = 8;
            }
            else return;
        }
        else if (tiles[pos.x, pos.y].transform.childCount == 0) {
            tiles[pos.x, pos.y].meshRender.material = _selectableMat;
            tiles[pos.x, pos.y].gameObject.layer = 7;
        }
    }

    public void Move(Vector2Int pos, Vector2Int tar, bool isAttack) {
        if (tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove)
            tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove = false;

        DeselectAll();

        if(isAttack) {
            Destroy(tiles[tar.x, tar.y].transform.GetChild(0).gameObject);
        }
        else if(tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.type == Type.King) {
            if(pos.x - tar.x == 2) {
                Transform rook = tiles[0, pos.y].transform.GetChild(0);
                rook.parent = tiles[tar.x + 1, tar.y].transform;
                rook.localPosition = new Vector3(0, rook.localPosition.y, 0);
            }
            else if(pos.x - tar.x == -2) {
                Transform rook = tiles[7, pos.y].transform.GetChild(0);
                rook.parent = tiles[tar.x - 1, tar.y].transform;
                rook.localPosition = new Vector3(0, rook.localPosition.y, 0);
            }
        }
        Transform trm = tiles[pos.x, pos.y].transform.GetChild(0);
        trm.parent = tiles[tar.x, tar.y].transform;
        trm.localPosition = new Vector3(0, trm.localPosition.y, 0);

        curTeam = curTeam == Team.White ? Team.Black : Team.White;
    }

    public void DeselectAll() {
        foreach(var item in tiles) {
            item.GetComponent<MeshRenderer>().material = _normalMat;
            item.gameObject.layer = 6;
        }
    }
}
