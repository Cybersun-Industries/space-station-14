﻿using System.Numerics;
using Content.Client._Shitcode.UserInterface.Systems.Surgery.Widgets.Systems;
using Content.Client.Guidebook.Richtext;
using Content.Radium.Common.Medical.Surgery;
using Content.Shared.Body.Part;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Shitcode.UserInterface.Systems.Surgery;

[GenerateTypedNameReferences]
public sealed partial class DamagePartsUi : UIWidget
{
    public TextureRect DollBase = new()
    {
        Texture = Texture.Transparent,
    };

    public Box DollBox = new();

    public Box HeadBox = new()
    {
        Margin = new Thickness(48, 0, 48, 0),
        HorizontalAlignment = HAlignment.Center
    };

    public Box RArmBox = new()
    {
        Margin = new Thickness(48, 0, 48, 0),
        HorizontalAlignment = HAlignment.Center
    };

    public Box BodyBox = new()
    {
        Margin = new Thickness(48, 0, 48, 0),
        HorizontalAlignment = HAlignment.Center
    };

    public Box RLegBox = new()
    {
        Margin = new Thickness(48, 0, 48, 0),
        HorizontalAlignment = HAlignment.Center
    };

    public Box LLegBox = new()
    {
        Margin = new Thickness(48, 0, 48, 0),
        HorizontalAlignment = HAlignment.Center
    };
    public Box LArmBox = new()
    {
        Margin = new Thickness(48, 0, 48, 0),
        HorizontalAlignment = HAlignment.Center
    };

    public TextureRect
        Head = new() { Stretch = TextureRect.StretchMode.Scale, SetSize = new Vector2(128, 128) },
        Body = new() { Stretch = TextureRect.StretchMode.Scale, SetSize = new Vector2(128, 128) },
        LArm = new() { Stretch = TextureRect.StretchMode.Scale, SetSize = new Vector2(128, 128) },
        RArm = new() { Stretch = TextureRect.StretchMode.Scale, SetSize = new Vector2(128, 128) },
        LLeg = new() { Stretch = TextureRect.StretchMode.Scale, SetSize = new Vector2(128, 128) },
        RLeg = new() { Stretch = TextureRect.StretchMode.Scale, SetSize = new Vector2(128, 128) };

