using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities.Solvers
{
    public class ActiveIndicator : Solver
    {
        /// <summary>
        /// The GameObject transform to point the indicator towards when this object is in view.
        /// </summary>
        [Tooltip("The GameObject transform to point the indicator towards when this object is in view.\nThe frame of reference for viewing is defined by the Solver Handler Tracked Target Type")]
        public Transform ActiveTarget;

        /// <summary>
        /// The offset from center to place the indicator.
        /// </summary>
        [Tooltip("The offset from center to place the indicator.")]
        public Vector3 IndicatorOffset = new Vector3(0, 1.0f, 0);

        /// <summary>
        /// The scale for the indicator object
        /// </summary>
        [Tooltip("The offset from center to place the indicator. If frame of reference is the camera, then the object will be this distance from center of screen")]
        [Min(0.0f)]
        public float IndicatorScale = 0.4f;

        /// <summary>
        /// Multiplier factor to increase or decrease FOV range for testing if object is visible and thus turn off indicator
        /// </summary>
        [Tooltip("Multiplier factor to increase or decrease FOV range for testing if object is visible and thus turn off indicator")]
        [Min(0.1f)]
        public float VisibilityScaleFactor = 0.8f;

        private bool indicatorShown = false;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            SetIndicatorVisibility(ShouldShowIndicator());
        }

        // Update is called once per frame
        void Update()
        {
            bool showIndicator = ShouldShowIndicator();
            if (showIndicator != indicatorShown)
            {
                SetIndicatorVisibility(showIndicator);
            }
        }

        private bool ShouldShowIndicator()
        {
            if (ActiveTarget == null || SolverHandler.TransformTarget == null)
            {
                return false;
            }

            return MathUtilities.IsInFOV(ActiveTarget.position, SolverHandler.TransformTarget,
                VisibilityScaleFactor * CameraCache.Main.fieldOfView, VisibilityScaleFactor * CameraCache.Main.GetHorizontalFieldOfViewDegrees(),
                CameraCache.Main.nearClipPlane, CameraCache.Main.farClipPlane);
        }

        private void SetIndicatorVisibility(bool showIndicator)
        {
            SolverHandler.UpdateSolvers = showIndicator;

            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = showIndicator;
            }

            indicatorShown = showIndicator;
        }

        public override void SolverUpdate()
        {
            if (ActiveTarget == null)
            {
                return;
            }

            var solverReferenceFrame = SolverHandler.TransformTarget;

            Vector3 origin = solverReferenceFrame.position + solverReferenceFrame.forward;

            Vector3 trackerToTargetDirection = -IndicatorOffset.normalized;

            // Project the vector (from the frame of reference (SolverHandler target) to the Directional Target) onto the "viewable" plane
            Vector3 indicatorDirection = Vector3.ProjectOnPlane(trackerToTargetDirection, -solverReferenceFrame.forward).normalized;

            GoalPosition = ActiveTarget.position + IndicatorOffset;

            GoalRotation = Quaternion.LookRotation(solverReferenceFrame.forward, indicatorDirection);

            // Scale the solver based to be more prominent if the object is far away from the field of view
            //float minVisiblityAngle = VisibilityScaleFactor * CameraCache.Main.fieldOfView * 0.5f;

            //float angleToVisbilityFOV = Vector3.Angle(trackerToTargetDirection - solverReferenceFrame.position, solverReferenceFrame.forward) - minVisiblityAngle;
            //float visibilityScale = 180f - minVisiblityAngle;
            GoalScale = Vector3.one * IndicatorScale;
            //GoalScale = Vector3.one * Mathf.Lerp(MinIndicatorScale, MaxIndicatorScale, angleToVisbilityFOV / visibilityScale);
        }
    }
}

