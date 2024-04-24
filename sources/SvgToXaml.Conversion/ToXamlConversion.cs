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
using System.Windows.Markup;
using System.Windows.Media;
using DustInTheWind.SvgToXaml.SvgModel;

namespace DustInTheWind.SvgToXaml.Conversion;

internal abstract class ToXamlConversion<TSvg, TXaml> : IConversion<TXaml>
    where TSvg : SvgElement
    where TXaml : UIElement
{
    private readonly SvgElement referrer;

    protected TSvg SvgElement { get; }

    protected TXaml XamlElement { get; private set; }

    protected ToXamlConversion(TSvg svgElement, SvgElement referrer = null)
    {
        SvgElement = svgElement ?? throw new ArgumentNullException(nameof(svgElement));
        this.referrer = referrer;
    }

    public TXaml Execute()
    {
        try
        {
            XamlElement = CreateXamlElement();

            if (XamlElement is FrameworkElement frameworkElement)
            {
                SetLanguage(frameworkElement);
            }

            if (SvgElement.Transforms.Count > 0)
                ApplyTransforms();

            if (SvgElement.ClipPath != null)
                ApplyClipPath();

            List<SvgElement> inheritedSvgElements = EnumerateInheritedElements().ToList();
            InheritPropertiesFrom(inheritedSvgElements);

            return XamlElement;
        }
        catch (SvgConversionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new SvgConversionException(ex);
        }
    }

    protected abstract TXaml CreateXamlElement();

    private void SetLanguage(FrameworkElement frameworkElement)
    {
        string languageId = SvgElement.Language ?? SvgElement.XmlLanguage;

        if (languageId != null)
            frameworkElement.Language = XmlLanguage.GetLanguage(languageId);
    }

    private void ApplyTransforms()
    {
        XamlElement.RenderTransform = SvgElement.Transforms.ToXaml(XamlElement.RenderTransform);
    }

    private void ApplyClipPath()
    {
        string referencedId = SvgElement.ClipPath.Url.ReferencedId;

        if (referencedId == null)
            return;

        SvgElement referencedElement = SvgElement.GetParentSvg()?.FindChild(referencedId);

        if (referencedElement is not SvgClipPath svgClipPath)
            return;

        SvgElement firstChild = svgClipPath.Children.FirstOrDefault();

        Geometry geometry = ConvertToGeometry(firstChild);

        if (geometry == null)
            return;

        XamlElement.Clip = geometry;
    }

    private static Geometry ConvertToGeometry(SvgElement svgElement)
    {
        switch (svgElement)
        {
            case SvgCircle svgCircle:
                {
                    Point centerPoint = new(svgCircle.CenterX, svgCircle.CenterY);
                    return new EllipseGeometry(centerPoint, svgCircle.Radius, svgCircle.Radius);
                }

            case SvgEllipse svgEllipse:
                {
                    Point centerPoint = new(svgEllipse.CenterX, svgEllipse.CenterY);
                    return new EllipseGeometry(centerPoint, svgEllipse.RadiusX, svgEllipse.RadiusY);
                }

            case SvgPath svgPath:
                {
                    return Geometry.Parse(svgPath.Data);
                }

            case SvgLine svgLine:
                {
                    Point startPoint = new(svgLine.X1, svgLine.Y1);
                    Point endPoint = new(svgLine.X2, svgLine.Y2);
                    return new LineGeometry(startPoint, endPoint);
                }

            case SvgRectangle svgRectangle:
                {
                    Rect rect = new(svgRectangle.X, svgRectangle.Y, svgRectangle.Width, svgRectangle.Height);
                    return new RectangleGeometry(rect);
                }

            case SvgPolygon svgPolygon:
                throw new NotImplementedException();

            case SvgPolyline svgPolyline:
                throw new NotImplementedException();

            case SvgUse svgUse:
            {
                string referencedId = svgUse.Href.Id;

                if (referencedId == null)
                    return Geometry.Empty;

                SvgElement referencedElement = svgElement.GetParentSvg().FindChild(referencedId);

                return ConvertToGeometry(referencedElement);
            }

            default:
                throw new UnknownElementTypeException(svgElement?.GetType());
        }
    }

    protected virtual IEnumerable<SvgElement> EnumerateInheritedElements()
    {
        yield return SvgElement;

        if (referrer == null)
        {
            IEnumerable<SvgElement> ancestors = SvgElement.EnumerateAncestors();

            foreach (SvgElement ancestor in ancestors)
                yield return ancestor;
        }
        else
        {
            IEnumerable<SvgElement> ancestors = SvgElement.EnumerateAncestors()
                .TakeWhile(x => x.GetType() != typeof(SvgDefinitions));

            foreach (SvgElement ancestor in ancestors)
                yield return ancestor;

            yield return referrer;

            IEnumerable<SvgElement> referrerAncestors = referrer.EnumerateAncestors();

            foreach (SvgElement ancestor in referrerAncestors)
                yield return ancestor;
        }
    }

    protected virtual void InheritPropertiesFrom(List<SvgElement> svgElements)
    {
        SetOpacity(svgElements);
    }

    private void SetOpacity(List<SvgElement> svgElements)
    {
        double? opacity = svgElements
            .Select(x => x.ComputeOpacity())
            .FirstOrDefault(x => x != null);

        if (opacity != null)
            XamlElement.Opacity = opacity.Value;
    }
}