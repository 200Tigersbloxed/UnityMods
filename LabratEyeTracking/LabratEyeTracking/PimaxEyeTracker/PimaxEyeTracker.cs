using UnityEngine;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

namespace Pimax.EyeTracking
{
    public enum EyeParameter
    {
        GazeX, // Gaze point on the X axis (not working!)
        GazeY, // Gaze point on then Y axis (not working!)
        GazeRawX, // Gaze point on the X axis before smoothing is applied (not working!)
        GazeRawY, // Gaze point on the Y axis before smoothing is applied (not working!)
        GazeSmoothX, // Gaze point on the X axis after smoothing is applied (not working!)
        GazeSmoothY, // Gaze point on the Y axis after smoothing is applied (not working!)
        GazeOriginX, // Pupil gaze origin on the X axis
        GazeOriginY, // Pupil gaze origin on the Y axis
        GazeOriginZ, // Pupil gaze origin on the Z axis
        GazeDirectionX, // Gaze vector on the X axis (not working!)
        GazeDirectionY, // Gaze vector on the Y axis (not working!)
        GazeDirectionZ, // Gaze vector on the Z axis (not working!)
        GazeReliability, // Gaze point reliability (not working!)
        PupilCenterX, // Pupil center on the X axis, normalized between 0 and 1
        PupilCenterY, // Pupil center on the Y axis, normalized between 0 and 1
        PupilDistance, // Distance between pupil and camera lens, measured in millimeters
        PupilMajorDiameter, // Pupil major axis diameter, normalized between 0 and 1
        PupilMajorUnitDiameter, // Pupil major axis diameter, measured in millimeters
        PupilMinorDiameter, // Pupil minor axis diameter, normalized between 0 and 1
        PupilMinorUnitDiameter, // Pupil minor axis diameter, measured in millimeters
        Blink, // Blink state (not working!)
        Openness, // How open the eye is - 100 (closed), 50 (partially open, unreliable), 0 (open)
        UpperEyelid, // Upper eyelid state (not working!)
        LowerEyelid // Lower eyelid state (not working!)
    }

    public enum EyeExpression
    {
        PupilCenterX, // Pupil center on the X axis, smoothed and normalized between -1 (looking left) ... 0 (looking forward) ... 1 (looking right)
        PupilCenterY, // Pupil center on the Y axis, smoothed and normalized between -1 (looking up) ... 0 (looking forward) ... 1 (looking down)
        Openness, // How open the eye is, smoothed and normalized between 0 (fully closed) ... 1 (fully open)
        Blink // Blink, 0 (not blinking) or 1 (blinking)
    }

    public enum Eye
    {
        Any,
        Left,
        Right
    }

    public enum CallbackType
    {
        Start,
        Stop,
        Update
    }

    public struct EyeExpressionState
    {
        public Eye Eye { get; private set; }
        public Vector2 PupilCenter { get; private set; }
        public float Openness { get; private set; }
        public bool Blink { get; private set; }

        public EyeExpressionState(Eye eyeType, EyeTracker eyeTracker)
        {
            this.Eye = eyeType;
            this.PupilCenter = new Vector2(eyeTracker.GetEyeExpression(this.Eye, EyeExpression.PupilCenterX), eyeTracker.GetEyeExpression(this.Eye, EyeExpression.PupilCenterY));
            this.Openness = eyeTracker.GetEyeExpression(this.Eye, EyeExpression.Openness);
            this.Blink = (eyeTracker.GetEyeExpression(this.Eye, EyeExpression.Blink) != 0.0f);
        }
    }

    public struct EyeState
    {
        public Eye Eye { get; private set; }
        public Vector2 Gaze { get; private set; }
        public Vector2 GazeRaw { get; private set; }
        public Vector2 GazeSmooth { get; private set; }
        public Vector3 GazeOrigin { get; private set; }
        public Vector3 GazeDirection { get; private set; }
        public float GazeReliability { get; private set; }
        public Vector2 PupilCenter { get; private set; }
        public float PupilDistance { get; private set; }
        public float PupilMajorDiameter { get; private set; }
        public float PupilMajorUnitDiameter { get; private set; }
        public float PupilMinorDiameter { get; private set; }
        public float PupilMinorUnitDiameter { get; private set; }
        public float Blink { get; private set; }
        public float Openness { get; private set; }
        public float UpperEyelid { get; private set; }
        public float LowerEyelid { get; private set; }
        public EyeExpressionState Expression { get; private set; }

