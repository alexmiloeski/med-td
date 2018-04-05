enum TileType { Blank, SSPoint, Path }

class TileEditor extends EditorWindow
{
	var selectedType : TileType = TileType.Blank;
	var selectedTile : GameObject;
	var selectedFolder : GameObject;
	var passiveSelect : boolean = true;
	var tilePrefab : Object;

	@MenuItem ("Window/TileEditor")
	static function Init()
	{
		print("init......");
		var window : TileEditor = EditorWindow.GetWindow (TileEditor);
		window.Show();
	}

	function OnSelectionChange ()
	{
		Repaint();

		if (passiveSelect == false)
			ChangeTile();
	}

	function OnGUI ()
	{    
		GUILayout.Label ("Selected tile: " + (Selection.activeGameObject != null ? Selection.activeGameObject.name : "None"), EditorStyles.boldLabel);
		GUILayout.Label ("Type to apply:", EditorStyles.boldLabel);
		selectedType = EditorGUILayout.EnumPopup(selectedType);

		passiveSelect = EditorGUILayout.BeginToggleGroup ("Manually Apply", passiveSelect);

		if (GUI.Button (Rect(10,85,60,20), "Apply"))
		{
			print("buttonclick");
			ChangeAllTiles();
			//ChangeTile();
		}

		EditorGUILayout.EndToggleGroup ();           
	}

	function ChangeAllTiles()
	{
		print("bleh");
		selectedFolder = Selection.activeGameObject;

		if (selectedFolder == null)
		{
			print("nothing selected");
			return;
		}

		switch (selectedType)
		{
			case selectedType.Blank:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Blank.prefab", Object);
			break;

			case selectedType.SSPoint:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/SSPoint.prefab", Object);
			break;
			
			case selectedType.Path:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Path.prefab", Object);
			break;

			default:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Blank.prefab", Object);
			break;
			/*
			case selectedType.Ground:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Outside.prefab", Object);
			break;
			*/
		}

		print("count: " + selectedFolder.transform.childCount);
		var cnt = selectedFolder.transform.childCount;
		for (i = 0; i < cnt; i++)
		//for (i = 1; i < 4; i++)
		{
			//GameObject child = selectedFolder.transform.GetChild(i);
			var newTile : GameObject = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
			newTile.transform.position = selectedFolder.transform.GetChild(i).transform.position;
			newTile.transform.parent = selectedFolder.transform.GetChild(i).transform.parent;
			//Selection.activeGameObject = newTile;
			//DestroyImmediate(selectedFolder.transform.GetChild(i));
			if (i > 200)
			{
				break;
			}
		}
		tilePrefab = null;
	}

	function ChangeTile ()
	{
		selectedTile = Selection.activeGameObject;

		if (selectedTile == null)
			return;

		if (selectedTile.layer != LayerMask.NameToLayer("Tile"))
			return;
		
		switch (selectedType)
		{
			case selectedType.Blank:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Blank.prefab", Object);
			break;

			case selectedType.SSPoint:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/SSPoint.prefab", Object);
			break;
			/*
			case selectedType.Earth:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Path.prefab", Object);
			break;

			case selectedType.Ground:
				tilePrefab = AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Nodes/Outside.prefab", Object);
			break;
			*/
		}

		if (!tilePrefab)
			return;
		
		var newTile : GameObject = PrefabUtility.InstantiatePrefab(tilePrefab) as GameObject;
		newTile.transform.position = selectedTile.transform.position;
		newTile.transform.parent = selectedTile.transform.parent;
		Selection.activeGameObject = newTile;
		DestroyImmediate(selectedTile);
		tilePrefab = null;
	}
}
