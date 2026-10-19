using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-only procedural layout tool for Aisle Be Back.
/// Creates a replaceable store layout and never deletes scene objects outside its own generated root.
/// </summary>
public class AisleBeBackStoreGenerator : EditorWindow
{
    private const string AssetRoot = "Assets/ModernSupermarket/Prefabs";
    private const string GeneratedRootName = "GENERATED - Aisle Be Back Store";

    [Header("Store Layout")]
    [SerializeField] private int aisleRows = 4;
    [SerializeField] private int shelfUnitsPerRow = 7;
    [SerializeField] private float shelfSpacing = 3.0f;
    [SerializeField] private float aisleSpacing = 6.5f;
    [SerializeField] private float storeWidth = 42f;
    [SerializeField] private float storeLength = 52f;
    [SerializeField] private float wallHeight = 6f;
    [SerializeField] private float wallThickness = 0.5f;

    [Header("Asset Prefabs (optional; auto-detected on first open)")]
    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject shelfPrefab;
    [SerializeField] private GameObject checkoutPrefab;
    [SerializeField] private GameObject productPrefab;
    [SerializeField] private GameObject producePrefab;
    [SerializeField] private GameObject signPrefab;

    [Header("Shelf Stocking")]
    [SerializeField] private int productsPerShelfLevel = 3;
    [SerializeField] private int shelfLevels = 4;
    [SerializeField] private bool stockBothSides = true;
    [SerializeField] private bool addProducts = true;

    [Header("Details")]
    [SerializeField] private bool addCheckoutArea = true;
    [SerializeField] private bool addCeiling = false;
    [SerializeField] private int randomSeed = 25;

    private List<GameObject> shelfOptions = new List<GameObject>();
    private List<GameObject> refrigerationOptions = new List<GameObject>();
    private List<GameObject> displayOptions = new List<GameObject>();
    private List<GameObject> aisleSignOptions = new List<GameObject>();
    private List<GameObject> productOptions = new List<GameObject>();
    private List<List<GameObject>> productCategories = new List<List<GameObject>>();
    private const string ProductPrefabRoot = "Assets/ModernSupermarket/Prefabs/Props/Products";
    private const string ProducePrefabRoot = "Assets/ModernSupermarket/Prefabs/Props/Produce";

    [MenuItem("Tools/Aisle Be Back/Store Generator")]
    public static void Open()
    {
        var window = GetWindow<AisleBeBackStoreGenerator>("Store Generator");
        window.minSize = new Vector2(370, 580);
        window.TryAutoAssignAssets();
    }

    private void OnEnable()
    {
        TryAutoAssignAssets();
    }

