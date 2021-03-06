﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.EditorCoroutines.Editor;
namespace UGame
{
    [CustomEditor(typeof(BlockTerrain))]
    public class BlockTerrainEditor : Editor
    {
        public string[] panelNames = new string[] {"笔刷", "图块", "图层", "设置"};
        public int selectPanelIndex =0;
        public string[] blockCreateModes = new string[] {"One Tex", "Two Tex", "Tree Tex", "Six Tex"};
        public int blockCreateModeIndex = 0;
        public BlockDefinition activeBlockDef;
        public Sprite top, bottom, left, right, front, back;
        public Vector2 blockScrollViewPos = Vector2.zero;
        public int selectBlockDefIndex = 0;
        public int selectLayerDefIndex = 0;
        //画笔
        public string[] brushNames = new string[] {"笔","油漆桶"};
        public int activeBrushIndex = 0;
        //笔刷使用的图块类型
        public List<GUIContent> blockTypes = new List<GUIContent>();
        public int selectBlockTypeIndex = 0;
        //*****************************
//        private List<Bounds> chunkBounds;
//        private List<Bounds> activeBlocksBounds;
        private bool isDirty = false;
//        private int selectedChunkIndex = -1;
//        private int selectedBlockIndex = -1;
        private bool isMouseIn = false;
        private BlockTerrain _terrain;
        private BlockTerrainData _terrainData;

        private int layer = 0;
        private void OnEnable()
        {
            _terrain = (BlockTerrain) target;
            if (_terrain != null)
            {
                _terrainData = _terrain.data;
            }
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(_terrainData);
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(_terrainData);
        }

