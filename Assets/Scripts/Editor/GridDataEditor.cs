using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridSettings))]
public class GridDataEditor : Editor
{
    private GridSettings GridData;

    private int LastGridWidth;
    private int LastGridHeight;

    void OnEnable()
    {
        GridData = (GridSettings)target;
        if (GridData == null) return;
        SceneView.duringSceneGui += DuringSceneGUI;
        Undo.undoRedoPerformed += OnUndoRedo;

        LastGridWidth = GridData.GridWidth;
        LastGridHeight = GridData.GridHeight;
    }

    protected void OnDisable()
    {
        if (GridData == null) return;
        SceneView.duringSceneGui -= DuringSceneGUI;
        Undo.undoRedoPerformed -= OnUndoRedo;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GridData == null) return;

        EditorGUILayout.LabelField(new GUIContent($"Number of player units to spawn: {GridData.PlayerUnitCount}"));
        if (GridData.PlayerUnitCount <= 0)
        {
            EditorGUILayout.HelpBox("Enemy spawn cell has not been selected.", MessageType.Warning);
        }
        EditorGUILayout.LabelField(new GUIContent($"Number of enemy units to spawn: {GridData.EnemyUnitCount}"));
        if (GridData.EnemyUnitCount <= 0)
        {
            EditorGUILayout.HelpBox("Player spawn cell has not been selected.", MessageType.Warning);
        }
        
        if(LastGridWidth != GridData.GridWidth || 
           LastGridHeight != GridData.GridHeight)
        {
            GridData.ResizeCellArray(LastGridWidth, LastGridHeight);
            LastGridWidth = GridData.GridWidth;
            LastGridHeight = GridData.GridHeight;
        }
    }

    private void OnUndoRedo()
    {
        LastGridWidth = GridData.GridWidth;
        LastGridHeight = GridData.GridHeight;
    }
    
    private void DuringSceneGUI(SceneView sceneView)
    {
        if (GridData == null) return;

        Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        int width = GridData.GridWidth;
        int height = GridData.GridHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                DrawCell(x, y);
                HandleClick(x, y);
            }
        }
    }

    private void DrawCell(int coordX, int coordY)
    {
        float cellSize = GridData.CellSize;
        float halfCellSize = cellSize / 2.0f;

        Vector2 pos = new Vector2(
            coordX * cellSize - halfCellSize,
            coordY * cellSize - halfCellSize);
        var rect = new Rect(
            pos.x,
            pos.y,
            cellSize,
            cellSize);

        Handles.DrawSolidRectangleWithOutline(
            new Vector3[]
            {
                new(rect.xMin, 0, rect.yMin),
                new(rect.xMax, 0, rect.yMin),
                new(rect.xMax, 0, rect.yMax),
                new(rect.xMin, 0, rect.yMax),
            },
            GetColor(GridData.GetCellType(coordX, coordY)),
            Color.black);

        switch (GridData.GetUnitSpawnType(coordX, coordY))
        {
            case UnitSpawnType.Friendly:
                DrawUnitMarker(Color.green);
                break;
            case UnitSpawnType.Enemy:
                DrawUnitMarker(Color.red);
                break;
        }
        return;

        void DrawUnitMarker(Color color)
        {
            Color prevCol = Handles.color;
            Handles.color = color;
            Handles.DrawSolidDisc(new Vector3(rect.center.x, 0, rect.center.y),
                Vector3.up,
                cellSize * 0.5f);
            Handles.color = prevCol;
        }
        Color GetColor(CellType type) => type switch
        {
            CellType.Traversable => new Color(0, 1, 0, 0.4f),
            CellType.Obstacle => new Color(1, 0, 0, 0.4f),
            CellType.Cover => new Color(0, 0, 1, 0.4f),
            _ => Color.gray
        };
    }

    private void HandleClick(int coordX, int coordY)
    {
        if (Event.current.type != EventType.MouseDown || Event.current.button != 0) return;
        
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (!new Plane(Vector3.up, Vector3.zero).Raycast(ray, out var enter)) return;

        Vector3 hit = ray.GetPoint(enter);
        float cellSize = GridData.CellSize;
        if (Mathf.RoundToInt(hit.x / cellSize) != coordX || 
            Mathf.RoundToInt(hit.z / cellSize) != coordY) return;

        CellType cellType = GridData.GetCellType(coordX, coordY);
        UnitSpawnType spawnType = GridData.GetUnitSpawnType(coordX, coordY);
        
        GenericMenu menu = new GenericMenu();
        foreach (CellType type in Enum.GetValues(typeof(CellType)))
        {
            AddMenuItem(spawnType == UnitSpawnType.None,
                new GUIContent(type.ToString()),
                cellType == type,
                SetCellType, type);
        }
        menu.AddSeparator(string.Empty);
        foreach (UnitSpawnType type in Enum.GetValues(typeof(UnitSpawnType)))
        {
            AddMenuItem(cellType == CellType.Traversable,
                new GUIContent(type.ToString()),
                spawnType == type,
                SetSpawnType, type);
        }

        menu.ShowAsContext();
        Event.current.Use();
        return;

        void AddMenuItem(bool enabled, GUIContent content, bool on, GenericMenu.MenuFunction2 onPress, object data)
        {
            if (enabled) menu.AddItem(content, on, onPress, data);
            else menu.AddDisabledItem(content, on);
        }
        
        void SetCellType(object data)
        {
            if (data is not CellType type) return;
            Undo.RecordObject(GridData, "Change Cell Type");
            GridData.SetCellType(coordX, coordY, type);
            EditorUtility.SetDirty(GridData);
        }

        void SetSpawnType(object data)
        {
            if (data is not UnitSpawnType type) return;
            Undo.RecordObject(GridData, "Change Spawn Unit Type");
            GridData.SetUnitSpawnType(coordX, coordY, type);
            EditorUtility.SetDirty(GridData);
        }
    }
}
