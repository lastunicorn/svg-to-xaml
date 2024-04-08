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
using DustInTheWind.SvgToXaml.SvgSerialization.Conversion;
using DustInTheWind.SvgToXaml.SvgSerialization.XmlModels;

namespace DustInTheWind.SvgToXaml.SvgSerialization;

public class SvgSerializer
{
    public DeserializationResult Deserialize(string text)
    {
        using StringReader stringReader = new(text);

        XmlSerializer xmlSerializer = new(typeof(XmlSvg));
        object deserializedObject = xmlSerializer.Deserialize(stringReader);

        if (deserializedObject is not XmlSvg svgObject)
            throw new InvalidSvgException();

        DeserializationContext deserializationContext = new();

        return new DeserializationResult
        {
            Svg = svgObject.ToSvgModel(deserializationContext),
            Errors = deserializationContext.Errors,
            Warnings = deserializationContext.Warnings
        };
    }

    public DeserializationResult Deserialize(Stream stream)
    {
        XmlSerializer xmlSerializer = new(typeof(XmlSvg));
        object deserializedObject = xmlSerializer.Deserialize(stream);

        if (deserializedObject is not XmlSvg svgObject)
            throw new InvalidSvgException();

        DeserializationContext deserializationContext = new();

        return new DeserializationResult
        {
            Svg = SvgExtensions.ToSvgModel(svgObject, deserializationContext),
            Errors = deserializationContext.Errors,
            Warnings = deserializationContext.Warnings
        };
    }

    public DeserializationResult Deserialize(TextReader textReader)
    {
        XmlSerializer xmlSerializer = new(typeof(XmlSvg));
        object deserializedObject = xmlSerializer.Deserialize(textReader);

        if (deserializedObject is not XmlSvg svgObject)
            throw new InvalidSvgException();

        DeserializationContext deserializationContext = new();

        return new DeserializationResult
        {
            Svg = SvgExtensions.ToSvgModel(svgObject, deserializationContext),
            Errors = deserializationContext.Errors,
            Warnings = deserializationContext.Warnings
        };
    }
}