        /// <summary>
        /// 场景视图GUI绘制
        /// </summary>
        public void OnSceneGUI()
        {
            //如果map为null 不绘制
            if (_terrain == null) return;
            //绘制地形chunk的位置图
            this.DrawChunkRectangle(_terrain);

            List<int> hitBlockBounds = new List<int>();
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            var blockPoint = GetHitBlockMapPoint(mouseRay);
            //绘制鼠标所指的Block
            Handles.BeginGUI();
            OnSceneViewGUI();
            GUI.Label(new Rect(Event.current.mousePosition, new Vector2(100, 30)), blockPoint.ToString());
            Handles.EndGUI();

            //绘制地图碰撞盒
            var mapBounds = this.GetMapBounds(_terrain);
            //判断鼠标是不是在地图内
            if (mapBounds.IntersectRay(mouseRay))
            {
                this.DrawMapBounds(mapBounds);
                isMouseIn = true;
            }
            else
            {
                isMouseIn = false;
            }

            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0 && isMouseIn)
                {
                    Debug.Log("Mouse Left Button Click! " + blockPoint.ToString());
                    this.SetBlockOnPoint(blockPoint,1);
                }
                else if (Event.current.button == 1 && isMouseIn)
                {
                    this.SetBlockOnPoint(blockPoint, 0);
                    Debug.Log("Mouse Right Button Click! " + blockPoint.ToString());
                }
            }
        }

        #region SenceView相关函数

        public void OnSceneViewGUI()
        {
            GUILayout.Label(layer.ToString());
            GUILayout.BeginVertical(GUILayout.Width(20),GUILayout.Height(200));
            if (GUILayout.Button("+"))
            {
                if (layer < 255)
                {
                    layer++;
                }
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
           layer=(int) GUILayout.VerticalSlider(layer, 255, 0);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("-"))
            {
                if (layer >0)
                {
                    layer--;
                }
            }
            GUILayout.EndVertical();
        }
        public void SetBlockOnPoint(Vector3Int point,byte blockID)
        {
            this._terrain.SetBlockByMapPoint(point.x, point.y, point.z, blockID);
            EditorCoroutineUtility.StartCoroutine(this._terrain.UpdateTerrainAsyn(),this);
        }
        /// <summary>
        /// 获取鼠标所在block的地图点坐标
        /// </summary>
        /// <param name="mouseRay">鼠标射线</param>
        /// <returns></returns>
        public Vector3Int GetHitBlockMapPoint(Ray mouseRay)
        {
            var point = new Vector3Int();
            //1、创建chunk的bounds，判断鼠标在哪个chunk
            List<ChunkBounds> chunkBounds = this.CreateChunkBounds(_terrain);
            List<int> hitChunkBounds = new List<int>();
            for (int i = 0; i < chunkBounds.Count; i++)
            {

                if (chunkBounds[i].bounds.IntersectRay(mouseRay))
                {
                    hitChunkBounds.Add(i);
                }
            }
            if (hitChunkBounds.Count > 0)
            {
                //                Debug.Log("hit chunk!");
                float distance = 10000000;
                int activeID = -1;
                for (int i = 0; i < hitChunkBounds.Count; i++)
                {
                    var index = hitChunkBounds[i];
                    float tempDistance = (mouseRay.origin - chunkBounds[index].bounds.center).magnitude;
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                        activeID = index;
                    }

                }
                var hitChunk = chunkBounds[activeID];
                this.DrawHitChunkBounds(hitChunk);
                var blocksBounds = this.CreateActiveChunkBlocksBounds(hitChunk);
                var hitBlock = this.CheckHitBlockBounds(mouseRay, blocksBounds);
                if (hitBlock != null)
                {
                    this.DrawHitBlockBounds(hitBlock);
                    return new Vector3Int(hitChunk.x * 16 + hitBlock.x, hitBlock.y, hitChunk.z * 16 + hitBlock.z);
                }

            }
            //2、创建所在chunk的block的bounds，判断鼠标在哪个block
            //3、转换为地图点坐标
            return new Vector3Int(-1, -1, -1);

        }

        private List<BlockBounds> CreateActiveChunkBlocksBounds(ChunkBounds activeChunkBounds)
        {
            var start = activeChunkBounds.bounds.center;
            start.x = start.x - 8;
            start.z = start.z - 8;

            var blocksBounds = new List<BlockBounds>();
            for (int j = 0; j < 16; j++)
            {
                for (int i = 0; i < 16; i++)
                {
                    var b = new Bounds();
                    b.center = new Vector3(start.x + i + 0.5f, start.y, start.z + j + 0.5f);
                    b.size = new Vector3(1, 1, 1);
                    blocksBounds.Add(new BlockBounds(i, 0, j, b));
                }
            }
            return blocksBounds;
        }

        public void DrawHitBlockBounds(BlockBounds b)
        {
            var c = Handles.color;
            Handles.color = Color.white;
            Handles.DrawWireCube(b.bounds.center, b.bounds.size);
            Handles.color = c;

        }

        public BlockBounds CheckHitBlockBounds(Ray mouseRay, List<BlockBounds> blocksBounds)
        {
            for (int i = 0; i < blocksBounds.Count; i++)
            {
                var b = blocksBounds[i].bounds;
                if (b.IntersectRay(mouseRay))
                {
                    return blocksBounds[i];
                }
            }
            return null;
        }



        private void DrawHitChunkBounds(ChunkBounds cb)
        {
            var c = Handles.color;
            Handles.color = Color.green;
            Handles.DrawWireCube(cb.bounds.center, cb.bounds.size);
            Handles.color = c;
        }

        private void DrawMapBounds(Bounds b)
        {
            var c = Handles.color;
            Handles.color = Color.yellow;
            Handles.DrawWireCube(b.center, b.size);
            Handles.color = c;
        }

        private Bounds GetMapBounds(BlockTerrain blockTerrain)
        {
            var pos = blockTerrain.gameObject.transform.position;

            var center = new Vector3(pos.x + blockTerrain.width / 2 * 16, 0.5f, pos.z + blockTerrain.depth / 2 * 16);
            var size = new Vector3(blockTerrain.width * 16, 1, blockTerrain.depth * 16);
            Bounds b = new Bounds(center, size);
            return b;
        }

        private List<ChunkBounds> CreateChunkBounds(BlockTerrain blockTerrain)
        {
            var chunksBounds = new List<ChunkBounds>();
            var pos = blockTerrain.gameObject.transform.position;

            for (int j = 0; j < blockTerrain.depth; j++)
            {
                for (int i = 0; i < blockTerrain.width; i++)
                {
                    Bounds b = new Bounds();
                    b.center = new Vector3(pos.x + i * 16 + 8,
                        pos.y + 0.5f,
                        pos.z + j * 16 + 8);
                    //一层的高度，一个chunk的宽度和深度
                    b.size = new Vector3(16, 1, 16);
                    chunksBounds.Add(new ChunkBounds(i, j, b));
                }

            }
            return chunksBounds;
        }

        private void DrawChunkRectangle(BlockTerrain blockTerrain)
        {
            var pos = blockTerrain.gameObject.transform.position;
            for (int j = 0; j < blockTerrain.depth; j++)
            {
                for (int i = 0; i < blockTerrain.width; i++)
                {
                    Vector3[] verts = new Vector3[]
                    {
                        new Vector3(pos.x + i * 16, pos.y, pos.z + j * 16),
                        new Vector3(pos.x + (i + 1) * 16, pos.y, pos.z + j * 16),
                        new Vector3(pos.x + (i + 1) * 16, pos.y, pos.z + (j + 1) * 16),
                        new Vector3(pos.x + i * 16, pos.y, pos.z + (j + 1) * 16),
                    };
                    if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.1f),
                            new Color(1f, 1f, 1f, 0.2f));
                    }
                    else
                    {
                        Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.01f),
                            new Color(1f, 1f, 1f, 0.2f));
                    }
