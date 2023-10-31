using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Whumpus;

namespace Whumpus.Editor
{
    using UnityEditor;
    [CustomEditor(typeof(GameEvent))]
    public class GameEventEditor : Editor
    {
        System.Type[] _types;
        string[] _typesStr;
        int _dragStartID, _dragEndID;

        private void OnEnable()
        {
            List<string> types = new List<string>();
            types.Add("Add new feedback");

            List<System.Type> gameEventTypes = (from domainAssembly in System.AppDomain.CurrentDomain.GetAssemblies()
                                                from assemblyType in domainAssembly.GetTypes()
                                                where assemblyType.IsSubclassOf(typeof(GameFeedback))
                                                select assemblyType).ToList();
            _types = gameEventTypes.ToArray();

            foreach (var item in gameEventTypes)
            {
                string str = item.ToString();
                string[] parts = str.Split(".");
                types.Add(parts[1]);
            }


            _typesStr = types.ToArray();
        }


        public override void OnInspectorGUI()
        {
            int controlId = GUIUtility.GetControlID(FocusType.Passive);

            serializedObject.Update();

            GameEvent gameEvent = target as GameEvent;

            gameEvent.Name = EditorGUILayout.TextField(gameEvent.Name);

            int newItem = EditorGUILayout.Popup(0, _typesStr) - 1;
            if (newItem >= 0)
            {
                GameFeedback feedback = Activator.CreateInstance(_types[newItem]) as GameFeedback;

                EditorUtility.SetDirty(gameEvent);
                gameEvent.Feedbacks.Add(feedback);
            }

            SerializedProperty feedbacks = serializedObject.FindProperty("Feedbacks");

            /*
            _feedbacks.arraySize++;
            SerializedProperty newProp = _feedbacks.GetArrayElementAtIndex(_feedbacks.arraySize - 1);
            newProp.managedReferenceValue =
            */

            for (int i = 0; i < feedbacks.arraySize; i++)
            {
                SerializedProperty property = feedbacks.GetArrayElementAtIndex(i);

                Rect horizontal = EditorGUILayout.BeginHorizontal();

                #region Osef c'est du dessin de rectangles
                var backgroundRect = GUILayoutUtility.GetRect(5f, 17f);
                var offset = 4f;
                backgroundRect.xMax = 5;
                backgroundRect.xMin = 0;
                var foldoutRect = backgroundRect;
                foldoutRect.xMin += offset;
                foldoutRect.width = 300;
                foldoutRect.height = 17;

                EditorGUI.DrawRect(backgroundRect, gameEvent.Feedbacks[i].ReturnColor());
                #endregion

                Color color = gameEvent.Feedbacks[i].ReturnColor();
                color = new Color(color.r, color.g, color.b, 0.2f);
                EditorGUI.DrawRect(horizontal, color);

                int indexRemove = -1;
                bool deleted = false;

                if (GUILayout.Button("-", EditorStyles.miniButton, GUILayout.Width(EditorStyles.miniButton.CalcSize(new GUIContent("-")).x)))
                {
                    indexRemove = i;

                    if (indexRemove != -1)
                    {
                        feedbacks.DeleteArrayElementAtIndex(indexRemove);
                        deleted = true;
                    }
                }
                if (!deleted)
                {
                    property.isExpanded = GUI.Toggle(foldoutRect, property.isExpanded, gameEvent.Feedbacks[i].ToString(), EditorStyles.foldout);
                }

                EditorGUILayout.EndHorizontal();


                #region DragnDrop
                /*
                var eventCurrent = Event.current;
                if (eventCurrent.type == EventType.MouseDown)
                {
                    if (horizontal.Contains(eventCurrent.mousePosition))
                    {
                        GUIUtility.hotControl = controlId;

                        _dragStartID = i;
                        eventCurrent.Use();
                    }
                }

                if (_dragStartID == i)
                {
                    Color color = new Color(0, 1, 0, 0.3f);
                    EditorGUI.DrawRect(horizontal, color);
                }

                if (horizontal.Contains(eventCurrent.mousePosition))
                {
                    if (_dragStartID >= 0)
                    {
                        _dragEndID = i;

                        Rect headerSplit = horizontal;
                        headerSplit.height *= 0.5f;
                        headerSplit.y += headerSplit.height;
                        if (headerSplit.Contains(eventCurrent.mousePosition))
                            _dragEndID = i + 1;
                    }
                }

                if (_dragStartID >= 0 && _dragEndID >= 0)
                {
                    if (_dragEndID != _dragStartID)
                    {
                        if (_dragEndID > _dragStartID)
                            _dragEndID--;
                        feedbacks.MoveArrayElement(_dragStartID, _dragEndID);
                        _dragStartID = _dragEndID;
                    }
                }

                if (_dragStartID >= 0 || _dragEndID >= 0)
                {
                    if (eventCurrent.type == EventType.MouseUp)
                    {
                        _dragStartID = -1;
                        _dragEndID = -1;
                        eventCurrent.Use();
                    }
                }
                */
                #endregion

                if (!deleted)
                {
                    if (property.isExpanded)
                    {
                        foreach (var item in GetChildren(property))
                        {
                            EditorGUILayout.PropertyField(item);
                        }
                    }

                    var line = GUILayoutUtility.GetRect(1f, 1f);
                    EditorGUI.DrawRect(line, Color.black);
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        public IEnumerable<SerializedProperty> GetChildren(SerializedProperty serializedProperty)
        {
            SerializedProperty currentProperty = serializedProperty.Copy();
            SerializedProperty nextSiblingProperty = serializedProperty.Copy();
            {
                nextSiblingProperty.Next(false);
            }

            if (currentProperty.Next(true))
            {
                do
                {
                    if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                        break;

                    yield return currentProperty;
                }
                while (currentProperty.Next(false));
            }
        }
    }
}
