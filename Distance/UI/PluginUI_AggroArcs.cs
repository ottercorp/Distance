﻿using System;
using System.Linq;
using System.Numerics;

using CheapLoc;

using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Utility;

using FFXIVClientStructs.FFXIV.Client.Graphics.Scene;

using ImGuiNET;

namespace Distance;

internal sealed class PluginUI_AggroArcs : IDisposable
{
	public PluginUI_AggroArcs( Plugin plugin, PluginUI ui, Configuration configuration )
	{
		mPlugin = plugin;
		mUI = ui;
		mConfiguration = configuration;
	}

	public void Dispose()
	{
	}

	public void DrawConfigOptions()
	{
		if( ImGui.CollapsingHeader( Loc.Localize( "Config Section Header: Aggro Widget Arc", "Aggro Arc Settings" ) + $"###Aggro Widget Arc Header." ) )
		{
			ImGui.Checkbox( Loc.Localize( "Config Option: Show Aggro Arc", "Show an arc indicating aggro range." ) + "###Show aggro arc.", ref mConfiguration.mDrawAggroArc );
			ImGuiUtils.HelpMarker( Loc.Localize( "Help: Show Aggro Arc", "This is a visual representation of the distance readout shown by the aggro widget.  If you wish to change colors or which target type is used, adjust those settings for the aggro widget, and they will apply to this arc." ) );
			if( mConfiguration.DrawAggroArc )
			{
				ImGui.Text( Loc.Localize( "Config Option: Aggro Arc Length", "Length of the aggro arc (deg):" ) );
				ImGui.SliderInt( "###AggroArcLengthSlider", ref mConfiguration.mAggroArcLength_Deg, 0, 15 );
			}
		}
	}

	public void DrawOnOverlay()
	{
		if( mConfiguration.DrawAggroArc && mPlugin.ShouldDrawAggroDistanceInfo() )
		{
			DrawAggroDistanceArc();
		}
	}

	private void DrawAggroDistanceArc()
	{
		var distanceInfo = mPlugin.GetDistanceInfo( mConfiguration.AggroDistanceApplicableTargetType );
		if( !distanceInfo.IsValid ) return;

		//***** TODO: Maybe grab the alpha off of the focus target addon when aggro node is attached to focus target bar to make things make sense.
		Vector4 color = mConfiguration.AggroDistanceTextColor;
		Vector4 edgeColor = mConfiguration.AggroDistanceTextEdgeColor;
		if( distanceInfo.DistanceFromTargetAggro_Yalms < mConfiguration.AggroWarningDistance_Yalms )
		{
			color = mConfiguration.AggroDistanceWarningTextColor;
			edgeColor = mConfiguration.AggroDistanceWarningTextEdgeColor;
		}
		else if( distanceInfo.DistanceFromTargetAggro_Yalms < mConfiguration.AggroCautionDistance_Yalms )
		{
			color = mConfiguration.AggroDistanceCautionTextColor;
			edgeColor = mConfiguration.AggroDistanceCautionTextEdgeColor;
		}

		ArcUtils.DrawArc_ScreenSpace(
			distanceInfo.TargetPosition,
			distanceInfo.PlayerPosition,
			distanceInfo.AggroRange_Yalms + distanceInfo.TargetRadius_Yalms,
			mConfiguration.AggroArcLength_Deg,
			false,
			true,
			color,
			edgeColor );
	}

	private readonly Plugin mPlugin;
	private readonly PluginUI mUI;
	private readonly Configuration mConfiguration;
}