    private void OnGUI()
    {
        EditorGUILayout.HelpBox(
            "Builds a blockout supermarket using the ModernSupermarket prefab library. " +
            "Generate again to replace only the previous generated store. Existing scene objects are left alone.",
            MessageType.Info);

        EditorGUILayout.LabelField("Store Dimensions", EditorStyles.boldLabel);
        aisleRows = Mathf.Clamp(EditorGUILayout.IntField("Aisle Rows", aisleRows), 1, 12);
        shelfUnitsPerRow = Mathf.Clamp(EditorGUILayout.IntField("Shelf Units Per Row", shelfUnitsPerRow), 1, 20);
        shelfSpacing = Mathf.Max(0.5f, EditorGUILayout.FloatField("Shelf Unit Spacing", shelfSpacing));
        aisleSpacing = Mathf.Max(2f, EditorGUILayout.FloatField("Aisle Spacing", aisleSpacing));
        storeWidth = Mathf.Max(15f, EditorGUILayout.FloatField("Store Width", storeWidth));
        storeLength = Mathf.Max(20f, EditorGUILayout.FloatField("Store Length", storeLength));
        wallHeight = Mathf.Max(2f, EditorGUILayout.FloatField("Wall Height", wallHeight));
        wallThickness = Mathf.Max(0.1f, EditorGUILayout.FloatField("Wall Thickness", wallThickness));

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Prefab Assignments", EditorStyles.boldLabel);
        floorPrefab = (GameObject)EditorGUILayout.ObjectField("Floor", floorPrefab, typeof(GameObject), false);
        shelfPrefab = (GameObject)EditorGUILayout.ObjectField("Shelf / Rack", shelfPrefab, typeof(GameObject), false);
        checkoutPrefab = (GameObject)EditorGUILayout.ObjectField("Checkout", checkoutPrefab, typeof(GameObject), false);
        productPrefab = (GameObject)EditorGUILayout.ObjectField("Product", productPrefab, typeof(GameObject), false);
        producePrefab = (GameObject)EditorGUILayout.ObjectField("Produce", producePrefab, typeof(GameObject), false);
        signPrefab = (GameObject)EditorGUILayout.ObjectField("Sign", signPrefab, typeof(GameObject), false);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Generation Options", EditorStyles.boldLabel);
        addProducts = EditorGUILayout.Toggle("Stock Shelves", addProducts);
        productsPerShelfLevel = Mathf.Clamp(EditorGUILayout.IntField("Products Across Each Shelf Level", productsPerShelfLevel), 1, 10);
        shelfLevels = Mathf.Clamp(EditorGUILayout.IntField("Stocked Levels Per Shelf", shelfLevels), 1, 6);
        stockBothSides = EditorGUILayout.Toggle("Stock Both Sides", stockBothSides);
        addCheckoutArea = EditorGUILayout.Toggle("Add Checkout Area", addCheckoutArea);
        addCeiling = EditorGUILayout.Toggle("Add Ceiling", addCeiling);
        randomSeed = EditorGUILayout.IntField("Random Seed", randomSeed);

        EditorGUILayout.Space(10);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Auto-Find Assets", GUILayout.Height(30)))
                TryAutoAssignAssets();
            if (GUILayout.Button("Generate / Replace Store", GUILayout.Height(30)))
                GenerateStore();
        }

