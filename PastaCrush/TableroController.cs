using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TableroController : MonoBehaviour {
    readonly int tableroHeigth = 10;
    readonly int tableroWidth = 12;
    public List<List<CasillaCursor>> tablero;
    public List<CasillaCursor> casillasToRelocatable;
    public List<GameObject> casillasPrefabs;
    public GameObject cursorPrefab;
    public bool timeToCheck = false;
    public bool timeToRelocate = false;
    public Text blockText;
    public int destroyableCount;
    string block = "";
    bool autofill = false;
    public int timesBeingZero = 0;

    private CasillaCursor firstCasillaCursor;
    private void Awake() {
        StartGame();
        Print();
    }
    private void Update() {
        if (timeToCheck) {
            destroyableCount = CheckAndDestroy();
            Print();
            if (destroyableCount == 0) {
                timesBeingZero++;
                Invoke(nameof(TimeToRecolocate), 1f);
            } else {
                timesBeingZero = 0;
            }
        }
        if (timeToRelocate) {
            Recolocate(FindRelocatable());
            Print();
            if (casillasToRelocatable.Count == 0) {

                if (timesBeingZero >= 5) {
                    timesBeingZero = 0;
                    timeToRelocate = false;
                    timeToCheck = false;
                    //Invoke(nameof(TimeToRefill), 3f);
                } else {
                    timesBeingZero++;
                    Invoke(nameof(TimeToCheck), 1f);
                }
            } else {
                timesBeingZero = 0;
            }
        }

    }
    private void TimeToRecolocate() {
        timeToRelocate = true;
        timeToCheck = false;

    }
    private void TimeToCheck() {
        timeToCheck = true;
        timeToRelocate = false;
    }
    private void TimeToRefill() {

        Refill();
    }

    public void StartGame() {
        tablero = new List<List<CasillaCursor>>();
        for (int i = 0; i < tableroHeigth; i++) {
            tablero.Add(new List<CasillaCursor>());
            for (int k = 0; k < tableroWidth; k++) {
                int typeCasilla = Random.Range(0, casillasPrefabs.Count);
                tablero[i].Add(Instantiate(cursorPrefab, new Vector3Int(k, i, 0), Quaternion.identity).GetComponent<CasillaCursor>());
                InitCasilla(tablero[i][k], k, i);
            }
        }
        Invoke(nameof(TimeToCheck), 1f);
    }

    public List<CasillaCursor> FindRelocatable() {
        casillasToRelocatable = new List<CasillaCursor>();
        for (int y = 0; y < tableroHeigth; y++) {
            for (int x = 0; x < tableroWidth; x++) {
                if (y - 1 >= 0) {
                    try {
                        if (tablero[y - 1][x].isNull && !tablero[y][x].isNull) {
                            casillasToRelocatable.Add(tablero[y][x]);
                        }
                    }
                    catch (System.NullReferenceException e) {
                        Debug.LogError(e.Message);
                    }
                }
            }
        }
        return casillasToRelocatable;
    }

    public void Recolocate(List<CasillaCursor> casillasToRelocatable) {
        for (int i = 0; i < casillasToRelocatable.Count; i++) {
            CasillaCursor tmp = tablero[casillasToRelocatable[i].y - 1][casillasToRelocatable[i].x];
            SwitchCasillas(casillasToRelocatable[i], tmp);
        }
    }

    public int CheckAndDestroy() {
        List<CasillaCursor> casillasToDestroy = new List<CasillaCursor>();
        casillasToDestroy.AddRange(Check(tableroWidth, tableroHeigth, 0));
        casillasToDestroy.AddRange(Check(tableroHeigth, tableroWidth, 1));

        for (int i = 0; i < casillasToDestroy.Count; i++) {
            //tablero[casillasToDestroy[i].x, casillasToDestroy[i].y] = null;
            casillasToDestroy[i].Die();
            //if (autofill) {
            //    InitCasilla(casillasToDestroy[i], casillasToDestroy[i].x, casillasToDestroy[i].y);
            //}
        }
        return casillasToDestroy.Count;
    }

    public List<CasillaCursor> Check(int x, int y, int orientation) {
        List<CasillaCursor> casillasToDestroy = new List<CasillaCursor>();
        for (int i = 0; i < x; i++) {
            List<CasillaCursor> line = new List<CasillaCursor>();
            for (int k = 0; k < y; k++) {
                if (GetCasilla(x, y, i, k, orientation).isNull) {
                    if (line.Count >= 3) {
                        casillasToDestroy.AddRange(line);
                    }
                    line = new List<CasillaCursor>();
                } else if (line.Count == 0 || line[0].type == GetCasilla(x, y, i, k, orientation).type) {
                    line.Add(GetCasilla(x, y, i, k, orientation));
                } else {
                    if (line.Count >= 3) {
                        casillasToDestroy.AddRange(line);
                    }
                    line = new List<CasillaCursor>();
                    line.Add(GetCasilla(x, y, i, k, orientation));
                }
                block += "[" + i + " " + GetCasilla(x, y, i, k, orientation).type + " " + k + "]";
            }
            if (line.Count >= 3) {
                casillasToDestroy.AddRange(line);
            }
            block += "\n";
        }
        //blockText.text = block;
        return casillasToDestroy;
    }

    public CasillaCursor GetCasilla(int x, int y, int i, int k, int orientation) {
        try {
            switch (orientation) {
                case 0:
                    return tablero[k][i];
                case 1:
                    return tablero[i][k];
                default:
                    return null;
            }
        }
        catch (System.NullReferenceException e) {
            Debug.LogError(e.Message);
            return null;
        }
    }
    public void Print() {
        block = "";
        for (int y = tableroHeigth - 1; y >= 0; y--) {
            for (int x = 0; x < tableroWidth; x++) { block += tablero[y][x].GetSortName(); }
            block += "\n";
        }
        blockText.text = block;
    }

    public void OnCasillaClickated(CasillaCursor casillaCursor) {
        if (firstCasillaCursor == null) {
            timeToRelocate = false;
            timeToCheck = false;
            firstCasillaCursor = casillaCursor;
        } else {
            if (!firstCasillaCursor.Equals(casillaCursor)) {
                SwitchCasillas(firstCasillaCursor, casillaCursor);
            }
            firstCasillaCursor = null;
            timeToRelocate = true;
        }
    }
    private void SwitchCasillas(CasillaCursor casilla1, CasillaCursor casilla2) {
        int tmpX1 = casilla1.x, tmpY1 = casilla1.y, tmpX2 = casilla2.x, tmpY2 = casilla2.y;
        tablero[casilla2.y][casilla2.x] = casilla1;
        tablero[casilla1.y][casilla1.x] = casilla2;
        casilla1.MoveTo(tmpX2, tmpY2);
        casilla2.MoveTo(tmpX1, tmpY1);
    }

    public void Refill() {
        for (int i = 0; i < tableroHeigth; i++) {
            tablero.Add(new List<CasillaCursor>());
            for (int k = 0; k < tableroWidth; k++) { if (tablero[i][k].isNull) { InitCasilla(tablero[i][k], k, i); } }
        }
    }

    private void InitCasilla(CasillaCursor cursor, int x, int y) {
        int typeCasilla = Random.Range(0, casillasPrefabs.Count);
        cursor.type = typeCasilla;
        cursor.x = x;
        cursor.y = y;
        cursor.Done();
    }
}
