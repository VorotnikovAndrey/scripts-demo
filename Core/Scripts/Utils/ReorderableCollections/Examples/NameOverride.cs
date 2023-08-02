﻿using UnityEngine;

namespace ZeroVector.Common.Reorderable {
    public class NameOverride : MonoBehaviour {
        public string nameOverride = "Car";
        public string nestedNameOverride = "Car Part";

        [Reorderable(null, "Car", null)] public ExampleChildList autoNameList;

        [Reorderable] public DynamicExampleChildList dynamicNameList;

        [System.Serializable]
        public class ExampleChild {
            [Reorderable(null, "Car Part", null)] public StringList nested;
        }

        [System.Serializable]
        public class DynamicExampleChild {
            [Reorderable] public StringList nested;
        }

        [System.Serializable]
        public class ExampleChildList : ReorderableList<ExampleChild> { }

        [System.Serializable]
        public class DynamicExampleChildList : ReorderableList<DynamicExampleChild> { }

        [System.Serializable]
        public class StringList : ReorderableList<string> { }
    }
}