        EditorGUILayout.HelpBox(
            "Uses all detected shelf variants, a wider aisle plan, refrigerated wall units, specialty displays, " +
            "named department signs, and checkout lanes. Generate again to replace only the previous generated store.",
            MessageType.None);
    }

    private void TryAutoAssignAssets()
    {
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { AssetRoot });
        var prefabs = guids.Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(p => p != null).ToList();

        if (prefabs.Count == 0)
            return;

        if (floorPrefab == null)
            floorPrefab = FindByName(prefabs, "floor");
        string[] furnitureGuids = AssetDatabase.FindAssets("t:Prefab", new[] { AssetRoot + "/Furniture" });
        var furniture = furnitureGuids
            .Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(p => p != null).ToList();

        shelfOptions = furniture
            .Where(p => p.name.StartsWith("Shelf_", System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.name).ToList();
        refrigerationOptions = furniture
            .Where(p => p.name.StartsWith("Fridge_", System.StringComparison.OrdinalIgnoreCase) ||
                        p.name.StartsWith("Freezer_", System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.name).ToList();
        displayOptions = furniture
            .Where(p => p.name.StartsWith("Stall_", System.StringComparison.OrdinalIgnoreCase) ||
                        p.name.StartsWith("HeritageStall_", System.StringComparison.OrdinalIgnoreCase) ||
                        p.name.StartsWith("Table_", System.StringComparison.OrdinalIgnoreCase) ||
                        p.name.StartsWith("HeritageBuffet_", System.StringComparison.OrdinalIgnoreCase) ||
                        p.name.StartsWith("Counter_", System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.name).ToList();
        aisleSignOptions = furniture
            .Where(p => p.name.StartsWith("Sign_04h2fw_", System.StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.name).ToList();

        if (shelfPrefab == null)
            shelfPrefab = shelfOptions.FirstOrDefault() ?? FindByName(prefabs, "shelf", "rack", "gondola");
        if (checkoutPrefab == null)
            checkoutPrefab = FindByName(prefabs, "checkout", "cashier", "cash_register", "register");
        if (productPrefab == null)
            productPrefab = FindByName(prefabs, "product", "box", "can", "bottle");
        if (producePrefab == null)
            producePrefab = FindByName(prefabs, "produce", "fruit", "vegetable");
        if (signPrefab == null)
            signPrefab = FindByName(prefabs, "sign");

        // Product prefabs live in dedicated folders. Search those folders directly so the
        // generator uses the actual packaged groceries instead of one generic prefab.
        string[] productGuids = AssetDatabase.FindAssets("t:Prefab", new[] { ProductPrefabRoot, ProducePrefabRoot });
        productOptions = productGuids
            .Select(g => AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(g)))
            .Where(p => p != null && !IsDisplayOrFixture(p.name))
            .Distinct()
            .OrderBy(p => p.name)
            .ToList();

        productCategories = productOptions
            .GroupBy(p => GetProductCategory(p.name))
            .OrderBy(g => g.Key)
            .Select(g => g.ToList())
            .ToList();

        if (productPrefab == null && productOptions.Count > 0)
            productPrefab = productOptions[0];
        if (producePrefab == null)
        {
            string[] produceGuids = AssetDatabase.FindAssets("t:Prefab", new[] { ProducePrefabRoot });
            if (produceGuids.Length > 0)
                producePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(produceGuids[0]));
        }

        Repaint();
    }

    private static GameObject FindByName(List<GameObject> prefabs, params string[] keywords)
    {
        foreach (string keyword in keywords)
        {
            var match = prefabs.FirstOrDefault(p => p.name.ToLowerInvariant().Contains(keyword));
            if (match != null) return match;
        }
        return null;
    }

    private void GenerateStore()
    {
        if (shelfPrefab == null)
        {
            EditorUtility.DisplayDialog("Shelf prefab needed",
                "Auto-Find could not identify a shelf/rack prefab. Drag one from Assets/ModernSupermarket/Prefabs into the Shelf / Rack field, then generate again.",
                "OK");
            return;
        }

        var oldRoot = GameObject.Find(GeneratedRootName);
        if (oldRoot != null && !EditorUtility.DisplayDialog("Replace generated store?",
            "This deletes and rebuilds only '" + GeneratedRootName + "'. Other scene objects will not be touched.",
            "Replace", "Cancel"))
            return;

        if (oldRoot != null)
            Undo.DestroyObjectImmediate(oldRoot);

        Random.InitState(randomSeed);
        var root = new GameObject(GeneratedRootName);
        Undo.RegisterCreatedObjectUndo(root, "Generate Aisle Be Back Store");

        Transform architecture = CreateGroup(root.transform, "Architecture");
        Transform shelves = CreateGroup(root.transform, "Aisle Shelving");
        Transform products = CreateGroup(root.transform, "Products");
        Transform checkout = CreateGroup(root.transform, "Checkout Area");
        Transform signs = CreateGroup(root.transform, "Signs");

        CreateFloor(architecture);
        CreateWalls(architecture);

        // A real store has fewer, longer aisle runs and distinct departments—not a dense grid.
        float totalRowsWidth = (aisleRows - 1) * aisleSpacing;
        float startX = -totalRowsWidth / 2f;
        float rowLength = Mathf.Min((shelfUnitsPerRow - 1) * shelfSpacing + 2.4f, storeLength - 17f);
        float startZ = -rowLength / 2f + 2.5f;

        for (int row = 0; row < aisleRows; row++)
        {
            float x = startX + row * aisleSpacing;
            for (int unit = 0; unit < shelfUnitsPerRow; unit++)
            {
                float z = startZ + unit * shelfSpacing;
                GameObject chosenShelf = shelfOptions.Count > 0
                    ? shelfOptions[(row + unit) % shelfOptions.Count]
                    : shelfPrefab;
                float yaw = row % 2 == 0 ? 0f : 180f;
                var shelf = InstantiatePrefab(chosenShelf, new Vector3(x, 0f, z),
                    Quaternion.Euler(0f, yaw, 0f), shelves,
                    "Aisle " + (row + 1) + " Shelf " + (unit + 1));
                if (shelf != null && addProducts)
                    PlaceProductsOnShelf(shelf.transform, products, row, unit);
            }
        }

        CreateStoreDepartments(root.transform, shelves, signs);

        if (addCheckoutArea)
            CreateCheckout(checkout, signs, root.transform);

        if (addCeiling)
            CreateCeiling(architecture);

        Selection.activeGameObject = root.gameObject;
        EditorSceneManagerMarkDirty();
        Debug.Log("Aisle Be Back store generated. Rows: " + aisleRows + ", shelf units: " + (aisleRows * shelfUnitsPerRow) + ".");
    }

    private void CreateFloor(Transform parent)
    {
        if (floorPrefab != null)
        {
            var floor = InstantiatePrefab(floorPrefab, Vector3.zero, Quaternion.identity, parent, "Store Floor");
            if (floor != null)
            {
                var bounds = GetCombinedBounds(floor);
                if (bounds.size.x > 0.01f && bounds.size.z > 0.01f)
                    floor.transform.localScale = Vector3.Scale(floor.transform.localScale,
                        new Vector3(storeWidth / bounds.size.x, 1f, storeLength / bounds.size.z));
            }
        }
        else
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Store Floor (generated)";
            floor.transform.SetParent(parent);
            floor.transform.position = new Vector3(0f, -0.25f, 0f);
            floor.transform.localScale = new Vector3(storeWidth, 0.5f, storeLength);
            Undo.RegisterCreatedObjectUndo(floor, "Create Store Floor");
        }
    }

    private void CreateWalls(Transform parent)
    {
        MakeCube(parent, "Back Wall", new Vector3(0f, wallHeight / 2f, storeLength / 2f), new Vector3(storeWidth, wallHeight, wallThickness));
        MakeCube(parent, "Left Wall", new Vector3(-storeWidth / 2f, wallHeight / 2f, 0f), new Vector3(wallThickness, wallHeight, storeLength));
        MakeCube(parent, "Right Wall", new Vector3(storeWidth / 2f, wallHeight / 2f, 0f), new Vector3(wallThickness, wallHeight, storeLength));
        // Front wall is split to leave an entrance in the center.
        float sideWidth = Mathf.Max(2f, (storeWidth - 8f) / 2f);
        MakeCube(parent, "Front Wall Left", new Vector3(-(4f + sideWidth / 2f), wallHeight / 2f, -storeLength / 2f), new Vector3(sideWidth, wallHeight, wallThickness));
        MakeCube(parent, "Front Wall Right", new Vector3(4f + sideWidth / 2f, wallHeight / 2f, -storeLength / 2f), new Vector3(sideWidth, wallHeight, wallThickness));
    }

    private void CreateCheckout(Transform parent, Transform signs, Transform root)
    {
        float z = -storeLength / 2f + 7f;
        int count = 3;
        for (int i = 0; i < count; i++)
        {
            float x = (i - (count - 1) / 2f) * 4f;
            if (checkoutPrefab != null)
                InstantiatePrefab(checkoutPrefab, new Vector3(x, 0f, z), Quaternion.identity, parent, "Checkout " + (i + 1));
            else
                MakeCube(parent, "Checkout Counter " + (i + 1), new Vector3(x, 0.6f, z), new Vector3(2.5f, 1.2f, 1.2f));

            if (signPrefab != null)
                InstantiatePrefab(signPrefab, new Vector3(x, 2.2f, z + 0.5f), Quaternion.identity, signs, "Checkout Sign " + (i + 1));
        }
    }

    private void CreateStoreDepartments(Transform root, Transform fixtures, Transform signs)
    {
        // Refrigerated wall: alternate actual fridge/freezer assets along the back wall.
        if (refrigerationOptions.Count > 0)
        {
            int coldCount = Mathf.Min(6, Mathf.Max(4, refrigerationOptions.Count));
            for (int i = 0; i < coldCount; i++)
            {
                float x = -storeWidth * 0.36f + i * (storeWidth * 0.72f / Mathf.Max(1, coldCount - 1));
                GameObject prefab = refrigerationOptions[i % refrigerationOptions.Count];
                InstantiatePrefab(prefab, new Vector3(x, 0f, storeLength * 0.5f - 2.0f),
                    Quaternion.Euler(0f, 180f, 0f), fixtures, "Refrigerated Wall " + (i + 1));
            }
        }

        // Produce and specialty displays near the front, leaving the entrance approach clear.
        if (displayOptions.Count > 0)
        {
            int count = Mathf.Min(4, displayOptions.Count);
            for (int i = 0; i < count; i++)
            {
                float x = i % 2 == 0 ? -storeWidth * 0.34f : storeWidth * 0.34f;
                float z = -storeLength * 0.20f + (i / 2) * 5.0f;
                GameObject prefab = displayOptions[i % displayOptions.Count];
                InstantiatePrefab(prefab, new Vector3(x, 0f, z),
                    Quaternion.Euler(0f, i % 2 == 0 ? 90f : -90f, 0f),
                    fixtures, "Department Display " + (i + 1));
            }
        }

        // Named department signs from the asset pack.
        if (aisleSignOptions.Count > 0)
        {
            for (int row = 0; row < aisleRows; row++)
            {
                GameObject sign = aisleSignOptions[row % aisleSignOptions.Count];
                float x = -((aisleRows - 1) * aisleSpacing) / 2f + row * aisleSpacing;
                InstantiatePrefab(sign, new Vector3(x, wallHeight - 0.8f, -storeLength * 0.12f),
                    Quaternion.identity, signs, "Department Sign " + (row + 1));
            }
        }
    }

    private void CreateCeiling(Transform parent)
    {
        MakeCube(parent, "Ceiling", new Vector3(0f, wallHeight + 0.2f, 0f), new Vector3(storeWidth, 0.4f, storeLength));
    }

    private void PlaceProductsOnShelf(Transform shelf, Transform parent, int row, int unit)
    {
        if (productOptions.Count == 0 && productPrefab == null && producePrefab == null)
            return;

        Bounds shelfBounds = GetLocalBounds(shelf.gameObject);
        float width = Mathf.Max(0.6f, shelfBounds.size.x);
        float depth = Mathf.Max(0.25f, shelfBounds.size.z);
        float height = Mathf.Max(1.2f, shelfBounds.size.y);
        int across = Mathf.Max(1, productsPerShelfLevel);
        int faces = stockBothSides ? 2 : 1;
        float cellWidth = (width - 0.12f) / across;
        float levelSpacing = height / (shelfLevels + 1f);

        // Keep each aisle visually coherent: each row gets a product category, while
        // variants within that category provide natural packaging variety.
        List<GameObject> category = productCategories.Count > 0
            ? productCategories[row % productCategories.Count]
            : productOptions;

        for (int level = 0; level < shelfLevels; level++)
        {
            float shelfSurfaceY = shelfBounds.min.y + ((level + 1f) * height / (shelfLevels + 1f));
            for (int face = 0; face < faces; face++)
            {
                int faceSign = (stockBothSides && face == 1) ? -1 : 1;
                for (int slot = 0; slot < across; slot++)
                {
                    GameObject chosen = category.Count > 0
                        ? category[Random.Range(0, category.Count)]
                        : (producePrefab != null ? producePrefab : productPrefab);
                    if (chosen == null) continue;

                    Bounds itemBounds = GetLocalBounds(chosen);
                    Vector3 itemSize = itemBounds.size;
                    if (itemSize.x < 0.001f) itemSize.x = 0.25f;
                    if (itemSize.y < 0.001f) itemSize.y = 0.3f;
                    if (itemSize.z < 0.001f) itemSize.z = 0.2f;

                    // Scale oversized products down to fit their shelf slot, but keep
                    // small packages at their authored size so the shelves look full.
                    float faceDepth = Mathf.Max(0.18f, depth * 0.43f);
                    float scale = Mathf.Min(1f,
                        (cellWidth * 0.88f) / itemSize.x,
                        (faceDepth * 0.82f) / itemSize.z,
                        (levelSpacing * 0.78f) / itemSize.y);
                    scale = Mathf.Clamp(scale, 0.35f, 1f);

                    float localX = shelfBounds.min.x + 0.06f + cellWidth * (slot + 0.5f);
                    float localZ = faceSign * Mathf.Max(0.03f, shelfBounds.extents.z - itemSize.z * scale * 0.5f - 0.025f);
                    Vector3 shelfPoint = new Vector3(localX, shelfSurfaceY, localZ);
                    Quaternion rotation = shelf.rotation * Quaternion.Euler(0f, faceSign < 0 ? 180f : 0f, 0f);
                    Vector3 boundsOffset = new Vector3(itemBounds.center.x * scale, itemBounds.min.y * scale, itemBounds.center.z * scale);
                    Vector3 worldPosition = shelf.TransformPoint(shelfPoint) - rotation * boundsOffset;

                    // Avoid thousands of separate Undo records; the generated root is
                    // registered with Undo and owns all these generated instances.
                    GameObject item = (GameObject)PrefabUtility.InstantiatePrefab(chosen);
                    if (item == null) item = Instantiate(chosen);
                    item.name = "Stock R" + (row + 1) + " S" + (unit + 1) + " L" + (level + 1) + " F" + face + " P" + (slot + 1);
                    item.transform.SetParent(parent, true);
                    item.transform.SetPositionAndRotation(worldPosition, rotation);
                    item.transform.localScale = chosen.transform.localScale * scale;
                }
            }
        }
    }

    private static bool IsDisplayOrFixture(string prefabName)
    {
        string n = prefabName.ToLowerInvariant();
        return n.Contains("shelf") || n.Contains("rack") || n.Contains("display") ||
               n.Contains("checkout") || n.Contains("cashier") || n.Contains("register") ||
               n.Contains("basket") || n.Contains("shoppingcart") || n.Contains("shopping_cart");
    }

    private static string GetProductCategory(string prefabName)
    {
        int separator = prefabName.IndexOf('_');
        return separator > 0 ? prefabName.Substring(0, separator) : prefabName;
    }

    private static Bounds GetLocalBounds(GameObject target)
    {
        Renderer[] renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, new Vector3(0.25f, 0.3f, 0.2f));

        // Use each renderer's local bounds transformed into the target's local space.
        // Renderer.bounds is a world-space AABB and can be misleading for prefab assets
        // that have not been instantiated into the active scene.
        bool initialized = false;
        Bounds localBounds = new Bounds(Vector3.zero, Vector3.zero);
        foreach (Renderer renderer in renderers)
        {
            Bounds rendererBounds = renderer.localBounds;
            Vector3 min = rendererBounds.min;
            Vector3 max = rendererBounds.max;
            for (int corner = 0; corner < 8; corner++)
            {
                Vector3 rendererLocalCorner = new Vector3(
                    (corner & 1) == 0 ? min.x : max.x,
                    (corner & 2) == 0 ? min.y : max.y,
                    (corner & 4) == 0 ? min.z : max.z);
                Vector3 worldCorner = renderer.transform.TransformPoint(rendererLocalCorner);
                Vector3 localCorner = target.transform.InverseTransformPoint(worldCorner);
                if (!initialized)
                {
                    localBounds = new Bounds(localCorner, Vector3.zero);
                    initialized = true;
                }
                else
                    localBounds.Encapsulate(localCorner);
            }
        }
        return initialized ? localBounds : new Bounds(Vector3.zero, new Vector3(0.25f, 0.3f, 0.2f));
    }

    private static Transform CreateGroup(Transform parent, string name)
    {
        var group = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(group, "Create " + name);
        group.transform.SetParent(parent, false);
        return group.transform;
    }

    private static GameObject MakeCube(Transform parent, string name, Vector3 position, Vector3 scale)
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = name;
        cube.transform.SetParent(parent, true);
        cube.transform.position = position;
        cube.transform.localScale = scale;
        Undo.RegisterCreatedObjectUndo(cube, "Create " + name);
        return cube;
    }

    private static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent, string objectName)
    {
        if (prefab == null) return null;
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        if (instance == null) instance = Instantiate(prefab);
        Undo.RegisterCreatedObjectUndo(instance, "Place " + objectName);
        instance.name = objectName;
        instance.transform.SetParent(parent, true);
        instance.transform.SetPositionAndRotation(position, rotation);
        return instance;
    }

    private static Bounds GetCombinedBounds(GameObject target)
    {
        var renderers = target.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return new Bounds(target.transform.position, Vector3.one);
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        return bounds;
    }

    private static void EditorSceneManagerMarkDirty()
    {
        if (UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().IsValid())
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }
}