﻿using System.Collections.Generic;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;
using KamiLib.Interfaces;
using KamiLib.Utilities;

namespace KamiLib.Windows;

public abstract class SelectionWindow : Window, IDrawable
{
    private readonly float horizontalWeight;
    private readonly float verticalHeight;
    private const bool ShowBorders = false;

    private readonly SelectionList selectionList = new();

    protected abstract IEnumerable<ISelectable> GetSelectables();
    protected bool ShowScrollBar = true;

    protected SelectionWindow(string windowName, float xPercent, float height) : base(windowName)
    {
        horizontalWeight = xPercent;
        verticalHeight = height;
    }

    public override void Draw()
    {
        var region = ImGui.GetContentRegionAvail();
        var itemSpacing = ImGui.GetStyle().ItemSpacing;

        var leftSideWidth = region.X * horizontalWeight - itemSpacing.X / 2.0f;
        var topLeftSideHeight = region.Y - verticalHeight - itemSpacing.Y / 2.0f;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, Vector2.Zero);
        
        if(ImGui.BeginChild($"###{KamiLib.PluginName}LeftSide", new Vector2( leftSideWidth, topLeftSideHeight), ShowBorders))
        {
            selectionList.Draw(GetSelectables());
        }
        ImGui.EndChild();

        var bottomLeftChildPosition = ImGui.GetCursorPos();
        
        ImGui.SameLine();
        DrawVerticalLine();

        var rightSideWidth = region.X * (1.0f - horizontalWeight) - itemSpacing.X / 2.0f;
        
        if(ImGui.BeginChild($"###{KamiLib.PluginName}RightSide", new Vector2(rightSideWidth, 0), ShowBorders, ShowScrollBar ? ImGuiWindowFlags.AlwaysVerticalScrollbar : ImGuiWindowFlags.None))
        {
            selectionList.DrawSelected();
        }
        ImGui.EndChild();
        
        ImGui.SetCursorPos(bottomLeftChildPosition);
        
        if(ImGui.BeginChild($"###{KamiLib.PluginName}BottomLeftSide", new Vector2(leftSideWidth, verticalHeight), ShowBorders))
        {
            DrawExtras();
        }
        ImGui.EndChild();
        
        ImGui.PopStyleVar();
    }

    protected virtual void DrawExtras()
    {
        
    }

    private static void DrawVerticalLine()
    {
        var contentArea = ImGui.GetContentRegionAvail();
        var itemSpacing = ImGui.GetStyle().ItemSpacing;
        var cursor = new Vector2(ImGui.GetCursorScreenPos().X - itemSpacing.X / 2.0f, ImGui.GetCursorScreenPos().Y );
        var drawList = ImGui.GetWindowDrawList();
        var color = ImGui.GetColorU32(Colors.White);

        drawList.AddLine(cursor, cursor with {Y = cursor.Y + contentArea.Y}, color, 1.0f);
    }
}