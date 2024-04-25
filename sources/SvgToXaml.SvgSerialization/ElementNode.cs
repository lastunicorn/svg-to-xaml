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

namespace DustInTheWind.SvgToXaml.SvgSerialization;

internal class Node
{
    public virtual Node Previous { get; set; }

    public virtual Node Next { get; set; }

    public override string ToString()
    {
        return string.Empty;
    }
}

internal class AttributeNode : Node
{
    private readonly string name;

    public AttributeNode(string name)
    {
        this.name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public override string ToString()
    {
        return $"@{name}";
    }
}

internal class ElementNode : Node
{
    private readonly string name;
    private readonly string id;
    private int childCount;
    private Node next;
    private Node previous;

    public int Index { get; private set; }

    public override Node Previous
    {
        get => previous;
        set
        {
            if (value == null)
                Index = 0;

            previous = value;
        }
    }

    public override Node Next
    {
        get => next;
        set
        {
            if (Next != null)
                Next.Previous = null;

            if (value != null)
            {
                if (value is ElementNode elementNodeValue)
                {
                    childCount++;

                    elementNodeValue.Index = childCount;
                }

                value.Previous = this;
            }

            next = value;
        }
    }

    public ElementNode(string name, string id = null)
    {
        this.name = name ?? throw new ArgumentNullException(nameof(name));
        this.id = id;
    }

    public override string ToString()
    {
        string text = $"{name}";
        
        if(Index > 0)
            text += $"({Index})";

        if (id != null)
            text += $"[{id}]";

        return text;
    }
}