        public EyeState(Eye eyeType, EyeTracker eyeTracker)
        {
            this.Eye = eyeType;
            this.Gaze = new Vector2(eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeX), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeY));
            this.GazeRaw = new Vector2(eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeRawX), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeRawY));
            this.GazeSmooth = new Vector2(eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeSmoothX), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeSmoothY));
            this.GazeOrigin = new Vector3(eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeOriginX), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeOriginY), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeOriginZ));
            this.GazeDirection = new Vector3(eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeDirectionX), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeDirectionY), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeDirectionZ));
            this.GazeReliability = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.GazeReliability);
            this.PupilDistance = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilDistance);
            this.PupilMajorDiameter = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilMajorDiameter);
            this.PupilMajorUnitDiameter = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilMajorUnitDiameter);
            this.PupilMinorDiameter = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilMinorDiameter);
            this.PupilMinorUnitDiameter = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilMinorUnitDiameter);
            this.Blink = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.Blink);
            this.UpperEyelid = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.UpperEyelid);
            this.LowerEyelid = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.LowerEyelid);
            this.Openness = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.Openness);
            this.PupilCenter = new Vector2(eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilCenterX), eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilCenterY));
            this.Expression = new EyeExpressionState(eyeType, eyeTracker);

            // Convert range from 0...1 to -1...1, defaulting eyes to center (0) when unavailable
            //float x = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilCenterX);  
            //float y = eyeTracker.GetEyeParameter(this.Eye, EyeParameter.PupilCenterY);
            //this.PupilCenter = new Vector2(x <= float.Epsilon ? 0.0f : (x * 2.0f - 1.0f), y <= float.Epsilon ? 0.0f : (y * 2.0f - 1.0f));
            //this.Openness = (x <= float.Epsilon && y <= float.Epsilon) ? 0.0f : 1.0f;
        }
    }

    public delegate void EyeTrackerEventHandler();

    public class EyeTracker
    {
        [DllImport("PimaxEyeTracker", EntryPoint = "RegisterCallback")] private static extern void _RegisterCallback(CallbackType type, EyeTrackerEventHandler callback);
        [DllImport("PimaxEyeTracker", EntryPoint = "Start")] private static extern bool _Start();
        [DllImport("PimaxEyeTracker", EntryPoint = "Stop")] private static extern void _Stop();
        [DllImport("PimaxEyeTracker", EntryPoint = "IsActive")] private static extern bool _IsActive();
        [DllImport("PimaxEyeTracker", EntryPoint = "GetTimestamp")] private static extern System.Int64 _GetTimestamp();
        [DllImport("PimaxEyeTracker", EntryPoint = "GetRecommendedEye")] private static extern Eye _GetRecommendedEye();
        [DllImport("PimaxEyeTracker", EntryPoint = "GetEyeParameter")] private static extern float _GetEyeParameter(Eye eye, EyeParameter param);
        [DllImport("PimaxEyeTracker", EntryPoint = "GetEyeExpression")] private static extern float _GetEyeExpression(Eye eye, EyeExpression expression);

        public EyeTrackerEventHandler OnStart { get; set; }
        private EyeTrackerEventHandler _OnStartHandler = null;

        public EyeTrackerEventHandler OnStop { get; set; }
        private EyeTrackerEventHandler _OnStopHandler = null;

        public EyeTrackerEventHandler OnUpdate { get; set; }
        private EyeTrackerEventHandler _OnUpdateHandler = null;

        public EyeState LeftEye { get; private set; }
        public EyeState RightEye { get; private set; }
        public EyeState RecommendedEye { get; private set; }

        public System.Int64 Timestamp => _GetTimestamp();
        //public Eye RecommendedEye => _GetRecommendedEye();

        public bool Active => _IsActive();

        public bool Start()
        {
            _OnStartHandler = _OnStart;
            _RegisterCallback(CallbackType.Start, _OnStartHandler);

            _OnStopHandler = _OnStop;
            _RegisterCallback(CallbackType.Stop, _OnStopHandler);

            _OnUpdateHandler = _OnUpdate;
            _RegisterCallback(CallbackType.Update, _OnUpdateHandler);

            return _Start();
        }

        public void Stop() => _Stop();

        public float GetEyeParameter(Eye eye, EyeParameter param) => _GetEyeParameter(eye, param);
        public float GetEyeExpression(Eye eye, EyeExpression expression) => _GetEyeExpression(eye, expression);

        private void _OnUpdate()
        {
            if (Active)
            {
                LeftEye = new EyeState(Eye.Left, this);
                RightEye = new EyeState(Eye.Right, this);
                RecommendedEye = new EyeState(_GetRecommendedEye(), this);
                OnUpdate?.Invoke();
            }
        }

        private void _OnStart() => OnStart?.Invoke();
        private void _OnStop() => OnStop?.Invoke();
    }

    public class PimaxEyeTracker : MonoBehaviour
    {
        public EyeTracker EyeTracker;

        void Awake()
        {
            EyeTracker = new EyeTracker();
        }

        void OnDestroy()
        {
            if (EyeTracker.Active) EyeTracker.Stop();
        }
    }

