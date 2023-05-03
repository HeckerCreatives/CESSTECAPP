using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
[CustomPropertyDrawer(typeof(TopicData))]
[CustomPropertyDrawer(typeof(AirplaneSystems))]
[CustomPropertyDrawer(typeof(AirplaneTopicContent))]
[CustomPropertyDrawer(typeof(QuizData))]
[CustomPropertyDrawer(typeof(QuizNumber))]
[CustomPropertyDrawer(typeof(Airplane))]
[CustomPropertyDrawer(typeof(Developers))]
[CustomPropertyDrawer(typeof(SystemStats))]
[CustomPropertyDrawer(typeof(AirplaneSystemStats))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
