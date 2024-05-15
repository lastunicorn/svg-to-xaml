﻿// SvgToXaml
// Copyright (C) 2022-2024 Dust in the Wind
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System.Xml.Serialization;

namespace DustInTheWind.SvgDotnet.Serialization.XmlModels;

public class XmlContainer : XmlElement
{
    [XmlElement("title")]
    public XmlTitle[] Titles { get; set; }

    // Descriptive elements
    //[XmlElement("desc", typeof())]
    //[XmlElement("title", typeof())]
    //[XmlElement("metadata", typeof())]

    // Paint server elements
    [XmlElement("linearGradient", typeof(XmlLinearGradient))]
    [XmlElement("radialGradient", typeof(XmlRadialGradient))]
    //[XmlElement("pattern", typeof(XmlRadialGradient))]

    // Shape elements
    [XmlElement("circle", typeof(XmlCircle))]
    [XmlElement("ellipse", typeof(XmlEllipse))]
    [XmlElement("line", typeof(XmlLine))]
    [XmlElement("path", typeof(XmlPath))]
    [XmlElement("polygon", typeof(XmlPolygon))]
    [XmlElement("polyline", typeof(XmlPolyline))]
    [XmlElement("rect", typeof(XmlRect))]

    // Structural elements
    [XmlElement("defs", typeof(XmlDefs))]
    [XmlElement("g", typeof(XmlG))]
    [XmlElement("svg", typeof(XmlSvg))]
    //[XmlElement("symbol", typeof(XmlSymbol))]
    [XmlElement("use", typeof(XmlUse))]

    // Other elements
    [XmlElement("clipPath", typeof(XmlClipPath))]
    [XmlElement("style", typeof(XmlStyle))]
    [XmlElement("text", typeof(XmlText))]
    public object[] Children { get; set; }
}