#if UNITY_EDITOR
	[CustomEditor(typeof(PimaxEyeTracker))]
	public class PimaxEyeTrackerEditor : Editor {
		EyeTracker eyeTracker = null;

		public override void OnInspectorGUI() {
			base.DrawDefaultInspector();

			eyeTracker = ((PimaxEyeTracker)target).EyeTracker;
			if(eyeTracker == null) return;

			if(eyeTracker.Active) {
                Action DrawEyePreview = () => {
                    var eyePreviewSize = 200.0f;
                    var eyeBlinkPreviewHeight = 50.0f;
                    var eyeSize = 20.0f;

                    GUILayout.BeginHorizontal();
                    Rect rtBackgroundLeft = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, GUILayout.MinWidth(eyePreviewSize + eyeSize), GUILayout.MinHeight(eyePreviewSize + eyeSize));
                    GUILayout.Space(80);
                    Rect rtBackgroundRight = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, GUILayout.MinWidth(eyePreviewSize + eyeSize), GUILayout.MinHeight(eyePreviewSize + eyeSize));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    Rect rtBlinkBackgroundLeft = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, GUILayout.MinWidth(eyePreviewSize + eyeSize), GUILayout.MinHeight(eyeBlinkPreviewHeight));
                    GUILayout.Space(80);
                    Rect rtBlinkBackgroundRight = GUILayoutUtility.GetRect(GUIContent.none, GUI.skin.box, GUILayout.MinWidth(eyePreviewSize + eyeSize), GUILayout.MinHeight(eyeBlinkPreviewHeight));
                    GUILayout.EndHorizontal();

                    Rect rtEyeLeft = new Rect(rtBackgroundLeft.x + (Mathf.InverseLerp(-1.0f, 1.0f, eyeTracker.LeftEye.Expression.PupilCenter.x) * eyePreviewSize), rtBackgroundLeft.y + (Mathf.InverseLerp(-1.0f, 1.0f, -eyeTracker.LeftEye.Expression.PupilCenter.y) * eyePreviewSize), 20, 20);
                    Rect rtEyeRight = new Rect(rtBackgroundRight.x + (Mathf.InverseLerp(-1.0f, 1.0f, eyeTracker.RightEye.Expression.PupilCenter.x) * eyePreviewSize), rtBackgroundRight.y + (Mathf.InverseLerp(-1.0f, 1.0f, -eyeTracker.RightEye.Expression.PupilCenter.y) * eyePreviewSize), 20, 20);

                    //Rect rtEyeLeft = new Rect(rtBackgroundLeft.x + (eyeTracker.LeftEye.PupilCenter.x * eyePreviewSize), rtBackgroundLeft.y + (eyeTracker.LeftEye.PupilCenter.y * eyePreviewSize), 20, 20);
                    //Rect rtEyeRight = new Rect(rtBackgroundRight.x + (eyeTracker.RightEye.PupilCenter.x * eyePreviewSize), rtBackgroundRight.y + (eyeTracker.RightEye.PupilCenter.y * eyePreviewSize), 20, 20);

                    var oldColor = GUI.backgroundColor;
                    GUI.backgroundColor = Color.white;

                    GUI.Box(rtBackgroundLeft, "");
                    GUI.Box(rtBackgroundRight, "");

                    GUI.backgroundColor = Color.red;

                    GUI.Box(rtEyeLeft, "");
                    GUI.Box(rtEyeRight, "");

                    if(eyeTracker.LeftEye.Expression.Openness <= 0.2f) GUI.backgroundColor = Color.green;
                    else if(eyeTracker.LeftEye.Expression.Openness >= 0.8f) GUI.backgroundColor = Color.grey;
                    else GUI.backgroundColor = Color.yellow;

                    GUI.Box(rtBlinkBackgroundLeft, "");

                    if(eyeTracker.RightEye.Expression.Openness <= 0.2f) GUI.backgroundColor = Color.green;
                    else if(eyeTracker.RightEye.Expression.Openness >= 0.8f) GUI.backgroundColor = Color.grey;
                    else GUI.backgroundColor = Color.yellow;

                    GUI.Box(rtBlinkBackgroundRight, "");

                    GUI.backgroundColor = oldColor;
                };

				if(GUILayout.Button("Stop Eye Tracking")) eyeTracker.Stop();

                EditorGUILayout.LabelField("Eye Tracking Preview", EditorStyles.boldLabel);
                DrawEyePreview();
                EditorGUILayout.Space();

                EditorGUILayout.LabelField("Expression Metrics", EditorStyles.boldLabel);
                EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Center", EditorStyles.boldLabel);
				EditorGUILayout.Vector2Field("Left", eyeTracker.LeftEye.Expression.PupilCenter);
				EditorGUILayout.Vector2Field("Right", eyeTracker.RightEye.Expression.PupilCenter);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Blink", EditorStyles.boldLabel);
				EditorGUILayout.Toggle("Left", eyeTracker.LeftEye.Expression.Blink);
				EditorGUILayout.Toggle("Right", eyeTracker.RightEye.Expression.Blink);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Openness", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.Expression.Openness);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.Expression.Openness);
				EditorGUILayout.Space();

                EditorGUILayout.LabelField("Raw Metrics", EditorStyles.boldLabel);
                EditorGUILayout.Space();

				EditorGUILayout.TextField("Timestamp", eyeTracker.Timestamp.ToString());
				EditorGUILayout.TextField("Recommended Eye", eyeTracker.RecommendedEye.Eye.ToString());
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Gaze", EditorStyles.boldLabel);
				EditorGUILayout.Vector2Field("Left", eyeTracker.LeftEye.Gaze);
				EditorGUILayout.Vector2Field("Right", eyeTracker.RightEye.Gaze);
				EditorGUILayout.Vector2Field("Recommended", eyeTracker.RecommendedEye.Gaze);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Gaze (Raw)", EditorStyles.boldLabel);
				EditorGUILayout.Vector2Field("Left", eyeTracker.LeftEye.GazeRaw);
				EditorGUILayout.Vector2Field("Right", eyeTracker.RightEye.GazeRaw);
				EditorGUILayout.Vector2Field("Recommended", eyeTracker.RecommendedEye.GazeRaw);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Gaze (Smooth)", EditorStyles.boldLabel);
				EditorGUILayout.Vector2Field("Left", eyeTracker.LeftEye.GazeSmooth);
				EditorGUILayout.Vector2Field("Right", eyeTracker.RightEye.GazeSmooth);
				EditorGUILayout.Vector2Field("Recommended", eyeTracker.RecommendedEye.GazeSmooth);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Gaze Origin", EditorStyles.boldLabel);
				EditorGUILayout.Vector3Field("Left", eyeTracker.LeftEye.GazeOrigin);
				EditorGUILayout.Vector3Field("Right", eyeTracker.RightEye.GazeOrigin);
				EditorGUILayout.Vector3Field("Recommended", eyeTracker.RecommendedEye.GazeSmooth);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Gaze Direction", EditorStyles.boldLabel);
				EditorGUILayout.Vector3Field("Left", eyeTracker.LeftEye.GazeDirection);
				EditorGUILayout.Vector3Field("Right", eyeTracker.RightEye.GazeDirection);
				EditorGUILayout.Vector3Field("Recommended", eyeTracker.RecommendedEye.GazeDirection);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Gaze Reliability", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.GazeReliability);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.GazeReliability);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.GazeReliability);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Center", EditorStyles.boldLabel);
				EditorGUILayout.Vector2Field("Left", eyeTracker.LeftEye.PupilCenter);
				EditorGUILayout.Vector2Field("Right", eyeTracker.RightEye.PupilCenter);
				EditorGUILayout.Vector2Field("Recommended", eyeTracker.RecommendedEye.PupilCenter);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Distance", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.PupilDistance);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.PupilDistance);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.PupilDistance);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Diameter (Major)", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.PupilMajorDiameter);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.PupilMajorDiameter);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.PupilMajorDiameter);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Unit Diameter (Major)", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.PupilMajorUnitDiameter);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.PupilMajorUnitDiameter);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.PupilMajorUnitDiameter);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Diameter (Minor)", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.PupilMinorDiameter);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.PupilMinorDiameter);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.PupilMinorDiameter);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Pupil Unit Diameter (Minor)", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.PupilMinorUnitDiameter);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.PupilMinorUnitDiameter);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.PupilMinorUnitDiameter);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Blink", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.Blink);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.Blink);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.Blink);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Openness", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.Openness);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.Openness);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.Openness);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Upper Eyelid", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.UpperEyelid);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.UpperEyelid);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.UpperEyelid);
				EditorGUILayout.Space();

				EditorGUILayout.LabelField("Lower Eyelid", EditorStyles.boldLabel);
				EditorGUILayout.FloatField("Left", eyeTracker.LeftEye.LowerEyelid);
				EditorGUILayout.FloatField("Right", eyeTracker.RightEye.LowerEyelid);
				EditorGUILayout.FloatField("Recommended", eyeTracker.RecommendedEye.LowerEyelid);
			} else {
				if(GUILayout.Button("Start Eye Tracking")) eyeTracker.Start();
			}
		}

		public override bool RequiresConstantRepaint() => eyeTracker != null && eyeTracker.Active;
	}
#endif
}