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

using System.Windows;
using DustInTheWind.SvgToXaml.SvgModel;
using TranslateTransform = System.Windows.Media.TranslateTransform;

namespace DustInTheWind.SvgToXaml.Conversion;

internal class SvgUseToXamlConversion : ToXamlConversion<SvgUse, UIElement>
{
    public SvgUseToXamlConversion(SvgUse svgUse, ConversionContext conversionContext, SvgElement referrer = null)
        : base(svgUse, conversionContext, referrer)
    {
    }

    protected override UIElement CreateXamlElement()
    {
        SvgElement referencedElement = SvgElement.GetReferencedElement();

        if (referencedElement == null)
            return null;

        IConversion<UIElement> conversion = ConvertReferencedElement(referencedElement);
        UIElement uiElement = conversion.Execute();

        return uiElement;
    }

    private IConversion<UIElement> ConvertReferencedElement(SvgElement referencedSvgElement)
    {
        switch (referencedSvgElement)
        {
            case SvgCircle svgCircle:
                return new SvgCircleToXamlConversion(svgCircle, ConversionContext, SvgElement);

            case SvgEllipse svgEllipse:
                return new SvgEllipseToXamlConversion(svgEllipse, ConversionContext, SvgElement);

            case SvgPath svgPath:
                return new SvgPathToXamlConversion(svgPath, ConversionContext, SvgElement);

            case SvgLine svgLine:
                return new SvgLineToXamlConversion(svgLine, ConversionContext, SvgElement);

            case SvgRectangle svgRect:
                return new SvgRectangleToXamlConversion(svgRect, ConversionContext, SvgElement);

            case SvgPolygon svgPolygon:
                return new SvgPolygonToXamlConversion(svgPolygon, ConversionContext, SvgElement);

            case SvgGroup svgGChild:
                return new SvgGroupToXamlConversion(svgGChild, ConversionContext, SvgElement);

            case SvgText svgText:
                return new SvgTextToXamlConversion(svgText, ConversionContext, SvgElement);

            case SvgUse childSvgUse:
                return new SvgUseToXamlConversion(childSvgUse, ConversionContext, SvgElement);

            default:
                Type inheritedElementType = referencedSvgElement.GetType();
                throw new UnknownElementTypeException(inheritedElementType);
        }
    }

    protected override void ConvertProperties(List<SvgElement> inheritedSvgElements)
    {
        base.ConvertProperties(inheritedSvgElements);

        double left = SvgElement.X;
        double top = SvgElement.Y;

        if (left != 0 || top != 0)
        {
            TransformGroupBuilder transformGroupBuilder = new(XamlElement.RenderTransform);

            TranslateTransform translateTransform = new(left, top);
            transformGroupBuilder.Add(translateTransform);
            XamlElement.RenderTransform = transformGroupBuilder.RootTransform;
        }
    }
}