//                        Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.1f), new Color(0f, 1f, 0f, 1f));
                }
            }
        }

        private void DrawMapRectangle(BlockTerrain blockTerrain)
        {
            var pos = blockTerrain.gameObject.transform.position;
            Vector3[] verts = new Vector3[]
            {
                new Vector3(pos.x, pos.y, pos.z),
                new Vector3(pos.x + blockTerrain.width * 16, pos.y, pos.z),
                new Vector3(pos.x + blockTerrain.width * 16, pos.y, pos.z + blockTerrain.depth * 16),
                new Vector3(pos.x, pos.y, pos.z + blockTerrain.depth * 16),

            };
            Handles.DrawSolidRectangleWithOutline(verts, new Color(1f, 1f, 1f, 0.1f), new Color(0f, 1f, 0f, 1f));
        }

        private void DrawMapCube(BlockTerrain blockTerrain)
        {

            var pos = blockTerrain.gameObject.transform.position;
            var center = new Vector3();
            center.x = pos.x + blockTerrain.width / 2 * 16;
            center.y = pos.y;
            center.z = pos.z + blockTerrain.depth / 2 * 16;
            Handles.DrawWireCube(center, new Vector3(blockTerrain.width * 16, 1, blockTerrain.depth * 16));
        }

        /// <summary>
        /// 为每个chunk绘制一个白色的线框
        /// </summary>
        /// <param name="blockTerrain"></param>
        private void DrawChunkCube(BlockTerrain blockTerrain)
        {
            var pos = blockTerrain.gameObject.transform.position;
            for (int i = 0; i < blockTerrain.width; i++)
            {
                for (int j = 0; j < blockTerrain.depth; j++)
                {
                    var center = new Vector3();
                    center.x = pos.x + i * 16 + 8;
                    center.y = pos.y;
                    center.z = pos.z + j * 16 + 8;
                    Handles.DrawWireCube(center, new Vector3(16, 16, 16));
                }
            }
        }

        #endregion

        #region MapData操作函数

        /// <summary>
        /// 获取地图某个Block的数据
        /// </summary>
        /// <param name="x">地图X</param>
        /// <param name="y">地图Y</param>
        /// <param name="z">地图Z</param>
        /// <returns>byte类型数据</returns>
        public byte GetBlockFromData(int x, int y, int z)
        {
            Vector2Int chunkIndex = GetChunkIndexFromMapPoint(x, z);
            Vector3Int chunkPoint = GetChunkPointFromMapPoint(x, y, z);
            return _terrainData.chunkDatas[chunkIndex.x][chunkIndex.y][chunkPoint.x, chunkPoint.y, chunkPoint.z];
        }

        /// <summary>
        /// 设置地图某个Block的数据
        /// </summary>
        /// <param name="x">地图X</param>
        /// <param name="y">地图Y</param>
        /// <param name="z">地图Z</param>
        /// <param name="blockData">数据</param>
        public void SetBlockToData(int x, int y, int z, byte blockData)
        {
            Vector2Int chunkIndex = GetChunkIndexFromMapPoint(x, z);
            Vector3Int chunkPoint = GetChunkPointFromMapPoint(x, y, z);
            _terrainData.chunkDatas[chunkIndex.x][chunkIndex.y][chunkPoint.x, chunkPoint.y, chunkPoint.z] = blockData;
        }

        /// <summary>
        /// 将地图坐标转换为Chunk的数组索引
        /// </summary>
        /// <param name="mapX">地图X</param>
        /// <param name="mapZ">地图Z</param>
        /// <returns></returns>
        private Vector2Int GetChunkIndexFromMapPoint(int mapX, int mapZ)
        {
            int x = Mathf.FloorToInt(mapX / _terrainData.width);
            int z = Mathf.FloorToInt(mapZ / _terrainData.depth);
            return new Vector2Int(x, z);
        }

        /// <summary>
        /// 将地图坐标转换为在Chunk中的局部坐标
        /// </summary>
        /// <param name="mapX">地图X</param>
        /// <param name="mapY">地图Y</param>
        /// <param name="mapZ">地图Z</param>
        /// <returns></returns>
        private Vector3Int GetChunkPointFromMapPoint(int mapX, int mapY, int mapZ)
        {
            int x = mapX % _terrainData.width;
            int z = mapZ % _terrainData.depth;
            return new Vector3Int(x, mapY, z);
        }

        #endregion

        public override void OnInspectorGUI()
        {
//            base.OnInspectorGUI();
            BlockTerrain blockTerrain = (BlockTerrain) target;
            var data = blockTerrain.data;
            data = EditorGUILayout.ObjectField("Data", data, data.GetType()) as BlockTerrainData;
            data.name = EditorGUILayout.TextField("Name", data.name);
            EditorGUILayout.TextField("Version", data.version);
            data.width = EditorGUILayout.IntField("Wdith(X)", data.width);
            data.height = EditorGUILayout.IntField("Height(Y)", data.height);
            data.depth = EditorGUILayout.IntField("Depth(Z)", data.depth);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("创建地形",(GUIStyle)"ButtonLeft"))
            {
                //创建map的chunks
                MyTools.DeleteAllChildren(blockTerrain.transform);
                InitTerrainData();
                EditorCoroutineUtility.StartCoroutine(blockTerrain.LoadTerrainAsyn(), this);
            }
            if (GUILayout.Button("清空Mesh缓存",(GUIStyle)"ButtonMid"))
            {
                //创建map的chunks
                MyTools.DeleteAllChildren(blockTerrain.transform);
            }
            if (GUILayout.Button("更新Mesh", (GUIStyle)"ButtonRight"))
            {
                //blockTerrain.CreateRandomMap();
            }
            GUILayout.EndHorizontal();
            selectPanelIndex = GUILayout.Toolbar(selectPanelIndex, panelNames);
            if (panelNames[selectPanelIndex] == "笔刷")
            {
                this.DrawBrushesPanel(data);
            }
            else if (panelNames[selectPanelIndex] == "图块")
            {
                this.DrawBlocksPanel(data);
            }
            else if (panelNames[selectPanelIndex] == "图层")
            {
                this.DrawLayersPanel(data);
            }
            else if (panelNames[selectPanelIndex] == "设置")
            {
            }
        }

        #region OnInspectorGUI相关函数

        public void InitTerrainData()
        {
            _terrainData.InitChunksData();
        }
        /// <summary>
        /// 绘制笔刷页面
        /// </summary>
        /// <param name="data"></param>
        public void DrawBrushesPanel(BlockTerrainData data)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            activeBrushIndex= GUILayout.Toolbar(activeBrushIndex, brushNames);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            //根据block定义，刷新block类型
            this.blockTypes.Clear();
            for (int i = 0; i < data.blockDefinitions.Count; i++)
            {
                this.blockTypes.Add(new GUIContent(data.blockDefinitions[i].name));
            }
            selectBlockTypeIndex = GUILayout.SelectionGrid(selectBlockTypeIndex, blockTypes.ToArray(), 5);
        }

        /// <summary>
        /// 绘制Layers页面
        /// </summary>
        /// <param name="data"></param>
        public void DrawLayersPanel(BlockTerrainData data)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MyGUITools.SetBackgroundColor(Color.green);
            if (GUILayout.Button("Add New Layer"))
            {
                BlockTerrainLayer layer = new BlockTerrainLayer(0, 255);
                data.layers.Add(layer);

            }
            MyGUITools.RestoreBackgroundColor();
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < data.layers.Count; i++)
            {
                var layer = data.layers[i];
                if (i == selectLayerDefIndex)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical((GUIStyle)"MeTransitionSelect",GUILayout.Height(100));
                    GUILayout.Toggle(false, string.Format("{0}:Start:{1}-End:{2}", i, layer.start, layer.end),
                        (GUIStyle)"MeTransitionSelectHead", GUILayout.Height(30));
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Start");
                    layer.start = EditorGUILayout.IntField(layer.start);
                    GUILayout.Label("End  ");
                    layer.end = EditorGUILayout.IntField(layer.end);
                    GUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    MyGUITools.SetBackgroundColor(Color.green);
                    if (GUILayout.Button("Add New Item"))
                    {
                        BlockTerrainLayerItem newItem = new BlockTerrainLayerItem();
                        layer.items.Add(newItem);
                    }
                    MyGUITools.RestoreBackgroundColor();
                    EditorGUILayout.EndHorizontal();
                    for (int j = 0; j < layer.items.Count; j++)
                    {
                        var item = layer.items[j];
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Label("Block");
                        item.blockId = EditorGUILayout.IntPopup(item.blockId, GetBlockNames(data), GetBlockIDs(data));
                        GUILayout.Label("Weight ");
                        item.weight = EditorGUILayout.FloatField(item.weight);
                        MyGUITools.SetBackgroundColor(Color.red);
                        if (GUILayout.Button("Del"))
                        {
                            layer.items.RemoveAt(j);
                        }
                        MyGUITools.RestoreBackgroundColor();
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                else
                {
                    var flag = GUILayout.Toggle(false, string.Format("{0}:Start:{1}-End:{2}", i, layer.start, layer.end),
                        (GUIStyle) "OL Title");
                    if (flag == true)
                    {
                        selectLayerDefIndex = i;
                    }
                }
            }
        }

        public int[] GetBlockIDs(BlockTerrainData data)
        {
            List<int> ids = new List<int>();
            foreach (var d in data.blockDefinitions)
            {
                ids.Add(d.id);
            }
            return ids.ToArray();
        }

        public string[] GetBlockNames(BlockTerrainData data)
        {
            List<string> names = new List<string>();
            foreach (var d in data.blockDefinitions)
            {
                names.Add(d.name);
            }
            return names.ToArray();
        }

        /// <summary>
        /// 绘制Blocks页面
        /// </summary>
        /// <param name="data"></param>
        public void DrawBlocksPanel(BlockTerrainData data)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            MyGUITools.SetBackgroundColor(Color.green);
            if (GUILayout.Button("Add New Block Definition"))
            {
                BlockDefinition newdef = new BlockDefinition();
                newdef.id = data.blockDefinitions.Count;
                data.blockDefinitions.Add(newdef);
            }
            MyGUITools.RestoreBackgroundColor();
            GUILayout.EndHorizontal();
            //            EditorGUI.indentLevel++;
            GUILayout.Label("Block Count(" + data.blockDefinitions.Count + ")");

            var contents = new List<GUIContent>();
            for (int i = 0; i < data.blockDefinitions.Count; i++)
            {
                var def = data.blockDefinitions[i];
                if (i == selectBlockDefIndex)
                {
                    EditorGUILayout.Space();
                    EditorGUILayout.BeginVertical((GUIStyle)"MeTransitionSelect", GUILayout.Height(200));
                    GUILayout.Toggle(true, string.Format("ID:{0},Name:{1}", def.id, def.name),
                        (GUIStyle)"MeTransitionSelectHead",GUILayout.Height(30));
                    def = data.blockDefinitions[i];
                    if (def is SpriteBlockDefinition)
                    {
                         var sprDef = def as SpriteBlockDefinition;

                        sprDef.id = EditorGUILayout.IntField("ID", sprDef.id);
                        sprDef.name = EditorGUILayout.TextField("Name", sprDef.name);
                        //贴图设置
                        EditorGUILayout.PrefixLabel("CreateMode");
                        blockCreateModeIndex = GUILayout.Toolbar(blockCreateModeIndex, blockCreateModes);
                        if (blockCreateModeIndex == 0)
                        {
                            var temp = EditorGUILayout.ObjectField("All Face", sprDef.top, typeof(Sprite), false) as Sprite;
                            if (temp != null && temp != sprDef.top)
                            {
                                sprDef.top = temp;
                                sprDef.bottom = sprDef.front = sprDef.back = sprDef.left = sprDef.right = sprDef.top;
                            }
                        }
                        else if (blockCreateModeIndex == 1)
                        {
                            sprDef.top = EditorGUILayout.ObjectField("Top Face", sprDef.top, typeof(Sprite), false) as Sprite;
                            var temp =
                                EditorGUILayout.ObjectField("Other Face", sprDef.bottom, typeof(Sprite), false) as Sprite;
                            if (temp != null && temp != sprDef.bottom)
                            {
                                sprDef.bottom = temp;
                                sprDef.front = sprDef.back = sprDef.left = sprDef.right = sprDef.bottom;
                            }
                        }
                        else if (blockCreateModeIndex == 2)
                        {
                            sprDef.top = EditorGUILayout.ObjectField("Top Face", sprDef.top, typeof(Sprite), false) as Sprite;
                            sprDef.bottom =
                                EditorGUILayout.ObjectField("Bottom Face", sprDef.bottom, typeof(Sprite), false) as Sprite;
                            var temp =
                                EditorGUILayout.ObjectField("Other Face", sprDef.front, typeof(Sprite), false) as Sprite;
                            if (temp != null && temp != sprDef.front)
                            {
                                sprDef.front = temp;
                                sprDef.back = sprDef.left = sprDef.right = sprDef.front;
                            }
                        }
                        else if (blockCreateModeIndex == 3)
                        {
                            sprDef.top = EditorGUILayout.ObjectField("Top Face", sprDef.top, typeof(Sprite), false) as Sprite;
                            sprDef.bottom =
                                EditorGUILayout.ObjectField("Bottom Face", sprDef.bottom, typeof(Sprite), false) as Sprite;
                            sprDef.front =
                                EditorGUILayout.ObjectField("Front Face", sprDef.front, typeof(Sprite), false) as Sprite;
                            sprDef.back =
                                EditorGUILayout.ObjectField("Back Face", sprDef.back, typeof(Sprite), false) as Sprite;
                            sprDef.left =
                                EditorGUILayout.ObjectField("Left Face", sprDef.left, typeof(Sprite), false) as Sprite;
                            sprDef.right =
                                EditorGUILayout.ObjectField("Right Face", sprDef.right, typeof(Sprite), false) as Sprite;
                        }
                    }
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    MyGUITools.SetBackgroundColor(Color.red);
                    if (GUILayout.Button("Del", GUILayout.MinWidth(80)))
                    {
                        data.blockDefinitions.RemoveAt(selectBlockDefIndex);
                    }
                    MyGUITools.RestoreBackgroundColor();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                else
                {
                    var flag = GUILayout.Toggle(false, string.Format("ID:{0},Name:{1}", def.id, def.name),
                        (GUIStyle) "OL Title");
                    if (flag == true)
                    {
                        selectBlockDefIndex = i;
                    }
                }
            }
        }

        #endregion
    } //end class

    public class ChunkBounds
    {
        public int x;
        public int z;
        public Bounds bounds;

        public ChunkBounds()
        {
            bounds = new Bounds();
        }

        public ChunkBounds(int setX, int setZ, Bounds setBounds)
        {
            x = setX;
            z = setZ;
            bounds = setBounds;
        }
    }

    public class BlockBounds
    {
        public int x;
        public int y;
        public int z;
        public Bounds bounds;

        public BlockBounds()
        {
            bounds = new Bounds();
        }

        public BlockBounds(int setX, int setY, int setZ, Bounds setBounds)
        {
            x = setX;
            y = setY;
            z = setZ;
            bounds = setBounds;
        }
    }
}