    public void Clear()
    {
        Head.Texture = Texture.Transparent;
        RArm.Texture = Texture.Transparent;
        Body.Texture = Texture.Transparent;
        LArm.Texture = Texture.Transparent;
        LLeg.Texture = Texture.Transparent;
        RLeg.Texture = Texture.Transparent;
    }
    public void SyncControls(ClientDamagePartsSystem partsSystem,
        IReadOnlyDictionary<(BodyPartType, BodyPartSymmetry), (int, bool)> partsStates)
    {
        Clear();
        foreach (var pair in partsStates)
        {
            switch (pair.Key.Item1)
            {
                case BodyPartType.Head:
                    if (pair.Value.Item2)
                    {
                        Head.TexturePath = "/Textures/_Radium/Interface/Surgery/test_doll/head7.png";
                        break;
                    }

                    Head.TexturePath = pair.Value.Item1 switch
                    {
                        0 => "/Textures/_Radium/Interface/Surgery/test_doll/head0.png",
                        1 => "/Textures/_Radium/Interface/Surgery/test_doll/head1.png",
                        2 => "/Textures/_Radium/Interface/Surgery/test_doll/head2.png",
                        3 => "/Textures/_Radium/Interface/Surgery/test_doll/head3.png",
                        4 => "/Textures/_Radium/Interface/Surgery/test_doll/head4.png",
                        5 => "/Textures/_Radium/Interface/Surgery/test_doll/head5.png",
                        6 => "/Textures/_Radium/Interface/Surgery/test_doll/head6.png",
                        _ => "/Textures/_Radium/Interface/Surgery/test_doll/head6.png"
                    };
                    break;
                case BodyPartType.Other:
                    break;
                case BodyPartType.Torso:
                    if (pair.Value.Item2)
                    {
                        Body.TexturePath = "/Textures/_Radium/Interface/Surgery/test_doll/chest7.png";
                        break;
                    }

                    Body.TexturePath = pair.Value.Item1 switch
                    {
                        0 => "/Textures/_Radium/Interface/Surgery/test_doll/chest0.png",
                        1 => "/Textures/_Radium/Interface/Surgery/test_doll/chest1.png",
                        2 => "/Textures/_Radium/Interface/Surgery/test_doll/chest2.png",
                        3 => "/Textures/_Radium/Interface/Surgery/test_doll/chest3.png",
                        4 => "/Textures/_Radium/Interface/Surgery/test_doll/chest4.png",
                        5 => "/Textures/_Radium/Interface/Surgery/test_doll/chest5.png",
                        6 => "/Textures/_Radium/Interface/Surgery/test_doll/chest6.png",
                        _ => "/Textures/_Radium/Interface/Surgery/test_doll/chest6.png"
                    };
                    break;
                case BodyPartType.Arm:
                    if (pair.Key.Item2 == BodyPartSymmetry.Left)
                    {
                        if (pair.Value.Item2)
                        {
                            LArm.TexturePath = "/Textures/_Radium/Interface/Surgery/test_doll/l_arm7.png";
                            break;
                        }
                        LArm.TexturePath = pair.Value.Item1 switch
                        {
                            0 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm0.png",
                            1 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm1.png",
                            2 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm2.png",
                            3 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm3.png",
                            4 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm4.png",
                            5 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm5.png",
                            6 => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm6.png",
                            _ => "/Textures/_Radium/Interface/Surgery/test_doll/l_arm6.png"
                        };
                    }
                    else
                    {
                        if (pair.Value.Item2)
                        {
                            RArm.TexturePath = "/Textures/_Radium/Interface/Surgery/test_doll/r_arm7.png";
                            break;
                        }

                        RArm.TexturePath = pair.Value.Item1 switch
                        {
                            0 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm0.png",
                            1 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm1.png",
                            2 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm2.png",
                            3 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm3.png",
                            4 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm4.png",
                            5 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm5.png",
                            6 => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm6.png",
                            _ => "/Textures/_Radium/Interface/Surgery/test_doll/r_arm6.png"
                        };
                    }
                    break;
                case BodyPartType.Hand:
                    break;
                case BodyPartType.Leg:
                    if (pair.Key.Item2 == BodyPartSymmetry.Left)
                    {
                        if (pair.Value.Item2)
                        {
                            LLeg.TexturePath = "/Textures/_Radium/Interface/Surgery/test_doll/l_leg7.png";
                            break;
                        }
                        LLeg.TexturePath = pair.Value.Item1 switch
                        {
                            0 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg0.png",
                            1 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg1.png",
                            2 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg2.png",
                            3 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg3.png",
                            4 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg4.png",
                            5 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg5.png",
                            6 => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg6.png",
                            _ => "/Textures/_Radium/Interface/Surgery/test_doll/l_leg6.png"
                        };
                    }
                    else
                    {
                        if (pair.Value.Item2)
                        {
                            RLeg.TexturePath = "/Textures/_Radium/Interface/Surgery/test_doll/r_leg7.png";
                            break;
                        }
                        RLeg.TexturePath = pair.Value.Item1 switch
                        {
                            0 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg0.png",
                            1 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg1.png",
                            2 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg2.png",
                            3 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg3.png",
                            4 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg4.png",
                            5 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg5.png",
                            6 => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg6.png",
                            _ => "/Textures/_Radium/Interface/Surgery/test_doll/r_leg6.png"
                        };
                    }
                    break;
                case BodyPartType.Foot:
                    break;
                case BodyPartType.Tail:
                    break;
                default:
                    throw new Exception($"No body part with value: {pair.Key.Item1}");
            }
        }
    }

    public DamagePartsUi()
    {
        RobustXamlLoader.Load(this);
        DollBase.Stretch = TextureRect.StretchMode.Scale;
        DollBase.SetSize = new Vector2(128, 128);
        DollBox.SetSize = new Vector2(128, 128);
        DollBox.HorizontalAlignment = HAlignment.Center;
        RArmBox.Align = AlignMode.Begin;

        HeadBox.AddChild(Head);
        RArmBox.AddChild(RArm);
        LArmBox.AddChild(LArm);
        LLegBox.AddChild(LLeg);
        RLegBox.AddChild(RLeg);
        BodyBox.AddChild(Body);

        DollBase.AddChild(HeadBox);
        DollBase.AddChild(RArmBox);
        DollBase.AddChild(LArmBox);
        DollBase.AddChild(BodyBox);
        DollBase.AddChild(RLegBox);
        DollBase.AddChild(LLegBox);
        Surface.AddChild(DollBase);
    }
}
