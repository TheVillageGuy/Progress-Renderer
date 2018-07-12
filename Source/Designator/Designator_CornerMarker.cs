﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ProgressRenderer
{

	public abstract class Designator_CornerMarker : Designator
	{

        private DesignateMode mode;

        public Designator_CornerMarker(DesignateMode mode)
		{
			this.mode = mode;
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.useMouseIcon = true;
		}

		protected override DesignationDef Designation
		{
			get
			{
				return DesignationDefOf.CornerMarker;
			}
		}

		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			AcceptanceReport result;
			if (!c.InBounds(Map))
			{
				result = false;
			}
			else
			{
				if (mode == DesignateMode.Add)
				{
					if (Map.designationManager.DesignationAt(c, Designation) != null)
					{
						return false;
					}
				}
				else if (mode == DesignateMode.Remove)
				{
					if (Map.designationManager.DesignationAt(c, Designation) == null)
					{
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public override void DesignateSingleCell(IntVec3 c)
        {
			if (mode == DesignateMode.Add)
			{
                Map.designationManager.AddDesignation(new Designation(c, Designation));
			}
			else if (mode == DesignateMode.Remove)
			{
                Map.designationManager.DesignationAt(c, Designation).Delete();
            }
            // Give feedback for the new amount of markers on the map
            DesignateSingleCellFeedback();
		}

        private void DesignateSingleCellFeedback()
        {
            List<Designation> markers = Map.designationManager.allDesignations.FindAll(des => des.def == DesignationDefOf.CornerMarker);
            // Message for the amount of markers on the map
            int markerCount = markers.Count;
            string message = "LPR_MessageCornerMarkerAmount".Translate(new object[] { markerCount }) + " ";
            if (markerCount < 2)
            {
                message += "LPR_MessageCornerMarkerTooLess".Translate();
            }
            else if (markerCount > 2)
            {
                message += "LPR_MessageCornerMarkerTooMany".Translate();
            }
            else
            {
                message += "LPR_MessageCornerMarkerCorrect".Translate();
            }
            Messages.Message(message, MessageTypeDefOf.CautionInput, false);
            // Message for the created area (if enough markers)
            if (markerCount > 1)
            {
                int startX = Map.Size.x;
                int startZ = Map.Size.z;
                int endX = 0;
                int endZ = 0;
                foreach (Designation des in markers)
                {
                    IntVec3 cell = des.target.Cell;
                    if (cell.x < startX) { startX = cell.x; }
                    if (cell.z < startZ) { startZ = cell.z; }
                    if (cell.x > endX) { endX = cell.x; }
                    if (cell.z > endZ) { endZ = cell.z; }
                }
                endX += 1;
                endZ += 1;
                int distX = endX - startX;
                int distZ = endZ - startZ;
                string messageRect = "LPR_MessageCornerMarkersRect".Translate(new object[] { distX, distZ });
                if (distZ <= 20)
                {
                    messageRect += " " + "LPR_MessageCornerMarkesRectHeightTooLow".Translate();
                }
                Messages.Message(messageRect, MessageTypeDefOf.CautionInput, false);
            }
        }

		public override void SelectedUpdate()
		{
			GenUI.RenderMouseoverBracket();
		}

		public override void RenderHighlight(List<IntVec3> dragCells)
		{
			DesignatorUtility.RenderHighlightOverSelectableCells(this, dragCells);
		}

	}

}
