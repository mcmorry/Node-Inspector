﻿using UnityEngine;
using UnityEditor;


namespace NodeInspector.Editor{
    public class Joint {


        const float BezierNormalMagnitude = 30.0f;
        public Rect               FieldInternalRect     {get; private set;}
		public JointType 	      JointType			    {get; private set;}
        public SerializedProperty SerializedProperty    {get; private set;} //we have this property if it's link to property
        public Object             ObjectRefferenceValue {get; private set;}  //value or link to anonimous class


        public GUIStyle KnobButtonStyle             {get; private set;}
        public Vector2  BezierNormal                {get; private set;}
        public Vector2  BezierSidePoint             {get; private set;}
        public string   KnobButtonCaption           {get; private set;}
        //relative rect
        public Rect     KnobButtonInternalRect      {get; private set;} 
        //External (same as windoow coords ) rect
        public Rect     KnobButtonExternalRect      {
            get{
                Rect rect = KnobButtonInternalRect ;
                rect.position+= ParentNode.WindowRect.position;
                return rect;
            }
        }

        public int ControlID {get; private set;}

        public Node ParentNode {get; private set;}


        public static Joint GetInstance(SerializedProperty serializedProperty, Rect fieldInternalRect
            , JointType jointType, Vector2 parentWindowGlobalPosition, Node parentNode){
            Joint result = GetInstance(fieldInternalRect, jointType, parentWindowGlobalPosition);
            result.SerializedProperty = serializedProperty ;
            result.ObjectRefferenceValue = serializedProperty.objectReferenceValue;
            result.ParentNode = parentNode;
            return result;
        }

        public static Joint GetInstance(Object scriptableObject, Rect fieldInternalRect
            , JointType jointType, Vector2 parentWindowGlobalPosition, Node parentNode){
            Joint result = GetInstance(fieldInternalRect, jointType,parentWindowGlobalPosition);
            result.ObjectRefferenceValue = scriptableObject;
            result.ParentNode = parentNode;
            return result;
        }

        static Joint GetInstance( Rect fieldInternalRect, JointType jointType, Vector2 parentWindowGlobalPosition){
            int controlID = GUIUtility.GetControlID(FocusType.Passive);
            Joint result = (Joint)GUIUtility.GetStateObject(typeof(Joint), controlID);
            result.ControlID = controlID;
            if (Event.current.type == EventType.repaint){
                result.FieldInternalRect = fieldInternalRect;                              
            }

            result.JointType = jointType;
            result.SetupGUIVariables(parentWindowGlobalPosition);
            return result;
        }

        public void ClearJointLink(){
            
            switch(JointType){
                case JointType.OneToOne_Incognito_OUT:
                    SerializedProperty.serializedObject.Update();
                    SerializedProperty.objectReferenceValue = null;
                    SerializedProperty.serializedObject.ApplyModifiedProperties();

                    GUI.changed = true;
                    break;
                case JointType.Incognito_In:
                    //don't need to clear anything;
                    break;
                default:
                    Debug.LogError("Unsupported type");
                    break;
            }
        }

        public void OnGUI(){            
            if (Event.current.GetTypeForControl(ControlID) == EventType.Repaint){                                       
                if (ParentNode.ParentWindow.JointHighlight.JointType != JointType.Nan){
                    GUI.enabled = ParentNode.ParentWindow.JointHighlight.JointType == JointType;
                }

                GUI.Button(KnobButtonInternalRect, KnobButtonCaption, KnobButtonStyle);                       
                GUI.enabled = true;
            }                            
        }


        void SetupGUIVariables(Vector2 parentWindowGlobalPosition)
        {
            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonInternalRect = new Rect(Node.KnobSize * 2,  0, Node.KnobSize, Node.KnobSize);
                    BezierSidePoint = parentWindowGlobalPosition + new Vector2(Node.KnobSize*2.5f, 0.0f);
                    BezierNormal = BezierSidePoint + Vector2.down * BezierNormalMagnitude;
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_IN:
                case JointType.OneToOne_IN:
                    KnobButtonInternalRect = new Rect(0, FieldInternalRect.y, Node.KnobSize, Node.KnobSize);
                    BezierSidePoint = parentWindowGlobalPosition + KnobButtonInternalRect.position + new Vector2(0.0f, Node.KnobSize*0.5f);
                    BezierNormal = BezierSidePoint + Vector2.left * BezierNormalMagnitude;
                    break;
                default:
                    KnobButtonInternalRect = new Rect( FieldInternalRect.x + FieldInternalRect.width + Node.OriginalRightPatdding, FieldInternalRect.y, Node.KnobSize, Node.KnobSize);
                    BezierSidePoint = parentWindowGlobalPosition + KnobButtonInternalRect.position+ new Vector2(Node.KnobSize, Node.KnobSize*0.5f);
                    BezierNormal = BezierSidePoint + Vector2.right * BezierNormalMagnitude;
                    break;
            }

            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonCaption = "i";
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_OUT:
                case JointType.ManyToMany_OUT:                        
                    KnobButtonCaption = "∞";
                    break;
                default:
                    KnobButtonCaption = "1";
                    break;                
            }

            switch(JointType){
                case JointType.Incognito_In:
                    KnobButtonStyle = EditorStyles.miniButton;
                    break;
                case JointType.ManyToMany_IN:
                case JointType.ManyToOne_IN:
                case JointType.OneToMany_IN:
                case JointType.OneToOne_IN:
                    KnobButtonStyle = EditorStyles.miniButtonLeft;
                    break;
                default:
                    KnobButtonStyle = EditorStyles.miniButtonRight;
                    break;   
            }
        }


           

    }
}