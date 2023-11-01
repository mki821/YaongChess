using UnityEngine;

public class ChessBoard : MonoBehaviour
{
    public ChessTile[,] tiles = new ChessTile[8,8];
    public Team team = Team.None;
    public Team curTeam = Team.White;

    [SerializeField] private Transform _camTrm;
    [SerializeField] private Material _normalMat;
    [SerializeField] private Material _selectedMat;
    [SerializeField] private Material _selectableMat;
    [SerializeField] private Material _attackableMat;

    private void Awake() {
        int count = 0;
        for(int y = 0; y < 8; y++) {
            for(int x = 0; x < 8; x++) {
                tiles[x, y] = transform.GetChild(count).GetComponent<ChessTile>();
                tiles[x, y].pos = new Vector2Int(x, y);
                count++;
            }
        }
    }

    private void Start() {
        TCPClient.EventListener["set.team"] = ReceiveTeam;
        TCPClient.EventListener["chess"] = ReceiveChessInfo;
    }

    public void ReceiveTeam(LitJson.JsonData jsondata) {
        if ((int)jsondata % 2 == 0) {
            team = Team.Black;
            _camTrm.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
            team = Team.White;
    }

    public void ReceiveChessInfo(LitJson.JsonData jsondata) {
        ChessInfo data = LitJson.JsonMapper.ToObject<ChessInfo>(jsondata.ToJson());

        if(data.isMove)
            Move(new Vector2Int(data.selectTile[0], data.selectTile[1]), new Vector2Int(data.moveTile[0], data.moveTile[1]), data.isAttack);
        else
            Select(new Vector2Int(data.selectTile[0], data.selectTile[1]), data.type, data.team);
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
                return;
            }
            else if (pos.x + 1 < 8 && pos.y - 1 >= 0 && tiles[pos.x + 1, pos.y - 1].transform.childCount > 0 && team != tiles[pos.x + 1, pos.y - 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x + 1, pos.y - 1));
                return;
            }
        }
        else if (team == Team.White) {
            if (pos.x - 1 >= 0 && pos.y + 1 < 8 && tiles[pos.x - 1, pos.y + 1].transform.childCount > 0 && team != tiles[pos.x - 1, pos.y + 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x - 1, pos.y + 1));
                return;
            }
            else if (pos.x + 1 < 8 && pos.y + 1 < 8 && tiles[pos.x + 1, pos.y + 1].transform.childCount > 0 && team != tiles[pos.x + 1, pos.y + 1].GetComponentInChildren<ChessPiece>().piece.team) {
                SetSelectedTile(new Vector2Int(pos.x + 1, pos.y + 1));
                return;
            }
        }

        if (tiles[pos.x, pos.y].transform.GetChild(0).GetComponent<ChessPiece>().piece.firstMove) {
            if (team == Team.Black)
                SetSelectedTile(new Vector2Int(pos.x, pos.y - 2), false);
            else
                SetSelectedTile(new Vector2Int(pos.x, pos.y + 2), false);
        }
        if (team == Team.Black)
            SetSelectedTile(new Vector2Int(pos.x, pos.y - 1), false);
        else
            SetSelectedTile(new Vector2Int(pos.x, pos.y + 1), false);
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

    private void King(Vector2Int pos) {
        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if((x | y) == 0 || !(pos.x + x >= 0 && pos.x + x < 8 && pos.y + y >= 0 && pos.y + y < 8)) continue;
                SetSelectedTile(new Vector2Int(pos.x + x, pos.y + y));
            }
        }
    }

    private void Queen(Vector2Int pos) {
        Rook(pos);
        Bishop(pos);
    }

    private void SetSelectedTile(Vector2Int pos, bool attack = true) {
        if(attack && tiles[pos.x, pos.y].transform.childCount > 0) {
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
